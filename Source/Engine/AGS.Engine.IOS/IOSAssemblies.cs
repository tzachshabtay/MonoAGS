using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AGS.Engine.IOS
{
    public class IOSAssemblies : IAssemblies
    {
        public IOSAssemblies()
        {
            EntryAssembly = getEntryAssembly();
        }

        public Assembly EntryAssembly { get; private set; }

        private Assembly getEntryAssembly()
        {
            var methodFrames = new StackTrace().GetFrames().Select(t => t.GetMethod()).ToArray();
            foreach (var frame in methodFrames)
            {
                if (frame.Name == "Main") return frame.Module.Assembly;
            }
            return Assembly.GetAssembly(typeof(AGSEngineIOS));
        }
    }
}
