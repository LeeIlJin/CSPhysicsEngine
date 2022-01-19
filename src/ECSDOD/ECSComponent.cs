using System;
using System.Collections.Generic;

namespace ECS
{
	public interface IComponentData { void DeepCopy(); }
	
	public class ComponentDataArrayGroup : IDispose
	{
		private IDictionary<Type,IComponentDataArray> datasGroup;
		public ComponentDataArrayGroup()
		{
			datasGroup = new Dictionary<Type,IComponentDataArray>();
		}
		public void Dispose()
		{
			foreach(KeyValuePair<Type,IComponentDataArray> kvp in datasGroup)
				kvp.Value.Dispose();
			datasGroup.Clear();
			datasGroup = null;
		}
		public void AddArray(IComponentDataArraySource src)
		{
			datasGroup.Add(src.GetDataType(),src.CreateArray());
		}
		public ComponentDataArray<T> GetArray<T>() where T : struct, IComponentData
		{
			return (ComponentDataArray<T>)datasGroup[typeof(T)];
		}
	}
	
	public interface IComponentDataArray : IDispose
	{
		int Size();
		Type GetDataType();
	}
	
	public class ComponentDataArray<T> : IComponentDataArray where T : struct, IComponentData
	{
		public T[] datas;
		public T this[int index]
		{
			get{ return datas[index]; }
			set{ datas[index] = value; }
		}
		
		public void Dispose(){ Array.Clear(datas,0,datas.Length); datas = null; }
		public int Size(){ return datas.Length; }
		public Type GetDataType(){ return typeof(T); }
	}
	
	public interface IComponentDataArraySource : IDispose
	{
		Type GetDataType();
		int CreateData(IComponentData data);
		IComponentDataArray CreateArray();
	}
	
	public class ComponentDataArraySource<T> : IComponentDataArraySource where T : struct, IComponentData
	{
		public List<T> list;
		
		public ComponentDataArraySource(){ list = new List<T>(); }
		
		public void Dispose(){ list.Clear(); list = null; }
		public Type GetDataType() { return typeof(T); }
		public int CreateData(IComponentData data)
		{
			if(data == null)
				data = new T();
			data.DeepCopy();
			list.Add((T)data);
			return list.Count - 1;
		}
		public IComponentDataArray CreateArray() { ComponentDataArray<T> a = new ComponentDataArray<T>(); a.datas = list.ToArray(); return a; }
	}
}