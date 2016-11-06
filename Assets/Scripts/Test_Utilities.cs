using System;
using NUnit.Framework;
using Utilities;

namespace Test_Utilities
{
	public class Test_Framework
	{
		private int[] threeSymbolSolution;
		private int[] fourSymbolSolution;
		private int[] fiveSymbolSolution;
		private int[] sixSymbolSolution;
		private int[] sevenSymbolSolution;
		private int[] eightSymbolSolution;

		public Test_Framework()
		{
			SetupSolutions ();
		}

		private void SetupSolutions ()
		{
			// Create solutions manually so we know what the expected hints are (note that each row is actually a column)
			threeSymbolSolution = new int[]
			{
				0, 1, 2, // col 1 
				2, 1, 0, // col 2
				0, 2, 1, // col 3
				1, 2, 0, // col 4
				2, 0, 1, // col 5
				1, 0, 2  // col 6
			};
			
			fourSymbolSolution = new int[]
			{
				3, 2, 1, 0, // col 1
				1, 2, 0, 3, // col 2
				2, 3, 1, 0, // col 3
				0, 3, 2, 1, // col 4
				3, 0, 2, 1, // col 5
				2, 0, 3, 1  // col 6
			};
			fiveSymbolSolution = new int[]
			{
				4, 0, 3, 2, 1, // col 1
				2, 0, 4, 3, 1, // col 2
				1, 3, 0, 4, 2, // col 3
				2, 4, 3, 1, 0, // col 4
				0, 3, 2, 4, 1, // col 5
				1, 3, 0, 2, 4  // col 6
			};
			sixSymbolSolution = new int[]
			{
				5, 0, 4, 2, 1, 3, // col 1
				4, 0, 5, 3, 2, 1, // col 2
				0, 3, 5, 2, 1, 4, // col 3
				4, 5, 2, 1, 0, 3, // col 4
				5, 0, 3, 1, 2, 4, // col 5
				1, 2, 3, 0, 4, 5  // col 6
			};
			sevenSymbolSolution = new int[]
			{
				6, 0, 4, 5, 3, 2, 1, // col 1
				3, 5, 0, 6, 2, 1, 4, // col 2
				4, 0, 5, 2, 6, 3, 1, // col 3
				5, 0, 3, 6, 1, 4, 2, // col 4
				6, 0, 4, 5, 1, 2, 3, // col 5
				1, 2, 6, 5, 0, 4, 3  // col 6
			};
			eightSymbolSolution = new int[]
			{
				7, 0, 5, 3, 4, 6, 2, 1, // col 1
				4, 2, 5, 3, 6, 1, 0, 7, // col 2
				1, 2, 5, 4, 6, 3, 7, 0, // col 3
				0, 3, 5, 7, 4, 2, 1, 6, // col 4
				6, 2, 4, 3, 5, 1, 0, 7, // col 5
				2, 6, 5, 7, 0, 3, 1, 4  // col 6
			};
		}
	
		public int[] GetSolution (int cols, int rows)
		{
			switch (rows)
			{
			case 3:
				return threeSymbolSolution.SubArray (0, rows * cols);
			case 4:
				return fourSymbolSolution.SubArray (0, rows * cols);
			case 5:
				return fiveSymbolSolution.SubArray (0, rows * cols);
			case 6:
				return sixSymbolSolution.SubArray (0, rows * cols);
			case 7:
				return sevenSymbolSolution.SubArray (0, rows * cols);
			case 8:
				return eightSymbolSolution.SubArray (0, rows * cols);
			default:
				UnityEngine.Debug.Log ("Error: Test case trying to get a solution with " + rows + " rows.");
				return null;
			}
		}
	
		public void DebugSolution ()
		{

		}
	}
}
