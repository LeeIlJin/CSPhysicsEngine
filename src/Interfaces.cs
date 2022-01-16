using System;

public interface IReset<T>
{
	void Reset(T t);
}

public interface IReset<T1,T2>
{
	void Reset(T1 t1, T2 t2);
}

public interface IDispose
{
	void Dispose();
}
