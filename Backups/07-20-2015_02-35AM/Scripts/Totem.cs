using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour
{
	// This class is the container for the tiles present in an individual totem
	public int id = 0;
	public bool isActive = true;
	public bool isEmbiggened = false;
	private Renderer rend;
	private Tile tileParent;

	public void Start ()
	{
		rend = GetComponent<Renderer> ();
		tileParent = transform.parent.gameObject.GetComponent<Tile> ();
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
			tileParent.Ensmallen (id);
		}
		else if (isActive)
		{
			Deactivate ();
			tileParent.DeactivateTotem (id, true);

		}
		else
		{
			tileParent.ReactivateTotem (id);
			Reactivate ();
		}
	}

	public void Deactivate ()
	{
		// Totem is active, so deactivate it and change color to dark gray
		rend.material.SetColor ("_Color", new Color (.35f, .35f, .35f, 1f));
		isActive = false;
	}

	public void Reactivate ()
	{
		// Totem is inactive, so reactivate it and change color to white
		rend.material.SetColor ("_Color", Color.white);
		isActive = true;
	}
}
