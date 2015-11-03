using UnityEngine;

/// <summary>
/// Moving parallax
/// </summary>
public class MovingParallax : MonoBehaviour
{

	/// <summary>
	/// Constant parallax velocity
	/// </summary>
	public Vector2 Velocity;

	/// <summary>
	/// Velocity multiplier
	/// </summary>
	public Vector2 VelocityMultiplier;

	/// <summary>
	/// Gets the velocity multiplied by its multiplier
	/// </summary>
	/// <value>The multiplied velocity</value>
	public Vector2 MultipliedVelocity
	{
		get
		{
			return new Vector2
			(
				Velocity.x * VelocityMultiplier.x,
				Velocity.y * VelocityMultiplier.y
			);
		}
	}

	public void Update()
	{
		// Gets the velocity multiplied by its multiplier and multiplies it by the elapsed time
		Vector2 finalVelocity = MultipliedVelocity * Time.smoothDeltaTime;;

		// And moves the texture offset
		renderer.material.mainTextureOffset = new Vector2
		(
			(renderer.material.mainTextureOffset.x + finalVelocity.x) % 1.0f,
			(renderer.material.mainTextureOffset.y + finalVelocity.y) % 1.0f
		);
	}

}