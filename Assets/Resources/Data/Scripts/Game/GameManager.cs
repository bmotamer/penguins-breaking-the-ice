using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the game scene
/// </summary>
public class GameManager : MonoBehaviour
{

	public Camera2D                      MainCamera;
	public AudioSource                   Music;
	public MovingParallax                BackMountains;
	public MovingParallax                BackClouds;
	public MovingParallax                MiddleMountains;
	public MovingParallax                MiddleClouds;
	public MovingParallax                FrontMountains;
	public Penguin                       Penguin;
	public TextMesh                      ScoreTextMesh;
	public Collider2D                    JumpButton;
	public Collider2D                    AttackButton;
	public FloatAnim                     FadeInAnimation;
	public FloatAnim                     FadeOutAnimation;
	public WaitAnim                      GeneratorInterval;
	public MovingGameObjectDescription[] MovingGameObjects;
	public FloatAnim                     VelocityMultiplier;
	public float                         VelocityStep;

	public bool JumpButtonWasTriggered   { get; protected set; }
	public bool AttackButtonWasTriggered { get; protected set; }

	public void Start()
	{
		// As the velocity multiplier animation is disabled,
		// manually starts it
		VelocityMultiplier.Start();

		// Resets the score back to 0
		ScoreHolder.Instance.TotalScore = 0;

		// Sets up the generator to repeat
		GeneratorInterval.OnComplete += (Anim<float> anim, float lateTime) =>
		{
			GeneratorInterval.Reset(lateTime);

			// If the fade in animation is still happening, nothing can be generated yet
			if (!FadeInAnimation.Finished)
				return;


			// Generates a number from 0% to 100%
			// Adds every prefab that has its appearance probability under the percentage generated
			List<GameObject> prefabs = new List<GameObject>();
			float            random  = Random.Range(0.0f, 1.0f);
			int              count   = 0;

			foreach (MovingGameObjectDescription description in MovingGameObjects)
			{
				if (random > description.Probability.Value)
					continue;

				prefabs.Add(description.Prefab);
				++count;
			}

			// If none was added
			if (count == 0)
				// Generates a total random prefab
				Instantiate(MovingGameObjects[Random.Range(0, MovingGameObjects.Length)].Prefab);
			else
				// Generates a random from the list
				Instantiate(prefabs[Random.Range(0, count)]);
		};

		// Sets up the fade out animation to move the game to the score scene as that's the only scene it's moving to
		// after this one
		FadeOutAnimation.OnComplete += (Anim<float> anim, float lateTime) =>
		{
			Application.LoadLevel("SceneScore");
		};
	}

	public void Update()
	{
		// Makes sure to update everything's speed to the "global speed"
		GeneratorInterval.Speed = VelocityMultiplier.Value;

		Vector2 velocityMultiplier = new Vector2(VelocityMultiplier.Value, 1.0f);

		BackMountains.VelocityMultiplier   = velocityMultiplier;
		BackClouds.VelocityMultiplier      = velocityMultiplier;
		MiddleMountains.VelocityMultiplier = velocityMultiplier;
		MiddleClouds.VelocityMultiplier    = velocityMultiplier;
		FrontMountains.VelocityMultiplier  = velocityMultiplier;

		// Updates the score
		ScoreTextMesh.text = string.Format("{0:0000000000}", ScoreHolder.Instance.TotalScore);

		JumpButtonWasTriggered   = false;
		AttackButtonWasTriggered = false;

		// If there's any fade animation going on, sets the camera fade alpha to the current animation's value
		if (FadeOutAnimation.enabled)
		{
			MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeOutAnimation.Value);
			Music.volume = 1.0f - FadeOutAnimation.Value;
		}
		else if (!FadeInAnimation.Finished)
			MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeInAnimation.Value);
		// Otherwise, if the screen was touched or clicked
		else if (Input.GetMouseButtonUp(0))
		{
			Vector3    worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D collider      = Physics2D.OverlapPoint(worldPosition);

			// Checks if the jump or attack buttons were pressed
			JumpButtonWasTriggered   = collider == JumpButton;
			AttackButtonWasTriggered = collider == AttackButton;
		}
	}

}