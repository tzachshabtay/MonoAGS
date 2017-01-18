using System.Reflection;

namespace AGS.Engine.Desktop
{
    public class DesktopAssemblies : IAssemblies
    {
        public DesktopAssemblies()
        {
            EntryAssembly = Assembly.GetEntryAssembly();
        }

        public Assembly EntryAssembly { get; private set; }
    }
}
