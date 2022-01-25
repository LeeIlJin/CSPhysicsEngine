using System;

namespace Resource
{
	public interface IPolygon : IDeepCopy<Resource.IPolygon>, IDispose
	{
		int VertexCount();
		Vector2 Vertex(int i);
	}
	
	public interface ICircle : IDeepCopy<Resource.ICircle>, IDispose
	{
		float Radius();
	}
}