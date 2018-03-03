using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace AGS.Editor.Desktop
{
    public class DotnetProject : IDotnetProject
    {
        private Project _project;

        public string OutputFilePath => _project.OutputFilePath;

        public async Task Load(string path)
        {
            MSBuildPath.Setup();
            var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += onWorkspaceFailed;
            _project = await workspace.OpenProjectAsync(path);
        }

        public async Task<string> Compile()
        {
            var compilation = await _project.GetCompilationAsync();
            var emitResult = compilation.Emit(OutputFilePath);
            if (emitResult.Success) return null;
            return string.Join(Environment.NewLine, emitResult.Diagnostics
                               .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                               .Select(d => d.GetMessage()));
        }

        private void onWorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Debug.WriteLine($"Workspace failed: {e.Diagnostic.ToString()}");
        }
    }
}
