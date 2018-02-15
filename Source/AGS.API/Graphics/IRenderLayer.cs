namespace AGS.API
{
    /// <summary>
    /// A render layer is an abstract grouping of objects with similar rendering properties.
    /// A render layer has a "Z" property for sorting which takes precedence over specific objects "Z" property.
    /// This means that if you have a UI render layer which is configured to be in front of a Background render layer,
    /// all objects which are associated with the UI layer will always be in front of the objects that are associated
    /// with the background layer.
    /// In addition, render layers add support for assigning different parallax speeds, to create beautiful parallax effects.
    /// 
    /// AGS comes with a few built in layers that are used by default (Background, Foreground, UI and Speech).
    /// Speech layer is in front to ensure that displayed text can always be seen. This is the default layer used for Character.Say.
    /// UI layer is next, to ensure that it can always be seen (unless obstructed by text). This is the default layer used for all UI controls.
    /// This is followed by the Foreground layer, which is the default for all non UI objects.
    /// And lastly the Background layer, which is the default for the room's background graphics.
    /// You can assign those layers yourself, using AGSLayers static class:    
    /// <example>
    /// <code language="lang-csharp">
    /// cEgo.RenderLayer = AGSLayers.UI; //The player is now on the UI layer!
    /// </code>
    /// </example>
    /// </summary>
    public interface IRenderLayer
	{
        /// <summary>
        /// Gets the Z ordering for the layer. The smaller the number, the more visible the layer gets.
        /// <example>
        /// <code language="lang-csharp">
        /// Debug.WriteLine("Z value for the background layer is: " + AGSLayers.Background.Z);
        /// </code>
        /// </example>
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        int Z { get; }

        /// <summary>
        /// Gets the parallax speed associated with the layer (https://en.wikipedia.org/wiki/Parallax).
        /// The speed is the factor by which the location is changed based to the normal speed.
        /// Giving different speeds for different layers creates the parallax effect which gives the illusion of 3D depth.
        /// This is mostly effective in scrolling screens, as the objects move in relation to the camera.
        /// If the room scrolls horizontally (the majority of scrolling rooms), you'd like to change the X value of the parallax speed.
        /// If the room scrolls vertically (rare but can happen), you'd like to change the Y value of the parallax speed.
        /// All the default layers have parallax speed of 1 (i.e normal speed), so you'll need to create your own layers to enable the parallax effect.
        /// The closer the object is to the front, the faster it should go, so it should have a bigger parallax speed.       
        /// Objects in the foreground should have values larger than 1, and objects in the background should have values smaller than 1.         
        /// <example>
        /// <code language="lang-csharp">
        /// PointF parallaxSpeed = new PointF (1.4f, 1f); //all objects in this layer will move at a faster horizontal speed than the rest of the objects.
        /// AGSRenderLayer parallaxForegroundLayer = new AGSRenderLayer(-50, parallaxSpeed); //This layer is positioned before the foreground layer and behind the UI layer.
        /// 
        /// myObjInFront.RenderLayer = parallaxForegroundLayer;
        /// </code>
        /// </example>
        /// </summary>
        /// <value>
        /// The parallax speed.
        /// </value>
        PointF ParallaxSpeed { get; }

        /// <summary>
        /// A custom virtual resolution that will be used for all objects in this layer, different than the default
        /// game's virtual resolution.
        /// Null means that the layer does not have a custom resolution and uses the game's resolution.
        /// </summary>
        /// <example>
        /// Let's assume the game resolution is (320,200):
        /// The character is using the game's resolution, but the house in the room is in a layer with a (640,400) resolution.
        /// This means that if the character's position is (100,100) and the house's position is (200,200) they will 
        /// actually be drawn in the same location.
        /// </example>
        /// <value>The independent resolution.</value>
        Size? IndependentResolution { get; }
	}
}

