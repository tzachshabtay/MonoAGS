using System;
using System.Reflection;
using AGS.API;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class EditorProvider
    {
        private readonly IGameFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private readonly IGameState _state;
        private readonly IGameSettings _settings;
        private readonly AGSEditor _editor;
        private readonly IForm _parentForm;

        public EditorProvider(IGameFactory factory, ActionManager actions, StateModel model, IGameState state, 
                              IGameSettings settings, AGSEditor editor, IForm parentForm)
        {
            _parentForm = parentForm;
            _editor = editor;
            _factory = factory;
            _actions = actions;
            _model = model;
            _state = state;
            _settings = settings;
        }

        public IInspectorPropertyEditor GetEditor(Type propType, IEntity entity, Action refreshNode)
        {
            if (propType == typeof(bool)) return new BoolPropertyEditor(_factory, _actions, _model);
            if (propType == typeof(Color)) return new ColorPropertyEditor(_factory, _actions, _model);
            if (propType == typeof(int)) return new NumberPropertyEditor(_actions, _state, _factory, _model, true, false);
            if (propType == typeof(float)) return new NumberPropertyEditor(_actions, _state, _factory, _model, false, false);
            if (propType == typeof(SizeF)) return new SizeFPropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Size)) return new SizePropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(PointF)) return new PointFPropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Point)) return new PointPropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Vector2)) return new Vector2PropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Vector3)) return new Vector3PropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Vector4)) return new Vector4PropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Position))
            {
                var drawable = entity == null ? null : entity.GetComponent<IDrawableInfoComponent>();
                return new LocationPropertyEditor(_actions, _state, _factory, _model, false, _settings, drawable);
            }
            if (propType == typeof(RectangleF)) return new RectangleFPropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(Rectangle)) return new RectanglePropertyEditor(_actions, _state, _factory, _model, false);
            if (propType == typeof(int?)) return new NumberPropertyEditor(_actions, _state, _factory, _model, true, true);
            if (propType == typeof(float?)) return new NumberPropertyEditor(_actions, _state, _factory, _model, false, true);
            if (propType == typeof(SizeF?)) return new SizeFPropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(Size?)) return new SizePropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(PointF?)) return new PointFPropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(Point?)) return new PointPropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(Vector2?)) return new Vector2PropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(Vector3?)) return new Vector3PropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(Vector4?)) return new Vector4PropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(RectangleF?)) return new RectangleFPropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(Rectangle?)) return new RectanglePropertyEditor(_actions, _state, _factory, _model, true);
            if (propType == typeof(string)) return new StringPropertyEditor(_factory, true, _actions, _model);
            if (propType.IsEnum) return new EnumPropertyEditor(_factory.UI, _actions, _model);
            if (propType.IsInterface || propType.IsClass)
            {
                return new InstancePropertyEditor(_factory.UI, _actions, _model, _editor, _parentForm, refreshNode);
            }
            return new StringPropertyEditor(_factory, false, _actions, _model);
        }
    }
}