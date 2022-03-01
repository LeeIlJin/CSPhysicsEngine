using System;

namespace Resource
{
	public interface IColorShape : IDeepCopy<Resource.IColorShape>, IDispose
	{
		void Visible(bool b);
		bool IsVisible();
		
		void SetPositionThisFrame(Vector2 position);
		void SetScaleThisFrame(Vector2 scale);
		void SetAngleThisFrame(float angle);
		
		byte A();
		byte R();
		byte G();
		byte B();
		
		void SetA(byte v);
		void SetR(byte v);
		void SetG(byte v);
		void SetB(byte v);
		
		void SetARGB(byte a, byte r, byte g, byte b);
	}
	
	public interface IColorPolygon : Resource.IColorShape
	{
		int VertexCount();
		Vector2 Vertex(int i);
	}
	
	public interface IColorCircle : Resource.IColorShape
	{
		float Radius();
	}
	
}