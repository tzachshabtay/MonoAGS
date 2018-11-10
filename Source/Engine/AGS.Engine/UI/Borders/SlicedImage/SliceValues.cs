using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class SliceValues
	{
		public SliceValues(SliceMeasurement measurement, float value) : this(measurement, value, value, value, value)
		{
		}

		public SliceValues(SliceMeasurement measurement, float left, float right, float top, float bottom)
			: this(new SliceMeasureUnit (left, measurement), new SliceMeasureUnit (right, measurement),
			       new SliceMeasureUnit (top, measurement), new SliceMeasureUnit (bottom, measurement))
		{
		}

		public SliceValues(SliceMeasureUnit left, SliceMeasureUnit right, SliceMeasureUnit top, SliceMeasureUnit bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		public SliceMeasureUnit Left { get; set; }
		public SliceMeasureUnit Right { get; set; }
		public SliceMeasureUnit Top { get; set; }
		public SliceMeasureUnit Bottom { get; set; }

		public SliceValues ToPixels(float width, float height)
		{
			return new SliceValues (SliceMeasurement.Pixels, 
				Left.MeasureUnit == SliceMeasurement.Pixels ? Left.Value : width*Left.Value,
				Right.MeasureUnit == SliceMeasurement.Pixels ? Right.Value : width*Right.Value,
				Top.MeasureUnit == SliceMeasurement.Pixels ? Top.Value : height*Top.Value,
				Bottom.MeasureUnit == SliceMeasurement.Pixels ? Bottom.Value : height*Bottom.Value);
		}

		public SliceValues ToPercentage(float width, float height)
		{
			return new SliceValues (SliceMeasurement.Percentage, 
				Left.MeasureUnit == SliceMeasurement.Percentage ? Left.Value : Left.Value / width,
				Right.MeasureUnit == SliceMeasurement.Percentage ? Right.Value : Right.Value / width,
				Top.MeasureUnit == SliceMeasurement.Percentage ? Top.Value : Top.Value / height,
				Bottom.MeasureUnit == SliceMeasurement.Percentage ? Bottom.Value : Bottom.Value / height);
		}

		public override string ToString()
		{
			return $"[SliceValues: Left={Left.Value}, Right={Right.Value}, Top={Top.Value}, Bottom={Bottom.Value}]";
		}
	}
}