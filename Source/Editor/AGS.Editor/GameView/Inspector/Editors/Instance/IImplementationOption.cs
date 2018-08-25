using System.Threading.Tasks;
using AGS.API;

namespace AGS.Editor
{
    public interface IImplementationOption
    {
        string Name { get; }
        Task<SelectEditor.ReturnValue> Create(IObject parentDialog);
    }
}