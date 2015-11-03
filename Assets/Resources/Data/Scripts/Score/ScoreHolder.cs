using UnityEngine;

/// <summary>
/// Singleton class that holds the player score
/// </summary>
public class ScoreHolder : MonoBehaviour
{

	/// <summary>
	/// The only one instance
	/// </summary>
	/// <value>The instance</value>
	public static ScoreHolder Instance { get; protected set; }

	/// <summary>
	/// Initializes the <see cref="ScoreHolder"/> class.
	/// </summary>
	static ScoreHolder()
	{
		// Creates an object that doesn't destroy as the level changes called "ScoreHolder"
		// Adds the ScoreHolder component to it
		// This way it won't show up on the list unless it's called at least once
		GameObject gameObject = new GameObject("ScoreHolder");
		DontDestroyOnLoad(gameObject);
		Instance = gameObject.AddComponent<ScoreHolder>();
	}

	public int TotalScore;

	public void Save()
	{
		PlayerPrefs.SetInt ("PBIScore", TotalScore);
		PlayerPrefs.Save();
	}

}