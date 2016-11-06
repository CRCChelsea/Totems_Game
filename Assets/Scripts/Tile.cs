using UnityEngine;
using System.Collections;

// This class is a tile container for potential symbols
//
// Is an empty object
// Generates the right number of potential symbols at start
// Keeps track of the potential symbols that are left for the tile
// Displays the correct symbol in the correct size for that game

public class Tile : MonoBehaviour
{
	// Members set from the editor
	public Transform prefab_symbol;

	// Public members
	public bool hasEmbiggened = false;

	// Members passed in from parent
	private GridSpace gparent_gridSpace;
	private Totem parent_totem;
	public int index;
	private int rows;
	
	// Private members
	private Symbol[] symbolList;
	private int symbolCount = 0;
	private float tileHeight = .7f;

	public void Init (GridSpace gparent, Totem parent, int totalRows, int tileIndex)
	{
		// Initialize members from parent
		gparent_gridSpace = gparent;
		parent_totem = parent;
		rows = totalRows;
		index = tileIndex;

		symbolList = new Symbol[rows];

		// Scale the prefab to the appropriate width for the number of symbols per row
		// Use rows of 4 for 7/8 symbols, 3 for 5/6 symbols, or 2 for 3/4 symbols
		int perRow = (rows + 1) / 2;
		prefab_symbol.transform.localScale = new Vector3 ((1f / perRow), .1f, tileHeight / 2);

		GenerateSymbols (perRow);
	}

	private void GenerateSymbols (int perRow)
	{
		// Loop through each row and column to create the symbols
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < perRow && (i*perRow) + (j % perRow) < rows; j++)
			{
				int symbolID = (i * perRow) + (j % perRow);
				Vector3 position = new Vector3 (-(1f / perRow) * j, 0, i * 0.35f);
				GenerateSymbol (symbolID, position);
			}
		}
	}

	private void GenerateSymbol (int id, Vector3 position)
	{
		// Instantiate the symbol at the origin using the prefab
		Transform newSymbolTF = Instantiate (prefab_symbol, position, Quaternion.LookRotation (Vector3.back)) as Transform;
		
		// Set the symbol to be a child of this empty object and then set the local position
		newSymbolTF.parent = transform;
		newSymbolTF.localPosition = position;
		
		// Set the material according to the symbol ID
		Material newSymbolMat = newSymbolTF.gameObject.GetComponent<Renderer> ().material;
		newSymbolMat.SetColor ("_EmissionColor", Color.black);

		// Set the texture for the material to the correct symbol (from Assets/Resources/Totem#/#.png)
		string fileName = "Totem" + parent_totem.GetTotemNum () + "/" + id;
		Texture texture = Resources.Load (fileName) as Texture;
		newSymbolMat.mainTexture = texture;
		newSymbolMat.SetTexture ("_EmissionMap", texture);
		
		// Set the symbol's id and update the symbols list
		Symbol newSymbol = newSymbolTF.gameObject.GetComponent<Symbol> ();
		newSymbol.Init (gparent_gridSpace, parent_totem, id, rows, index, tileHeight);
		symbolList [id] = newSymbol;
		
		symbolCount++;
	}
	
	// Called by the parent symbol when one of the tiles has the last of a particular symbol
	//[Called from Totem:UpdateSymbolTracking]
	public bool CheckLastInTotem (int id, int callingTileID)
	{
		// Check for the indicated symbol and set it as the only one, if found
		if (symbolList [id].isActive)
		{
			if (symbolList [id].isEmbiggened)
			{
				// The symbol is already embiggened, just return true
				return true;
			}

			if (hasEmbiggened)
			{
				// The tile has an embiggened symbol, but it's not the one we wanted, set error condition
				//symbolList [id].Deactivate (true);
				//CheckLastInTile (-1);
				//parent_totem.Debug (callingTileID, id);
				return true;
			}

			// Always set hasEmbiggened before calling Embiggen or we end up recursively back here and 
			// the !hasEmbiggened check doesn't work
			hasEmbiggened = true;
			symbolList [id].Embiggen (false);
			return true;
		}
		return false;
	}
	
	//[Called from Totem:UpdateSymbolTracking]
	public void CheckLastInTile (int amount)
	{
		symbolCount += amount;
		
		// Check for embiggened since we don't care about symbol count if the tile already has an embiggened symbol
		print ("hasEMbiggened = " + hasEmbiggened + ", symbolCount = " + symbolCount);
		if (!hasEmbiggened && symbolCount == 1)
		{
			print ("Last symbol");
			foreach (Symbol sym in symbolList)
			{
				if (sym.isActive)
				{
					hasEmbiggened = true;
					sym.Embiggen (true);
					break;
				}
			}
		}
	}

	//[Called from Totem:ActivateOtherSymbols]
	public void ActivateSymbol (int symbolID)
	{
		symbolList [symbolID].Activate ();
	}

	//[Called from Totem:DeactivateOtherSymbols]
	public void DeactivateSymbol (int symbolID)
	{
		symbolList [symbolID].Deactivate ();
	}
}
