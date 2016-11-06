using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;

public class Solver
{
	private int cols;
	private int rows;
	private int[] solution;
	private GameBoard gameBoard;
	private List<Hint> hintList;
	private int hintListIndex;
	private bool isSolved = false;

	// Constructor
	public Solver(int _cols, int _rows, int[] _solution)
	{
		cols = _cols;
		rows = _rows;
		solution = _solution;
		gameBoard = new GameBoard (cols, rows, solution);
	}

	public bool Solve (List<Hint> hints)
	{
		// Attempts to solve the puzzle given the hints in hintList and finalHintList
		// Returns true if the puzzle was solvable, and false if not
		gameBoard.ResetBoard ();
		bool hasChanged = true;

		// Create a new list so that we can remove hints as we finish them without affecting the real list
		hintList = new List<Hint> (hints);

		UnityEngine.Debug.Log ("HintList contains " + hintList.Count + " hints");

		// Now try to solve the puzzle
		while (!isSolved && hasChanged)
		{
			hasChanged = false;
			//foreach (Hint hint in hintList)
			for (hintListIndex = 0; hintListIndex < hintList.Count; hintListIndex++)
			{
				Hint hint = hintList [hintListIndex];
				switch ((HintGenerator.Relations)hint.relation)
				{
				case HintGenerator.Relations.Below:
					hasChanged |= checkBelow (hint);
					break;
				case HintGenerator.Relations.Between:
					hasChanged |= checkBetween (hint);
					break;
				case HintGenerator.Relations.OnTopOf:
					hasChanged |= checkOnTopOf (hint);
					break;
				case HintGenerator.Relations.Touching:
					hasChanged |= checkTouching (hint);
					break;
				case HintGenerator.Relations.SameRow:
					hasChanged |= checkSameRow (hint);
					break;
				default:
					break;
				}
				if (gameBoard.IsSolved ())
				{
					UnityEngine.Debug.Log ("Solved!");
					gameBoard.PrintSolverSolution ();
					return true;
				}
			}
		}
		gameBoard.Print ();
		gameBoard.PrintSolverSolution ();

		return false;
	}

	private bool checkBelow (Hint hint)
	{
		bool madeUpdate = false;
		// Hint: sym1 is below sym2 in this totem
		int sym1 = hint.symbol1;
		int sym2 = hint.symbol2;
		int totemNum = hint.totem1;

		UnityEngine.Debug.Log (hint.ToString ());

		// Check them to see if we've already identified the tiles sym1 and sym2 are on
		int index1 = gameBoard.FindEmbiggenedSymbol (totemNum, sym1); // Relative index
		int index2 = gameBoard.FindEmbiggenedSymbol (totemNum, sym2); // Relative index
		 
		if (index1 != -1)
		{
			// We have a dedicated tile for symbol 1, so check symbol 2
			if (index2 != -1)
			{
				// We also have a dedicated tile for symbol 2, meaning the hint is done, remove it and return false
				hintList.Remove (hint);
				hintListIndex--;
				return false;
			}
			else
			{
				// We don't have a dedicated tile for symbol 2, so remove symbol 2 from any tiles below symbol 1's tile
				for (int i = rows - 1; i > index1; i--)
				{
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + i, sym2);
				}
				return madeUpdate;
			}
		}
		// We don't have a dedicated tile for symbol 1 yet, so check symbol 2
		else if (index2 != -1)
		{
			// We have a dedicated tile for symbol 2, so remove symbol 1 from any tiles above symbol 2's tile
			for (int j = 0; j < index2; j++)
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + j, sym1);
			}
			return madeUpdate;
		}

		// We don't have a dedicated tile for either symbol, so go into elimination loop
		// First tile has special checking, since nothing can be above it
		madeUpdate |= gameBoard.RemoveSymbol (rows * totemNum, sym1);

		// Middle tiles should be checked for both above and below
		for (int k = 1; k < rows-1; k++)
		{
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + k, sym1))
			{
				// Tile has symbol 1 available on it, so make sure symbol 2 is somewhere above it
				bool removeSym1 = true;
				for (int m = 0; m < k; m++)
				{
					if (gameBoard.IsSymbolOnTile ((rows * totemNum) + m, sym2))
					{
						removeSym1 = false;
						break;
					}
				}

				if (removeSym1)
				{
					// Symbol 2 wasn't found above the tile, so remove symbol 1
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + k, sym1);
				}
			}

			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + k, sym2))
			{
				// Tile has symbol 2 available on it, so make sure symbol 1 is somewhere below it
				bool removeSym2 = true;
				for (int n = k + 1; n <= rows-1; n++)
				{
					if (gameBoard.IsSymbolOnTile ((rows * totemNum) + n, sym1))
					{
						removeSym2 = false;
						break;
					}
				}

				if (removeSym2)
				{
					// Symbol 1 wasn't found below the tile, so remove symbol 2
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + k, sym2);
				}
			}
		}

		// Last tile also has special checking, since nothing can be below it
		madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym2);

		gameBoard.Print ();
		/*
		 * For BELOW (sym1 is below sym 2 - totem only)

				Elimination loop- loop through every tile in totem:
				Does tile have symbol 1?
				Yes. Do any of the above tiles have symbol 2?
				Yes. Check next tile.
				No. Remove symbol 1 from tile.
				No. Check next tile.
				Does tile have symbol 2?
				Yes. Do any of the below tiles have symbol 1?
				Yes. Check next tile.
				No. Remove symbol 2 from tile.
				No. Check next tile
				*/
		return false;
	}

	private bool checkBetweenCenterGiven (int index1, int sym2, int sym3, int totemNum)
	{
		bool madeUpdate = false;
		for (int i = 0; i < rows; i++)
		{
			if (i == index1)
			{
				// We can just skip the tile symbol 1 is on
				continue;
			}
			
			if (i != index1 - 1 && i != index1 + 1)
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + i, sym2);
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + i, sym3);
			}
			else
			{
				// For the two touching tiles, we can actually remove all other symbols
				for (int j = 0; j < rows; j++)
				{
					if (j != sym2 && j != sym3)
					{
						madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + i, j);
					}
				}
			}
		}
		return madeUpdate;
	}

	private bool checkBetweenEdgeGiven (int index, int sym1, int otherSym, int totemNum)
	{
		bool madeUpdate = false;
		// The between "stack" can only go upward from the edge symbol's tile if there are two tiles above it
		if (index > 1)
		{
			for (int n = 0; n < index; n++)
			{
				if (n == index - 2)
				{
					// This one could be the other edge symbol, so just remove the center symbol
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + n, sym1);
				}
				else if (n == index - 1)
				{
					// This one could be the center symbol, so just remove the other edge symbol
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + n, otherSym);
				}
				else
				{
					// This one can't be either the center or the other edge symbols, so remove both
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + n, sym1);
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + n, otherSym);
				}
			}
		}
		
		// The between "stack" can only go downward from the edge symbol's tile if there are two tiles below it
		if (index < rows - 2)
		{
			for (int p = index + 1; p < rows; p++)
			{
				// The between "stack" can only go downward from the edge symbol's tile if there are two tiles below it
				if (p == index + 2)
				{
					// This one could be the other edge symbol, so just remove the center symbol
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + p, sym1);
				}
				else if (p == index + 1)
				{
					// This one could be the other edge symbol, so just remove the center symbol
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + p, otherSym);
				}
				else
				{
					// This one can't be either the center or the other edge symbols, so remove both
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + p, sym1);
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + p, otherSym);
				}
			}
		}
		return madeUpdate;
	}

	private bool checkBetween0Given (int sym1, int sym2, int sym3, int totemNum)
	{
		bool madeUpdate = false;

		// Check for 3-tile case first
		if (rows == 3)
		{
			madeUpdate |= gameBoard.RemoveSymbol (rows * totemNum, sym1);
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym1);
			return madeUpdate;
		}		
		
		// First tile has special checking for edge symbols & center symbol can never be on this tile, so remove right away
		madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum), sym1);
		
		// Symbol 2 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum), sym2) 
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + 2, sym3)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum), sym2);
		}
		
		// Symbol 3 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum), sym3) 
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + 2, sym2)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum), sym3);
		}
		
		// Second tile also has special checking for edge symbols
		// Symbol 1 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym1)
			&& !((gameBoard.IsSymbolOnTile ((rows * totemNum), sym2) && gameBoard.IsSymbolOnTile ((rows * totemNum) + 2, sym3)) 
			|| (gameBoard.IsSymbolOnTile ((rows * totemNum), sym3) && gameBoard.IsSymbolOnTile ((rows * totemNum) + 2, sym2))))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + 1, sym1);
		}
		
		// Symbol 2 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym2)
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + 2, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + 3, sym3)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + 1, sym2);
		}
		
		// Symbol 3 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym3)
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + 2, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + 3, sym2)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + 1, sym3);
		}
		
		// Center tiles (in 4-tile or more games) need to be checked for both directions
		for (int r = 2; r < rows - 2; r++)
		{
			// Symbol 1 check
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + r, sym1)
				&& !((gameBoard.IsSymbolOnTile ((rows * totemNum) + r - 1, sym2) && gameBoard.IsSymbolOnTile ((rows * totemNum) + r + 1, sym3)) 
				|| (gameBoard.IsSymbolOnTile ((rows * totemNum) + r - 1, sym3) && gameBoard.IsSymbolOnTile ((rows * totemNum) + r + 1, sym2))))
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + r, sym1);
			}
			
			// Symbol 2 check
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + r, sym2)
				&& !((gameBoard.IsSymbolOnTile ((rows * totemNum) + r + 1, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + r + 2, sym3)) 
				|| (gameBoard.IsSymbolOnTile ((rows * totemNum) + r - 1, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + r - 2, sym3))))
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + r, sym2);
			}
			
			// Symbol 3 check
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + r, sym3)
				&& !((gameBoard.IsSymbolOnTile ((rows * totemNum) + r + 1, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + r + 2, sym2)) 
				|| (gameBoard.IsSymbolOnTile ((rows * totemNum) + r - 1, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + r - 2, sym2))))
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + r, sym3);
			}
		}
		
		// Second to last tile also has special checking for edge symbols
		// Symbol 1 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym1)
			&& !((gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 3, sym2) && gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym3)) 
			|| (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 3, sym3) && gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym2))))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 2, sym1);
		}
		
		// Symbol 2 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym2)
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 3, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 4, sym3)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 2, sym2);
		}
		
		// Symbol 3 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym3)
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 3, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 4, sym2)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 2, sym3);
		}
		
		// Last tile has special checking for edge symbols & center symbol can never be on this tile, so remove right away
		madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym1);
		
		// Symbol 2 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym2) 
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 3, sym3)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym2);
		}
		
		// Symbol 3 check
		if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym3) 
			&& !(gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym1) && gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 3, sym2)))
		{
			madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym3);
		}
		return madeUpdate;
	}

	private bool checkBetween (Hint hint)
	{
		bool madeUpdate = false;
		// Hint: sym1 is between sym2 and sym3 in this totem
		int sym1 = hint.symbol1;
		int sym2 = hint.symbol2;
		int sym3 = hint.symbol3;
		int totemNum = hint.totem1;
		
		UnityEngine.Debug.Log (hint.ToString ());
		
		// Check them to see if we've already identified the tiles sym1, sym2 or sym3 are on
		int index1 = gameBoard.FindEmbiggenedSymbol (totemNum, sym1); // Relative index
		int index2 = gameBoard.FindEmbiggenedSymbol (totemNum, sym2); // Relative index
		int index3 = gameBoard.FindEmbiggenedSymbol (totemNum, sym3); // Relative index
		
		if (index1 != -1 && index2 != -1 && index3 != -1)
		{
			// We have a dedicated tile for all three symbols, so the hint is done, remove it and return false
			hintList.Remove (hint);
			hintListIndex--;
			return false;
		}

		if (index1 != -1 && (index2 != -1 || index3 != -1))
		{
			// We have a dedicated tile for the center symbol and one edge symbol
			// So we can figure out where the other edge symbol belongs
			if (index2 != -1)
			{
				gameBoard.Embiggen ((rows * totemNum) + ((index1 < index2) ? (index1 - 1) : (index1 + 1)), sym3);
			}
			else
			{
				gameBoard.Embiggen ((rows * totemNum) + ((index1 < index3) ? (index1 - 1) : (index1 + 1)), sym2);
			}

			// Since we now have a dedicated tile for all three symbols, remove the hint from the list
			hintList.Remove (hint);
			hintListIndex--;
			return true;
		}

		if (index2 != -1 && index3 != -1)
		{
			// We have dedicated tiles for the two edge symbols, so we know where the center symbol belongs
			// Just remove all the other symbols on the tile
			gameBoard.Embiggen ((rows * totemNum) + (index2 + index3) / 2, sym1);
			// Since we now have a dedicaed tile for all three symbols, remove the hint from the list
			hintList.Remove (hint);
			hintListIndex--;
			return true;
		}

		// Otherwise, we don't have enough information to determine which tile the symbols belong on, so try using just one index
		if (index1 != -1 || index2 != -1 || index3 != -1)
		{
			// We have a dedicated tile for one of the symbols, so try and use that to narrow down the others
			if (index1 != -1)
			{
				// The edge symbols must be touching the center symbol, so remove those symbols from non-touching tiles
				madeUpdate = checkBetweenCenterGiven (index1, sym2, sym3, totemNum);
			}
			else if (index2 != -1)
			{
				madeUpdate = checkBetweenEdgeGiven (index2, sym1, sym3, totemNum);
			}
			else
			{
				madeUpdate = checkBetweenEdgeGiven (index3, sym1, sym2, totemNum);
			}

			return madeUpdate;
		}

		// We don't have a dedicated tile for any symbol, so go into elimination loop
		madeUpdate = checkBetween0Given (sym1, sym2, sym3, totemNum);
		return madeUpdate;
	}

	private bool checkOnTopOf (Hint hint)
	{
		bool madeUpdate = false;
		// Hint: sym1 is on top of sym2 in this totem
		int sym1 = hint.symbol1;
		int sym2 = hint.symbol2;
		int totemNum = hint.totem1;
		
		UnityEngine.Debug.Log (hint.ToString ());
		
		// Check them to see if we've already identified the tiles sym1 and sym2 are on
		int index1 = gameBoard.FindEmbiggenedSymbol (totemNum, sym1); // Relative index
		int index2 = gameBoard.FindEmbiggenedSymbol (totemNum, sym2); // Relative index
		
		if (index1 != -1)
		{
			// We have a dedicated tile for symbol 1, so check symbol 2
			if (index2 != -1)
			{
				// We also have a dedicated tile for symbol 2, so the hint is done, remove it from the list and return false
				hintList.Remove (hint);
				hintListIndex--;
				return false;
			}
			else
			{
				// We don't have a dedicated tile for symbol 2, but we know it's directly below symbol 1's tile
				gameBoard.Embiggen ((rows * totemNum) + (index1 + 1), sym2);
				// Since we now have a dedicated tile for both symbols, the hint is done, remove it from the list
				hintList.Remove (hint);
				hintListIndex--;
				return true;
			}
		}
		else
		{
			// We don't have a dedicated tile for symbol 1 yet, so check symbol 2
			if (index2 != -1)
			{
				// We have a dedicated tile for symbol 2, so we know symbol 1's tile is directly above it
				gameBoard.Embiggen ((rows * totemNum) + (index2 - 1), sym1);
				// Since we now have a dedicated tile for both symbols, the hint is done, remove it from the list
				hintList.Remove (hint);
				hintListIndex--;
				return true;
			}
			
			// We don't have a dedicated tile for either symbol, so go into elimination loop
			// First tile has special checking, since nothing can be on top of it (so symbol 2 can't be here)
			if (gameBoard.IsSymbolOnTile (rows * totemNum, sym2))
			{
				madeUpdate |= gameBoard.RemoveSymbol (rows * totemNum, sym2);
			}
			
			// Middle tiles should be checked for both symbols
			for (int k = 1; k < rows-1; k++)
			{
				if (gameBoard.IsSymbolOnTile ((rows * totemNum) + k, sym1))
				{
					// Tile has symbol 1 available on it, so make sure symbol 2 is directly below it
					if (!gameBoard.IsSymbolOnTile ((rows * totemNum) + k + 1, sym2))
					{
						// Symbol 2 wasn't found, so remove symbol 1
						madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + k, sym1);
					}
				}
				
				if (gameBoard.IsSymbolOnTile ((rows * totemNum) + k, sym2))
				{
					// Tile has symbol 2 available on it, so make sure symbol 1 directly above it
					if (!gameBoard.IsSymbolOnTile ((rows * totemNum) + k - 1, sym1))
					{
						// Symbol 1 wasn't found below the tile, so remove symbol 2
						madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + k, sym2);
					}
				}
			}
			
			// Last tile also has special checking, since nothing can be under it (so symbol 1 can't be here)
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym1))
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym1);
			}
		}
		return madeUpdate;
	}

	private bool checkTouching (Hint hint)
	{
		bool madeUpdate = false;
		// Hint: sym1 is touching sym2 in this totem
		int sym1 = hint.symbol1;
		int sym2 = hint.symbol2;
		int totemNum = hint.totem1;
		
		UnityEngine.Debug.Log (hint.ToString ());
		
		// Check them to see if we've already identified the tiles sym1 and sym2 are on
		int index1 = gameBoard.FindEmbiggenedSymbol (totemNum, sym1); // Relative index
		int index2 = gameBoard.FindEmbiggenedSymbol (totemNum, sym2); // Relative index
		
		if (index1 != -1)
		{
			// We have a dedicated tile for symbol 1, so check symbol 2
			if (index2 != -1)
			{
				// We also have a dedicated tile for symbol 2, so the hint is done, remove it from the list and return false
				hintList.Remove (hint);
				hintListIndex--;
				return false;
			}
			else
			{
				// We don't have a dedicated tile for symbol 2, but we know it's directly above or below symbol 1's tile
				// So remove symbol 2 from all other tiles
				for (int i = 0; i < rows; i++)
				{
					if (i != index1 - 1 && i != index1 + 1)
					{
						madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + i, sym2);
					}
				}
				return madeUpdate;
			}
		}
		else
		{
			// We don't have a dedicated tile for symbol 1 yet, so check symbol 2
			if (index2 != -1)
			{
				// We have a dedicated tile for symbol 2, so we know symbol 1's tile is directly above or below it
				// Remove symbol 1 from all other tiles
				for (int j = 0; j < rows; j++)
				{
					if (j != index2 - 1 && j != index2 + 1)
					{
						madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + j, sym1);
					}
				}
				return madeUpdate;
			}
			
			// We don't have a dedicated tile for either symbol, so go into elimination loop
			// First tile has special checking, since nothing can be on top of it
			if (gameBoard.IsSymbolOnTile (rows * totemNum, sym1) && !gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym2))
			{
				madeUpdate |= gameBoard.RemoveSymbol (rows * totemNum, sym1);
			}

			if (gameBoard.IsSymbolOnTile (rows * totemNum, sym2) && !gameBoard.IsSymbolOnTile ((rows * totemNum) + 1, sym1))
			{
				madeUpdate |= gameBoard.RemoveSymbol (rows * totemNum, sym2);
			}
			
			// Check middle tiles to ensure symbol 1 is always touching symbol 2, and vice versa
			for (int k = 1; k < rows-1; k++)
			{
				// Symbol 1 check
				if (gameBoard.IsSymbolOnTile ((rows * totemNum) + k, sym1) && !(gameBoard.IsSymbolOnTile ((rows * totemNum) + (k - 1), sym2) || gameBoard.IsSymbolOnTile ((rows * totemNum) + (k + 1), sym2)))
				{
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + k, sym1);
				}

				// Symbol 2 check
				if (gameBoard.IsSymbolOnTile ((rows * totemNum) + k, sym2) && !(gameBoard.IsSymbolOnTile ((rows * totemNum) + (k - 1), sym1) || gameBoard.IsSymbolOnTile ((rows * totemNum) + (k + 1), sym1)))
				{
					madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + k, sym2);
				}
			}
			
			// Last tile also has special checking, since nothing can be under it
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym1) && !gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym2))
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym1);
			}
			
			if (gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 1, sym2) && !gameBoard.IsSymbolOnTile ((rows * totemNum) + rows - 2, sym1))
			{
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totemNum) + rows - 1, sym2);
			}
		}
		return madeUpdate;
	}

	private bool checkSameRow (Hint hint)
	{
		bool madeUpdate = false;
		// Hint: sym1 in totem 1 is in the same row as sym2 in totem 2
		int sym1 = hint.symbol1;
		int sym2 = hint.symbol2;
		int totem1 = hint.totem1;
		int totem2 = hint.totem2;
		
		UnityEngine.Debug.Log (hint.ToString ());
		
		// Check them to see if we've already identified the tiles sym1 and sym2 are on
		int index1 = gameBoard.FindEmbiggenedSymbol (totem1, sym1); // Relative index
		int index2 = gameBoard.FindEmbiggenedSymbol (totem2, sym2); // Relative index
		
		if (index1 != -1)
		{
			// We have a dedicated tile for symbol 1, so check symbol 2
			if (index2 != -1)
			{
				// We also have a dedicated tile for symbol 2, so the hint is done, remove it from the list and return false
				hintList.Remove (hint);
				hintListIndex--;
				return false;
			}
			else
			{
				// We don't have a dedicated tile for symbol 2, but we know where it goes
				gameBoard.Embiggen ((rows * totem2) + index1, sym2);
				// Now we have a dedicated tile for both symbols, so remove the hint from the list
				hintList.Remove (hint);
				hintListIndex--;
				return true;
			}
		}
		else
		{
			// We don't have a dedicated tile for symbol 1 yet, so check symbol 2
			if (index2 != -1)
			{
				// We have a dedicated tile for symbol 2, so we know symbol 1's tile is in the same row
				gameBoard.Embiggen ((rows * totem1) + index2, sym1);
				// Now we have a dedicated tile for both symbols, so remove the hint from the list
				hintList.Remove (hint);
				hintListIndex--;
				return true;
			}
		}
			
		// We don't have a dedicated tile for either symbol, so go into elimination loop
		for (int k = 0; k < rows; k++)
		{
			if (gameBoard.IsSymbolOnTile ((rows * totem1) + k, sym1) && !gameBoard.IsSymbolOnTile ((rows * totem2) + k, sym2))
			{
				// Symbol 1 is available in this row but symbol 2 isn't, so remove symbol 1
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totem1) + k, sym1);
			}

			if (gameBoard.IsSymbolOnTile ((rows * totem2) + k, sym2) && !gameBoard.IsSymbolOnTile ((rows * totem1) + k, sym1))
			{
				// Symbol 2 is available in this row but symbol 1 isn't, so remove symbol 2
				madeUpdate |= gameBoard.RemoveSymbol ((rows * totem2) + k, sym2);
			}
		}
		return madeUpdate;
	}
}
