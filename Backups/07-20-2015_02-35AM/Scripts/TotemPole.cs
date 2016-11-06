using UnityEngine;
using System.Collections;

public class TotemPole : MonoBehaviour
{
	public int totalRows = 5;
	public int totemNum;
	public Transform tile;

	// Private members
	private int[] totemCounts;
	private Tile[] tilesList;
		
	public void Start ()
	{
		totemCounts = new int[totalRows];
		for (int i = 0; i < totalRows; i++)
		{
			totemCounts [i] = totalRows;
		}
		tilesList = new Tile[totalRows];
		CreateTotems ();
	}

	private void CreateTotems ()
	{
		// Loops through rows
		for (int i = 0; i < totalRows; i++)
		{
			// Instantiates a new empty object (tile) for each row
			Transform newTileTF = Instantiate (tile, new Vector3 (transform.position.x + totemNum, 0, -i), Quaternion.LookRotation (Vector3.back)) as Transform;
			newTileTF.parent = transform;

			// Add the new tile to the tiles array and update the totemNum it stores
			tilesList [i] = newTileTF.gameObject.GetComponent<Tile> ();
			tilesList [i].rowNum = i;
		}
	}

	// Keeps track of how many of each totem are active in this totem pole
	// When one totem gets down to 1, all other totems for that tile should be deactivated
	public void trackTotemCounts (int totemID, int number)
	{
		totemCounts [totemID] += number;

		if (totemCounts [totemID] == 1)
		{
			// Only one tile has this totem, so call LastTotem on each tile until that tile is found
			for (int i = 0; i < totalRows && !tilesList[i].LastTotem (totemID); i++)
			{
				;
			}
		}
	}

	// When a child tile has only one totem remaining, DeactivateOtherTotems is called to
	// deactivate that totem in the other tiles
	public void DeactivateOtherTotems (int totemID, int rowNum)
	{
		foreach (Tile tile in tilesList)
		{
			if (tile.rowNum != rowNum)
			{
				tile.DeactivateTotem (totemID, false);
			}
			else
			{
				tile.Embiggen (totemID);
			}
		}
	}

	public void ReactivateOtherTotems (int totemID, int rowNum)
	{
		if (totemCounts [totemID] == 1)
		{
			foreach (Tile tile in tilesList)
			{
				if (tile.rowNum != rowNum && !tile.hasEmbiggened)
				{
					tile.ReactivateTotem (totemID);
				}
			}
		}

		// Activate all the totems in the tile that was just ensmallened too
		for (int i = 0; i < totalRows; i++)
		{
			tilesList [rowNum].ReactivateTotem (i);
		}
	}
}
