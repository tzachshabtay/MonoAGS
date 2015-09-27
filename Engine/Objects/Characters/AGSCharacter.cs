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

		public AGSCharacter (IObject obj, IOutfit outfit, Resolver resolver, IPathFinder pathFinder)
		{
			Outfit = outfit;

			TypedParameter objParameter = new TypedParameter (typeof(IObject), obj);
			_faceDirectionBehavior = resolver.Container.Resolve<IAGSFaceDirectionBehavior>(objParameter);

			TypedParameter faceDirectionParameter = new TypedParameter (typeof(IAGSFaceDirectionBehavior), _faceDirectionBehavior);
			TypedParameter outfitParameter = new TypedParameter (typeof(IOutfitHolder), this);

			ISayLocation location = resolver.Container.Resolve<ISayLocation>(objParameter);

			TypedParameter locationParameter = new TypedParameter (typeof(ISayLocation), location);
			_sayBehavior = resolver.Container.Resolve<ISayBehavior>(locationParameter);

			_walkBehavior = resolver.Container.Resolve<IWalkBehavior>(objParameter, outfitParameter, faceDirectionParameter);

			_obj = obj;
			IgnoreScalingArea = false;
		}

		#region ICharacter implementation

		public ILocation Location 
		{ 
			get { return _obj.Location; } 
			set 
			{ 
				StopWalking();
				_obj.Location = value; 
			} 
		}

		public float X 
		{ 
			get { return _obj.X; } 
			set 
			{
				StopWalking();
				_obj.X = value; 
			} 
		}

		public float Y 
		{ 
			get { return _obj.Y; } 
			set 
			{ 
				StopWalking();
				_obj.Y = value; 
			} 
		}

		public float Z { get { return _obj.Z; } set { _obj.Z = value; } }

		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get { return _obj.TreeNode; } }

		public IImage Image { get { return _obj.Image; } set { _obj.Image = value; } }
		public IImageRenderer CustomRenderer 
		{ 
			get { return _obj.CustomRenderer; } 
			set { _obj.CustomRenderer = value; } 
		}

		public bool Enabled { get { return _obj.Enabled; } set { _obj.Enabled = value; } }
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
			return await AnimateAsync (animation);
		}

		public void ChangeRoom(IRoom room, float? x = null, float? y = null)
		{
			_obj.ChangeRoom(room);
			if (x != null) X = x.Value;
			if (y != null) Y = y.Value;
		}

		public void PlaceOnWalkableArea()
		{
			_walkBehavior.PlaceOnWalkableArea();
		}

		public IRoom Room { get { return _obj.Room; } }

		public IAnimation Animation { get { return _obj.Animation; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public bool Visible { get { return _obj.Visible; } set { _obj.Visible = value; } }

		public override string ToString ()
		{
			return Hotspot ?? base.ToString ();
		}

		#endregion
	}
}

