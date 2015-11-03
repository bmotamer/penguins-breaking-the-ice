using UnityEngine;

/// <summary>
/// Does the same as an generic item does, but it heals the player if they don't have the maximum value of lives
/// </summary>
public class HealingItem : Item
{

	/// <summary>
	/// The healing sound
	/// </summary>
	public AudioClip HealingSound;

	public new void OnTriggerEnter2D(Collider2D coll)
	{
		// When it touches the player
		// Tries to heal it
		if (_GameManager.Penguin.Heal())
		{
			// If the player doesn't have max lives
			// Plays the healing sound and destroys this item
			_GameManager.audio.PlayOneShot(HealingSound, 0.5f);
			Destroy(gameObject);
		}
		else
			// If the player has max lives
			// Just adds up a value to the score, just like the other items
			base.OnTriggerEnter2D(coll);
	}

}
