using System;
using System.Drawing;

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
	
	public static float ClampAngle(float degree)
	{
		if(degree < 0.0f)
			return degree + 360.0f;
		if(degree >= 360.0f)
			return degree - 360.0f;
		
		return degree;
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
	
	public static float Repeat(float v, float min, float max)
	{
		return min + ((v - min) % (max - min));
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
	
	public static Vector2 Transform(Vector2 point, Vector2 position, Vector2 scale, float angle)
	{
		float rad = angle * D2R;
		
		Vector2 result = new Vector2(0.0f, 0.0f);
		
		point.x *= scale.x;
		point.y *= scale.y;
		
		result.x = point.x * (float)Math.Cos(rad) - point.y * (float)Math.Sin(rad);
		result.y = point.x * (float)Math.Sin(rad) + point.y * (float)Math.Cos(rad);
		
		result.x += position.x;
		result.y += position.y;
		
		return result;
	}
	
	public static PointF Transform(PointF point, Vector2 position, Vector2 scale, float angle)
	{
		float rad = angle * D2R;
		
		PointF result = new PointF(0.0f, 0.0f);
		
		point.X *= scale.x;
		point.Y *= scale.y;
		
		result.X = point.X * (float)Math.Cos(rad) - point.Y * (float)Math.Sin(rad);
		result.Y = point.X * (float)Math.Sin(rad) + point.Y * (float)Math.Cos(rad);
		
		result.X += position.x;
		result.Y += position.y;
		
		return result;
	}
}
