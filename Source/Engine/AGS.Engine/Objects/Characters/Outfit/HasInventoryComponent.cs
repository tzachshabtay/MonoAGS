﻿using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class HasInventoryComponent : AGSComponent, IInventoryComponent
	{
		public HasInventoryComponent(Resolver resolver)
		{
			Inventory = resolver.Container.Resolve<IInventory>();
		}

		public IInventory Inventory { get; set; }
	}
}

