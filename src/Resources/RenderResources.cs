using System;

namespace Resource
{
	public interface IColor : IDeepCopy<Resource.IColor>, IDispose
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