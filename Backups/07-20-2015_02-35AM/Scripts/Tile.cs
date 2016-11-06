using UnityEngine;
using System.Collections;

// This class is a tile container for potential totems
//
// Is an empty object
// Generates the right number of potential totems at start
// Keeps track of the potential totems that are left for the tile
// Displays the correct totem in the correct size for that game
public class Tile : MonoBehaviour
{
	// Public members
	public int rows;
	public int rowNum;
	public float tileHeight = .7f;
	public bool hasEmbiggened = false;

	// Objects
	public Transform totem_prefab;
	private TotemPole totemPole;
	private Totem[] totemList;
	private Texture[]textures;

	// Private members
	private int totemCount = 0;
	private int perRow;

	public void Start ()
	{
		totemPole = transform.parent.gameObject.GetComponent<TotemPole> ();

		// Initialize textures and totems array
		textures = new Texture[rows];
		totemList = new Totem[rows];

		// Populate textures array with textures from Resources/Totems#/#.extension
		for (int i = 0; i < rows; i++)
		{
			string fileName = "Totem" + totemPole.totemNum + "/" + i;
			textures [i] = Resources.Load (fileName) as Texture;
		}

		// Scale the prefab to the appropriate width for the number of totems per row
		// Use rows of 4 for 7/8 totems, 3 for 5/6 totems, or 2 for 3/4 totems
		perRow = (rows + 1) / 2;
		totem_prefab.transform.localScale = new Vector3 ((1f / perRow), .1f, tileHeight / 2);

		// Loop through each row to create the totems
		for (int i = 0; i < 2; i++)
		{
			// Loop through each column
			for (int j = 0; j < perRow; j++)
			{
				int totemID = (i * perRow) + (j % perRow);
				Vector3 position = new Vector3 (-(1f / perRow) * j, 0, i * 0.35f);
				CreateTotem (totemID, position);
			}
		}
	}

	private void CreateTotem (int id, Vector3 position)
	{
		// Instantiate the totem at the origin using the prefab
		Transform newTotemTF = Instantiate (totem_prefab, position, Quaternion.LookRotation (Vector3.back)) as Transform;

		// Set the totem to be a child of this empty object and then set the local position
		newTotemTF.parent = transform;
		newTotemTF.localPosition = position;

		// Set the material according to the totem ID
		Material newTotemMat = newTotemTF.gameObject.GetComponent<Renderer> ().material;
		newTotemMat.mainTexture = textures [id];
		newTotemMat.SetTexture ("_EmissionMap", textures [id]);
		newTotemMat.SetColor ("_EmissionColor", Color.black);

		// Set the totem's id and update the totems list
		Totem newTotem = newTotemTF.gameObject.GetComponent<Totem> ();
		newTotem.id = id;
		totemList [id] = newTotem;

		totemCount++;
	}

	public void ReactivateTotem (int id)
	{
		if (!totemList [id].isActive)
		{
			totemCount++;
			totemPole.trackTotemCounts (id, 1);
			totemList [id].Reactivate ();
		}
	}

	public void DeactivateTotem (int id, bool userAction)
	{
		if (totemList [id].isActive || userAction)
		{
			// This totem was active, so decrement totemCount
			totemCount--;
			if (!userAction)
			{
				// This action was due to a cascading (non-user) effect so Deactivate hasn't been called
				totemList [id].Deactivate ();
			}

			totemList [id].isActive = false;
			// If there's only one totem left, figure out which one it is and deactivate it in other tiles
			if (totemCount == 1)
			{
				for (int i = 0; i < rows; i++)
				{
					if (totemList [i].isActive)
					{
						totemPole.DeactivateOtherTotems (i, rowNum);
					}
				}
			}

			totemPole.trackTotemCounts (id, -1);
		}

		print ("Number of totems left = " + totemCount);
	}

	// Called by the parent totem pole when one of the tiles has the last of a particular totem
	public bool LastTotem (int id)
	{
		// Check for the indicated totem and set it as the only one, if found
		if (totemList [id].isActive)
		{
			// This tile is the one with the totem, also return true so we don't check any other tiles
			for (int i = 0; i < rows; i++)
			{
				if (i != id)
				{
					DeactivateTotem (i, false);
				}
			}
			return true;
		}
		return false;
	}

	public void Embiggen (int totemID)
	{
		// Transform the totem so that it takes up the entire tile's space
		Transform totemTF = totemList [totemID].transform;
		totemTF.localScale = new Vector3 (1f, .1f, .7f);
		totemTF.localPosition = new Vector3 (-(1f / perRow), .0001f, tileHeight / 4);
		totemList [totemID].isEmbiggened = true;
		hasEmbiggened = true;

	}

	public void Ensmallen (int totemID)
	{
		// Transform the totem so that it takes up the normal amount of space again
		Transform totemTF = totemList [totemID].transform;
		totemTF.localScale = new Vector3 ((1f / perRow), .1f, tileHeight / 2);
		totemTF.localPosition = new Vector3 (-(1f / perRow) * (totemID % perRow), 0, (totemID / perRow) * (tileHeight / 2));
		totemList [totemID].isEmbiggened = false;
		totemPole.ReactivateOtherTotems (totemID, rowNum);
		hasEmbiggened = false;
	}
}
