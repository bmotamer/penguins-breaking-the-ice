using UnityEngine;

/// <summary>
/// Manages the title scene
/// </summary>
public class TitleManager : MonoBehaviour
{
	
	public Camera2D        MainCamera;
	public GameObject      Title;
	public Collider2D      ExitButton;
	public Collider2D      StartButton;
	public FloatSerialAnim ZoomAnimation;
	public FloatAnim       FadeInAnimation;
	public FloatAnim       FadeOutAnimation;
	public AudioSource     Music;

	public void Start()
	{
		// Sets the camera 
		MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeInAnimation.Value);

		// Makes the zoom in and out animation repeat
		ZoomAnimation.OnComplete += (SerialAnim<float> serialAnim, float lateTime) =>
		{
			ZoomAnimation.Reset(lateTime);
		};
	}
	
	public void Update()
	{
		// Sets the title scale to the zoom animation's current value
		Title.transform.localScale = new Vector3
		(
			ZoomAnimation.Value,
			ZoomAnimation.Value,
			1.0f
		);

		// If a fade animation is going on, prevents the player from doing any action
		// When fading in, sets the fade alpha
		if (!FadeInAnimation.Finished)
			MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeInAnimation.Value);
		// When fading out, sets the fade alpha and the music volume
		else if (FadeOutAnimation.enabled)
		{
			MainCamera.FadeColor = new Color(0.0f, 0.0f, 0.0f, FadeOutAnimation.Value);
			Music.volume = 1.0f - FadeOutAnimation.Value;
		}
		else
		{
			// When the player finishes touching or clicking
			if (Input.GetMouseButtonUp(0))
			{
				Vector3    worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Collider2D collider      = Physics2D.OverlapPoint(worldPosition);

				// Checks what button was clicked
				if (collider == StartButton)
				{
					// If the start button was clicked, then does a fade out animation and move
					// to the game scene
					FadeOutAnimation.OnComplete += (Anim<float> anim, float lateTime) =>
					{
						Application.LoadLevel("SceneGame");
					};
					
					FadeOutAnimation.enabled = true;
				}
				else if (collider == ExitButton)
				{
					// If the exit button was clicked, then does a fade out animation and leaves
					// the game
					FadeOutAnimation.OnComplete += (Anim<float> anim, float lateTime) =>
					{
						Application.Quit();
					};
					
					FadeOutAnimation.enabled = true;
				}
			}
		}
	}
	
}