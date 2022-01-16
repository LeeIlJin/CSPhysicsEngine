using System;

public struct Vector2
{
	public float x;
	public float y;
	
	public Vector2(float _x = 0.0f, float _y = 0.0f){ this.x = _x; this.y = _y; }
	public Vector2(double _x, double _y){ this.x = (float)_x; this.y = (float)_y; }
	public Vector2(int _x, int _y){ this.x = (float)_x; this.y = (float)_y; }
	
	public override string ToString()
	{
		return string.Format("{0},{1}",x,y);
	}
	
	public Vector2 negateX{ get{ return new Vector2(-x,y); } }
	public Vector2 negateY{ get{ return new Vector2(x,-y); } }
	public Vector2 negate{ get{ return new Vector2(-x,-y); } }
	public Vector2 normalize{ get{ return this.Normalize(); } }
	public float length{ get{ return this.Length(); } }
	public float lengthSquared{ get{ return this.LengthSquared(); } }
	
	public Vector2 left{ get{ return new Vector2(-y,x); } }
	public Vector2 right{ get{ return new Vector2(y,-x); } }
	
	public static Vector2 operator+(Vector2 a){ return a; }
	public static Vector2 operator-(Vector2 a){ return new Vector2(-a.x,-a.y); }
	
	public static Vector2 operator+(Vector2 a, Vector2 b){ return new Vector2(a.x + b.x, a.y + b.y); }
	public static Vector2 operator+(Vector2 a, float b){ return new Vector2(a.x + b, a.y + b); }
	
	public static Vector2 operator-(Vector2 a, Vector2 b){ return new Vector2(a.x - b.x, a.y - b.y); }
	public static Vector2 operator-(Vector2 a, float b){ return new Vector2(a.x - b, a.y - b); }
	
	public static Vector2 operator*(Vector2 a, Vector2 b){ return new Vector2(a.x * b.x, a.y * b.y); }
	public static Vector2 operator*(Vector2 a, float b){ return new Vector2(a.x * b, a.y * b); }
	
	public static Vector2 operator/(Vector2 a, Vector2 b){ return new Vector2(a.x / b.x, a.y / b.y); }
	public static Vector2 operator/(Vector2 a, float b){ return new Vector2(a.x / b, a.y / b); }
	
	
	public static float Dot(Vector2 a, Vector2 b){ return (a.x * b.x) + (a.y * b.y); }
	public static float Cross(Vector2 a, Vector2 b){ return (a.x * b.y) - (a.y * b.x); }
	public static float Distance(Vector2 a, Vector2 b){ return (b - a).Length(); }
	
	public float Length(){ return (float)Math.Sqrt(x * x + y * y); }
	public float LengthSquared(){ return x * x + y * y; }
	public Vector2 Normalize()
	{
		float length = this.Length();
		if(length <= 0.0000001f)
			return Vector2.Zero;
		
		return new Vector2(x/length, y/length);
	}
	public void SetNormalize()
	{
		float length = this.Length();
		if(length <= 0.0000001f)
		{
			return;
		}
		else
		{
			x /= length;
			y /= length;
		}
	}
	public void SetNegate(){ x = -x; y = -y; }
	
	public static Vector2 Zero = new Vector2(0.0f,0.0f);
	public static Vector2 Left = new Vector2(-1.0f,0.0f);
	public static Vector2 Right = new Vector2(1.0f,0.0f);
	public static Vector2 Down = new Vector2(0.0f,-1.0f);
	public static Vector2 Up = new Vector2(0.0f,1.0f);
	public static Vector2 Half = new Vector2(0.5f,0.5f);
	public static Vector2 One = new Vector2(1.0f,1.0f);
}