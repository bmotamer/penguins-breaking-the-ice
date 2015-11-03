using UnityEngine;

/// <summary>
/// Penguin
/// </summary>
public class Penguin : MonoBehaviour
{
	
	protected Vector3     _StartingPosition; // Starting position (this is saved to help with the skidding animation)
	protected GameManager _GameManager;      // Game manager
	protected bool        _SkidPending;      // It makes sure the player will skid after jumping or attacking
	protected Sprite[]    _SkiddingSprites;  // Skidding sprites
	protected Sprite[]    _AttackSprites;    // Attacking sprites

	public int   Health      = 3;
	public int   MaxHealth   = 3;
	public float AttackRange = 0.125f;

	public FloatSerialAnim SkidAnimation;
	public FloatSerialAnim JumpingAnimation;
	public FloatAnim       AttackAnimation;
	public WaitAnim        InvincibilityTimer;
	public AudioClip       AttackSound;
	public AudioClip       DamageSound;
	public AudioClip       DeathSound;

	public bool Attacking
	{
		get { return AttackAnimation.enabled; }
		protected set { AttackAnimation.enabled = value; }
	}

	public bool Jumping
	{
		get { return JumpingAnimation.enabled; }
		protected set { JumpingAnimation.enabled = value; }
	}

	public bool Invincible
	{
		get { return InvincibilityTimer.enabled; }
		protected set { InvincibilityTimer.enabled = value; }
	}

	public void Start()
	{
		// Retrieves the sprites from the folder
		_SkiddingSprites = Resources.LoadAll<Sprite>("Graphics/Characters/PenguinSkidding");
		_AttackSprites = Resources.LoadAll<Sprite>("Graphics/Characters/PenguinAttacking");

		_StartingPosition = transform.position;
		_GameManager = GameObject.FindObjectOfType<GameManager>();

		// Repeats the skidding animation
		// If the player is attacking or jumps it, saves that skidding is pending
		SkidAnimation.OnComplete += (SerialAnim<float> serialAnim, float lateTime) =>
		{
			if (Attacking || Jumping)
				_SkidPending = true;
			else
				SkidAnimation.Reset(lateTime);
		};

		// Disables the jump animation when it's complete
		JumpingAnimation.OnComplete += (SerialAnim<float> SerialAnim, float lateTime) =>
		{
			Jumping = false;
		};

		// Disables the attack animation when it's complete
		AttackAnimation.OnComplete += (Anim<float> anim, float lateTime) =>
		{
			Attacking = false;
		};

		InvincibilityTimer.OnComplete += (Anim<float> anim, float lateTime) =>
		{
			Invincible = false;
		};
	}

	public void Update()
	{
		// Makes sure the animations' speed follows the global speed
		SkidAnimation.Speed    = _GameManager.VelocityMultiplier.Value;
		JumpingAnimation.Speed = _GameManager.VelocityMultiplier.Value;
		AttackAnimation.Speed  = _GameManager.VelocityMultiplier.Value;

		// If the player is not jumping and the jump button was triggered
		if (_GameManager.JumpButtonWasTriggered && !Jumping)
		{
			// Makes it jump
			JumpingAnimation.Reset(Time.smoothDeltaTime);
			JumpingAnimation.enabled = true;
		}

		// Same goes for attacking
		if (_GameManager.AttackButtonWasTriggered && !Attacking)
		{
			_GameManager.audio.PlayOneShot(AttackSound, 0.25f);
			AttackAnimation.Reset(Time.smoothDeltaTime);
			AttackAnimation.To = AttackRange;
			AttackAnimation.enabled = true;
		}

		SpriteRenderer spriteRenderer = (SpriteRenderer)renderer;

		// If the player is invincible
		if (Invincible)
			// Makes it blink
			spriteRenderer.enabled = Mathf.RoundToInt(InvincibilityTimer.ProgressRaw * 100) % 2 == 0;
		else
			spriteRenderer.enabled = true;
		
		// When the player is attacking
		if (Attacking)
		{
			// Assigns the proper sprite according to the animation completion
			spriteRenderer.sprite = _AttackSprites[Mathf.RoundToInt(AttackAnimation.Progress * (_AttackSprites.Length - 1))];

			// Creates a ray to check for collision
			RaycastHit2D hit2D = Physics2D.Raycast
			(
				new Vector2
				(
					transform.position.x + 0.45f,
					transform.position.y
				),
				Vector2.right,
				AttackAnimation.Value
			);

			// If the collision hits something
			if (hit2D.collider != null)
			{
				Obstacle obstacle = hit2D.collider.gameObject.GetComponent<Obstacle>();

				// And it's an obstacle
				if (obstacle != null)
					// Breaks it
					obstacle.Break();
			}
		}
		// When jumping
		else if (Jumping)
			// Assigns the jumping sprite
			spriteRenderer.sprite = _SkiddingSprites[7];
		else
		{
			// When not doing anything, make sure to skid if skidding is pending
			if (_SkidPending)
			{
				SkidAnimation.Reset(Time.smoothDeltaTime);
				_SkidPending = false;
			}

			// Assigns the appropriate skidding sprite according to its animation completion
			if (SkidAnimation.Index == 0)
				spriteRenderer.sprite = _SkiddingSprites[Mathf.RoundToInt(SkidAnimation.Current.Progress * (_SkiddingSprites.Length - 1))];
			else
				spriteRenderer.sprite = _SkiddingSprites[0];
		}

		// Also updates the position according to the skidding and jumping animation values
		transform.position = new Vector3
		(
			_StartingPosition.x + SkidAnimation.Value,
			_StartingPosition.y + JumpingAnimation.Value,
			_StartingPosition.z
		);
	}

	/// <summary>
	/// Heal the penguin
	/// </summary>
	public bool Heal()
	{
		// If the health is maxed out
		if (Health >= MaxHealth)
			// Returns false, because then the player can't be healed
			return false;

		// Otherwise, heals the player and returns true
		++Health;
		return true;
	}

	/// <summary>
	/// Damages the penguin
	/// </summary>
	public void Hit()
	{
		// If the player is invincible
		if (Invincible)
			// Does nothing
			return;

		// If the reduced health is 0
		if (--Health <= 0)
		{
			// Fades out to the score scene
			_GameManager.FadeOutAnimation.enabled = true;
			_GameManager.audio.PlayOneShot(DeathSound);
			Destroy(gameObject);
		}
		else
		{
			// Makes the player invincible
			InvincibilityTimer.Reset(Time.smoothDeltaTime);
			_GameManager.audio.PlayOneShot(DamageSound);
			Invincible = true;
		}
	}

}