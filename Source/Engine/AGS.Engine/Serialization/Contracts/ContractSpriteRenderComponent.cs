using System;
using ProtoBuf;
using AGS.API;


namespace AGS.Engine
{
    [ProtoContract]
    public class ContractSpriteRenderComponent : IContract<ISpriteRenderComponent>
    {
        public ContractSpriteRenderComponent()
        {
        }
        
        [ProtoMember(1)]
        public IContract<ISpriteProvider> SpriteProvider { get; set; }

        [ProtoMember(2)]
        public bool DebugDrawPivot { get; set; }

        [ProtoMember(3)]
        public IContract<IBorderStyle> Border { get; set; }

        #region IContract implementation

        public ISpriteRenderComponent ToItem(AGSSerializationContext context)
        {
            AGSSpriteRenderComponent container = new AGSSpriteRenderComponent();
            ToItem(context, container);
            return container;
        }

        public void ToItem(AGSSerializationContext context, ISpriteRenderComponent container)
        {
            container.SpriteProvider = SpriteProvider.ToItem(context);
            container.Border = Border.ToItem(context);
            container.DebugDrawPivot = DebugDrawPivot;
        }

        public void FromItem(AGSSerializationContext context, ISpriteRenderComponent item)
        {
            SpriteProvider = context.GetContract(item.SpriteProvider);
            Border = context.GetContract(item.Border);
            DebugDrawPivot = item.DebugDrawPivot;
        }

        #endregion
    }
}

