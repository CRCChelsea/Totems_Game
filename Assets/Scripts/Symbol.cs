using UnityEngine;
using System.Collections;

public class Symbol : MonoBehaviour
{
	// Public members
	public int id;
	public bool isActive = true;
	public bool isEmbiggened = false;

	// Members passed from parent
	private GridSpace ggparent_gridSpace;
	private Totem gparent_totem;
	//private Tile parent_tile;
	private int index;
	private int tileNum;
	private int perRow;
	private float tileHeight;
	
	// Private members
	private Renderer rend;
	
	public void Init (GridSpace ggparent, Totem gparent, int symbolID, int rows, int tileIndex, float _tileHeight)
	{
		// Initialize members from parent
		gparent_totem = gparent;
		ggparent_gridSpace = ggparent;
		id = symbolID;
		index = tileIndex;
		tileNum = index % rows;
		perRow = (rows + 1) / 2;
		tileHeight = _tileHeight;

		rend = GetComponent<Renderer> ();
	}
	
	public void OnMouseOver ()
	{
		rend.material.SetColor ("_EmissionColor", new Color (.25f, .25f, .25f, 1f));
	}
	
	public void OnMouseExit ()
	{
		rend.material.SetColor ("_EmissionColor", Color.black);
	}

	public void OnMouseDown ()
	{
		if (isEmbiggened)
		{
			Ensmallen ();
		}
		else if (!isActive)
		{
			Activate ();
		}
		else
		{
			Deactivate ();
		}
	}

	//[Called from Symbol:OnMouseDown, Totem:ActivateAllSymbols, and Totem:ActivateOtherSymbols]
	public void Activate ()
	{
		if (!isActive)
		{
			// Symbol is inactive, so activate it and change the color to white
			rend.material.SetColor ("_Color", Color.white);
			isActive = true;

			gparent_totem.CheckLastInTile (id, tileNum, 1);
		}
	}

	//[Called from Symbol:OnMouseDown, Totem:DeactivateOtherSymbols, and Totem:DeactivateAllSymbols]
	public void Deactivate ()
	{
		if (isActive && !isEmbiggened)
		{
			// Change isActive before calling UpdateSymbolTracking
			//Symbol is active, so deactivate it and change the color to dark gray
			isActive = false;

			gparent_totem.CheckLastInTile (id, tileNum, -1);

			rend.material.SetColor ("_Color", new Color (.35f, .35f, .35f, 1f));
		}
	}

	//[Called from Tile:CheckLastSymbol_Totem and Tile:UpdateTileSymbolCount]
	public void Embiggen (bool isLastOnTile)
	{
		print ("Embiggening symbol " + id);
		gparent_totem.DeactivateOtherSymbols (id, tileNum, isLastOnTile);
		

		// Transform the symbol so that it takes up the entire tile's space
		transform.localScale = new Vector3 (1f, .1f, .7f);
		transform.localPosition = new Vector3 (-(1f / perRow), .0001f, tileHeight / 4);
		isEmbiggened = true;
		
		ggparent_gridSpace.UpdateEmbiggenedSymbols (index, id, true);
	}
	
	//[Called from Symbol:OnMouseDown]
	public void Ensmallen ()
	{
		// Transform the symbol so that it takes up the normal amount of space again
		transform.localScale = new Vector3 ((1f / perRow), .1f, tileHeight / 2);
		transform.localPosition = new Vector3 (-(1f / perRow) * (id % perRow), 0, (id / perRow) * (tileHeight / 2));
		isEmbiggened = false;

		ggparent_gridSpace.UpdateEmbiggenedSymbols (index, -1, false);
		gparent_totem.ActivateOtherSymbols (id, tileNum);
	}
}
