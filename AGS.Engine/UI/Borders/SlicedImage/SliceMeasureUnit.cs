using System;

namespace AGS.Engine
{
	public enum SliceMeasurement
	{
		Pixels,
		Percentage,
	}

	public class SliceMeasureUnit
	{
		public SliceMeasureUnit(float value, SliceMeasurement measureUnit)
		{
			Value = value;
			MeasureUnit = measureUnit;
		}

		public float Value { get; set; }
		public SliceMeasurement MeasureUnit { get; set; }
	}
}

