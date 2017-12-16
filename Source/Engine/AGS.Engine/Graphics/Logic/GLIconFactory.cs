using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class GLIconFactory : IIconFactory
    {
        private readonly Resolver _resolver;

        public GLIconFactory(Resolver resolver)
        {
            _resolver = resolver;
        }

        public IBorderStyle GetArrowIcon(ArrowDirection direction, Color? color = default)
        {
            TypedParameter dirParam = new TypedParameter(typeof(ArrowDirection), direction);
            TypedParameter colorParam = new TypedParameter(typeof(Color?), color);
            return _resolver.Container.Resolve<ArrowIcon>(dirParam, colorParam);
        }

        public ISelectableIcon GetFileIcon(bool isSelected = false, Color? color = default, Color? foldColor = default, Color? selectedColor = default, Color? selectedFoldColor = default)
        {
            NamedParameter colorParam = new NamedParameter(nameof(color), color);
            NamedParameter foldColorParam = new NamedParameter(nameof(foldColor), foldColor);
            NamedParameter selectedColorParam = new NamedParameter(nameof(selectedColor), selectedColor);
            NamedParameter selectedFoldColorParam = new NamedParameter(nameof(selectedFoldColor), selectedFoldColor);
            var icon = _resolver.Container.Resolve<FileIcon>(colorParam, foldColorParam, selectedColorParam, selectedFoldColorParam);
            icon.IsSelected = isSelected;
            return icon;
        }

        public ISelectableIcon GetFolderIcon(bool isSelected = false, Color? color = default, Color? foldColor = default, Color? selectedColor = default, Color? selectedFoldColor = default)
        {
            NamedParameter colorParam = new NamedParameter(nameof(color), color);
            NamedParameter foldColorParam = new NamedParameter(nameof(foldColor), foldColor);
            NamedParameter selectedColorParam = new NamedParameter(nameof(selectedColor), selectedColor);
            NamedParameter selectedFoldColorParam = new NamedParameter(nameof(selectedFoldColor), selectedFoldColor);
            var icon = _resolver.Container.Resolve<FolderIcon>(colorParam, foldColorParam, selectedColorParam, selectedFoldColorParam);
            icon.IsSelected = isSelected;
            return icon;
        }

        public IBorderStyle GetXIcon(float lineWidth = 3, float padding = 3, Color? color = default)
        {
            TypedParameter colorParam = new TypedParameter(typeof(Color?), color);
            NamedParameter lineWidthParam = new NamedParameter(nameof(lineWidth), lineWidth);
            NamedParameter paddingParam = new NamedParameter(nameof(padding), padding);
            return _resolver.Container.Resolve<XIcon>(colorParam, lineWidthParam, paddingParam);
        }
    }
}
