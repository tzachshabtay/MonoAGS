using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProtoBuf;
using System.Diagnostics;
using System.Reflection;
using ProtoBuf.Meta;
using System.Linq;
using System.Runtime.CompilerServices;
using Autofac;
using System.IO;

namespace AGS.Engine
{
	public class AGSSaveLoad : ISaveLoad
	{
		private readonly Resolver _resolver;
		private readonly IGameFactory _factory;
        private readonly IDictionary<string, ITexture> _textures;
		private readonly IGameState _state;
		private readonly IGameEvents _events;
		private static bool _firstTimeSetup = true;
		private const string RESTART_FILENAME = "RestartPoint.bin";

		public AGSSaveLoad(Resolver resolver, IGameFactory factory, 
                           IDictionary<string, ITexture> textures, IGame game)
		{
			_resolver = resolver;
			_factory = factory;
			_textures = textures;
			_state = game.State;
			_events = game.Events;
		}

		#region ISaveLoad implementation

		public void Save(string saveName)
		{
            try
            {
                saveName = Path.Combine(Hooks.FileSystem.StorageFolder, saveName);
                _state.Paused = true;
                firstTimeSetup();

                var context = getContext();
                ContractGameState state = new ContractGameState();
                state.FromItem(context, _state);
                using (var file = Hooks.FileSystem.Create(saveName))
                {
                    Serializer.Serialize(file, state);
                }
            }
            catch (Exception e)
            {
                string error = e.ToString();

				Debug.WriteLine("Failed to save game:");
				Debug.WriteLine(error);

				//throw;
			}
            finally
            {
                _state.Paused = false;   
            }
		}

		public async Task SaveAsync(string saveName)
		{
			await Task.Run(() => Save(saveName));
		}

		public void Load(string saveName)
		{
			try
			{
                saveName = Path.Combine(Hooks.FileSystem.StorageFolder, saveName);
				_state.Paused = true;
				firstTimeSetup();

				_state.Clean();
				var context = getContext();
				ContractGameState state;
				using (var file = Hooks.FileSystem.Open(saveName)) 
				{
					state = Serializer.Deserialize<ContractGameState>(file);
				}
				IGameState newState = state.ToItem(context);
				context.Rewire(newState);
				_state.CopyFrom(newState);

				_events.OnSavedGameLoad.Invoke(this, new AGSEventArgs());

				_state.Paused = false;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Failed to load game:");
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		public async Task LoadAsync(string saveName)
		{
			await Task.Run(() => Load(saveName));
		}

		public void SetRestartPoint()
		{
			Save(RESTART_FILENAME);
		}

		public void Restart()
		{
			Load(RESTART_FILENAME);
		}

		#endregion

		private AGSSerializationContext getContext()
		{
			var context = new AGSSerializationContext (_factory, _textures, 
                                                       _resolver, _resolver.Container.Resolve<IGLUtils>());
			context.Player = _state.Player;
			return context;
		}

		private void firstTimeSetup()
		{
			if (!_firstTimeSetup) return;
			_firstTimeSetup = false;

			setupContracts();
		}

		private void setupContracts()
		{
			RuntimeTypeModel.Default.AutoAddMissingTypes = true;

			Type contractType = typeof(IContract<>);
			IEnumerable<TypeInfo> types = typeof(AGSGame).GetTypeInfo().Assembly.DefinedTypes;
			foreach (var type in types)
			{
				if (!type.IsGenericType && type.GetCustomAttribute<ProtoContractAttribute>() != null)
				{
					RuntimeHelpers.RunClassConstructor(type.AsType().TypeHandle); //Forcing static constructors for contracts
				}
				Type contract = getSupportedInterfaces(type.AsType(), contractType);
				if (contract == null) continue;

				if (contractType.Equals(type)) continue;

				RuntimeTypeModel.Default.Add(contract, true).AddSubType(ContractsFactory.RunningID++, type.AsType());

				Type contractWrapper = typeof(Contract<>);
				contractWrapper = contractWrapper.MakeGenericType(contract.GetTypeInfo().GenericTypeArguments[0]);
				RuntimeTypeModel.Default.Add(contract, true).AddSubType(ContractsFactory.RunningID++, contractWrapper);
			}
		}

		private static Type getSupportedInterfaces(Type type,Type inter)
		{
			if (inter.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
				return type;
			List<Type> interfaces = type.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType 
				&& i.GetGenericTypeDefinition() == inter).ToList();

			interfaces.Sort(new TypeComparer ());
			return interfaces.FirstOrDefault();
		}

		private class TypeComparer : IComparer<Type>
		{
			#region IComparer implementation
			public int Compare(Type x, Type y)
			{
				TypeInfo xInfo = x.GetTypeInfo();
				if (!xInfo.IsGenericType) return -1;
				TypeInfo yInfo = y.GetTypeInfo();
				if (!yInfo.IsGenericType) return 1;

				if (xInfo.GenericTypeArguments[0].GetTypeInfo().IsAssignableFrom(yInfo.GenericTypeArguments[0].GetTypeInfo()))
					return 1;
				return -1;
			}
			#endregion
			
		}
	}
}

