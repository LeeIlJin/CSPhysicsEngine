using System;
using System.Collections.Generic;

namespace ECS
{
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