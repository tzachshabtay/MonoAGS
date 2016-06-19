using AGS.API;
using ProtoBuf;
using Autofac;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractTextComponent : IContract<ITextComponent>
    {
        [ProtoMember(1)]
        public IContract<ITextConfig> TextConfig { get; set; }

        [ProtoMember(2)]
        public string Text { get; set; }

        [ProtoMember(3)]
        public float Width { get; set; }

        [ProtoMember(4)]
        public float Height { get; set; }

        #region IContract implementation

        public ITextComponent ToItem(AGSSerializationContext context)
        {
            var component = context.Resolver.Container.Resolve<ITextComponent>();
            component.TextConfig = TextConfig.ToItem(context);
            component.Text = Text;
            component.LabelRenderSize = new SizeF(Width, Height);
            return component;
        }

        public void FromItem(AGSSerializationContext context, ITextComponent item)
        {
            TextConfig = context.GetContract(item.TextConfig);
            Text = item.Text;
            Width = item.LabelRenderSize.Width;
            Height = item.LabelRenderSize.Height;
        }

        #endregion
    }
}
