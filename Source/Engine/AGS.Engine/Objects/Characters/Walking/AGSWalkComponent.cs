using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace AGS.Engine
{
    public class AGSWalkComponent : AGSComponent, IWalkComponent
    {
        private WalkInstruction _currentWalk;
        private ConcurrentQueue<WalkInstruction> _incomingInstructions;
        private IPathFinder _pathFinder;
        private List<IObject> _debugPath;
        private IFaceDirectionComponent _faceDirection;
        private IOutfitComponent _outfit;
        private IHasRoomComponent _room;
        private IDrawableInfoComponent _drawable;
        private IObjectFactory _objFactory;
        private ITranslate _translate;
        private ICutscene _cutscene;
        private IGameState _state;
        private IGameEvents _events;
        private IAnimationComponent _animation;
        private readonly IGLUtils _glUtils;

        public AGSWalkComponent(IPathFinder pathFinder, IObjectFactory objFactory, IGame game, IGLUtils glUtils)
        {
            _incomingInstructions = new ConcurrentQueue<WalkInstruction>();
            _state = game.State;
            _cutscene = _state.Cutscene;
            _events = game.Events;
            _pathFinder = pathFinder;
            _objFactory = objFactory;
            _glUtils = glUtils;

            _debugPath = new List<IObject>();
            AdjustWalkSpeedToScaleArea = true;
            MovementLinkedToAnimation = true;
            WalkStep = new PointF(8f, 8f);

            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public static int WalkLineTimeoutInMilliseconds = 15000; //15 seconds

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            var instructions = _incomingInstructions;
            if (instructions != null)
            {
                while (instructions.TryDequeue(out var instruction))
                {
                    onWalkLineCompleted(instruction, instruction.CurrentLine, false);
                }
            }
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IAnimationComponent>(c => _animation = c, _ => _animation = null);
            Entity.Bind<ITranslateComponent>(c => _translate = c, _ => _translate = null);
            Entity.Bind<IOutfitComponent>(c => _outfit = c, _ => _outfit = null);
            Entity.Bind<IHasRoomComponent>(c => _room = c, _ => _room = null);
            Entity.Bind<IDrawableInfoComponent>(c => _drawable = c, _ => _drawable = null);
            Entity.Bind<IFaceDirectionComponent>(c => _faceDirection = c, _ => _faceDirection = null);
        }

        #region IWalkBehavior implementation

        public async Task<bool> WalkAsync(Position position, bool walkAnywhere = false)
        {
            return await walkAsync(position, walkAnywhere, walkAnywhere);
        }

        public async Task<bool> WalkStraightAsync(Position position)
        {
            return await walkAsync(position, true, false);
        }

        public async Task StopWalkingAsync()
        {
            await addNewInstructionAsync((0, 0), false, false, true);
        }

        public void PlaceOnWalkableArea()
        {
            PointF current = new PointF(_translate.X, _translate.Y);
            PointF? closestPoint = getClosestWalkablePoint(current);
            if (closestPoint != null)
            {
                _translate.Position = (closestPoint.Value.X, closestPoint.Value.Y);
            }
        }

        public PointF WalkStep { get; set; }

        public bool AdjustWalkSpeedToScaleArea { get; set; }

        public bool MovementLinkedToAnimation { get; set; }

        public bool IsWalking
        {
            get
            {
                Task task = _currentWalk?.OnCompletion.Task;
                if (task == null) return false;
                return (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted);
            }
        }

        public Position WalkDestination { get; private set; }

        public bool DebugDrawWalkPath { get; set; }

        #endregion

        private void onRepeatedlyExecute()
        {
            WalkInstruction currentWalk = getWalkInstruction();
            WalkLineInstruction currentLine = currentWalk?.CurrentLine;
            if (currentLine == null) return;

            if (currentWalk.CancelToken.IsCancellationRequested ||
                (!currentWalk.WalkAnywhere && !isWalkable(_translate.Position)) || _room?.Room != currentWalk.Room)
            {
                onWalkLineCompleted(currentWalk, currentLine, false);
                return;
            }
            if (currentLine.NumSteps <= 1f)
            {
                onWalkLineCompleted(currentWalk, currentLine, true);
                return;
            }
            if (_cutscene.IsSkipping)
            {
                _translate.Position = currentLine.Destination;
                onWalkLineCompleted(currentWalk, currentLine, true);
                return;
            }
            PointF walkSpeed = adjustWalkSpeed(WalkStep);
            float xStep = currentLine.XStep * walkSpeed.X;
            float yStep = currentLine.YStep * walkSpeed.Y;
            if (MovementLinkedToAnimation && _animation != null && _animation.Animation.Frames.Count > 1 &&
                _animation.Animation.Sprite == currentLine.LastFrame)
            {
                //If the movement is linked to the animation and the animation speed is slower the the viewport movement, it can lead to flickering
                //so we do a smooth movement for this scenario.
                (var compensateX, var compensateY) = currentLine.Compensate;
                (var lastViewportX, var lastViewportY) = currentLine.Viewport;
                var candidateX = _translate.X + compensateForViewScrollIfNeeded(_state.Viewport.X, xStep, ref compensateX, ref lastViewportX);
                var candidateY = _translate.Y + compensateForViewScrollIfNeeded(_state.Viewport.Y, yStep, ref compensateY, ref lastViewportY);
                if (currentWalk.WalkAnywhere || isWalkable(new Position(candidateX, candidateY)))
                {
                    currentLine.Compensate = (compensateX, compensateY);
                    _translate.Position = (candidateX, candidateY);
                    return;
                }
            }
            currentLine.LastFrame = _animation?.Animation.Sprite;
            currentLine.Viewport = (_state.Viewport.X, _state.Viewport.Y);

            currentLine.NumSteps -= Math.Abs(currentLine.IsBaseStepX ? xStep : yStep);
            if (currentLine.NumSteps >= 0f)
            {
                var candidateX = _translate.X + (xStep - currentLine.Compensate.x);
                var candidateY = _translate.Y + (yStep - currentLine.Compensate.y);
                if (currentWalk.WalkAnywhere || isWalkable(new Position(candidateX, candidateY)))
                {
                    _translate.Position = (candidateX, candidateY);
                }
                else
                {
                    onWalkLineCompleted(currentWalk, currentLine, false);
                    return;
                }
            }
            currentLine.Compensate = (0f, 0f);
        }

        private WalkInstruction getWalkInstruction()
        {
            WalkInstruction instruction = _currentWalk;
            while (_incomingInstructions.TryDequeue(out var nextInstruction))
            {
                if (instruction != null)
                {
                    onWalkLineCompleted(instruction, instruction.CurrentLine, false);
                }
                if (nextInstruction != null)
                {
                    instruction = nextInstruction;
                }
            }
            if (_currentWalk != instruction)
            {
                if (instruction.StopOnly)
                {
                    instruction.OnCompletion.TrySetResult(false);
                    _currentWalk = null;
                    return null;
                }
                var previousInstruction = _currentWalk;
                _currentWalk = instruction;
                processWalkInstruction(previousInstruction, instruction);
            }
            return instruction;
        }

        private void onWalkLineCompleted(WalkInstruction currentWalk, WalkLineInstruction currentLine, bool success)
        {
            currentWalk.CurrentLine = null;
            if (currentLine == null) return;
            currentLine.LastFrame = null;
            currentLine.OnCompletion.TrySetResult(success);
        }

        private float compensateForViewScrollIfNeeded(float currentViewport, float step, ref float compensateStep, ref float lastViewport)
        {
            if (currentViewport == lastViewport) return 0f;
            float smoothStep = step / (_animation.Animation.Configuration.DelayBetweenFrames + _animation.Animation.Frames[_animation.Animation.State.CurrentFrame].Delay);
            compensateStep += smoothStep;
            lastViewport = currentViewport;
            return smoothStep;
        }

        private async Task<bool> walkAsync(Position position, bool straightLine, bool walkAnywhere)
        {
            WalkDestination = position;
            List<IObject> debugRenderers = _debugPath;
            if (debugRenderers != null)
            {
                foreach (var renderer in debugRenderers)
                {
                    await renderer.ChangeRoomAsync(null);
                    renderer.Dispose();
                }
            }
            return await addNewInstructionAsync(position, walkAnywhere, straightLine, false);
        }

        private async void processWalkInstruction(WalkInstruction previousWalk, WalkInstruction currentWalk)
        {
            if (previousWalk != null)
            {
                await previousWalk.OnCompletion.Task;
            }
            var debugRenderers = DebugDrawWalkPath ? new List<IObject>() : null;
            _debugPath = debugRenderers;
            OnPropertyChanged(nameof(IsWalking));
            float xSource = _translate.X;
            float ySource = _translate.Y;
            bool completedWalk = false;
            try
            {
                completedWalk = await walkAsync(currentWalk, currentWalk.Destination, currentWalk.StraightLine, debugRenderers);
            }
            finally
            {
                _faceDirection.CurrentDirectionalAnimation = _outfit.Outfit[AGSOutfit.Idle];
                await _faceDirection.FaceDirectionAsync(_faceDirection.Direction);
                currentWalk.OnCompletion.TrySetResult(completedWalk);
                OnPropertyChanged(nameof(IsWalking));
            }
        }

        private async Task<bool> walkAsync(WalkInstruction currentWalk, Position location, bool straightLine, List<IObject> debugRenderers)
        {
            IEnumerable<Position> walkPoints = straightLine ? new List<Position> { location } : getWalkPoints(location);

            if (!walkPoints.Any())
                return false;
            foreach (var point in walkPoints)
            {
                if (point.X == _translate.X && point.Y == _translate.Y) continue;
                if (!await walkStraightLine(currentWalk, point, debugRenderers))
                    return false;
            }
            return true;
        }

        private async Task<bool> addNewInstructionAsync(Position destination, bool walkAnywhere, bool straightLine, bool stopOnly)
        {
            WalkInstruction newWalk = new WalkInstruction(_room?.Room, destination, walkAnywhere, straightLine, stopOnly);
            _incomingInstructions.Enqueue(newWalk);
            return await newWalk.OnCompletion.Task;
        }

        private PointF? getClosestWalkablePoint(PointF target)
        {
            var points = getClosestWalkablePoints(target);
            if (points.Count == 0) return null;
            return points[0];
        }

        private List<PointF> getClosestWalkablePoints(PointF target)
        {
            List<(PointF point, float distance)> points = new List<(PointF, float)>(_room.Room.Areas.Count);
            foreach (IArea area in getWalkableAreas())
            {
                float distance;
                PointF? point = area.FindClosestPoint(target, out distance);
                if (point == null) continue;
                points.Add((point.Value, distance));
            }
            return points.OrderBy(p => p.distance).Select(p => p.point).ToList();
        }

        private IEnumerable<Position> getWalkPoints(Position destination)
        {
            var from = _translate.Position;
            if (!isWalkable(from))
                return new List<Position>();
            List<PointF> closestPoints = new List<PointF>(_room.Room.Areas.Count + 1);
            if (isWalkable(destination))
            {
                closestPoints.Add(destination.XY);
            }

            closestPoints.AddRange(getClosestWalkablePoints(destination.XY));
            if (closestPoints.Count == 0)
                return new List<Position>();

            Point offset;
            bool[][] mask = getWalkableMask(out offset);
            from = new Position(from.X - offset.X, from.Y - offset.Y);
            _pathFinder.Init(mask);
            foreach (var closest in closestPoints)
            {
                destination = new Position(closest.X - offset.X, closest.Y - offset.Y);
                var walkPoints = _pathFinder.GetWalkPoints(from, destination);
                if (walkPoints.Any()) return walkPoints.Select(w => new Position(w.X + offset.X, w.Y + offset.Y));
            }
            return new List<Position>();
        }

        private bool isWalkable(Position location)
        {
            foreach (var area in getWalkableAreas())
            {
                if (area.IsInArea(location.XY)) return true;
            }
            return false;
        }

        private bool[][] getWalkableMask(out Point offset)
        {
            var walkables = getWalkableAreas().ToList();
            var minX = (int)walkables.Min(a => a.Mask.MinX);
            var maxX = (int)walkables.Max(a => a.Mask.MaxX);
            var minY = (int)walkables.Min(a => a.Mask.MinY);
            var maxY = (int)walkables.Max(a => a.Mask.MaxY);
            int width = maxX - minX;
            int height = maxY - minY;
            offset = new Point(minX, minY);
            bool[][] mask = new bool[width][];
            for (int i = 0; i < mask.Length; i++) mask[i] = new bool[height];
            foreach (var area in walkables)
            {
                area.Mask.ApplyToMask(mask, offset);
            }
            return mask;
        }

        private IEnumerable<IArea> getWalkableAreas()
        {
            var room = _room.Room;
            if (room == null) return Array.Empty<IArea>();
            return room.Areas.Where(area =>
            {
                if (!area.Enabled) return false;
                var walkable = area.GetComponent<IWalkableArea>();
                if (walkable == null || !walkable.IsWalkable) return false;
                var restrictionArea = area.GetComponent<IAreaRestriction>();
                return (restrictionArea == null || !restrictionArea.IsRestricted(Entity.ID));
            });
        }

        private async Task<bool> walkStraightLine(WalkInstruction currentWalk, Position destination, List<IObject> debugRenderers)
        {
            if (_room?.Room != currentWalk.Room) return false;

            if (debugRenderers != null)
            {
                IObject renderer = _objFactory.GetObject("Debug Line");
                var line = renderer.AddComponent<GLLineRenderer>();
                if (line != null)
                {
                    line.X1 = _translate.X;
                    line.Y1 = _translate.Y;
                    line.X2 = destination.X;
                    line.Y2 = destination.Y;
                }
                await renderer.ChangeRoomAsync(currentWalk.Room);
                debugRenderers.Add(renderer);
            }

            if (_cutscene.IsSkipping)
            {
                _translate.Position = destination;
                return true;
            }

            if (!isDistanceVeryShort(destination))
            {
                var lastDirection = _faceDirection.Direction;
                var walkAnimation = _outfit.Outfit[AGSOutfit.Walk];
                bool alreadyWalking = _faceDirection.CurrentDirectionalAnimation == walkAnimation;
                _faceDirection.CurrentDirectionalAnimation = walkAnimation;
                await _faceDirection.FaceDirectionAsync(_translate.X, _translate.Y, destination.X, destination.Y);
                if (lastDirection != _faceDirection.Direction && alreadyWalking)
                {
                    await Task.Delay(200);
                }
            }

            float xSteps = Math.Abs(destination.X - _translate.X);
            float ySteps = Math.Abs(destination.Y - _translate.Y);

            float numSteps = Math.Max(xSteps, ySteps);
            bool isBaseStepX = xSteps >= ySteps;

            float xStep = xSteps / numSteps;
            if (_translate.X > destination.X) xStep = -xStep;

            float yStep = ySteps / numSteps;
            if (_translate.Y > destination.Y) yStep = -yStep;

            WalkLineInstruction instruction = new WalkLineInstruction(numSteps, xStep, yStep,
                                                                      isBaseStepX, destination);
            currentWalk.CurrentLine = instruction;
            if (_currentWalk != currentWalk)
            {
                onWalkLineCompleted(currentWalk, instruction, false);
                currentWalk.CancelToken.Cancel();
                return false;
            }
            Task timeout = Task.Delay(WalkLineTimeoutInMilliseconds);
            Task completedTask = await Task.WhenAny(instruction.OnCompletion.Task, currentWalk.OnCompletion.Task, timeout);

            if (completedTask == timeout)
            {
                currentWalk.CancelToken.Cancel();
                return false;
            }

            if (completedTask == currentWalk.OnCompletion.Task)
            {
                return false;
            }

            if (!instruction.OnCompletion.Task.Result || currentWalk.CancelToken.IsCancellationRequested ||
                _room?.Room != currentWalk.Room || (!currentWalk.WalkAnywhere && !isWalkable(_translate.Position)))
            {
                return false;
            }

            if (currentWalk.WalkAnywhere || isWalkable(destination))
            {
                _translate.Position = destination;
                return true;
            }

            return false;
        }

        private bool isDistanceVeryShort(Position destination)
        {
            var deltaX = destination.X - _translate.X;
            var deltaY = destination.Y - _translate.Y;
            return (deltaX * deltaX) + (deltaY * deltaY) < 10;
        }

        private PointF adjustWalkSpeed(PointF walkSpeed)
        {
            walkSpeed = adjustWalkSpeedBasedOnArea(walkSpeed);
            return walkSpeed;
        }

        private PointF adjustWalkSpeedBasedOnArea(PointF walkSpeed)
        {
            if (_room?.Room?.Areas == null || _drawable.IgnoreScalingArea || !AdjustWalkSpeedToScaleArea)
                return walkSpeed;

            foreach (var area in _room.Room.Areas)
            {
                if (!area.Enabled || !area.IsInArea(_translate.Position.XY)) continue;
                var scalingArea = area.GetComponent<IScalingArea>();
                if (scalingArea == null || (!scalingArea.ScaleObjectsX && !scalingArea.ScaleObjectsY)) continue;
                float scale = scalingArea.GetScaling(scalingArea.Axis == ScalingAxis.X ? _translate.X : _translate.Y);
                if (scale != 1f)
                {
                    walkSpeed = new PointF(walkSpeed.X * (scalingArea.ScaleObjectsX ? scale : 1f),
                                           walkSpeed.Y * (scalingArea.ScaleObjectsY ? scale : 1f));
                    if (walkSpeed.X == 0f || walkSpeed.Y == 0f)
                    {
                        walkSpeed = new PointF(walkSpeed.X == 0f ? 1f : walkSpeed.X,
                                               walkSpeed.Y == 0f ? 1f : walkSpeed.Y);
                    }
                }
                break;
            }
            return walkSpeed;
        }

        private class WalkInstruction
        {
            public WalkInstruction(IRoom room, Position destination, bool walkAnywhere, bool straightLine, bool stopOnly)
            {
                Destination = destination;
                StraightLine = straightLine;
                StopOnly = stopOnly;
                CancelToken = new CancellationTokenSource();
                OnCompletion = new TaskCompletionSource<bool>();
                Room = room;
                WalkAnywhere = walkAnywhere;
            }

            public Position Destination { get; }
            public CancellationTokenSource CancelToken { get; }
            public TaskCompletionSource<bool> OnCompletion { get; }
            public IRoom Room { get; }
            public bool StopOnly { get; }
            public bool WalkAnywhere { get; }
            public bool StraightLine { get; }
            public WalkLineInstruction CurrentLine { get; set; }
        }

        private class WalkLineInstruction
        {
            public WalkLineInstruction(float numSteps, float xStep, float yStep,
                                       bool isBaseStepX, Position destination)
            {
                NumSteps = numSteps;
                XStep = xStep;
                YStep = yStep;
                IsBaseStepX = isBaseStepX;
                Destination = destination;
                OnCompletion = new TaskCompletionSource<bool>();
            }

            public TaskCompletionSource<bool> OnCompletion { get; }
            public float NumSteps { get; set; }
            public float XStep { get; }
            public float YStep { get; }
            public bool IsBaseStepX { get; }
            public Position Destination { get; }

            public (float x, float y) Compensate { get; set; }
            public (float x, float y) Viewport { get; set; }
            public ISprite LastFrame { get; set; }
        }
    }
}