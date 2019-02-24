using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractCharacter : IContract<IObject>
	{
		static ContractCharacter()
		{
			ContractsFactory.RegisterFactory(typeof(ICharacter), () => new ContractCharacter ());
			ContractsFactory.RegisterSubtype(typeof(IContract<IObject>), typeof(ContractCharacter));
		}

	    [ProtoMember(1)]
        public PointF WalkSpeed { get; set; }

		[ProtoMember(2)]
		public bool DebugDrawWalkPath { get; set; }

		[ProtoMember(3)]
		public IContract<IOutfit> Outfit { get; set; }

		[ProtoMember(4)]
		public ContractObject Obj { get; set; }

		[ProtoMember(5)]
		public IContract<IInventory> Inventory { get; set; }

		[ProtoMember(6)]
		public bool IsPlayer { get; set; }
        
        [ProtoMember(7)]
        public bool AdjustWalkSpeedToScaleArea { get; set; }

		//todo: Character's current animation will be cloned and not have the same reference to the animation in the outfit. This should be fixed.

		#region IContract implementation

		public ICharacter ToItem(AGSSerializationContext context)
		{
            if (Obj == null) return null;
			ICharacter item = context.Factory.Object.GetCharacter(Obj.ID, Outfit.ToItem(context), Obj.AnimationComponent.ToItem(context));
			Obj.ToItem(context, item);
			item.WalkStep = WalkSpeed;
            item.AdjustWalkSpeedToScaleArea = AdjustWalkSpeedToScaleArea;
			item.DebugDrawWalkPath = DebugDrawWalkPath;
			item.Inventory = Inventory.ToItem(context);

			if (IsPlayer) context.Player = item;

			return item;
		}

		IObject IContract<IObject>.ToItem(AGSSerializationContext context)
		{
			return ToItem(context);
		}

		public void FromItem(AGSSerializationContext context, ICharacter item)
		{
            if (item == null) return;
			WalkSpeed = item.WalkStep;
            AdjustWalkSpeedToScaleArea = item.AdjustWalkSpeedToScaleArea;
			DebugDrawWalkPath = item.DebugDrawWalkPath;

			Outfit = context.GetContract(item.Outfit);

			Obj = new ContractObject ();
			Obj.FromItem(context, item);

			Inventory = context.GetContract(item.Inventory);

			if (context.Player == item) IsPlayer = true;
		}

		void IContract<IObject>.FromItem(AGSSerializationContext context, IObject item)
		{
			FromItem(context, (ICharacter)item);
		}

		#endregion
	}
}

