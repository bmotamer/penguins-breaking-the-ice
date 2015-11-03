using UnityEngine;

/// <summary>
/// Represents a generic moving game object
/// </summary>
public abstract class MovingGameObject : MonoBehaviour
{

	protected GameManager _GameManager;

	public virtual void Start()
	{
		_GameManager = GameObject.FindObjectOfType<GameManager>();
	}
	
	public virtual void Update()
	{
		// Retrieves the front mountains
		// The front mountains will help us get the right translation for this item
		MovingParallax frontMountains = _GameManager.FrontMountains;

		// Updates this item's position
		transform.position = new Vector3
		(
			transform.position.x - frontMountains.MultipliedVelocity.x * frontMountains.transform.localScale.x * Time.smoothDeltaTime,
			transform.position.y,
			transform.position.z
		);
	}

	public virtual void OnTriggerEnter2D(Collider2D coll)
	{
		// When colliding with the player, destroys it
		Destroy(gameObject);
	}

	public void OnBecameInvisible()
	{
		// When out of the screen, destroys it
		Destroy(gameObject);
	}

}