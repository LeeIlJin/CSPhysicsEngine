using System;

public static class UMath
{
	public static readonly float PI = 3.141592653f;		// 180.0 in degree
	public static readonly float PI2 = 6.283185307f;	// 360.0 in degree
	public static readonly float R2D = 57.295779513f;	// Radian To Degree
	public static readonly float D2R = 0.017453292f;	// Degree To Radian
	
	public static float ToRadian(float degree)
	{
		degree = degree % 360.0f;
		return degree * D2R;
	}
	
	public static float ToDegree(float radian)
	{
		radian = radian % PI2;
		return radian * R2D;
	}
	
	public static float Clamp(float v, float min, float max)
	{
		if(v < min)
			return min;
		if(v > max)
			return max;
		return v;
	}
	
	public static float Clamp01(float v)
	{
		if(v < 0.0f)
			return 0.0f;
		if(v > 1.0f)
			return 1.0f;
		return v;
	}
	
	public static float Max(float a, float b)
	{
		if(a > b)
			return a;
		return b;
	}
	
	public static float Min(float a, float b)
	{
		if(a < b)
			return a;
		return b;
	}
	
	public static float Lerp(float p1, float p2, float r)
	{
		r = UMath.Clamp01(r);
		return (1.0f - r) * p1 + r * p2;
	}
	
	public static Vector2 Lerp(Vector2 p1, Vector2 p2, float r)
	{
		r = UMath.Clamp01(r);
		return p1 * (1.0f - r) + p2 * r;
	}
	
	public static Vector2 Slerp(Vector2 p1, Vector2 p2, float r)
	{
		float dotp = Vector2.Dot(p1.normalize, p2.normalize);
		if((dotp > 0.9999f) || (dotp < -0.9999f))
		{
			if(r <= 0.5f)
				return p1;
			return p2;
		}
		
		float theta = (float)Math.Acos(dotp);
		return (p1 * (float)Math.Sin((1.0f - r) * theta) + p2 * (float)Math.Sin(r * theta)) / (float)Math.Sin(theta);
	}
}
