using System;
using System.Collections.Generic;
using Utilities;

public class HintGenerator
{
	private int cols;
	private int rows;
	private int[] solution;
	private List<Hint> hintList;

	public enum Relations
	{
		Below,
		Between,
		OnTopOf,
		Touching,
		SameRow
	}
	;

	// Used for unit testing hint lists
	public HintGenerator(int _cols, int _rows, int[] _solution)
	{
		cols = _cols;
		rows = _rows;
		solution = _solution;
	}

	public List<Hint> GetPrunedHintList ()
	{
		// This generates a master list of all possible hints for the solution
		hintList = new List<Hint> ();
		hintList.AddRange (GetTouchingHints ());
		hintList.AddRange (GetBelowHints ());
		hintList.AddRange (GetSameRowHints ());

		// Then prunes the list by removing 1 hint at a time and only keeping the ones the solver needs
		Solver solver = new Solver (cols, rows, solution);
		//return AltPruneHints (solver);
		return PruneHints (solver);
	}
	private List<Hint> AltPruneHints (Solver solver)
	{
		// Instead of starting with a large list, start with just two hints and build upwards
		List<Hint> hintsToUse = new List<Hint> ();
		Random rand = new Random ();

		// Add the starting hints (equal to the number of rows + cols in the puzzle)
		for (int i = 0; i < rows + cols; i++)
		{
			int index = rand.Next (hintList.Count);
			hintsToUse.Add (hintList [index]);
			hintList.RemoveAt (index);
		}

		// Now try to solve and add another two hints every time we can't
		while (!solver.Solve (hintsToUse))
		{
			hintsToUse.Add (TakeHintFromHintList (rand));
			hintsToUse.Add (TakeHintFromHintList (rand));
		}

		// Once we have a list of hints we know can solve the puzzle, prune redundancies
		hintList = hintsToUse;
		return PruneHints (solver);
	}

	private Hint TakeHintFromHintList (Random rand)
	{
		int index = rand.Next (hintList.Count);
		Hint hint = hintList [index];
		hintList.RemoveAt (index);
		return hint;
	}

	private List<Hint> PruneHints (Solver solver)
	{
		// Remove hints at random until Solver can't solve the puzzle
		List<Hint> masterHintList = hintList;
		List<Hint> hintsToUse = new List<Hint> ();
		List<Hint> hintsRemoved = new List<Hint> ();
		Random rand = new Random ();

		// For better efficiency, remove half the hints right off the bat
		int half = masterHintList.Count / 2;
		while (masterHintList.Count > half)
		{
			int index = rand.Next (masterHintList.Count);
			hintsRemoved.Add (masterHintList [index]);
			masterHintList.RemoveAt (index);
		}

		// Try to solve with only half the hints, on the off chance we removed all the necessary
		// hints from the list, just use the removed hints as the list instead
		if (!solver.Solve (masterHintList))
		{
			UnityEngine.Debug.Log ("Could not solve with starting hint list. Trying with the opposite half.");
			masterHintList = hintsRemoved;
			if (!solver.Solve (masterHintList))
			{
				UnityEngine.Debug.Log ("Still couldn't solve it, so just pass the whole list and prepare for a long wait.");
				masterHintList = masterHintList.Append (hintsRemoved);
			}
		}

		while (masterHintList.Count > 0)
		{
			// Choose a random index to remove
			int index = rand.Next (masterHintList.Count);
			Hint lastHint = masterHintList [index];
			masterHintList.RemoveAt (index);

			if (!solver.Solve (hintsToUse.Append (masterHintList)))
			{
				// The solver couldn't complete without this hint, so we must need to keep it
				hintsToUse.Add (lastHint);
			}
		}

		// DEBUG
		UnityEngine.Debug.Log ("Finished pruning hint list:");
		hintGeneratorTests.Debug_HintList.Print (hintsToUse);

		return hintsToUse;
	}
	/*
	private List<int>[] CreateStartingGrid ()
	{
		// Initialize the array representing the tiles and the list of ints representing the symbols
		List<int>[] solverGrid = new List<int>[cols * rows];
		List<int> gridList = new List<int> ();
		
		// Add the symbol numbers to the grid list
		for (int i = 0; i < rows; i++)
		{
			gridList.Add (i);
		}
		// Add the grid list to the array in every position
		for (int j = 0; j < rows * cols; j++)
		{
			List<int> copy = new List<int> (gridList);
			solverGrid [j] = copy;
		}
		return solverGrid;
	}*/

	private List<Hint> GetBelowHints ()
	{
		List<Hint> belowHints = new List<Hint> ();
		for (int i = 0; i < solution.Length; i++)
		{
			int thisSym = solution [i];
			
			for (int j = 0; j < rows; j++)
			{
				if (j > i % rows && (((i / rows) * rows) + j) < solution.Length)
				{
					int belowSym = solution [(i / rows) * rows + j];
					belowHints.Add (new Hint (i / rows, belowSym, thisSym, (int)Relations.Below));
				}
			}
		}
		
		/*foreach (Hint hint in belowHints)
		{
			print (hint.GetPrintString ());
		}*/
		return belowHints;
	}
	
	private List<Hint> GetTouchingHints ()
	{
		List<Hint> touchingHints = new List<Hint> ();
		
		// "Touching" hints also emcompass "on top of" and "between" hints, so generate those here too
		for (int i = 0; i < solution.Length; i++)
		{
			int thisSym = solution [i];
			
			if (i % rows == 0)
			{
				// Top row symbols only have a "touching" relation, so we'll skip it because it's redundant
			}
			else if (i % rows < (rows - 1))
			{
				// Middle row symbols have "touching, "on top of" and "between" relations
				touchingHints.Add (new Hint (i / rows, solution [i - 1], thisSym, (int)Relations.OnTopOf));
				touchingHints.Add (new Hint (i / rows, solution [i - 1], thisSym, (int)Relations.Touching));
				touchingHints.Add (new Hint (i / rows, thisSym, solution [i - 1], solution [i + 1], (int)Relations.Between));
			}
			else
			{
				// Bottom row symbols have "touching" and "on top of" relations only
				touchingHints.Add (new Hint (i / rows, solution [i - 1], thisSym, (int)Relations.OnTopOf));
				touchingHints.Add (new Hint (i / rows, solution [i - 1], thisSym, (int)Relations.Touching));
			}
		}
		
		/*foreach (Hint hint in touchingHints)
		{
			print (hint.GetPrintString ());
		}*/
		
		return touchingHints;
	}
	
	private List<Hint> GetSameRowHints ()
	{
		List<Hint> sameRowHints = new List<Hint> ();
		for (int i = 0; i < solution.Length; i++)
		{
			int thisSym = solution [i];
			if (i / rows != cols - 1)
			{
				// Not the rightmost totem, so look right first
				for (int j = cols-1; j > i / rows; j--)
				{
					int index = (i % rows) + (j * rows);
					if (index < solution.Length)
					{
						sameRowHints.Add (new Hint (i / rows, thisSym, solution [index], j, (int)Relations.SameRow));
					}
				}
			}
			
			/* Technically if we have the same row looking right, then same row looking left is redundant
			if (i / rows != 0)
			{
				// Not the leftmost totem, so look left now
				for (int j = 0; j < i/rows; j++)
				{
					int index = (i % rows) + (j * rows);
					if (index < solution.Length)
					{
						sameRowHints.Add (new Hint (i / rows, thisSym, solution [(i % rows) + (j * rows)], j, (int)Relations.SameRow));
					}
				}
			}*/
		}
		
		/*foreach (Hint hint in sameRowHints)
		{
			print (hint.GetPrintString ());
		}*/
		
		return sameRowHints;
	}
	
	public List<Hint> TestGenerateHintList ()
	{
		hintList = new List<Hint> ();
		hintList.AddRange (GetBelowHints ());
		hintList.AddRange (GetTouchingHints ());
		hintList.AddRange (GetSameRowHints ());

		return hintList;
	}
}


