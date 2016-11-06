using System;
using NUnit.Framework;
using System.Collections.Generic;
using Utilities;
using Test_Utilities;

namespace hintGeneratorTests
{
	[TestFixture]
	public class Test_GenerateHints
	{
		// Test the number of hints generated, based on the number of totems and symbols
		Test_Framework framework = new Test_Framework ();
		public static int CalculateHintNumber (int cols, int rows)
		{
			// Above = Totems * ((Symbols * (Symbols-1))/2)
			// Between = Totems * (Symbols - 2)
			// OnTopOf = Totems * (Symbols - 1)
			// Touching = Totems * (Symbols - 1) [Because we're only doing half)
			// SameRow = Symbols * ((Totems * (Totems-1))/2)
		
			// Total = (Totems * Symbols * ((Totems + Symbols)/2 + 2)) - (4 * Totems)
			return (int)(cols * rows * (((cols + rows) / 2f) + 2)) - (4 * cols);
		}

		[Test]
		public void Test_3x3HintList ()
		{
			HintGenerator hints = new HintGenerator (3, 3, framework.GetSolution (3, 3));
			Assert.AreEqual (CalculateHintNumber (3, 3), hints.TestGenerateHintList ().Count); // 33
		}

		[Test]
		public void Test_3x4HintList ()
		{
			HintGenerator hints = new HintGenerator (3, 4, framework.GetSolution (3, 4));
			Assert.AreEqual (CalculateHintNumber (3, 4), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_4x3HintList ()
		{
			HintGenerator hints = new HintGenerator (4, 3, framework.GetSolution (4, 3));
			Assert.AreEqual (CalculateHintNumber (4, 3), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_3x5HintList ()
		{
			HintGenerator hints = new HintGenerator (3, 5, framework.GetSolution (3, 5));
			Assert.AreEqual (CalculateHintNumber (3, 5), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_5x3HintList ()
		{
			HintGenerator hints = new HintGenerator (5, 3, framework.GetSolution (5, 3));
			Assert.AreEqual (CalculateHintNumber (5, 3), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_4x4HintList ()
		{
			HintGenerator hints = new HintGenerator (4, 4, framework.GetSolution (4, 4));
			Assert.AreEqual (CalculateHintNumber (4, 4), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_3x6HintList ()
		{
			HintGenerator hints = new HintGenerator (3, 6, framework.GetSolution (3, 6));
			Assert.AreEqual (CalculateHintNumber (3, 6), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_6x3HintList ()
		{
			HintGenerator hints = new HintGenerator (6, 3, framework.GetSolution (6, 3));
			Assert.AreEqual (CalculateHintNumber (6, 3), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_4x5HintList ()
		{
			HintGenerator hints = new HintGenerator (4, 5, framework.GetSolution (4, 5));
			Assert.AreEqual (CalculateHintNumber (4, 5), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_5x4HintList ()
		{
			HintGenerator hints = new HintGenerator (5, 4, framework.GetSolution (5, 4));
			Assert.AreEqual (CalculateHintNumber (5, 4), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_3x7HintList ()
		{
			HintGenerator hints = new HintGenerator (3, 7, framework.GetSolution (3, 7));
			Assert.AreEqual (CalculateHintNumber (3, 7), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_3x8HintList ()
		{
			HintGenerator hints = new HintGenerator (3, 8, framework.GetSolution (3, 8));
			Assert.AreEqual (CalculateHintNumber (3, 8), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_4x6HintList ()
		{
			HintGenerator hints = new HintGenerator (4, 6, framework.GetSolution (4, 6));
			Assert.AreEqual (CalculateHintNumber (4, 6), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_6x4HintList ()
		{
			HintGenerator hints = new HintGenerator (6, 4, framework.GetSolution (6, 4));
			Assert.AreEqual (CalculateHintNumber (6, 4), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_5x5HintList ()
		{
			HintGenerator hints = new HintGenerator (5, 5, framework.GetSolution (5, 5));
			Assert.AreEqual (CalculateHintNumber (5, 5), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_4x7HintList ()
		{
			HintGenerator hints = new HintGenerator (4, 7, framework.GetSolution (4, 7));
			Assert.AreEqual (CalculateHintNumber (4, 7), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_5x6HintList ()
		{
			HintGenerator hints = new HintGenerator (5, 6, framework.GetSolution (5, 6));
			Assert.AreEqual (CalculateHintNumber (5, 6), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_6x5HintList ()
		{
			HintGenerator hints = new HintGenerator (6, 5, framework.GetSolution (6, 5));
			Assert.AreEqual (CalculateHintNumber (6, 5), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_4x8HintList ()
		{
			HintGenerator hints = new HintGenerator (4, 8, framework.GetSolution (4, 8));
			Assert.AreEqual (CalculateHintNumber (4, 8), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_5x7HintList ()
		{
			HintGenerator hints = new HintGenerator (5, 7, framework.GetSolution (5, 7));
			Assert.AreEqual (CalculateHintNumber (5, 7), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_6x6HintList ()
		{
			HintGenerator hints = new HintGenerator (6, 6, framework.GetSolution (6, 6));
			Assert.AreEqual (CalculateHintNumber (6, 6), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_5x8HintList ()
		{
			HintGenerator hints = new HintGenerator (5, 8, framework.GetSolution (5, 8));
			Assert.AreEqual (CalculateHintNumber (5, 8), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_6x7HintList ()
		{
			HintGenerator hints = new HintGenerator (6, 7, framework.GetSolution (6, 7));
			Assert.AreEqual (CalculateHintNumber (6, 7), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_6x8HintList ()
		{
			HintGenerator hints = new HintGenerator (6, 8, framework.GetSolution (6, 8));
			Assert.AreEqual (CalculateHintNumber (6, 8), hints.TestGenerateHintList ().Count);
		}

		[Test]
		public void Test_Persistance ()
		{
			HintGenerator hints = new HintGenerator (5, 5, framework.GetSolution (5, 5));
			List<Hint> listA = hints.TestGenerateHintList ();
			Debug_HintList.RemoveAHint (new List<Hint> (listA));
			Assert.AreEqual (CalculateHintNumber (5, 5), listA.Count);
		}
	}

	public class Debug_HintList
	{
		public static void Print (List<Hint> hintList)
		{
			foreach (Hint hint in hintList)
			{
				UnityEngine.Debug.Log (hint.ToString ());
			}
		}

		public static void RemoveAHint (List<Hint> listB)
		{
			List<Hint> listC = listB;
			listC.RemoveAt (0);
		}
	}
}
