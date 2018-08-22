using System;
using System.Threading.Tasks;

namespace AGS.Editor
{
    public interface IImplementationOption
    {
        string Name { get; }
        Task<SelectEditor.ReturnValue> Create();
    }
}