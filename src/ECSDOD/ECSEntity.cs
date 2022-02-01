using System;
using System.Collections.Generic;

namespace ECS
{
	public class Entity
	{
		private int[] component_indices;
		private IDictionary<Type,int> component_type_tracker;
		
		public int GetComponentIndex<T>() where T : struct, ECS.IComponentData
		{
			return component_indices[component_type_tracker[typeof(T)]];
		}
		public T GetComponent<T>(ECS.Manager manager) where T : struct, ECS.IComponentData
		{
			return manager.CDAGroup.GetArray<T>().datas[component_indices[component_type_tracker[typeof(T)]]];
		}
		public void SetComponent<T>(ECS.Manager manager, T t) where T : struct, ECS.IComponentData
		{
			manager.CDAGroup.GetArray<T>().datas[component_indices[component_type_tracker[typeof(T)]]] = t;
		}
		
		public Entity(int[] indices, Type[] types)
		{
			component_type_tracker = new Dictionary<Type,int>();
			
			component_indices = indices;
			
			int index = 0;
			foreach(Type type in types)
			{
				component_type_tracker.Add(type,index++);
			}
		}
	}
	
	
	public struct IndicesOfType
	{
		public int[] Indices;
	}
	
	public class Entities : IDispose
	{
		private IndicesOfType[] arr;
		
		public Entities(EntitiesSource src)
		{
			arr = new IndicesOfType[src.indexListArr.Length];
			for(int i=0; i<arr.Length; i++)
			{
				arr[i] = new IndicesOfType();
				arr[i].Indices = src.indexListArr[i].ToArray();
			}
		}
		public void Dispose() 
		{
			arr = null;
		}
		public int[] GetIndices(int TypeIndex)
		{
			return arr[TypeIndex].Indices;
		}
	}
	
	public class EntitiesSource : IDispose
	{
		public List<int>[] indexListArr;
		
		public EntitiesSource(int type_count)
		{
			indexListArr = new List<int>[type_count];
			for(int i=0; i<type_count; i++)
				indexListArr[i] = new List<int>();
		}
		public void Dispose()
		{
			foreach(List<int> list in indexListArr)
			{
				list.Clear();
			}
			Array.Clear(indexListArr,0,indexListArr.Length);
			indexListArr = null;
		}
		public void Add(int type_index, int component_index)
		{
			indexListArr[type_index].Add(component_index);
		}
	}
}