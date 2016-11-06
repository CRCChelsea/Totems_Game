using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour
{
	// Members set from the editor
	public Transform prefab_tile;

	// Members passed in from parent
	private GridSpace parent_gridSpace;
	public int totemNum;
	private int rows;
	
	// Private members
	private int[] symbolCounts;
	private Tile[] tilesList;
	private bool testFlag = true;
	
	public void Init (GridSpace parent, int totalRows, int colNum)
	{
		parent_gridSpace = parent;
		rows = totalRows;
		totemNum = colNum;
		symbolCounts = new int[rows];
		for (int i = 0; i < rows; i++)
		{
			symbolCounts [i] = rows;
		}
		tilesList = new Tile[rows];
		GenerateTotems ();
	}

	public int GetTotemNum ()
	{
		return totemNum;
	}
	
	private void GenerateTotems ()
	{
		// Loops through rows
		for (int i = 0; i < rows; i++)
		{
			// Instantiates a new empty object (tile) for each row
			Transform newTileTF = Instantiate (prefab_tile, new Vector3 (transform.position.x + totemNum, 0, -i - .35f), Quaternion.LookRotation (Vector3.back)) as Transform;
			newTileTF.parent = transform;
			
			// Add the new tile to the tiles array and update the totemNum it stores
			Tile newTile = newTileTF.gameObject.GetComponent<Tile> ();
			newTile.Init (parent_gridSpace, this, rows, (totemNum * rows) + i);
			tilesList [i] = newTile;
		}
	}
	
	// Keeps track of how many of each symbol are active in this totem
	// When one symbol gets down to 1, all other symbols for that tile should be deactivated
	//[Called from Symbol:Activate and Symbol:Deactivate]
	public bool CheckLastInTile (int symbolID, int tileID, int amount)
	{
		bool success = true;
		symbolCounts [symbolID] += amount;

		// If a symbol has only 1 left in the totem, we want to embiggen it
		if (symbolCounts [symbolID] == 1)
		{
			// Loop through each tile until we find the one that has the symbol
			for (int i = 0; i < rows; i++)
			{
				if (tilesList [i].CheckLastInTotem (symbolID, tileID))
				{
					break;
				}
			}
		}

		/*if (symbolCounts [symbolID] == -1)
		{
			// A problem was detected with this symbol, no tiles are available for it
			// Reactivate the symbol that caused this issue
			print ("Symbol " + symbolID + " count was -1");
			symbolCounts [symbolID] = 1;
			return false;
		}*/
		//if (!tilesList [tileID].hasEmbiggened)
		//{
		tilesList [tileID].CheckLastInTile (amount);
		//}

		return success;
	}

	//[Called from Symbol:Ensmallen]
	public void ActivateOtherSymbols (int symbolID, int tileNum)
	{

		// HACK: When we call Symbol:Ensmallen, we don't have a way to set hasEmbiggened on the tile back to false
		// So do that here instead, since Ensmallen always calls this function
		tilesList [tileNum].hasEmbiggened = false;

		// Only activate like symbols if another one hasn't been activated by the user already
		if (symbolCounts [symbolID] == 1)
		{
			foreach (Tile tile in tilesList)
			{
				// Don't activate symbols on tiles that have an embiggened symbol
				if (!tile.hasEmbiggened)
				{
					tile.ActivateSymbol (symbolID);
				}
			}
		}

		ActivateAllSymbols (tileNum, symbolID);
	}
	
	// When a child tile has only one totem remaining, DeactivateOtherSymbols is called to
	// deactivate that totem in the other tiles
	//[Called from Symbol:Embiggen]
	public void DeactivateOtherSymbols (int symbolID, int tileNum, bool isLastOnTile)
	{
		if (isLastOnTile)
		{
			// Only the other tiles need to be updated
			for (int i = 0; i < rows; i++)
			{
				if (i != tileNum)
				{
					tilesList [i].DeactivateSymbol (symbolID);
				}
			}
		}
		else
		{
			// Only this tile needs to be updated
			for (int i = 0; i < rows; i++)
			{
				if (i != symbolID)
				{
					tilesList [tileNum].DeactivateSymbol (i);
				}
			}
		}
	}

	//[Called from Totem:ActivateOtherSymbols]
	public void ActivateAllSymbols (int tileNum, int symbolID)
	{
		// Activate all the totems in the tile that was just ensmallened too, but only if they're valid
		for (int i = 0; i < rows; i++)
		{
			if (!parent_gridSpace.HasEmbiggenedSymbol (totemNum, i))
			{
				tilesList [tileNum].ActivateSymbol (i);
			}
		}
	}

	public void Debug (int tileID, int symbolID)
	{
		print ("tile " + tileID + " cannot be both the symbol it is and symbol " + symbolID);
		if (testFlag)
		{
			symbolCounts [symbolID] = -1;
		}
	}
}
