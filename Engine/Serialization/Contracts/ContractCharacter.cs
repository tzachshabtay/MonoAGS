using System;
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

		public ContractCharacter()
		{
		}

		[ProtoMember(1)]
		public int WalkSpeed { get; set; }

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

		//todo: Character's current animation will be cloned and not have the same reference to the animation in the outfit. This should be fixed.

		#region IContract implementation

		public ICharacter ToItem(AGSSerializationContext context)
		{
			ICharacter item = context.Factory.Object.GetCharacter(Obj.ToItem(context), Outfit.ToItem(context));
			item.WalkSpeed = WalkSpeed;
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
			WalkSpeed = item.WalkSpeed;
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

