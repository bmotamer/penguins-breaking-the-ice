using UnityEngine;

/// <summary>
/// Represents a generic item
/// </summary>
public class Item : MovingGameObject
{

	public FloatSerialAnim FloatingAnimation; // Floating animation
	public int             Score;             // Score obtained when it's collected
	public AudioClip       CollectionSound;   // Sound that plays when the item is collected
	
	protected float _StartingY; // Starting Y position (used to help with the floating animaiton)

	public override void Start()
	{
		base.Start();

		_StartingY = transform.position.y;

		// The floating animation repeats over and over
		FloatingAnimation.OnComplete += (SerialAnim<float> serialAnim, float lateTime) =>
		{
			FloatingAnimation.Reset(lateTime);
		};
	}

	public override void Update()
	{
		FloatingAnimation.Speed = _GameManager.VelocityMultiplier.Value;
		base.Update ();

		// Moves the item up and down
		transform.position = new Vector3
		(
			transform.position.x,
			_StartingY + FloatingAnimation.Value,
			transform.position.z
		);
	}

	public override void OnTriggerEnter2D (Collider2D coll)
	{
		// When collected, adds up this item's score to the total one
		// Plays the collection sound
		ScoreHolder.Instance.TotalScore += Score;
		_GameManager.audio.PlayOneShot(CollectionSound, 0.125f);
		base.OnTriggerEnter2D (coll);
	}

}