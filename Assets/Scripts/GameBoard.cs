using System;
using System.Collections.Generic;
using Utilities;

public class GameBoard
{
	// Holds the dimentions of the board
	private int cols;
	private int rows;

	// Tracks all the possibilities on the board
	private List<int>[] gameBoardTracker;

	// Tracks the number of a specific symbol left on the board
	private int[] countTracker;

	// Tracks the current solution-in-progress (embiggened symbols only)
	private int[] solutionTracker;

	// Contains the actual solution to the game
	private int[] solution;

	public GameBoard(int _cols, int _rows, int[] _solution)
	{
		cols = _cols;
		rows = _rows;
		gameBoardTracker = new List<int>[cols * rows];
		InitGameBoard ();
		solution = _solution;
	}

	public void ResetBoard ()
	{
		UnityEngine.Debug.Log ("Resetting board");

		// Loop through and reset every single element because C# is absolutely fucking stupid with deep copy
		InitGameBoard ();
		
		// Initialize countTracker and solutionTracker
		countTracker = new int[rows * cols];
		solutionTracker = new int[rows * cols];
		for (int i = 0; i < rows * cols; i++)
		{
			countTracker [i] = rows;
			solutionTracker [i] = -1;
		}
	}

	public bool RemoveSymbol (int index, int symbol)
	{
		bool wasFound = gameBoardTracker [index].Remove (symbol);
		if (wasFound)
		{
			UnityEngine.Debug.Log ("Removing " + symbol + " from tile " + index);
			countTracker [(index / rows) * rows + symbol]--;
			CheckOnlySymbolOnTile (index);
			CheckOnlyRemainingSymbol (index, symbol);
		}
		return wasFound;
	}

	public int FindEmbiggenedSymbol (int totemNum, int symbol)
	{
		// Finds the symbol in the given totem and returns the RELATIVE index
		return solutionTracker.SubArray ((totemNum * rows), rows).GetIndex (symbol);
	}

	public bool IsSymbolOnTile (int index, int symbol)
	{
		return gameBoardTracker [index].Contains (symbol);
	}

	public bool IsSolved ()
	{
		return !solutionTracker.Contains (-1);
	}

	private bool CheckOnlySymbolOnTile (int index)
	{
		// Check to see if there's only one potential symbol left on the tile
		if (gameBoardTracker [index].Count > 1)
		{
			// It's not the last symbol, so just return
			return false;
		}

		int symbol = gameBoardTracker [index] [0];
		UnityEngine.Debug.Log ("Symbol " + symbol + " is the only symbol on tile " + index + ".");
		// This is the last symbol on the tile, so assign it to the tile in the solution tracker
		solutionTracker [index] = symbol;

		// Then remove any other references to this symbol on other tiles
		int totemStartIndex = (index / rows) * rows;
		for (int i = totemStartIndex; i < totemStartIndex + rows; i++)
		{
			// Loop through the totem, but skip the tile that caused this
			if (i != index && gameBoardTracker [i].Remove (symbol))
			{
				// We successfully removed the symbol, so now update the count and call CheckOnlySymbolOnTile just in case there's only one left
				countTracker [totemStartIndex + symbol]--;
				CheckOnlySymbolOnTile (i);
				// We don't need to call CheckOnlyRemainingSymbol because we know we're eliminating all but one of the symbol
			}
		}
		return true;
	}

	private void CheckOnlyRemainingSymbol (int index, int symbol)
	{
		int totemStartIndex = (index / rows) * rows;
		// Check to see if there's only one instance of this symbol left in the totem (and that it's not already assigned to that tile)
		if (countTracker [totemStartIndex + symbol] > 1 || solutionTracker.SubArray (totemStartIndex, rows).Contains (symbol))
		{
			// It's not the last symbol, so just return
			return;
		}
	
		UnityEngine.Debug.Log ("Symbol " + symbol + " is the only symbol of its kind after being eliminated from tile (index " + index + ").");
		// This is the last symbol of its kind, so find the tile that has it and assign it there
		for (int i = totemStartIndex; i < totemStartIndex + rows; i++)
		{
			if (gameBoardTracker [i].Contains (symbol))
			{
				// Found the tile that has the only remaining symbol of its kind, set it in the solution tracker
				solutionTracker [i] = symbol;
				PrintSolverSolution ();
				// Then remove any other symbols from the tile
				for (int j = 0; j < gameBoardTracker[i].Count; j++)
				{
					if (j != symbol && gameBoardTracker [i].Remove (j))
					{
						// We successfully remove a symbol from the tile, so now update the count and call CheckOnlyRemainingSymbol in case there's only one of that kind left
						countTracker [totemStartIndex + j]--;
						CheckOnlyRemainingSymbol (i, j);
						// We don't need to call CheckOnlySymbolOnTile because we know we're eliminating all but one symbol on this tile
					}
				}
				break;
			}
		}
	}
	
	public bool AddSymbol (int index, int symbol)
	{
		if (!gameBoardTracker [index].Contains (symbol))
		{
			gameBoardTracker [index].Add (symbol);
			countTracker [(index / rows) * rows + symbol]++;
			CheckAddToTile (index);
			return true;
		}
		return false;
	}

	public void Ensmallen (int index, int symbol)
	{
		// Called only by the user directly
		// First just call CheckAddToTile since it will unassign the symbol and reset it in the other tiles
		CheckAddToTile (index);

		// Then add each other symbol back to the tile we ensmallened, as long as they're applicable
		for (int i = 0; i < rows; i++)
		{
			// Skip symbols which are already assigned elsewhere and the symbol that is already on this tile
			if (i != symbol && !solutionTracker.SubArray ((index / rows) * rows, rows).Contains (i))
			{
				gameBoardTracker [index].Add (i);
				countTracker [(index / rows) * rows + i]++;
			}
		}
	}

	public void Embiggen (int index, int symbol)
	{
		// Called by the solver when we know a symbol belongs on a tile
		// May be called by the user in the future if we implement left-click = embiggen

		// Loop through the symbols on the tile and remove each one until only the desired symbol is left
		for (int i = 0; i < rows; i++)
		{
			if (i != symbol)
			{
				RemoveSymbol (index, i);
			}
		}
	}
	private void CheckAddToTile (int index)
	{
		// A symbol was added to a tile, so check see if we had already assigned a symbol to that tile
		if (solutionTracker [index] == -1)
		{
			// We hadn't previously assigned a symbol here, so just return
			return;
		}
		
		// We had a symbol assigned to this tile before, so store it for later but clear it from the solution tracker
		int symbol = solutionTracker [index];
		solutionTracker [index] = -1;
		
		// Then reset the symbol we found in the solution tracker on any tile that doesn't have a symbol already assigned
		int totemStartIndex = (index / rows) * rows;
		// But first check to see if any other tiles have this symbol already, which would indicate that the user deliberately 
		// enabled it on a tile even though it was already embiggened. In this case, we don't want to reset any of the other symbols
		if (countTracker [totemStartIndex + symbol] > 1)
		{
			return;
		}
		
		// There were no other symbols of that kind deliberately enabled, so go ahead and add all of them back to the board
		for (int i = totemStartIndex; i < totemStartIndex + rows; i++)
		{
			if (solutionTracker [i] == -1 && i != index)
			{
				gameBoardTracker [i].Add (symbol);
				countTracker [totemStartIndex + symbol]++;
			}
		}
	}

	private void InitGameBoard ()
	{
		// Build the list of starting symbols
		List<int> gbList = new List<int> ();
		for (int i = 0; i < rows; i++)
		{
			gbList.Add (i);
		}

		// Assign each spot on the game board the list of starting symbols
		for (int j = 0; j < rows*cols; j++)
		{
			gameBoardTracker [j] = new List<int> (gbList);
		}
	}

	public void Print ()
	{
		string[] temp = new string[rows * cols];
		for (int i = 0; i < rows*cols; i++)
		{
			temp [i] = gameBoardTracker [i].GetString ("");
		}

		temp.PrintGrid (cols, rows);
	}

	public void PrintSolverSolution ()
	{
		solutionTracker.PrintGrid (cols, rows);
	}
}

