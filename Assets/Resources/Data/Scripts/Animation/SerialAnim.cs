using UnityEngine;

/// <summary>
/// Event that is called when a animation sequence is complete
/// </summary>
public delegate void SerialAnimComplete<T>(SerialAnim<T> serialAnim, float lateTime);

/// <summary>
/// Generic animation sequence
/// </summary>
public class SerialAnim<T> : MonoBehaviour
{

	public string                ID = "Serial Tween"; // Identifier
	public MonoBehaviour[]       List;                // List of generic animations
	public SerialAnimComplete<T> OnComplete;          // Completion event
	
	public int     Index    { get; protected set; }   // Current animation index
	public Anim<T> Current  { get; protected set; }   // Current animation
	public bool    Finished { get; protected set; }   // Is the sequence complete?

	/// <summary>
	/// Animation sequence speed
	/// </summary>
	protected float _Speed = 1.0f;
	
	public void Start()
	{
		Reset();
	}

	/// <summary>
	/// Gets the current animation value
	/// </summary>
	/// <value>Value</value>
	public T Value
	{
		get
		{
			return Current == null ? default(T) : Current.Value;
		}
	}

	/// <summary>
	/// Gets or sets the animation sequence speed
	/// </summary>
	/// <value>Speed</value>
	public float Speed
	{
		get { return _Speed; }
		set
		{
			_Speed = value;

			if (Current != null)
				Current.Speed = _Speed;
		}
	}

	/// <summary>
	/// This function is called when the current animation is completed
	/// </summary>
	/// <param name="anim">Animation</param>
	/// <param name="lateTime">Late time</param>
	protected void OnAnimComplete(Anim<T> anim, float lateTime)
	{
		// Disables the current animation
		Current.enabled = false;
		
		bool last;

		// Goes to the next animation unless it's the last one
		// Updates its progress by the late time
		// If the animation is complete, keeps moving to the next one until it's not complete
		do
		{
			last = Index == List.Length - 1;
			
			if (last)
				break;
			
			Current = (Anim<T>)List[++Index];
			Current.Speed = _Speed;
			Current.Reset(lateTime);

			lateTime -= Current.Duration;
		} while (Current.Finished);

		// If the last one was reached, raises the completion event
		if (last)
		{
			if (OnComplete != null)
				OnComplete(this, lateTime);
		}
		// Otherwise, activates the current animation
		else
			Current.enabled = true;
	}

	/// <summary>
	/// Resets the animation sequence
	/// </summary>
	public void Reset()
	{
		if ((List == null) || (List.Length == 0))
		{
			Index    = -1;
			Current  = null;
			Finished = true;
		}
		else
		{
			// Resets every animation and assign events
			for (Index = List.Length - 1; Index >= 0; Index--)
			{
				Current = (Anim<T>)List[Index];
				Current.Reset();
				
				Current.enabled    = Index == 0;
				Current.Speed      = _Speed;
				Current.OnComplete = OnAnimComplete;
			}

			Index    = 0;
			Finished = false;
		}
	}

	/// <summary>
	/// Resets the animation sequence and progresses it by the given time
	/// </summary>
	/// <param name="time">Time</param>
	public void Reset(float time)
	{
		if ((List == null) || (List.Length == 0))
		{
			Index    = -1;
			Current  = null;
			Finished = true;
		}
		else
		{
			// Resets every animation and assign events
			for (Index = List.Length - 1; Index >= 0; Index--)
			{
				Current = (Anim<T>)List[Index];
				
				if (Index == 0)
				{
					Current.Reset(time);
					Current.enabled = true;
				}
				else
				{
					Current.Reset();
					Current.enabled = false;
				}
				
				Current.Speed      = _Speed;
				Current.OnComplete = OnAnimComplete;
			}

			Index    = 0;
			Finished = false;
		}
	}
	
}