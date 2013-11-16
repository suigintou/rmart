using System;
using System.Collections.Generic;

namespace RMArt.Core
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<IList<T>> MergeNeighbors<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
		{
			var prew = default(T);
			var isFirst = true;
			var buffer = new List<T>();

			foreach (var current in source)
			{
				if (isFirst)
					isFirst = false;
				else if (!comparer(prew, current))
				{
					yield return buffer.ToArray();
					buffer.Clear();
				}

				buffer.Add(current);
				prew = current;
			}

			if (!isFirst)
				yield return buffer.ToArray();
		}
	}
}