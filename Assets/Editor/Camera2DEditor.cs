using System;
using System.Reflection;

using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds some features to the Camera2D editor
/// </summary>
[CustomEditor(typeof(Camera2D))]
public sealed class Camera2DEditor : Editor
{

	/// <summary>
	/// Inspector GUI
	/// </summary>
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		Camera2D camera2D = (Camera2D)target;

		// If the Camera2D component is enabled and the game isn't running, creates a button to update the camera
		// settings given the main game view resolution
		if (camera2D.enabled && !Application.isPlaying && GUILayout.Button("Update Preview"))
		{
			// Hack to retrieve the main game view resolution
			Type       gameViewType          = Type.GetType("UnityEditor.GameView, UnityEditor");
			MethodInfo getSizeOfMainGameView = gameViewType.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
			Vector2    sizeOfMainGameView    = (Vector2)getSizeOfMainGameView.Invoke(null, null);

			// At this point, as the resolution is stored in sizeOfMainGameView, makes sure there's not more than 1
			// Camera2D component added to this object and updates the camera settings
			camera2D.Awake();
			camera2D.Refresh(sizeOfMainGameView.x, sizeOfMainGameView.y);
		}
	}

}