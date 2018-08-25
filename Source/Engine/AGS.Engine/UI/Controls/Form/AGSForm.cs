using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSForm : IForm
    {
        public AGSForm(ILabel header, IPanel contents)
        {
            Header = header;
            Contents = contents;
        }

        public ILabel Header { get; }

        public IPanel Contents { get; }

        public float Width
        {
            get => Header.Width;
            set
            {
                Header.LabelRenderSize = (value, Header.BaseSize.Height);
                Contents.BaseSize = (value, Contents.BaseSize.Height);
            }
        }

        public float Height => Header.Height + Contents.Height;

        public float X { get => Header.X; set => Header.X = value; }
        public float Y { get => Header.Y; set => Header.Y = value; }
        public bool Visible { get => Header.Visible; set => Header.Visible = value; }
    }
}