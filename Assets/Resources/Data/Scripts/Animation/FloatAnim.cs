using UnityEngine;

/// <summary>
/// Float animation
/// </summary>
public class FloatAnim : FromToAnim<float>
{
	
	public override void UpdateValue()
	{
		// Does a simple lerp
		ValueRaw = Mathf.Lerp(From, To, ProgressRaw);
		Value    = Mathf.Lerp(From, To, Progress);
	}
	
}
