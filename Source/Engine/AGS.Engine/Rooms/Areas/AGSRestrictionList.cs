using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSRestrictionList : IRestrictionList
    {
		public AGSRestrictionList()
		{
			RestrictionList = new AGSConcurrentHashSet<string>();
		}

		public IConcurrentHashSet<string> RestrictionList { get; private set; }

		public RestrictionListType RestrictionType { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public bool IsRestricted(string id)
		{
			if (id == null || RestrictionList.Count == 0) return false;
			return RestrictionType == RestrictionListType.BlackList ?
						 RestrictionList.Contains(id) : !RestrictionList.Contains(id);
		}
    }
}
