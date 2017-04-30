namespace AGS.API
{
    /// <summary>
    /// The entry point for getting input from the user (keyboard/mouse/touch).
    /// </summary>
    public interface IInput : IInputEvents
	{
        /// <summary>
        /// Gets the mouse position in the room's co-ordinates (based on the game's virtual resolution), 
        /// after adjusting for the viewport.
        /// </summary>
        /// <value>The mouse position.</value>
		PointF MousePosition { get; }

        /// <summary>
        /// Gets the mouse's x position in the room's co-ordinates (based on the game's virtual resolution), 
        /// after adjusting for the viewport.
        /// </summary>
        /// <value>The mouse x.</value>
		float MouseX { get; }

        /// <summary>
        /// Gets the mouse's y position in the room's co-ordinates (based on the game's virtual resolution), 
        /// after adjusting for the viewport.
        /// </summary>
        /// <value>The mouse y.</value>
		float MouseY { get; }

        /// <summary>
        /// Is the left mouse button currently pressed down?
        /// </summary>
        /// <value><c>true</c> if left mouse button down; otherwise, <c>false</c>.</value>
		bool LeftMouseButtonDown { get; }

        /// <summary>
        /// Is the right mouse button currently pressed down?
        /// </summary>
        /// <value><c>true</c> if right mouse button down; otherwise, <c>false</c>.</value>
		bool RightMouseButtonDown { get; }

        /// <summary>
        /// Is the user currently touching and dragging (for touch screens)?
        /// </summary>
        /// <value><c>true</c> if is touch drag; otherwise, <c>false</c>.</value>
        bool IsTouchDrag { get; }

        /// <summary>
        /// Gets or sets the mouse cursor.
        /// </summary>
        /// <value>The cursor.</value>
		IObject Cursor { get; set; }

        /// <summary>
        /// Is the specified key on the keyboard currently pressed down?
        /// </summary>
        /// <returns><c>true</c>, if key down was ised, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        bool IsKeyDown(Key key);
	}
}

