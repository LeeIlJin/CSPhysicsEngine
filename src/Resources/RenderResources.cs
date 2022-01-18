using System;

namespace Resource
{
	public interface IPolygon : IDispose
	{
		int VertexCount();
		Vector2 Vertex(int i);
	}
	
	public interface ICircle : IDispose
	{
		float Radius();
	}
	
	public interface IColor : IDispose
	{
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
}