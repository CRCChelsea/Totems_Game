using System;
using System.Collections.Generic;
using NUnit.Framework;
using Test_Utilities;

[TestFixture]
public class Test_Solver
{
	Test_Framework framework;
	int[] solution;
	List<Hint> hintList;
	
	public Test_Solver()
	{
		framework = new Test_Framework ();
	}

	[Test]
	public void Test_3x4 ()
	{
		solution = framework.GetSolution (3, 4);

		HintGenerator hg = new HintGenerator (3, 4, solution);
		hintList = hg.TestGenerateHintList ();

		foreach (Hint hint in hintList)
		{
			UnityEngine.Debug.Log (hint.ToString ());
		}

		Assert.True (true);
	}

	// Test case
	// You have a tile that has a 1 and a 2 on it
	// and another tile that also has a 1 and a 2 on it
	// You remove symbol 2 from the first tile
	// We check the tile and find that only the 1 is left
	// We remove the 1 from the second tile because we are embiggening the 1 on the first tile
	// Then we check the second tile and find that only the 2 is left
	// So we embiggen the 2
	// But we return out of all that and then check to see if 2 (the symbol we removed in the first place) has only one remaining on the grid
	// We find that it does, and we try to take action on that, but we see that the last remaining symbol is already embiggened, so we should ignore
}

