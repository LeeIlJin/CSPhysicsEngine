using System;

public struct Matrix3x3
{
	public float[] m;
	
	public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
	{
		m = new float[9];
		m[0] = m11;
		m[1] = m12;
		m[2] = m13;
		m[3] = m21;
		m[4] = m22;
		m[5] = m23;
		m[6] = m31;
		m[7] = m32;
		m[8] = m33;
	}
	
	public Matrix3x3(float init = 0.0f)
	{
		m = new float[9];
		for(int i=0; i<9; i++)
		{
			m[i] = init;
		}
	}
	
	public override string ToString()
	{
		return string.Format("{0},{1},{2}|{3},{4},{5}|{6},{7},{8}",m[0],m[1],m[2],m[3],m[4],m[5],m[6],m[7],m[8]);
	}
	
	public float m11{ get{ return m[0]; } set{ m[0] = value; } }
	public float m12{ get{ return m[1]; } set{ m[1] = value; } }
	public float m13{ get{ return m[2]; } set{ m[2] = value; } }
	public float m21{ get{ return m[3]; } set{ m[3] = value; } }
	public float m22{ get{ return m[4]; } set{ m[4] = value; } }
	public float m23{ get{ return m[5]; } set{ m[5] = value; } }
	public float m31{ get{ return m[6]; } set{ m[6] = value; } }
	public float m32{ get{ return m[7]; } set{ m[7] = value; } }
	public float m33{ get{ return m[8]; } set{ m[8] = value; } }
	
	public float this[int r, int c]
	{
		get
		{
			return m[r * 3 + c];
		}
		set
		{
			m[r * 3 + c] = value;
		}
	}
	
	/*
		m[0] m[3] m[6]
		m[1] m[4] m[7]
		m[2] m[5] m[8]
	*/
	public Matrix3x3 transpose
	{
		get
		{
			return new Matrix3x3(m[0],m[3],m[6],m[1],m[4],m[7],m[2],m[5],m[8]);
		}
	}
	
	public Matrix3x3 inverse
	{
		get
		{
			float det = m[0] * (m[4] * m[8] - m[7] * m[5]) - m[1] * (m[3] * m[8] - m[5] * m[6]) + m[2] * (m[3] * m[7] - m[4] * m[6]);
			float invdet = 1.0f / det;
			
			return new Matrix3x3
			(
				(m[4] * m[8] - m[7] * m[5]) * invdet,
				(m[2] * m[7] - m[1] * m[8]) * invdet,
				(m[1] * m[5] - m[2] * m[4]) * invdet,
				(m[5] * m[6] - m[3] * m[8]) * invdet,
				(m[0] * m[8] - m[2] * m[6]) * invdet,
				(m[3] * m[2] - m[0] * m[5]) * invdet,
				(m[3] * m[7] - m[6] * m[4]) * invdet,
				(m[6] * m[1] - m[0] * m[7]) * invdet,
				(m[0] * m[4] - m[3] * m[1]) * invdet
			);
		}
	}
	
	//	Operator
	public static Matrix3x3 operator+(Matrix3x3 a)
	{
		return a;
	}
	public static Matrix3x3 operator-(Matrix3x3 a)
	{
		for(int i=0; i<9; i++)
			a.m[i] = -a.m[i];
		return a;
	}
	
	public static Matrix3x3 operator+(Matrix3x3 a, float b)
	{
		for(int i=0; i<9; i++)
			a.m[i] += b;
		return a;
	}
	public static Matrix3x3 operator+(Matrix3x3 a, Matrix3x3 b)
	{
		for(int i=0; i<9; i++)
			a.m[i] += b.m[i];
		return a;
	}
	
	public static Matrix3x3 operator-(Matrix3x3 a, float b)
	{
		for(int i=0; i<9; i++)
			a.m[i] -= b;
		return a;
	}
	public static Matrix3x3 operator-(Matrix3x3 a, Matrix3x3 b)
	{
		for(int i=0; i<9; i++)
			a.m[i] -= b.m[i];
		return a;
	}
	
	public static Matrix3x3 operator*(Matrix3x3 a, float b)
	{
		for(int i=0; i<9; i++)
			a.m[i] *= b;
		return a;
	}
	public static Matrix3x3 operator*(Matrix3x3 a, Matrix3x3 b)
	{
		Matrix3x3 mat = new Matrix3x3();
		for(int i=0; i<3; i++)
		{
			for(int j=0; j<3; j++)
			{
				for(int k=0; k<3; k++)
				{
					mat.m[i*3+j] += a.m[i*3+k] * b.m[k*3+j];
				}
			}
		}
		return mat;
	}
	public static Vector2 operator*(Matrix3x3 a, Vector2 b)
	{
		Vector2 result = Vector2.Zero;
		result.x = b.x * a.m[0] + b.y * a.m[1] + a.m[2];
		result.y = b.x * a.m[3] + b.y * a.m[4] + a.m[5];
		return result;
	}
	
	//	Static Method
	public static Matrix3x3 View(Vector2 camera_position)
	{
		return new Matrix3x3
		(
			1.0f		, 0.0f		, -camera_position.x ,
			0.0f		, 1.0f		, -camera_position.y ,
			0.0f		, 0.0f		, 1.0f
		);
	}
	
	public static void View(ref Matrix3x3 view, Vector2 camera_position)
	{
		view.m[2] = -camera_position.x;
		view.m[5] = -camera_position.y;
	}
	
	public static Matrix3x3 Ortho(float width, float aspect)
	{
		return new Matrix3x3
		(
			1.0f/width	, 0.0f				, 0.0f ,
			0.0f		, 1.0f/(width/aspect), 0.0f ,
			0.0f		, 0.0f				, 1.0f
		);
	}
	
	public static void Ortho(ref Matrix3x3 ortho ,float width, float aspect) // aspect = width / height
	{
		ortho.m[0] = 1.0f/width;
		ortho.m[4] = 1.0f/(width/aspect);
	}
	
	public static Matrix3x3 Viewport(int screen_width, int screen_height)
	{
		float wp2 = screen_width / 2.0f;
		float hp2 = screen_height / 2.0f;
		
		return new Matrix3x3
		(
			wp2	, 0.0f	, wp2 ,
			0.0f, -hp2	, hp2 ,
			0.0f, 0.0f	, 1.0f
		);
	}
	
	public static void Viewport(ref Matrix3x3 viewport, int screen_width, int screen_height)
	{
		float wp2 = (float)screen_width / 2.0f;
		float hp2 = (float)screen_height / 2.0f;
		
		viewport.m[0] = wp2;
		viewport.m[2] = wp2;
		viewport.m[4] = -hp2;
		viewport.m[5] = hp2;
	}
	
	
	public static Matrix3x3 Identity = new Matrix3x3(1.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,1.0f);
	public static Matrix3x3 Zero = new Matrix3x3(0.0f);
}