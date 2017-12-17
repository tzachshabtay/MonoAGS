using AGS.API;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSAnimationState : IAnimationState
	{
		public AGSAnimationState ()
		{
			OnAnimationCompleted = new TaskCompletionSource<AnimationCompletedEventArgs> ();
		}

		#region IAnimationState implementation

		public bool RunningBackwards { get; set; }

		public int CurrentFrame { get; set; }

		public int CurrentLoop { get; set; }

		public int TimeToNextFrame { get; set; }

		public bool IsPaused { get; set; }

        [Property(Browsable = false)]
		public TaskCompletionSource<AnimationCompletedEventArgs> OnAnimationCompleted { get; private set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        #endregion
    }
}

