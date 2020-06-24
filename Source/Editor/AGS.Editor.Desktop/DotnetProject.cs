﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.MSBuild;

namespace AGS.Editor.Desktop
{
    public class DotnetProject : IDotnetProject
    {
        //private Project _project;

        //public string OutputFilePath => _project.OutputFilePath;
        public string OutputFilePath { get; private set; }

        public async Task Load(string path)
        {
            //todo: tmp workaround for roslyn bug- https://github.com/dotnet/roslyn/issues/20848
            await loadProjXml(path);

            /*MSBuildPath.Setup();
            var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += onWorkspaceFailed;
            _project = await workspace.OpenProjectAsync(path);*/
        }

        private async Task loadProjXml(string path)
        {
            await Task.Delay(1);
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            Trace.Assert(doc.DocumentElement != null, "DocumentElement is missing from project xml file");
            mgr.AddNamespace("foo", doc.DocumentElement.NamespaceURI);
            XmlNode firstOutputType = doc.SelectSingleNode("//foo:OutputType", mgr);
            XmlNode firstOutputPath = doc.SelectSingleNode("//foo:OutputPath", mgr);
            XmlNode firstAssemblyName = doc.SelectSingleNode("//foo:AssemblyName", mgr);
            XmlNode firstTargetFramework = doc.SelectSingleNode("//foo:TargetFramework", mgr);

            Trace.Assert(firstTargetFramework != null, "TargetFramework is missing from project xml file");
            Trace.Assert(firstOutputType != null, "OutputType is missing from project xml file");
            Trace.Assert(firstAssemblyName != null, "AssemblyName is missing from project xml file");

            string outputPath = firstOutputPath == null ? $"bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}{firstTargetFramework.InnerText}" :
                firstOutputPath.InnerText.Replace("\\", Path.DirectorySeparatorChar.ToString());

            string fileExtension = firstOutputType.InnerText == "Exe" ? "exe" : "dll";
            string folder = Path.Combine(Directory.GetCurrentDirectory(), outputPath);
            OutputFilePath = Path.Combine(folder, $"{firstAssemblyName.InnerText}.{fileExtension}");
        }

        public async Task<string> Compile()
        {
            await Task.Delay(1);
            return "Compilation currently not supported";
            /*var compilation = await _project.GetCompilationAsync();
            var emitResult = compilation.Emit(OutputFilePath);
            if (emitResult.Success) return null;
            return string.Join(Environment.NewLine, emitResult.Diagnostics
                               .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                               .Select(d => d.GetMessage()));*/
        }

        /*private void onWorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Debug.WriteLine($"Workspace failed: {e.Diagnostic.ToString()}");
        }*/
    }
}
