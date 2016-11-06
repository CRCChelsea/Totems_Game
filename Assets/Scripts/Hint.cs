using UnityEngine;
using System.Collections;

public class Hint
{
	public GameObject hint_prefab;

	public int totem1;
	public int totem2;
	public int symbol1;
	public int symbol2;
	public int symbol3;
	public int relation;

	public Hint(int _totem1, int _symbol1, int _symbol2, int _relation)
	{
		totem1 = _totem1;
		totem2 = -1;
		symbol1 = _symbol1;
		symbol2 = _symbol2;
		symbol3 = -1;
		relation = _relation;
	}

	public Hint(int _totem1, int _symbol1, int _symbol2, int _symbol3_totem2, int _relation)
	{
		totem1 = _totem1;
		symbol1 = _symbol1;
		symbol2 = _symbol2;
		relation = _relation;
		if (_relation == (int)HintGenerator.Relations.Between)
		{
			totem2 = -1;
			symbol3 = _symbol3_totem2;
		}
		else
		{
			totem2 = _symbol3_totem2;
			symbol3 = -1;
		}
	}

	override
	public string ToString ()
	{
		string relationString;
		switch ((HintGenerator.Relations)relation)
		{
		case HintGenerator.Relations.Below:
			relationString = " is below ";
			break;
		case HintGenerator.Relations.Between:
			relationString = " is between ";
			break;
		case HintGenerator.Relations.OnTopOf:
			relationString = " is on top of ";
			break;
		case HintGenerator.Relations.Touching:
			relationString = " is touching ";
			break;
		case HintGenerator.Relations.SameRow:
			relationString = " is in the same row as ";
			break;
		default:
			relationString = " has an unknown relation to ";
			break;
		}
		return ("HINT: " 										// HINT:		HINT: 
			+ symbol1 											// 2			1
			+ ((totem2 != -1) ? (" totem " + totem1 + ")") : "")// 				(totem 0)
			+ relationString 									// is between 	is in the same row as 
			+ symbol2 											// 1			3
			+ ((symbol3 != -1) ? " and " + symbol3 : "") 		// and 4		
			+ " (totem " 										// (totem 		(totem 
			+ ((totem2 != -1) ? totem2 : totem1) 				// 0			1
			+ ").");											// ).			).
	}
}
