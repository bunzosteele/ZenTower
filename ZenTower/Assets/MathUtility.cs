using System;

public static class MathUtility
{
	public static bool AreEqual(float a, float b)
	{
		float difference = Math.Abs(a - b);
		return difference < .001f;
	}
}