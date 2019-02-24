using System.Text;

namespace AGS.Editor
{
    public interface ICodeGenerator
    {
        void GenerateCode(string namespaceName, EntityModel model, StringBuilder code);
    }
}