namespace AGS.API
{
    /// <summary>
    /// A sound emitter allows to bind an audio clip to an entity which is moving on a screen.
    /// Playing a sound from the emitter will pan it or set the volume (both can be turned off) to sound like
    /// it coming from the same location the entity is in.
    /// You can also assign the emitter to automatically play the sound on specific animation frames (so it's a 
    /// perfect fit for footsteps, for example).
    /// </summary>
    /// <example>
    /// <code language="lang-csharp">
    /// var footstep = await game.Factory.Sound.LoadAudioClipAsync("Sounds/footstep.ogg");
    /// ISoundEmitter emitter = new AGSSoundEmitter(game);
    /// emitter.AudioClip = footstep;
    /// emitter.Object = cHero;
    /// emitter.Assign(cHero.Outfit[AGSOutfit.Walk], 1, 5); //will play the footstep on frames 1 and 5 of the walking animation
    /// </code>
    /// </example>
	public interface ISoundEmitter : ISoundPlayer
	{
        /// <summary>
        /// Gets or sets the audio clip that is bound to the emitter.
        /// </summary>
        /// <value>The audio clip.</value>
		IAudioClip AudioClip { get; set; }
		
        /// <summary>
        /// Sets the object which is bound to the emitter. This is a short-hand to set
        /// the translate, room and entity id components.
        /// </summary>
        /// <value>The object.</value>
        IObject Object { set; }

        /// <summary>
        /// Gets or sets the world position component (for binding to location).
        /// You can set the Object property instead a short-hand convience. 
        /// </summary>
        /// <value>The translate.</value>
        IWorldPositionComponent WorldPosition { get; set; }

        /// <summary>
        /// Gets or sets the room component, for the emitter to not play the sound if the entity is not in currently rendered room.
        /// You can set the Object property instead a short-hand convience.
        /// </summary>
        /// <value>The has room.</value>
        IHasRoomComponent HasRoom { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier. This is used by the emitter to check if the entity is in a volume altering area,
        /// to change the volume of the sound accordingly.
        /// You can set the Object property instead a short-hand convience.
        /// </summary>
        /// <value>The entity identifier.</value>
        string EntityID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISoundEmitter"/> auto pans the sound
        /// based on the location (panning is the act of distributing the sound across left/right speakers to make the 
        /// sound appear more to the left/right). This is on by default.
        /// </summary>
        /// <value><c>true</c> if auto pan; otherwise, <c>false</c>.</value>
		bool AutoPan { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISoundEmitter"/> auto adjusts volume
        /// when the bound entity is standing on a volume changing area. This is on by default.
        /// </summary>
        /// <value><c>true</c> if auto adjust volume; otherwise, <c>false</c>.</value>
		bool AutoAdjustVolume { get; set; }

        /// <summary>
        /// Assign the specified animation frames (in all directions) to play the sound whenever the frames are showing.
        /// This assumes all animations in all directions have matching frames, otherwise use the more specific overloads.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="frames">The frame numbers (where 0 is the first frame).</param>
		void Assign(IDirectionalAnimation animation, params int[] frames);

        /// <summary>
        /// Assign the specified animation frames to play the sound whenever the frames are showing.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="frames">The frame numbers (where 0 is the first frame).</param>
		void Assign(IAnimation animation, params int[] frames);

        /// <summary>
        /// Assign the specified animation frames to play the sound whenever the frames are showing.
        /// </summary>
        /// <param name="frames">Frames.</param>
		void Assign(params IAnimationFrame[] frames);
	}
}

