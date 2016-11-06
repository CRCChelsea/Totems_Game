using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HintSpace : MonoBehaviour
{
	public Material bgMaterial;

	private int cols;
	private int rows;
	private int[] solution;
	List<Hint> hintList;

	public void BuildHints (int _cols, int _rows, float _totemSpacing, int[] _solution)
	{
		cols = _cols;
		rows = _rows;
		//totemSpacing = _totemSpacing;
		solution = _solution;

		hintList = new List<Hint> ();
		CreateBackground ();
		HintGenerator hints = new HintGenerator (cols, rows, solution);
		hintList = hints.GetPrunedHintList ();
	}

	private void CreateBackground ()
	{
		float xScale = .2f;
		float zScale = rows / 10f;
		
		GameObject bg = GameObject.CreatePrimitive (PrimitiveType.Plane);
		bg.transform.position = new Vector3 (-1.5f, -.002f, -3f);
		bg.transform.localScale = new Vector3 (xScale, 1f, zScale);
		bg.GetComponent<Renderer> ().material = bgMaterial;
		bg.transform.parent = transform;
	}
}
