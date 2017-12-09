using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;

namespace DemoGame
{
    public class PlayerAsFeature
    {
		private List<IArea> _lastAreas;
		private IRenderLayer _lastLayer;
		private IObject _lastParent;
		private ILocation _lastLocation;
        private float _lastScaleX, _lastScaleY;
        private readonly ICharacter _player;

        public PlayerAsFeature(ICharacter player)
        {
            _player = player;    
        }

        public void PlaceInFeatureWindow(IObject newParent)
        {
			_lastAreas = new List<IArea>(_player.Room.Areas);
			_lastLayer = _player.RenderLayer;
			_lastParent = _player.TreeNode.Parent;
			_lastLocation = _player.Location;
            _lastScaleX = _player.ScaleX;
            _lastScaleY = _player.ScaleY;

			_player.RenderLayer = newParent.RenderLayer;
			_player.Room.Areas.Clear();
			_player.Room.Edges.Left.Enabled = false;
			_player.Room.Edges.Right.Enabled = false;
			_player.TreeNode.SetParent(newParent.TreeNode);
        }

        public async Task Restore()
        {
			_player.RenderLayer = _lastLayer;
            _player.TreeNode.SetParent(_lastParent == null ? null : _lastParent.TreeNode);
            _player.Room.Areas.Clear();
			_player.Room.Edges.Left.Enabled = true;
			_player.Room.Edges.Right.Enabled = true;
			foreach (var area in _lastAreas) _player.Room.Areas.Add(area);
            await _player.StopWalkingAsync();
			_player.Location = _lastLocation;
            _player.Scale = new PointF(_lastScaleX, _lastScaleY);
        }
    }
}
