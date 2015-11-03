using UnityEngine;

/// <summary>
/// Event that is called when an animation has just completed
/// </summary>
public delegate void AnimComplete<T>(Anim<T> anim, float lateTime);

/// <summary>
/// Generic animation class
/// </summary>
public abstract class Anim<T> : MonoBehaviour
{
	
	public string          ID         = "Tween";       // Identifier
	public float           Duration   = 0.0f;          // Animation duration
	public Easing          Easing     = Easing.Linear; // Animation smoothing
	public float           Speed      = 1.0f;          // Animation speed
	public AnimComplete<T> OnComplete = null;          // Completion event
	
	public float ProgressRaw { get; protected set; }   // Value from 0% to 100%
	public float Progress    { get; protected set; }   // Smoothed version of the variable above
	public bool  Finished    { get; protected set; }   // Is the animation complete?
	public T     ValueRaw    { get; protected set; }   // Value generated from the raw progress
	public T     Value       { get; protected set; }   // Value generated from the smoothed progress

	/// <summary>
	/// Updates the generated values
	/// </summary>
	public abstract void UpdateValue();
	
	public void Start()
	{
		// Resets the progress to 0%
		UpdateProgress(0.0f, true);
	}

	/// <summary>
	/// Updates the animation
	/// </summary>
	/// <param name="time">Elapsed time</param>
	/// <param name="forceUpdate">When set to false, checks if the old and new progress are equal, so it avoids doing
	/// further calculations for nothing</param>
	public void UpdateProgress(float time, bool forceUpdate = false)
	{
		time *= Speed;

		// If the animation duration is 0
		if (Duration == 0.0f)
		{
			// Ends it up to avoid zero division
			if (!Finished)
			{
				ProgressRaw = 1.0f;
				Progress    = 1.0f;
				Finished    = true;
				
				UpdateValue();
				
				if (OnComplete != null)
					OnComplete(this, time);
			}
		}
		else
		{
			// Otherwise, calculates the new progress percentage
			float newProgress     = ProgressRaw + time / Duration;
			float clampedProgress = Mathf.Clamp(newProgress, 0.0f, 1.0f);

			// Compare values to avoid further calculations
			if ((ProgressRaw == clampedProgress) && !forceUpdate)
				return;

			// Assigns the new progress and calculates the new values
			ProgressRaw = clampedProgress;
			Progress    = EasingExtensions.Apply(Easing, ProgressRaw);
			Finished    = ProgressRaw == 1.0f;
			
			UpdateValue();
			
			if ((Finished) && (OnComplete != null))
				OnComplete(this, (newProgress - 1.0f) * Duration);
		}
	}
	
	public void Update()
	{
		// When the component is enabled, uses the 'smoothDeltaTime' as the elapsed time,
		// so that means animations gotta be in seconds
		UpdateProgress(Time.smoothDeltaTime);
	}

	/// <summary>
	/// Resets the animation
	/// </summary>
	public void Reset()
	{
		bool oldFinished = Finished;
		
		ProgressRaw = Duration == 0.0f ? 1.0f : 0.0f;
		Progress    = ProgressRaw;
		Finished    = ProgressRaw == 1.0f;
		
		UpdateValue();
		
		if (Finished && !oldFinished && (OnComplete != null))
			OnComplete(this, 0.0f);
	}

	/// <summary>
	/// Resets the animation and updates it progress by the given time
	/// It's handy to use this function when an animation repeats or is in a sequence
	/// </summary>
	/// <param name="lateTime">Late time</param>
	public void Reset(float lateTime)
	{
		bool oldFinished = Finished;
		
		ProgressRaw = Mathf.Clamp(Duration == 0.0f ? 1.0f : Speed * lateTime / Duration, 0.0f, 1.0f);
		Progress    = EasingExtensions.Apply(Easing, ProgressRaw);
		Finished    = ProgressRaw == 1.0f;
		
		UpdateValue();
		
		if (Finished && !oldFinished && (OnComplete != null))
			OnComplete(this, 0.0f);
	}
	
}