using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractButtonAnimation : IContract<ButtonAnimation>
    {
        public ContractButtonAnimation() { }

        [ProtoMember(1)]
        public IContract<IAnimation> Animation { get; set; }

        [ProtoMember(2)]
        public IContract<IBorderStyle> Border { get; set; }

        [ProtoMember(3)]
        public IContract<ITextConfig> TextConfig { get; set; }

        [ProtoMember(4)]
        public uint? Tint { get; set; }

        [ProtoMember(5)]
        public IContract<IImage> Image { get; set; }

        public void FromItem(AGSSerializationContext context, ButtonAnimation item)
        {
            Animation = context.GetContract(item.Animation);
            Border = context.GetContract(item.Border);
            TextConfig = context.GetContract(item.TextConfig);
            Tint = item.Tint == null ? (uint?)null : item.Tint.Value.Value;
            Image = context.GetContract(item.Image);
        }

        public ButtonAnimation ToItem(AGSSerializationContext context)
        {
            var button = new ButtonAnimation(Animation.ToItem(context), Image.ToItem(context), Border.ToItem(context), TextConfig.ToItem(context),
                                             Tint == null ? (Color?)null : Color.FromHexa(Tint.Value));
            return button;
        }
    }
}
