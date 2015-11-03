using UnityEngine;

/// <summary>
/// Extends the easing list with a function
/// </summary>
public static class EasingExtensions
{

	/// <summary>
	/// Transforms a raw progress into a smoothed one
	/// </summary>
	/// <param name="easing">Easing</param>
	/// <param name="progressRaw">Raw progress</param>
	public static float Apply(Easing easing, float progressRaw)
	{
		switch (easing)
		{
			// Quadratic
			case Easing.QuadraticIn:
				return progressRaw * progressRaw;
				
			case Easing.QuadraticOut:
				return progressRaw * (2 - progressRaw);
				
			case Easing.QuadraticInOut:
				if ((progressRaw *= 2) < 1)
					return 0.5f * progressRaw * progressRaw;
				
				return -0.5f * (--progressRaw * (progressRaw - 2) - 1);
				
			// Cubic
			case Easing.CubicIn:
				return progressRaw * progressRaw * progressRaw;
				
			case Easing.CubicOut:
				return --progressRaw * progressRaw * progressRaw + 1;
				
			case Easing.CubicInOut:
				if ((progressRaw *= 2) < 1)
					return 0.5f * progressRaw * progressRaw * progressRaw;
				
				return 0.5f * ((progressRaw -= 2) * progressRaw * progressRaw + 2);
				
			// Quartic
			case Easing.QuarticIn:
				return progressRaw * progressRaw * progressRaw * progressRaw;
				
			case Easing.QuarticOut:
				return 1 - (--progressRaw * progressRaw * progressRaw * progressRaw);
				
			case Easing.QuarticInOut:
				if ((progressRaw *= 2) < 1)
					return 0.5f * progressRaw * progressRaw * progressRaw * progressRaw;
				
				return -0.5f * ((progressRaw -= 2) * progressRaw * progressRaw * progressRaw - 2);
				
			// Quintic
			case Easing.QuinticIn:
				return progressRaw * progressRaw * progressRaw * progressRaw * progressRaw;
				
			case Easing.QuinticOut:
				return --progressRaw * progressRaw * progressRaw * progressRaw * progressRaw + 1;
				
			case Easing.QuinticInOut:
				if ((progressRaw *= 2) < 1)
					return 0.5f * progressRaw * progressRaw * progressRaw * progressRaw * progressRaw;
				
				return 0.5f * ((progressRaw -= 2) * progressRaw * progressRaw * progressRaw * progressRaw + 2);
				
			// Sinusoidal
			case Easing.SinusoidalIn:
				return 1 - Mathf.Cos(progressRaw * Mathf.PI / 2);
				
			case Easing.SinusoidalOut:
				return Mathf.Sin(progressRaw * Mathf.PI / 2);
				
			case Easing.SinusoidalInOut:
				return 0.5f * (1 - Mathf.Cos(Mathf.PI * progressRaw));
				
			// Exponential
			case Easing.ExponentialIn:
				return progressRaw == 0 ? 0 : Mathf.Pow(1024, progressRaw - 1);
				
			case Easing.ExponentialOut:
				return progressRaw == 1 ? 1 : 1 - Mathf.Pow(2, -10 * progressRaw);
				
			case Easing.ExponentialInOut:
				if (progressRaw == 0)
					return 0;
				
				if (progressRaw == 1)
					return 1;
				
				if ((progressRaw *= 2) < 1)
					return 0.5f * Mathf.Pow(1024, progressRaw - 1);
				
				return 0.5f * (-Mathf.Pow(2, -10 * (progressRaw - 1)) + 2);
				
			// Circular
			case Easing.CircularIn:
				return 1 - Mathf.Sqrt(1 - progressRaw * progressRaw);
				
			case Easing.CircularOut:
				return Mathf.Sqrt(1 - (--progressRaw * progressRaw));
				
			case Easing.CircularInOut:
				if ((progressRaw *= 2) < 1)
					return -0.5f * (Mathf.Sqrt(1 - progressRaw * progressRaw) - 1);
				
				return 0.5f * (Mathf.Sqrt(1 - (progressRaw -= 2) * progressRaw) + 1);
			
			// Linear
			default:
				return progressRaw;
		}
	}
	
}