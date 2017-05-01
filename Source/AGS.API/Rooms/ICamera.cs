using System;

namespace AGS.API
{
    /// <summary>
    /// The camera is a script that is executed each game tick, and manipulates the viewport.
    /// 
    /// The default camera tracks the target (usually the player) by slowly moving the viewport (in a smooth transition) 
    /// so the target is at the center, while also adjusting the zoom if the target is standing in a <see cref="IZoomArea"/>.
    /// 
    /// You can however implement your own camera, where each room can have a different camera if desired.
    /// </summary>
	public interface ICamera
	{
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ICamera"/> is enabled.
        /// If the camera is disabled then the viewport will not be updated by the camera, you can still update
        /// the viewport manually.
        /// If the camera is enabled and you attempt to set the viewport manually, the camera (depending on the implementation,
        /// but this is true for the default camera) will slowly change the viewport to center the target.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the target which the camera will be following.
        /// </summary>
        /// <value>The target.</value>
		Func<IObject> Target { get; set; }

        /// <summary>
        /// Executes the next tick. This is called on each tick by the engine, you don't need to call it yourself. 
        /// </summary>
        /// <param name="viewport">The viewport to manipulate.</param>
        /// <param name="roomSize">Room size.</param>
        /// <param name="virtualResolution">The game's virtual resolution.</param>
        /// <param name="resetPosition">If set to <c>true</c> reset position, i.e the camera will not do a smooth transition,
        /// but go straight to the target destination.</param>
		void Tick(IViewport viewport, Size roomSize, Size virtualResolution, bool resetPosition);
	}
}

