namespace AGS.Editor
{
    public class InspectorCategory
    {
        public InspectorCategory(string name, int z = 0, bool expand = false)
        {
            Name = name;
            Z = z;
            Expand = expand;
        }

        public string Name { get; }

        public int Z { get; }

        public bool Expand { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as InspectorCategory);
        }

        public bool Equals(InspectorCategory cat)
        {
            if (cat == null) return false;
            return Name == cat.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString() => Name;
    }
}
