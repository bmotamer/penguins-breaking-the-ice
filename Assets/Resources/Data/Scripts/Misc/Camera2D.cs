using System;
using UnityEngine;

/// <summary>
/// Image stretch mode
/// </summary>
public enum StretchMode
{
	/// <summary>
	/// No stretching (bars might be drawn)
	/// </summary>
	Center = 0,
	/// <summary>
	/// Stretches the image to fit the screen (bars might be drawn)
	/// </summary>
	Fit = 1,
	/// <summary>
	/// Stretches the image to cover the whole screen
	/// </summary>
	Fill = 2
}

/// <summary>
/// Aspect ratio preserving camera
/// </summary>
[AddComponentMenu("Rendering/Camera 2D")]
[RequireComponent(typeof(Camera))]
public sealed class Camera2D : MonoBehaviour
{

	/// <summary>
	/// Image stretch mode
	/// </summary>
	public StretchMode StretchMode = StretchMode.Fit;
	/// <summary>
	/// Original game resolution, in pixels
	/// </summary>
	public Vector2 Resolution = new Vector2(640.0f, 360.0f);
	/// <summary>
	/// How many pixels a unit contains
	/// </summary>
	public float PixelsToUnits = 100.0f;
	/// <summary>
	/// Zoom
	/// </summary>
	public float Zoom = 1.0f;
	/// <summary>
	/// Draw bars to fill out of resolution spaces?
	/// </summary>
	public bool Bars = true;
	/// <summary>
	/// Bars' color
	/// </summary>
	public Color BarsColor = Color.black;
	/// <summary>
	/// Fade color
	/// </summary>
	public Color FadeColor = Color.clear;

	/// <summary>
	/// Reference to the camera component
	/// </summary>
	private Camera _Camera;
	/// <summary>
	/// Material to draw the bars and the fade
	/// </summary>
	private Material _Material;

	/// <summary>
	/// Awake
	/// </summary>
	public void Awake()
	{
		// Makes sure there's no other Camera2D component
		if (GetComponent<Camera2D>() != this)
		{
			// By removing it immediately when it is added
			if (Application.isEditor)
				DestroyImmediate(this);
			else
				Destroy(this);
		}
	}

	/// <summary>
	/// Start
	/// </summary>
	public void Start()
	{
		// Gets the camera reference and loads the material to draw bars and fade
		_Camera   = GetComponent<Camera>();
		_Material = new Material(Shader.Find("Sprites/Default"));
	}

	/// <summary>
	/// Updates the camera settings if anything changed
	/// </summary>
	/// <param name="screenWidth">Screen width</param>
	/// <param name="screenHeight">Screen height</param>
	public void Refresh(float screenWidth, float screenHeight)
	{
		// If the camera reference is null for some reason
		if (_Camera == null)
		{
			// Tries to get it
			_Camera = GetComponent<Camera>();

			// If it couldn't be found
			if (_Camera == null)
				// Adds a new camera
				_Camera = gameObject.AddComponent<Camera>();
		}

		// As the camera is 2D, the projection must be orthographic
		if (!_Camera.isOrthoGraphic)
			_Camera.isOrthoGraphic = true;

		float orthographicSize;

		// Depending on the stretch mode, the projection size can be different, so it calculates it, preserving the
		// aspect ratio and trying to keep the image as pixel perfect as possible
		switch (StretchMode)
		{
			case StretchMode.Fit:
				orthographicSize = 0.5f / PixelsToUnits / Zoom * screenHeight * Mathf.Max
				(
					Resolution.x / screenWidth, Resolution.y / screenHeight
				);
				break;
			case StretchMode.Fill:
				orthographicSize = 0.5f / PixelsToUnits / Zoom * screenHeight * Mathf.Min
				(
					Resolution.x / screenWidth, Resolution.y / screenHeight
				);
				break;
			default:
				orthographicSize = 0.5f / PixelsToUnits / Zoom * screenHeight;
				break;
		}

		// Assigns the proper projection size
		if (_Camera.orthographicSize != orthographicSize)
			_Camera.orthographicSize = orthographicSize;
	}

	/// <summary>
	/// Update
	/// </summary>
	public void Update()
	{
		// Updates the camera settings given the current screen resolution
		Refresh(Screen.width, Screen.height);

		// The bars must always be fully opaque
		BarsColor.a = 1.0f;

		// If the material is null for some reason
		if (_Material == null)
			// Reloads it
			_Material = new Material(Shader.Find("Sprites/Default"));
	}

	/// <summary>
	/// Post Render
	/// </summary>
	public void OnPostRender()
	{
		// If this camera is not the one being used at the moment
		if (_Camera != Camera.current)
			// Then don't draw any bars or fade
			return;

		Vector2 barsThickness;
		bool    barsThicknessXOverZero;
		bool    barsThicknessYOverZero;
		bool    barsWillBeDrawn;

		// If bars are enabled
		if (Bars)
		{
			// Calculates the bars' width and height according to the stretch mode
			switch (StretchMode)
			{
				// Center - might create bars at the top, bottom and on the sides
				case StretchMode.Center:
					barsThickness = new Vector2
					(
						0.5f * (Screen.width  - Resolution.x) / Screen.width,
						0.5f * (Screen.height - Resolution.y) / Screen.height
					);
					break;
				// Fit - might create bars at the top and bottom or on the sides
				case StretchMode.Fit:
					Vector2 resolutionScale = new Vector2
					(
						Resolution.x / Screen.width,
						Resolution.y / Screen.height
					);
					
					if (resolutionScale.x > resolutionScale.y)
						barsThickness = new Vector2
						(
							0.0f,
							0.5f * (Screen.height - Resolution.y / resolutionScale.x) / Screen.height
						);
					else
						barsThickness = new Vector2
						(
							0.5f * (Screen.width - Resolution.x / resolutionScale.y) / Screen.width,
							0.0f
						);
					
					break;
				// Fill - no bars
				default:
					barsThickness = Vector2.zero;
					break;
			}

			// Checks if bars are going to be drawn
			barsThicknessXOverZero = barsThickness.x > 0.0f;
			barsThicknessYOverZero = barsThickness.y > 0.0f;
			barsWillBeDrawn        = barsThicknessXOverZero || barsThicknessYOverZero;
		}
		// If bars are disabled
		else
		{
			// Defines that bars aren't going to be drawn
			barsThickness          = Vector2.zero;
			barsThicknessXOverZero = false;
			barsThicknessYOverZero = false;
			barsWillBeDrawn        = false;
		}

		// Defines if the fade is going to be drawn
		bool fadeColorAOverZero = FadeColor.a > 0.0f;

		// If the bars aren't going to be drawn nor the fade
		if (!barsWillBeDrawn && !fadeColorAOverZero)
			// Then does nothing
			return;

		// Otherwise, prepares for drawing and sets up the material
		GL.PushMatrix();

		_Material.SetPass(0);

		GL.LoadOrtho();
		GL.Begin(GL.QUADS);

		// If the bars and fade have different colors
		if (barsWillBeDrawn && (BarsColor != FadeColor))
		{
			// Then changes to the color of the bars and draw them according to what was calculated.
			// There are different if statements to avoid useless draw calls.
			GL.Color(BarsColor);

			if (barsThicknessXOverZero && barsThicknessYOverZero)
			{
				Vector2 oneMinusBarsThickness = new Vector2(1.0f - barsThickness.x, 1.0f - barsThickness.y);

				GL.Vertex3(0.0f,            0.0f, 0.0f);
				GL.Vertex3(1.0f,            0.0f, 0.0f);
				GL.Vertex3(1.0f, barsThickness.y, 0.0f);
				GL.Vertex3(0.0f, barsThickness.y, 0.0f);
				
				GL.Vertex3(           0.0f,         barsThickness.y, 0.0f);
				GL.Vertex3(barsThickness.x,         barsThickness.y, 0.0f);
				GL.Vertex3(barsThickness.x, oneMinusBarsThickness.y, 0.0f);
				GL.Vertex3(           0.0f, oneMinusBarsThickness.y, 0.0f);
				
				GL.Vertex3(oneMinusBarsThickness.x,         barsThickness.y, 0.0f);
				GL.Vertex3(                   1.0f,         barsThickness.y, 0.0f);
				GL.Vertex3(                   1.0f, oneMinusBarsThickness.y, 0.0f);
				GL.Vertex3(oneMinusBarsThickness.x, oneMinusBarsThickness.y, 0.0f);
				
				GL.Vertex3(0.0f, oneMinusBarsThickness.y, 0.0f);
				GL.Vertex3(1.0f, oneMinusBarsThickness.y, 0.0f);
				GL.Vertex3(1.0f,                    1.0f, 0.0f);
				GL.Vertex3(0.0f,                    1.0f, 0.0f);

				// Notice that here and on the other if statements it's checked if the fade is not completely
				// transparent
				if (fadeColorAOverZero)
				{
					// If it's not transparent, changes the color to the fade color and draws it
					GL.Color(FadeColor);

					GL.Vertex3(        barsThickness.x,         barsThickness.y, 0.0f);
					GL.Vertex3(oneMinusBarsThickness.x,         barsThickness.y, 0.0f);
					GL.Vertex3(oneMinusBarsThickness.x, oneMinusBarsThickness.y, 0.0f);
					GL.Vertex3(        barsThickness.x, oneMinusBarsThickness.y, 0.0f);
				}
			}
			else if (barsThicknessXOverZero)
			{
				float oneMinusBarsThicknessX = 1.0f - barsThickness.x;

				GL.Vertex3(           0.0f, 0.0f, 0.0f);
				GL.Vertex3(barsThickness.x, 0.0f, 0.0f);
				GL.Vertex3(barsThickness.x, 1.0f, 0.0f);
				GL.Vertex3(           0.0f, 1.0f, 0.0f);
				
				GL.Vertex3(oneMinusBarsThicknessX, 0.0f, 0.0f);
				GL.Vertex3(                  1.0f, 0.0f, 0.0f);
				GL.Vertex3(                  1.0f, 1.0f, 0.0f);
				GL.Vertex3(oneMinusBarsThicknessX, 1.0f, 0.0f);

				if (fadeColorAOverZero)
				{
					GL.Color(FadeColor);
					
					GL.Vertex3(       barsThickness.x, 0.0f, 0.0f);
					GL.Vertex3(oneMinusBarsThicknessX, 0.0f, 0.0f);
					GL.Vertex3(oneMinusBarsThicknessX, 1.0f, 0.0f);
					GL.Vertex3(       barsThickness.x, 1.0f, 0.0f);
				}
			}
			else
			{
				float oneMinusBarsThicknessY = 1.0f - barsThickness.y;

				GL.Vertex3(0.0f,            0.0f, 0.0f);
				GL.Vertex3(1.0f,            0.0f, 0.0f);
				GL.Vertex3(1.0f, barsThickness.y, 0.0f);
				GL.Vertex3(0.0f, barsThickness.y, 0.0f);
				
				GL.Vertex3(0.0f, oneMinusBarsThicknessY, 0.0f);
				GL.Vertex3(1.0f, oneMinusBarsThicknessY, 0.0f);
				GL.Vertex3(1.0f,                   1.0f, 0.0f);
				GL.Vertex3(0.0f,                   1.0f, 0.0f);

				if (fadeColorAOverZero)
				{
					GL.Color(FadeColor);
					
					GL.Vertex3(0.0f,        barsThickness.y, 0.0f);
					GL.Vertex3(1.0f,        barsThickness.y, 0.0f);
					GL.Vertex3(1.0f, oneMinusBarsThicknessY, 0.0f);
					GL.Vertex3(0.0f, oneMinusBarsThicknessY, 0.0f);
				}
			}
		}
		// If the bars and fade share the same color
		else
		{
			// Changes the color to the fade color and draws a single quads on the whole screen
			GL.Color(FadeColor);

			GL.Vertex3(0.0f, 0.0f, 0.0f);
			GL.Vertex3(1.0f, 0.0f, 0.0f);
			GL.Vertex3(1.0f, 1.0f, 0.0f);
			GL.Vertex3(0.0f, 1.0f, 0.0f);
		}

		GL.End();
		GL.PopMatrix();
	}

	/// <summary>
	/// Destroy
	/// </summary>
	public void OnDestroy()
	{
		// When the object is destroyed, the material must be destroyed as well, as it was generated just for this
		// component
		if (Application.isEditor)
			DestroyImmediate(_Material);
		else
			Destroy(_Material);
		
		_Material = null;
	}

}