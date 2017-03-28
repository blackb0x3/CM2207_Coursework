using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using Combinatorics;
using Combinatorics.Collections;

namespace Solution
{
	// A class that allows me to create a deep copy of a dictionary in C#, to prevent the error of changing values over an iterable such as this Dictionary
	public class CloneableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : ICloneable
	{
		public CloneableDictionary<TKey, TValue> Clone()
		{
			CloneableDictionary<TKey, TValue> clone = new CloneableDictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> kvp in this)
			{
				clone.Add(kvp.Key, (TValue) kvp.Value.Clone());
			}
			return clone;
		}
	}

	/**
	 * Author: OriginalGriff
	 * Source URL: https://www.codeproject.com/questions/165992/how-do-i-clone-a-dictionary-in-c
	 * Date Posted: 7th March 2011 
	 * Date Accessed: 27th March 2017
	 */
}

