using UnityEngine;

/// <summary>
/// Generic from->to animation
/// </summary>
public abstract class FromToAnim<T> : Anim<T>
{
	
	public T From; // Original value
	public T To;   // Destination value

	/// <summary>
	/// Resets the progress to 0%
	/// </summary>
	/// <param name="from">Original value</param>
	/// <param name="to">Destination value</param>
	public void Reset(T from, T to)
	{
		From        = from;
		To          = to;
		ProgressRaw = Duration == 0.0f ? 1.0f : 0.0f;
		Progress    = ProgressRaw;
		Finished    = ProgressRaw == 1.0f;
		
		UpdateValue();
	}

	/// <summary>
	/// Changes the destination without ruining the animation
	/// </summary>
	/// <param name="to">Destination value</param>
	public void Reroute(T to)
	{
		From        = Value;
		To          = to;
		Duration    = Duration - ProgressRaw * Duration;
		ProgressRaw = Duration == 0.0f ? 1.0f : 0.0f;
		Progress    = ProgressRaw;
		Finished    = ProgressRaw == 1.0f;
		
		// Maybe change the easing according to the current one?
		
		UpdateValue();
	}
	
}