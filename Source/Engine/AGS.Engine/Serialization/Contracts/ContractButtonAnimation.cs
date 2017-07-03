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
        public Color? Tint { get; set; }

        public void FromItem(AGSSerializationContext context, ButtonAnimation item)
        {
            Animation = context.GetContract(item.Animation);
            Border = context.GetContract(item.Border);
            TextConfig = context.GetContract(item.TextConfig);
            Tint = item.Tint;
        }

        public ButtonAnimation ToItem(AGSSerializationContext context)
        {
            var button = new ButtonAnimation(Border.ToItem(context), TextConfig.ToItem(context), Tint);
            button.Animation = Animation.ToItem(context);
            return button;
        }
    }
}
