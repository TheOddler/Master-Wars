using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
	{
		foreach (T item in items)
			action(item);
		return items;
	}
}
