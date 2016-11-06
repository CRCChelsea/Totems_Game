using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public static class MyUtilities
	{
		// Extension method to get a part of an array
		public static T[] SubArray<T> (this T[] data, int index, int length)
		{
			T[] result = new T[length];
			Array.Copy (data, index, result, 0, length);
			return result;
		}

		/*// Extension method to combine two arrays
		public static T[] Append<T> (this T[] data, T[] appendData, int length)
		{
			T[] result = new T[length];
			Array.Copy (data, result, data.Length);
			Array.Copy (appendData, 0, result, data.Length, appendData.Length);
			return result;
		}*/

		// Extension method to combine two Lists into a new List
		public static List<T> Append<T> (this List<T> data, List<T> appendData)
		{
			List<T> result = new List<T> (data);
			result.AddRange (appendData);
			return result;
		}

		// Extension method to print a solution (int) arrays
		public static void PrintGrid<T> (this T[] data, int cols, int rows)
		{
			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < (cols*rows); j+=rows)
				{
					sb.Append (data [j + i]).Append (", ");
				}
				sb.Append ("\n");
			}
			Debug.Log (sb.ToString ());
		}

		public static int GetIndex (this int[] data, int sym)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data [i] == sym)
				{
					return i;
				}
			}
			return -1;
		}

		public static string Print<T> (this List<T> data)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (var thing in data)
			{
				sb.Append (thing).Append (", ");
			}

			return sb.ToString ();
		}

		public static bool Contains<T> (this T[] data, T item)
		{
			foreach (T listItem in data)
			{
				if (listItem.Equals (item))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetString<T> (this List<T> data, string delimiter)
		{
			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i < data.Count; i++)
			{
				sb.Append (data [i].ToString ());
				if (i != data.Count - 1)
				{
					sb.Append (delimiter);
				}
			}
			return sb.ToString ();
		}
	}
}