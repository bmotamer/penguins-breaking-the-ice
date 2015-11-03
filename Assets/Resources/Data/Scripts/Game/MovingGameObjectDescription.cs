using System;
using UnityEngine;

/// <summary>
/// Represents a prefab and its probability to show up on the game
/// </summary>
[Serializable]
public class MovingGameObjectDescription
{

	public GameObject Prefab;
	public FloatAnim  Probability;

}