using System;
using System.Threading.Tasks;

namespace AGS.Editor
{
    public interface IDotnetProject
    {
        string OutputFilePath { get; }

        Task Load(string path);

        Task<string> Compile();
    }
}
