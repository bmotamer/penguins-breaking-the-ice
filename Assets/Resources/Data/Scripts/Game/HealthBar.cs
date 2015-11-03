using UnityEngine;

/// <summary>
/// Health bar
/// </summary>
public class HealthBar : MonoBehaviour
{

	/// <summary>
	/// Health point icon
	/// </summary>
	public Sprite HealthIcon;

	protected GameManager      _GameManager;
	protected SpriteRenderer[] _HealthPoints;

	public void Start()
	{
		// Retrieves the game manager and the max lives
		_GameManager = GameObject.FindObjectOfType<GameManager>();
		_HealthPoints = new SpriteRenderer[_GameManager.Penguin.MaxHealth];

		// Creates health points for every 'life'
		for (int i = 0; i < _GameManager.Penguin.MaxHealth; i++)
		{
			// Sets its position, adds a sprite renderer and assigns the sprite icon
			GameObject healthPoint = new GameObject("HealthPoint");
			healthPoint.transform.parent = transform;
			healthPoint.transform.localPosition = new Vector3(i * 0.32f, 0.0f, 0.0f);

			(_HealthPoints[i] = healthPoint.AddComponent<SpriteRenderer>()).sprite = HealthIcon;
		}
	}

	public void Update()
	{
		// Iterates through the health points and show / hide them according to the amount of lives the player has
		for (int i = 0; i < _GameManager.Penguin.MaxHealth; i++)
			_HealthPoints[i].enabled = i < _GameManager.Penguin.Health;
	}

}