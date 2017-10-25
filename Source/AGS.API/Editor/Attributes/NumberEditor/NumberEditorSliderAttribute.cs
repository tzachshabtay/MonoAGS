using System;
namespace AGS.API
{
    /// <summary>
    /// Allows controlling the "suggested" minimum and maximum values that will be assigned to 
    /// a slider of a number editor in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NumberEditorSliderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.NumberEditorSliderAttribute"/> class.
        /// </summary>
        /// <param name="sliderMin">Slider minimum.</param>
        /// <param name="sliderMax">Slider max.</param>
        public NumberEditorSliderAttribute(float sliderMin, float sliderMax)
        {
            SliderMin = sliderMin;
            SliderMax = sliderMax;
        }

        /// <summary>
        /// Gets the slider minimum.
        /// </summary>
        /// <value>The slider minimum.</value>
        public float SliderMin { get; private set; }

        /// <summary>
        /// Gets the slider max.
        /// </summary>
        /// <value>The slider max.</value>
        public float SliderMax { get; private set; }
    }
}
