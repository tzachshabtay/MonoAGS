using AGS.API;

namespace AGS.Engine
{
    public class AGSAreaRestriction : AGSComponent, IAreaRestriction
    {
        private IRestrictionList _list;

        public AGSAreaRestriction(IRestrictionList list)
        {
            _list = list;
        }

        public IConcurrentHashSet<string> RestrictionList => _list.RestrictionList;

        public RestrictionListType RestrictionType { get => _list.RestrictionType; set => _list.RestrictionType = value; }

        public bool IsRestricted(string id)
        {
            return _list.IsRestricted(id);
        }
    }
}
