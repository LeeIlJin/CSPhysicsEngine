using System;

public static class URandom
{
	private static Random random = null;
	public static void CreateSeed(int seed = 0)
	{
		if(seed > 0)
			random = new Random(seed);
		else
			random = new Random();
	}
	
	public static int Int() { return random.Next(); }
	public static int Int(int max) { return random.Next(max); }
	public static int Int(int min, int max) { return random.Next(min, max); }
	
	public static double Double() { return random.NextDouble(); }
	public static double Double(double max) { return random.NextDouble() * max; }
	public static double Double(double min, double max) { return random.NextDouble() * (max - min) + min; }
	
	public static float Float() { return (float)URandom.Double(); }
	public static float Float(float max) { return (float)URandom.Double((double)max); }
	public static float Float(float min, float max) { return (float)URandom.Double((double)min,(double)max); }
	
	public static Vector2 Vector2() { return new Vector2(URandom.Float(), URandom.Float()); }
	public static Vector2 Vector2(float max) { return new Vector2(URandom.Float(max), URandom.Float(max)); }
	public static Vector2 Vector2(float min, float max) { return new Vector2(URandom.Float(min, max), URandom.Float(min, max)); }
	
	public static byte[] Bytes(uint length)
	{
		byte[] bs = new byte[length];
		random.NextBytes(bs);
		return bs;
	}
}