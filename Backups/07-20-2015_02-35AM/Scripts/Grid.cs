using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
	public int totemNum = 3;
	public int rowNum = 5;
	public float totemSpacing = .5f;
	public Transform totemPole;

	public void Start ()
	{
		CreateTotemPoles ();
	}

	private void CreateTotemPoleTest ()
	{
		int i = 0;
		Transform newTotemPoleTF = Instantiate (totemPole, new Vector3 (0, 0, 0), Quaternion.LookRotation (Vector3.back)) as Transform;
		newTotemPoleTF.gameObject.GetComponent<TotemPole> ().totemNum = i;
	}

	private void CreateTotemPoles ()
	{
		for (int i = 0; i < totemNum; i++)
		{
			Transform newTotemPoleTF = Instantiate (totemPole, new Vector3 ((i * totemSpacing) + i, 0, 0), Quaternion.LookRotation (Vector3.back)) as Transform;
			newTotemPoleTF.gameObject.GetComponent<TotemPole> ().totemNum = i;
		}

		/*// Loops through rows
		for (int i = 0; i < rows; i++)
		{
			// Loops through columns
			for (int j = 0; j < cols; j++)
			{
				// Instantiates new tile (empty object), row by row
				Instantiate (tile, new Vector3 (i + (totemSpacing * i), 0, -j), Quaternion.LookRotation (Vector3.back));
				//tile.position = new Vector3 (i + (totemSpacing * i), 0, -j);*/
	}

}
// Old grid now in totemPole
/*public int rows = 6;
	public int cols = 4;
	public int tileSize = 1;
	public float totemSpacing = .5f;

	public Transform tile;

	//public Tile prefab;
	//private float tileSize;
	
	//public Tile tilePrefab;
	//public int totemNum = 4;
	//public int totemHeight = 6;
	//public float distanceBetweenTiles = 2.0f;
	//public int tilesPerRow = 6;
	//public int numberOfTiles = 24;

	//static Tile[] tilesAll;
	//static List<Tile> tilesMined;
	//static List<Tile> tilesUnmined;

	public void Start ()
	{
		CreateTilesTest ();
	}

	private void CreateTilesTest ()
	{
		Instantiate (tile, new Vector3 (0, 0, 0), Quaternion.LookRotation (Vector3.back));
	}
	private void CreateTiles ()
	{
		// Loops through rows
		for (int i = 0; i < rows; i++)
		{
			// Loops through columns
			for (int j = 0; j < cols; j++)
			{
				// Instantiates new tile (empty object), row by row
				Instantiate (tile, new Vector3 (i + (totemSpacing * i), 0, -j), Quaternion.LookRotation (Vector3.back));
				//tile.position = new Vector3 (i + (totemSpacing * i), 0, -j);
			}
		}
	}

	public void SetTotem (int tileID, int totemID)
	{

	}
}
*/
//Old minesweeper code
/*
	public void CreateTiles ()
	{
		tilesAll = new Tile[totemNum * totemHeight];
		tilesMined = new List<Tile> ();
		tilesUnmined = new List<Tile> ();

		float xOffset = 0.0f;
		float zOffset = 0.0f;
		int count = 0;

		// Instantiates each column/totem of tiles
		for (int tilesCreated = 0; tilesCreated < totemNum; tilesCreated++)
		{
			for (int j = 0; j < totemHeight; j++)
			{
				Tile newTile = (Tile)Instantiate (tilePrefab, new Vector3 (transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset), transform.rotation);
				newTile.ID = tilesCreated;
				newTile.tilesPerRow = tilesPerRow;
				tilesAll [count] = newTile;
				xOffset += distanceBetweenTiles;
				count++;
			}
			zOffset += distanceBetweenTiles;
			xOffset = 0;

		}
		AssignMines ();
	}

	public void AssignMines ()
	{
		tilesUnmined.AddRange (tilesAll);
		for (int minesAssigned = 0; minesAssigned < (totemNum*totemHeight); minesAssigned++)
		{
			Tile currentTile = tilesUnmined [(Random.Range (0, tilesUnmined.Count))];
			tilesMined.Add (currentTile);
			tilesUnmined.Remove (currentTile);

			currentTile.isMined = true;


		}
	}
}*/
