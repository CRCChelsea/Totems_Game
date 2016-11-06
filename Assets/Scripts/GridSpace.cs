using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GridSpace : MonoBehaviour
{
	// Members set from the editor
	public Material bgMaterial;

	// Members passed in from parent
	private int rows;
	private int cols;
	private float totemSpacing;
	private int[] solution;
	public Transform totem; // Not in design

	// Private members
	private int embiggenedCount = 0;
	private int[] symbolMap;
	
	public void BuildGrid (int _rows, int _cols, float _totemSpacing, int[] _solution)
	{
		rows = _rows;
		cols = _cols;
		totemSpacing = _totemSpacing;
		solution = _solution;

		symbolMap = new int[rows * cols];
		// Populate symbolMap with -1 to indicate no values are embiggened
		for (int i = 0; i < rows * cols; i++)
		{
			symbolMap [i] = -1;
		}

		CreateBackground ();
		CreateTotems ();
	}
	
	private void CreateTotems ()
	{
		for (int i = 0; i < cols; i++)
		{
			Transform newTotemTF = Instantiate (totem, new Vector3 (i * totemSpacing, 0, 0), Quaternion.LookRotation (Vector3.back)) as Transform;
			Totem newTotem = newTotemTF.gameObject.GetComponent<Totem> ();
			newTotem.Init (this, rows, i);
		}
	}
	
	private void CreateBackground ()
	{
		float xPos = cols / 2f + totemSpacing;
		float zPos = -rows / 2f;
		float xScale = (cols + (totemSpacing * cols)) / 10f;
		float zScale = rows / 10f;
		
		GameObject bg = GameObject.CreatePrimitive (PrimitiveType.Plane);
		bg.transform.position = new Vector3 (xPos, -.001f, zPos);
		bg.transform.localScale = new Vector3 (xScale, 1f, zScale);
		bg.GetComponent<Renderer> ().material = bgMaterial;
	}

	//Called from Symbol:Embiggen and Symbol:Ensmallen
	public void UpdateEmbiggenedSymbols (int index, int symbolID, bool isEmbiggened)
	{
		// Add or subtract one, based on whether this is called from embiggen or ensmallen
		embiggenedCount += (isEmbiggened) ? 1 : -1;
		symbolMap [index] = symbolID;

		if (embiggenedCount == rows * cols)
		{
			// All tiles are embiggened
			if (symbolMap.SequenceEqual (solution))
			{
				print ("You win!");
			}
		}
	}

	public bool HasEmbiggenedSymbol (int totemNum, int symbolID)
	{
		for (int i = totemNum*rows; i < totemNum*rows+rows; i++)
		{
			if (symbolMap [i] == symbolID)
			{
				return true;
			}
		}
		return false;
	}
}
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
