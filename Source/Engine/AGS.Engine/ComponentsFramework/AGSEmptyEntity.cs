namespace AGS.Engine
{
    public class AGSEmptyEntity : AGSEntity
    {
        public AGSEmptyEntity(string id, Resolver resolver) : base(id, resolver)
        {
            InitComponents();
        }
    }
}
