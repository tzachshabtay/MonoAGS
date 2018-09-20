using System.ComponentModel;
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

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        #endregion

        public override string ToString() => Enabled ? $"{Value:0.##}" : "N/A";
    }
}

