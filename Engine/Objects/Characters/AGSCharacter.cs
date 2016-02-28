using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Autofac;

namespace AGS.Engine
{
	public class AGSCharacter : ICharacter, IOutfitHolder
	{
		private readonly IObject _obj;
		private readonly ISayBehavior _sayBehavior;
		private readonly IWalkBehavior _walkBehavior;
		private readonly IAGSFaceDirectionBehavior _faceDirectionBehavior;
		private readonly IHasRoom _roomBehavior;
		private readonly VisibleProperty _visible;
		private readonly EnabledProperty _enabled;

		public AGSCharacter (IObject obj, IOutfit outfit, Resolver resolver, IPathFinder pathFinder)
		{
			Outfit = outfit;
			TreeNode = new AGSTreeNode<IObject> (this);
			_visible = new VisibleProperty (this);
			_enabled = new EnabledProperty (this);

			TypedParameter objParameter = new TypedParameter (typeof(IObject), this);
			_faceDirectionBehavior = resolver.Container.Resolve<IAGSFaceDirectionBehavior>(objParameter);
			_faceDirectionBehavior.CurrentDirectionalAnimation = Outfit.IdleAnimation;

			TypedParameter faceDirectionParameter = new TypedParameter (typeof(IAGSFaceDirectionBehavior), _faceDirectionBehavior);
			TypedParameter outfitParameter = new TypedParameter (typeof(IOutfitHolder), this);

			ISayLocation location = resolver.Container.Resolve<ISayLocation>(objParameter);

			TypedParameter locationParameter = new TypedParameter (typeof(ISayLocation), location);
			_sayBehavior = resolver.Container.Resolve<ISayBehavior>(locationParameter, outfitParameter, faceDirectionParameter);

			_walkBehavior = resolver.Container.Resolve<IWalkBehavior>(objParameter, outfitParameter, faceDirectionParameter);

			Inventory = resolver.Container.Resolve<IInventory>();

			_roomBehavior = resolver.Container.Resolve<IHasRoom>(objParameter);

			_obj = obj;
			IgnoreScalingArea = false;
		}

		#region ICharacter implementation

		public string ID { get { return _obj.ID; } }

		public ICustomProperties Properties { get { return _obj.Properties; } }

		public ILocation Location 
		{ 
			get { return _obj.Location; } 
			set 
			{ 
				StopWalking();
				_obj.Location = value; 
			} 
		}

		public float X  { get { return _obj.X; } set { _obj.X = value; } }
		public float Y { get { return _obj.Y; } set { _obj.Y = value; } }
		public float Z { get { return _obj.Z; } set { _obj.Z = value; } }

		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get; private set; }

		public IImage Image { get { return _obj.Image; } set { _obj.Image = value; } }
		public IImageRenderer CustomRenderer 
		{ 
			get { return _obj.CustomRenderer; } 
			set { _obj.CustomRenderer = value; } 
		}

		public string Hotspot { get { return _obj.Hotspot; } set { _obj.Hotspot = value; } }

		public bool IsWalking { get { return _walkBehavior.IsWalking; } }

		public void ResetScale ()
		{
			_obj.ResetScale ();
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			_obj.ScaleBy (scaleX, scaleY);
		}

		public void ScaleTo (float width, float height)
		{
			_obj.ScaleTo (width, height);
		}

		public void FlipHorizontally()
		{
			_obj.FlipHorizontally();
		}

		public void FlipVertically()
		{
			_obj.FlipVertically();
		}

		public ISprite Clone()
		{
			return _obj.Clone();
		}

		public float Height { get { return _obj.Height; } }

		public float Width { get { return _obj.Width; } }

		public float ScaleX { get { return _obj.ScaleX; } }

		public float ScaleY { get { return _obj.ScaleY; } }

		public float Angle {get { return _obj.Angle;} set { _obj.Angle = value;}}

		public byte Opacity {get { return _obj.Opacity;} set { _obj.Opacity = value;}}

		public Color Tint {get { return _obj.Tint;} set { _obj.Tint = value;}}

		public IPoint Anchor {get { return _obj.Anchor;} set { _obj.Anchor = value;}}

		public ISquare BoundingBox { get { return _obj.BoundingBox; } set { _obj.BoundingBox = value; } }

		public void PixelPerfect(bool pixelPerfect)
		{
			_obj.PixelPerfect(pixelPerfect);
		}

		public IArea PixelPerfectHitTestArea
		{
			get
			{
				return _obj.PixelPerfectHitTestArea;
			}
		}

		public bool IgnoreViewport { get { return _obj.IgnoreViewport; } set { _obj.IgnoreViewport = value; } }
		public bool IgnoreScalingArea { get { return _obj.IgnoreScalingArea; } set { _obj.IgnoreScalingArea = value; } }

		public IPoint WalkPoint { get { return _obj.WalkPoint; } set { _obj.WalkPoint = value; } }

		public IPoint CenterPoint { get { return _obj.CenterPoint; } }

		public bool DebugDrawAnchor { get { return _obj.DebugDrawAnchor; } set { _obj.DebugDrawAnchor = value; } }
		public IBorderStyle Border { get { return _obj.Border; } set { _obj.Border = value; } }
		public bool DebugDrawWalkPath { get { return _walkBehavior.DebugDrawWalkPath; } set { _walkBehavior.DebugDrawWalkPath = value; } }

		public ISayConfig SpeechConfig { get { return _sayBehavior.SpeechConfig; } }

		public void Say (string text)
		{
			_sayBehavior.Say(text);
		}

		public async Task SayAsync (string text)
		{
			await _sayBehavior.SayAsync(text);
		}

		public bool Walk (ILocation location)
		{
			return _walkBehavior.Walk(location);
		}

		public async Task<bool> WalkAsync (ILocation location)	
		{
			return await _walkBehavior.WalkAsync(location);
		}

		public void StopWalking()
		{
			_walkBehavior.StopWalking();
		}

		public async Task StopWalkingAsync()
		{
			await _walkBehavior.StopWalkingAsync();
		}

		public IInventory Inventory { get; set; }

		public IOutfit Outfit { get; set; }

		public ITextConfig SpeechTextConfig { get; set; }

		public int WalkSpeed { get { return _walkBehavior.WalkSpeed; } set { _walkBehavior.WalkSpeed = value; } }

		public void FaceDirection(Direction direction)
		{
			_faceDirectionBehavior.FaceDirection(direction);
		}

		public async Task FaceDirectionAsync(Direction direction)
		{
			await _faceDirectionBehavior.FaceDirectionAsync(direction);
		}

		public void FaceDirection(IObject obj)
		{
			_faceDirectionBehavior.FaceDirection(obj);
		}

		public async Task FaceDirectionAsync(IObject obj)
		{
			await _faceDirectionBehavior.FaceDirectionAsync(obj);
		}

		public void FaceDirection(float x, float y)
		{
			_faceDirectionBehavior.FaceDirection(x, y);
		}

		public async Task FaceDirectionAsync(float x, float y)
		{
			await _faceDirectionBehavior.FaceDirectionAsync(x, y);
		}

		public void FaceDirection(float fromX, float fromY, float toX, float toY)
		{
			_faceDirectionBehavior.FaceDirection(fromX, fromY, toX, toY);
		}

		public async Task FaceDirectionAsync(float fromX, float fromY, float toX, float toY)
		{
			await _faceDirectionBehavior.FaceDirectionAsync(fromX, fromY, toX, toY);
		}

		public Direction Direction { get { return _faceDirectionBehavior.Direction; } }

		public IDirectionalAnimation CurrentDirectionalAnimation{ get { return _faceDirectionBehavior.CurrentDirectionalAnimation; } }

		#endregion

		#region IObject implementation

		public void StartAnimation (IAnimation animation)
		{
			_obj.StartAnimation (animation);
		}

		public AnimationCompletedEventArgs Animate (IAnimation animation)
		{
			return _obj.Animate (animation);
		}

		public async Task<AnimationCompletedEventArgs> AnimateAsync (IAnimation animation)
		{
			return await _obj.AnimateAsync (animation);
		}

		public void ChangeRoom(IRoom room, float? x = null, float? y = null)
		{
			StopWalking();
			_roomBehavior.ChangeRoom(room, x, y);
		}

		public bool CollidesWith(float x, float y)
		{
			return _obj.CollidesWith(x, y);
		}

		public void PlaceOnWalkableArea()
		{
			_walkBehavior.PlaceOnWalkableArea();
		}

		public IRoom Room { get { return _roomBehavior.Room; } }

		public IRoom PreviousRoom { get { return _roomBehavior.PreviousRoom; } }

		public IAnimation Animation { get { return _obj.Animation; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public bool Visible { get { return _visible.Value; } set { _visible.Value = value; } }

		public bool Enabled { get { return _enabled.Value; } set { _enabled.Value = value; } }

		public bool UnderlyingVisible { get { return _visible.UnderlyingValue; } }

		public bool UnderlyingEnabled { get { return _enabled.UnderlyingValue; } }

		public override string ToString ()
		{
			return string.Format("Character: {0}", ID ?? Hotspot ?? base.ToString ());
		}

		public void Dispose()
		{
			_obj.Dispose();
		}

		#endregion
	}
}

