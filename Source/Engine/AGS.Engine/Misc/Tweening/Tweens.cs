using System;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
	public static class Tweens
	{
        /// <summary>
        /// Tweens the x position.
        /// <example>
        /// <code>
        /// var tween = obj.TweenX(200f, 2f, Ease.QuadIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ITranslate.X"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toX">To x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenX(this ITranslate sprite, float toX, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.X, toX, x => sprite.X = x, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the y position.
        /// <example>
        /// <code>
        /// var tween = obj.TweenY(200f, 2f, Ease.QuadIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ITranslate.Y"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toY">To Y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenY(this ITranslate sprite, float toY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Y, toY, y => sprite.Y = y, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the z position.
        /// <example>
        /// <code>
        /// var tween = obj.TweenZ(200f, 2f, Ease.QuadIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ITranslate.Z"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toZ">To Z.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenZ(this ITranslate sprite, float toZ, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Z, toZ, z => sprite.Z = z, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the angle to rotate.
        /// <example>
        /// <code>
        /// var tween = obj.TweenAngle(45f, 2f, Ease.Linear);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IRotate.Angle"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toAngle">To angle.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenAngle(this IRotate sprite, float toAngle, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Angle, toAngle, a => sprite.Angle = a, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the opacity of the image.
        /// <example>
        /// <code>
        /// var tween = obj.TweenAngle(0, 2f, Ease.Linear); //Fade out
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Opacity"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toOpacity">To opacity.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenOpacity(this IHasImage sprite, byte toOpacity, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Opacity, (float)toOpacity, o => sprite.Opacity = (byte)o, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the red tint of the image.
        /// <example>
        /// <code>
        /// var tween = obj.TweenRed(255, 2f, Ease.QuadIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Tint"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toRed">To red.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenRed(this IHasImage sprite, byte toRed, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Tint.R, (float)toRed, r => sprite.Tint = Color.FromArgb(sprite.Tint.A, (byte)r,
				sprite.Tint.G, sprite.Tint.B), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the green tint of the image.
        /// <example>
        /// <code>
        /// var tween = obj.TweenGreen(255, 2f, Ease.QuadIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Tint"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toGreen">To green.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenGreen(this IHasImage sprite, byte toGreen, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Tint.G, (float)toGreen, g => sprite.Tint = Color.FromArgb(sprite.Tint.A, sprite.Tint.R,
				(byte)g, sprite.Tint.B), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the blue tint of the image.
        /// <example>
        /// <code>
        /// var tween = obj.TweenBlue(255, 2f, Ease.QuadIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Tint"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toBlue">To blue.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenBlue(this IHasImage sprite, byte toBlue, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Tint.B, (float)toBlue, b => sprite.Tint = Color.FromArgb(sprite.Tint.A, sprite.Tint.R,
				sprite.Tint.G, (byte)b), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the hue of the image (based on HSLA color scheme).
        /// <example>
        /// <code>
        /// var tween = obj.TweenHue(0, 2f, Ease.QuadOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Color.FromHsla"/>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Tint"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toHue">To hue.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenHue(this IHasImage sprite, int toHue, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run((float)sprite.Tint.GetHue(), (float)toHue, h => sprite.Tint = Color.FromHsla((int)h, sprite.Tint.GetSaturation(),
				sprite.Tint.GetLightness(), sprite.Tint.A), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the saturation of the image (based on HSLA color scheme).
        /// <example>
        /// <code>
        /// var tween = obj.TweenSaturation(0f, 2f, Ease.QuadOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Color.FromHsla"/>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Tint"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toSaturation">To saturation.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenSaturation(this IHasImage sprite, float toSaturation, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Tint.GetSaturation(), toSaturation, s => sprite.Tint = Color.FromHsla(sprite.Tint.GetHue(), s,
				sprite.Tint.GetLightness(), sprite.Tint.A), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the lightness of the image (based on HSLA color scheme).
        /// <example>
        /// <code>
        /// var tween = obj.TweenLightness(0f, 2f, Ease.QuadOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Color.FromHsla"/>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Tint"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toLightness">To lightness.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenLightness(this IHasImage sprite, float toLightness, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Tint.GetLightness(), toLightness, l => sprite.Tint = Color.FromHsla(sprite.Tint.GetHue(), 
				sprite.Tint.GetSaturation(), l, sprite.Tint.A), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the anchor (also called "pivot") point (its x position) of an image.
        /// <example>
        /// <code>
        /// var tween = obj.TweenAnchorX(0.5f, 2f, Ease.SineIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Anchor"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toAnchorX">To anchor x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenAnchorX(this IHasImage sprite, float toAnchorX, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Anchor.X, toAnchorX, x => sprite.Anchor = new PointF (x, sprite.Anchor.Y), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the anchor (also called "pivot") point (its y position) of an image.
        /// <example>
        /// <code>
        /// var tween = obj.TweenAnchorY(0.5f, 2f, Ease.SineIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IHasImage.Anchor"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toAnchorY">To anchor y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenAnchorY(this IHasImage sprite, float toAnchorY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Anchor.Y, toAnchorY, y => sprite.Anchor = new PointF (sprite.Anchor.X, y), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the horizontal scaling of an object.
        /// <example>
        /// <code>
        /// var tween = obj.TweenScaleX(0.5f, 2f, Ease.SineInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IScale.ScaleX"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toScaleX">To scale x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenScaleX(this IScale sprite, float toScaleX, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.ScaleX, toScaleX, x => sprite.ScaleBy(x, sprite.ScaleY), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the vertical scaling of an object.
        /// <example>
        /// <code>
        /// var tween = obj.TweenScaleY(0.5f, 2f, Ease.SineInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IScale.ScaleY"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toScaleY">To scale y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenScaleY(this IScale sprite, float toScaleY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.ScaleY, toScaleY, y => sprite.ScaleBy(sprite.ScaleX, y), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the width of an object.
        /// <example>
        /// <code>
        /// var tween = obj.TweenWidth(100f, 2f, Ease.SineInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IScale.Width"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toWidth">To width.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenWidth(this IScale sprite, float toWidth, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Width, toWidth, w => sprite.ScaleTo(w, sprite.Height), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the height of an object.
        /// <example>
        /// <code>
        /// var tween = obj.TweenHeight(100f, 2f, Ease.SineInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IScale.Height"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="toHeight">To height.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenHeight(this IScale sprite, float toHeight, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sprite.Height, toHeight, h => sprite.ScaleTo(sprite.Width, h), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the x position of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenX(100f, 2f, Ease.BounceIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.X"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toX">To x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenX(this IViewport viewport, float toX, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.X, toX, x => viewport.X = x, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the y position of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenY(100f, 2f, Ease.BounceIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.Y"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toY">To y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenY(this IViewport viewport, float toY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.Y, toY, y => viewport.Y = y, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the horizontal zoom of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenScaleX(4f, 2f, Ease.ElasticOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.ScaleX"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toScaleX">To scale x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenScaleX(this IViewport viewport, float toScaleX, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.ScaleX, toScaleX, x => viewport.ScaleX = x, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the vertical zoom of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenScaleY(4f, 2f, Ease.ElasticOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.ScaleY"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toScaleY">To scale y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenScaleY(this IViewport viewport, float toScaleY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.ScaleY, toScaleY, y => viewport.ScaleY = y, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the projected x position of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenProjectX(100f, 2f, Ease.ElasticInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.ProjectionBox"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toProjectX">To project x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenProjectX(this IViewport viewport, float toProjectX, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(viewport.ProjectionBox.X, toProjectX,
                             x => viewport.ProjectionBox = new RectangleF(x, viewport.ProjectionBox.Y, viewport.ProjectionBox.Width, viewport.ProjectionBox.Height),
                             timeInSeconds, easing);
        }

        /// <summary>
        /// Tweens the projected y position of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenProjectY(100f, 2f, Ease.ElasticInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.ProjectionBox"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toProjectY">To project y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenProjectY(this IViewport viewport, float toProjectY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.ProjectionBox.Y, toProjectY,
                             y => viewport.ProjectionBox = new RectangleF(viewport.ProjectionBox.X, y, viewport.ProjectionBox.Width, viewport.ProjectionBox.Height),
							 timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the projected width of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenProjectWidth(100f, 2f, Ease.ElasticInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.ProjectionBox"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toProjectWidth">To project width.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenProjectWidth(this IViewport viewport, float toProjectWidth, float timeInSeconds, Func<float, float> easing = null)
		{
            return Tween.Run(viewport.ProjectionBox.Width, toProjectWidth,
                             width => viewport.ProjectionBox = new RectangleF(viewport.ProjectionBox.X, viewport.ProjectionBox.Y, width, viewport.ProjectionBox.Height),
							 timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the projected height of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenProjectHeight(100f, 2f, Ease.ElasticInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.ProjectionBox"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toProjectHeight">To project height.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenProjectHeight(this IViewport viewport, float toProjectHeight, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.ProjectionBox.Width, toProjectHeight,
                             height => viewport.ProjectionBox = new RectangleF(viewport.ProjectionBox.X, viewport.ProjectionBox.Y, viewport.ProjectionBox.Width, height),
							 timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the rotated angle of a viewport.
        /// <example>
        /// <code>
        /// var tween = viewport.TweenAngle(45f, 2f, Ease.ElasticInOut);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="IViewport.Angle"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="viewport">viewport.</param>
        /// <param name="toAngle">To angle.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenAngle(this IViewport viewport, float toAngle, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(viewport.Angle, toAngle, a => viewport.Angle = a, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the volume of a sound.
        /// <example>
        /// <code>
        /// var tween = sound.TweenVolume(0f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ISoundProperties.Volume"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sound">sound.</param>
        /// <param name="toVolume">To volume.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenVolume(this ISound sound, float toVolume, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sound.Volume, toVolume, v => sound.Volume = v, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the pitch of a sound.
        /// <example>
        /// <code>
        /// var tween = sound.TweenPitch(0f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ISoundProperties.Pitch"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sound">sound.</param>
        /// <param name="toPitch">To pitch.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenPitch(this ISound sound, float toPitch, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(sound.Pitch, toPitch, p => sound.Pitch = p, timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the panning of a sound.
        /// Note: Panning can only work on Mono sounds (it will not work for Stereo sounds).
        /// <example>
        /// <code>
        /// var tween = sound.TweenPanning(-1f, 2f, Ease.CubeIn); //Pan the sound to the left speaker
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ISoundProperties.Panning"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="sound">sound.</param>
        /// <param name="toPanning">To panning.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenPanning(this ISound sound, float toPanning, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(sound.Panning, toPanning, p => sound.Panning = p, timeInSeconds, easing);
        }

        /// <summary>
        /// Tweens the x offset of a texture.
        /// <example>
        /// <code>
        /// var tween = texture.TweenX(0.5f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ITextureOffsetComponent"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="offset">Texture offset component.</param>
        /// <param name="toX">To x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenX(this ITextureOffsetComponent offset, float toX, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(offset.TextureOffset.X, toX, x => offset.TextureOffset = new PointF(x, offset.TextureOffset.Y), timeInSeconds, easing);
        }

        /// <summary>
        /// Tweens the y offset of a texture.
        /// <example>
        /// <code>
        /// var tween = texture.TweenY(0.5f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ITextureOffsetComponent"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="offset">Texture offset component.</param>
        /// <param name="toY">To y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
		public static Tween TweenY(this ITextureOffsetComponent offset, float toY, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(offset.TextureOffset.Y, toY, y => offset.TextureOffset = new PointF(offset.TextureOffset.X, y), timeInSeconds, easing);
		}

        /// <summary>
        /// Tweens the x offset (the left edge) of the crop area.
        /// <example>
        /// <code>
        /// var tween = crop.TweenX(15f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ICropSelfComponent"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="crop">Crop component.</param>
        /// <param name="toX">To x.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenX(this ICropSelfComponent crop, float toX, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(crop.CropArea.X, toX, x => crop.CropArea = new RectangleF(x, crop.CropArea.Y, crop.CropArea.Width, crop.CropArea.Height), timeInSeconds, easing);
        }

        /// <summary>
        /// Tweens the y offset (the bottom edge) of the crop area.
        /// <example>
        /// <code>
        /// var tween = crop.TweenX(15f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ICropSelfComponent"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="crop">Crop component.</param>
        /// <param name="toY">To y.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenY(this ICropSelfComponent crop, float toY, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(crop.CropArea.Y, toY, y => crop.CropArea = new RectangleF(crop.CropArea.X, y, crop.CropArea.Width, crop.CropArea.Height), timeInSeconds, easing);
        }

        /// <summary>
        /// Tweens the width of the crop area.
        /// <example>
        /// <code>
        /// var tween = crop.TweenWidth(15f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ICropSelfComponent"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="crop">Crop component.</param>
        /// <param name="toWidth">To width.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenWidth(this ICropSelfComponent crop, float toWidth, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(crop.CropArea.Width, toWidth, width => crop.CropArea = new RectangleF(crop.CropArea.X, crop.CropArea.Y, width, crop.CropArea.Height), timeInSeconds, easing);
        }

        /// <summary>
        /// Tweens the height of the crop area.
        /// <example>
        /// <code>
        /// var tween = crop.TweenHeight(15f, 2f, Ease.CubeIn);
        /// await tween.Task;
        /// </code>
        /// </example>
        /// <seealso cref="Tween"/>
        /// <seealso cref="ICropSelfComponent"/>
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="crop">Crop component.</param>
        /// <param name="toHeight">To height.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">Easing function.</param>
        public static Tween TweenHeight(this ICropSelfComponent crop, float toHeight, float timeInSeconds, Func<float, float> easing = null)
        {
            return Tween.Run(crop.CropArea.Height, toHeight, height => crop.CropArea = new RectangleF(crop.CropArea.X, crop.CropArea.Y, crop.CropArea.Width, height), timeInSeconds, easing);
        }
	}
}

