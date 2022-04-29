using System;

public struct Matrix2x2
{
	/*
		m[0] m[1]
		m[2] m[3]
	*/
	public float[] m;
	
	
	public Matrix2x2(float m11, float m12, float m21, float m22)
	{
		m = new float[4];
		m[0] = m11;
		m[1] = m12;
		m[2] = m21;
		m[3] = m22;
	}
	
	public Matrix2x2(float init = 0.0f)
	{
		m = new float[4];
		for(int i=0; i<4; i++)
		{
			m[i] = init;
		}
	}
	
	public float m11{ get{ return m[0]; } set{ m[0] = value; } }
	public float m12{ get{ return m[1]; } set{ m[1] = value; } }
	public float m21{ get{ return m[2]; } set{ m[2] = value; } }
	public float m22{ get{ return m[3]; } set{ m[3] = value; } }
	
	public float this[int r, int c]
	{
		get
		{
			return m[r * 2 + c];
		}
		set
		{
			m[r * 2 + c] = value;
		}
	}
	
	/*
		m[0] m[2]
		m[1] m[3]
	*/
	public Matrix2x2 transpose
	{
		get
		{
			return new Matrix2x2(m[0],m[2],m[1],m[3]);
		}
	}
	
	public Matrix2x2 inverse
	{
		get
		{
			float det = m[0] * m[3] - m[1] * m[2];
			float invdet = 1.0f / det;
			
			return new Matrix2x2
			(
				m[3] * invdet,
				-m[1] * invdet,
				-m[2] * invdet,
				m[0] * invdet
			);
		}
	}
	
	//	Operator
	public static Matrix2x2 operator+(Matrix2x2 a)
	{
		return a;
	}
	public static Matrix2x2 operator-(Matrix2x2 a)
	{
		for(int i=0; i<4; i++)
			a.m[i] = -a.m[i];
		return a;
	}
	
	public static Matrix2x2 operator+(Matrix2x2 a, float b)
	{
		for(int i=0; i<4; i++)
			a.m[i] += b;
		return a;
	}
	public static Matrix2x2 operator+(Matrix2x2 a, Matrix2x2 b)
	{
		for(int i=0; i<4; i++)
			a.m[i] += b.m[i];
		return a;
	}
	
	public static Matrix2x2 operator-(Matrix2x2 a, float b)
	{
		for(int i=0; i<4; i++)
			a.m[i] -= b;
		return a;
	}
	public static Matrix2x2 operator-(Matrix2x2 a, Matrix2x2 b)
	{
		for(int i=0; i<4; i++)
			a.m[i] -= b.m[i];
		return a;
	}
	
	public static Matrix2x2 operator*(Matrix2x2 a, float b)
	{
		for(int i=0; i<4; i++)
			a.m[i] *= b;
		return a;
	}
	public static Matrix2x2 operator*(Matrix2x2 a, Matrix2x2 b)
	{
		Matrix2x2 mat = new Matrix2x2();
		for(int i=0; i<2; i++)
		{
			for(int j=0; j<2; j++)
			{
				for(int k=0; k<2; k++)
				{
					mat.m[i*2+j] += a.m[i*2+k] * b.m[k*2+j];
				}
			}
		}
		return mat;
	}
	public static Vector2 operator*(Matrix2x2 a, Vector2 b)
	{
		Vector2 result = Vector2.Zero;
		result.x = b.x * a.m[0] + b.y * a.m[1];
		result.y = b.x * a.m[2] + b.y * a.m[3];
		return result;
	}
	
	public static Matrix2x2 Identity = new Matrix2x2(1.0f,0.0f,0.0f,1.0f);
	public static Matrix2x2 Zero = new Matrix2x2(0.0f);
}