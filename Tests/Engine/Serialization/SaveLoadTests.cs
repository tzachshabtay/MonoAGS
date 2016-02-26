using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using Autofac;
using System.Drawing;

namespace Tests
{
	[TestFixture]
	public class SaveLoadTests
	{
		private Resolver _resolver;
		private IDictionary<string, GLImage> _textures;
		private IGameState _state;
		private IGameFactory _factory;
		private AGSSaveLoad _saveLoad;
		private Mocks _mocks;

		[SetUp]
		public void Init()
		{
			_mocks = Mocks.Init();
			_resolver = new Resolver ();
			_resolver.Build();
			var updater = new ContainerBuilder ();
			updater.RegisterInstance(_mocks.Input().Object).As<IInput>();
			updater.Update(_resolver.Container);
			_textures = new Dictionary<string, GLImage> ();
			_state = _resolver.Container.Resolve<IGameState>();
			_factory = _resolver.Container.Resolve<IGameFactory>();
			_saveLoad = new AGSSaveLoad (_resolver, _factory, _textures, _resolver.Container.Resolve<IGame>());
			_state.Rooms.Add(_mocks.Room().Object);
		}

		[TearDown]
		public void Teardown()
		{
			_mocks.Dispose();
		}

		[Test]
		public void SaveLoad_Object_Test()
		{
			foreach (IObject obj in ObjectTests.GetImplementors(_mocks, null, _state))
			{
				setupObject(obj);

				_saveLoad.Save("test");
				_saveLoad.Load("test");

				Assert.AreEqual(1, _state.Rooms.Count);
				Assert.AreEqual(1, _state.Rooms[0].Objects.Count);

				IObject actual = _state.Rooms[0].Objects.First();
				testObject(actual);

				clearState();
			}
		}

		[Test]
		public void SaveLoad_ButtonInPanel_Test()
		{
			List<IObject> objs = ObjectTests.GetImplementors(_mocks, null, _state).ToList();
			IPanel panel = (IPanel)objs.First(o => o is IPanel);
			IButton button = (IButton)objs.First(o => o is IButton);
			button.TreeNode.SetParent(panel.TreeNode);

			setupObject(panel);
			setupObject(button);

			_saveLoad.Save("test");
			_saveLoad.Load("test");

			Assert.AreEqual(1, _state.Rooms.Count);
			Assert.AreEqual(2, _state.Rooms[0].Objects.Count);

			panel = null;
			button = null;
			foreach (IObject obj in _state.Rooms[0].Objects)
			{
				testObject(obj);
				if (button == null) button = obj as IButton;
				if (!(obj is IButton)) panel = obj as IPanel;
			}
			Assert.AreSame(button.TreeNode.Parent, panel);
			Assert.AreSame(button.TreeNode.Parent, panel.TreeNode.Node);

			RenderOrderSelector comparer = new RenderOrderSelector ();
			Assert.AreEqual(1, comparer.Compare(button, panel));
			Assert.AreEqual(-1, comparer.Compare(panel, button));

			clearState();
		}

		private void setupObject(IObject obj)
		{
			obj.Anchor = new AGSPoint (0.1f, 0.2f);
			obj.WalkPoint = new AGSPoint (0.3f, 0.4f);
			obj.Location = new AGSLocation (0.5f, 0.6f, 0.7f);
			obj.Angle = 0.8f;
			obj.Image = new EmptyImage (100f, 50f);
			obj.ScaleBy(2f, 2.5f);
			obj.Tint = Color.AliceBlue;

			IRoom room = _mocks.Room().Object;
			room.Objects.Add(obj);
		}

		private void testObject(IObject actual)
		{
			Assert.AreEqual(0.1f, actual.Anchor.X);
			Assert.AreEqual(0.2f, actual.Anchor.Y);
			Assert.AreEqual(0.3f, actual.WalkPoint.X);
			Assert.AreEqual(0.4f, actual.WalkPoint.Y);
			Assert.AreEqual(0.5f, actual.X);
			Assert.AreEqual(0.6f, actual.Y);
			Assert.AreEqual(0.7f, actual.Z);
			Assert.AreEqual(0.8f, actual.Angle);
			Assert.AreEqual(2f, actual.ScaleX);
			Assert.AreEqual(2.5f, actual.ScaleY);
			Assert.AreEqual(Color.AliceBlue.R, actual.Tint.R);
			Assert.AreEqual(Color.AliceBlue.G, actual.Tint.G);
			Assert.AreEqual(Color.AliceBlue.B, actual.Tint.B);
			Assert.AreEqual(Color.AliceBlue.A, actual.Tint.A);
		}

		private void clearState()
		{
			IRoom room = _mocks.Room().Object;
			room.Objects.Clear();
		}
	}
}

