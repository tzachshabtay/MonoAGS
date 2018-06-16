using System;
using System.Text;

namespace AGS.Editor
{
    public interface ICodeGenerator
    {
        void GenerateCode(EntityModel model, StringBuilder code);
    }
}