using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

	public Camera2D   MainCamera;
	public FloatAnim  FadeInAnimation;
	public FloatAnim  FadeOutAnimation;
	public TextMesh   ScoreTextMesh;
	public Collider2D ExitButton;

	public void Start()
	{
		// Sets the score on the score screen to the saved score
		ScoreTextMesh.text = ScoreHolder.Instance.TotalScore.ToString();

		// When the screen fades out, there's only one screen this one will take the player to (the title one)
		FadeOutAnimation.OnComplete += (Anim<float> anim, float lateTime) =>
		{
			Application.LoadLevel("SceneTitle");
		};
	}

	public void Update()
	{
		// If a fade animation is happening, sets the fade alpha value to the current fade animation's current value
		// Otherwise, checks if the exit button was pressed
		// When it is pressed, it activates the fade out animation
		if (!FadeInAnimation.Finished)
			MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeInAnimation.Value);
		else if (FadeOutAnimation.enabled)
			MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeOutAnimation.Value);
		else if (Input.GetMouseButtonUp(0))
		{
			Vector3    worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D collider      = Physics2D.OverlapPoint(worldPosition);

			if (collider == ExitButton)
				FadeOutAnimation.enabled = true;
		}
	}

}