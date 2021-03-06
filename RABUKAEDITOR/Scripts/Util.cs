﻿using UnityEngine;

public static class Util
{
	public static string GetFullPath(this GameObject obj)
	{
		return GetFullPath(obj.transform);
	}

	public static string GetFullPath(this Transform t)
	{
		string path = t.name;
		var parent = t.parent;
		while (parent)
		{
			path = $"{parent.name}/{path}";
			parent = parent.parent;
		}
		return path;
	}
}
