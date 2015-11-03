using UnityEngine;

public class Obstacle : MovingGameObject
{

	/// <summary>
	/// Breaking sound
	/// </summary>
	public AudioClip Sound;

	/// <summary>
	/// Score
	/// </summary>
	public int Score;

	public override void OnTriggerEnter2D(Collider2D coll)
	{
		// Plays the breaking sound
		// And damages the player
		_GameManager.audio.PlayOneShot(Sound);
		_GameManager.Penguin.Hit();
		base.OnTriggerEnter2D(coll);
	}

	/// <summary>
	/// This is called when the player attacks this obstacle
	/// </summary>
	public void Break()
	{
		// Plays the breaking sound
		// Speeds up the game
		_GameManager.audio.PlayOneShot(Sound);
		_GameManager.VelocityMultiplier.UpdateProgress(_GameManager.VelocityStep);

		ScoreHolder.Instance.TotalScore += Score;

		Destroy(gameObject);
	}

}