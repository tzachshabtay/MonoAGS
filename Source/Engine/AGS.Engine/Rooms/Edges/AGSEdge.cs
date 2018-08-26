using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSEdge : IEdge
	{
		public AGSEdge()
		{
			OnEdgeCrossed = new AGSEvent ();
            Enabled = true;
		}

		#region IEdge implementation

		public float Value { get; set; }

        public IBlockingEvent OnEdgeCrossed { get; private set; }

        public bool Enabled { get; set; }

        #endregion

        public override string ToString() => Enabled ? $"{Value:0.##}" : "N/A";
    }
}

