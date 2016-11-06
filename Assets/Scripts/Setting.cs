using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Utilities;

public class Setting : MonoBehaviour
{
	// Members set from the editor
	public GridSpace child_gridSpace;
	public HintSpace child_hintSpace;
	public Light mainLight;
	public Camera cam;
	public int rows;
	public int cols;
	public float totemSpacing;

	// Private members
	//private bool isVertical = false;
	private int[] solution;

	void Start ()
	{
		Stopwatch stopWatch = new Stopwatch ();
		solution = new int[rows * cols];
		PositionCamera ();

		stopWatch.Start ();
		GenerateHints ();
		TimeSpan ts = stopWatch.Elapsed;

		UnityEngine.Debug.Log ("Hint generation took " + ts.Minutes + " minutes and " + ts.Seconds + "." + (ts.Milliseconds / 10) + " seconds.");
		GenerateGrid ();
	}

	private void PositionCamera ()
	{
		// Sets the half hight of the orthographic viewport
		cam.orthographicSize = (rows / 2f) + 1;

		// Positions the camera in the center of the grid
		float x = 1;//Actually (cols + (cols-1 * totemSpacing) / 2f to center on the grid
		float z = -rows / 2f;
		cam.transform.position = new Vector3 (x, 200, z);
		
		// Positions the light in the center of the grid
		mainLight.transform.position = new Vector3 (x, 10, z);
	}

	private void GenerateGrid ()
	{
		child_gridSpace.BuildGrid (rows, cols, totemSpacing, solution);
	}

	private void GenerateHints ()
	{
		GenerateSolution ();
		child_hintSpace.BuildHints (cols, rows, totemSpacing, solution);
	}

	private void GenerateSolution ()
	{
		// Initialize an array for a single totem
		int[] totemSymbols = new int[rows];
		for (int i = 0; i < rows; i++)
		{
			totemSymbols [i] = i;
		}

		// Create a list for the solution
		List<int> solutionList = new List<int> ();

		for (int totemNum = 0; totemNum < cols; totemNum++)
		{
			solutionList.AddRange (Randomize (totemSymbols));
		}

		// Convert the solution list back to an array for comparison purposes
		solution = solutionList.ToArray ();
		solution.PrintGrid (cols, rows);
	}

	private int[] Randomize (int[] array)
	{
		// Now randomzie the array by using the Knuth-Fisher-Yates shuffle
		for (int index = rows -1; index > 0; index--)
		{
			int temp = array [index];

			// Randomly pick another index to switch with
			int swapIndex = UnityEngine.Random.Range (0, index + 1);
			
			// Switch the two values
			array [index] = array [swapIndex];
			array [swapIndex] = temp;
		}
		return array;
	}
}