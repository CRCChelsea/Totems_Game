using UnityEngine;
using System.Collections;

public class Setting : MonoBehaviour
{
	public int rows = 5;
	public int cols = 3;
	public float tileSize = 1;
	public float totemSpacing = .5f;
	public Light light;

	void Start ()
	{
		// Gets the main camera
		Camera cam = Camera.main;

		// Sets the half hight of the orthographic viewport
		cam.orthographicSize = rows / 2;

		// Positions the camera in the center of the grid
		float x = ((cols - tileSize) + (totemSpacing * (cols - 1))) / 2;
		float z = -(rows - tileSize) / 2;
		cam.transform.position = new Vector3 (x, 200, z);

		// Positions the light in the center of the grid
		light.transform.position = new Vector3 (x, 10, z);
	}
}


// The number of rows and columns being generated are not dynamic
// Additionally, the camera's width isn't showing enough
