using System;

namespace ECS
{
	public abstract class SystemBase : IReset<ComponentDataArrayGroup,EntitiesSource>, IDispose
	{
		protected Entities Entities { get; private set; }
		protected ComponentDataArrayGroup CDAGroup { get; private set; }
		protected Type[] Types { get; private set; }
		protected Type[] RejectTypes { get; private set; }
		public int Length { get; protected set; }
		
		public SystemBase(){ Types = null; RejectTypes = null; Entities = null; }
		public void Reset(ComponentDataArrayGroup cdag,EntitiesSource es){ CDAGroup = cdag;  Entities = new Entities(es); }
		public void Dispose()
		{
			Types = null;
			CDAGroup = null;
			Entities.Dispose();
			Entities = null;
		}
		
		protected void SetTypes(params Type[] ts) { Types = ts; }
		protected void SetRejectTypes(params Type[] ts) { RejectTypes = ts; }
		public int[] GetTypeIndices(Type[] in_types)
		{
			if(Types == null) return null;
			if(Types.Length > in_types.Length) return null;
			
			int[] result = new int[Types.Length];
			int counter = 0;
			for(int i=0; i<in_types.Length; i++)
			{
				for(int j=0; j<Types.Length; j++)
				{
					if(in_types[i] == Types[j])
					{
						result[j] = i;
						counter++;
						break;
					}
				}
				
				if(RejectTypes != null)
				{
					for(int j=0; j<RejectTypes.Length; j++)
					{
						if(in_types[i] == RejectTypes[j])
						{
							return null;
						}
					}
				}
			}
			if(counter != Types.Length) result = null;
			return result;
		}
		public int GetTypeIndex(Type type)
		{
			for(int i=0; i<Types.Length; i++)
				if(Types[i] == type) return i;
			return -1;
		}
		public int GetTypeCount()
		{
			if(Types == null) return 0;
			return Types.Length;
		}
		
		public abstract void RegistComponentTypeToFactory(Factory factory);
		public abstract void ArraySet();
		public abstract void ArrayDispose();
		
		public virtual void OnCreate() {}
		public virtual void OnBegin() {}
		public virtual void OnEnable() {}
		public virtual void OnDisable() {}
		public virtual void OnEnd() {}
		public virtual void OnDispose() {}
		
		public virtual void Run() {}
	}
	
	public class SystemBase<T1> : SystemBase
	where T1 : struct, IComponentData
	{
		protected int[] indices1;
		protected ComponentDataArray<T1> array1;
	
		public SystemBase() { SetTypes(typeof(T1)); }
		
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]]);
			}
		}
	}
	
	public class SystemBase<T1,T2> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2)); }
		
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]]);
			}
		}
	}
	
	public class SystemBase<T1, T2, T3> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	where T3 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected int[] indices3;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		protected ComponentDataArray<T3> array3;
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2), typeof(T3)); }
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
			factory.Second_RegistComponentType<T3>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			indices3 = Entities.GetIndices(2);
			array3 = CDAGroup.GetArray<T3>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
			indices3 = null;
			array3 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2, ref T3 t3);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]],ref array3.datas[indices3[i]]);
			}
		}
	}
	
	public class SystemBase<T1, T2, T3, T4> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	where T3 : struct, IComponentData
	where T4 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected int[] indices3;
		protected int[] indices4;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		protected ComponentDataArray<T3> array3;
		protected ComponentDataArray<T4> array4;
		
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4)); }
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
			factory.Second_RegistComponentType<T3>();
			factory.Second_RegistComponentType<T4>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			indices3 = Entities.GetIndices(2);
			array3 = CDAGroup.GetArray<T3>();
			indices4 = Entities.GetIndices(3);
			array4 = CDAGroup.GetArray<T4>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
			indices3 = null;
			array3 = null;
			indices4 = null;
			array4 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]],ref array3.datas[indices3[i]],ref array4.datas[indices4[i]]);
			}
		}
	}
	
	public class SystemBase<T1, T2, T3, T4, T5> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	where T3 : struct, IComponentData
	where T4 : struct, IComponentData
	where T5 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected int[] indices3;
		protected int[] indices4;
		protected int[] indices5;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		protected ComponentDataArray<T3> array3;
		protected ComponentDataArray<T4> array4;
		protected ComponentDataArray<T5> array5;
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)); }
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
			factory.Second_RegistComponentType<T3>();
			factory.Second_RegistComponentType<T4>();
			factory.Second_RegistComponentType<T5>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			indices3 = Entities.GetIndices(2);
			array3 = CDAGroup.GetArray<T3>();
			indices4 = Entities.GetIndices(3);
			array4 = CDAGroup.GetArray<T4>();
			indices5 = Entities.GetIndices(4);
			array5 = CDAGroup.GetArray<T5>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
			indices3 = null;
			array3 = null;
			indices4 = null;
			array4 = null;
			indices5 = null;
			array5 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]],ref array3.datas[indices3[i]],ref array4.datas[indices4[i]],ref array5.datas[indices5[i]]);
			}
		}
	}
	
	public class SystemBase<T1, T2, T3, T4, T5, T6> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	where T3 : struct, IComponentData
	where T4 : struct, IComponentData
	where T5 : struct, IComponentData
	where T6 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected int[] indices3;
		protected int[] indices4;
		protected int[] indices5;
		protected int[] indices6;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		protected ComponentDataArray<T3> array3;
		protected ComponentDataArray<T4> array4;
		protected ComponentDataArray<T5> array5;
		protected ComponentDataArray<T6> array6;
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)); }
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
			factory.Second_RegistComponentType<T3>();
			factory.Second_RegistComponentType<T4>();
			factory.Second_RegistComponentType<T5>();
			factory.Second_RegistComponentType<T6>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			indices3 = Entities.GetIndices(2);
			array3 = CDAGroup.GetArray<T3>();
			indices4 = Entities.GetIndices(3);
			array4 = CDAGroup.GetArray<T4>();
			indices5 = Entities.GetIndices(4);
			array5 = CDAGroup.GetArray<T5>();
			indices6 = Entities.GetIndices(5);
			array6 = CDAGroup.GetArray<T6>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
			indices3 = null;
			array3 = null;
			indices4 = null;
			array4 = null;
			indices5 = null;
			array5 = null;
			indices6 = null;
			array6 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]],ref array3.datas[indices3[i]],ref array4.datas[indices4[i]],ref array5.datas[indices5[i]],ref array6.datas[indices6[i]]);
			}
		}
	}
	
	public class SystemBase<T1, T2, T3, T4, T5, T6, T7> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	where T3 : struct, IComponentData
	where T4 : struct, IComponentData
	where T5 : struct, IComponentData
	where T6 : struct, IComponentData
	where T7 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected int[] indices3;
		protected int[] indices4;
		protected int[] indices5;
		protected int[] indices6;
		protected int[] indices7;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		protected ComponentDataArray<T3> array3;
		protected ComponentDataArray<T4> array4;
		protected ComponentDataArray<T5> array5;
		protected ComponentDataArray<T6> array6;
		protected ComponentDataArray<T7> array7;
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)); }
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
			factory.Second_RegistComponentType<T3>();
			factory.Second_RegistComponentType<T4>();
			factory.Second_RegistComponentType<T5>();
			factory.Second_RegistComponentType<T6>();
			factory.Second_RegistComponentType<T7>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			indices3 = Entities.GetIndices(2);
			array3 = CDAGroup.GetArray<T3>();
			indices4 = Entities.GetIndices(3);
			array4 = CDAGroup.GetArray<T4>();
			indices5 = Entities.GetIndices(4);
			array5 = CDAGroup.GetArray<T5>();
			indices6 = Entities.GetIndices(5);
			array6 = CDAGroup.GetArray<T6>();
			indices7 = Entities.GetIndices(6);
			array7 = CDAGroup.GetArray<T7>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
			indices3 = null;
			array3 = null;
			indices4 = null;
			array4 = null;
			indices5 = null;
			array5 = null;
			indices6 = null;
			array6 = null;
			indices7 = null;
			array7 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]],ref array3.datas[indices3[i]],ref array4.datas[indices4[i]],ref array5.datas[indices5[i]],ref array6.datas[indices6[i]],ref array7.datas[indices7[i]]);
			}
		}
	}
	
	public class SystemBase<T1, T2, T3, T4, T5, T6, T7, T8> : SystemBase
	where T1 : struct, IComponentData
	where T2 : struct, IComponentData
	where T3 : struct, IComponentData
	where T4 : struct, IComponentData
	where T5 : struct, IComponentData
	where T6 : struct, IComponentData
	where T7 : struct, IComponentData
	where T8 : struct, IComponentData
	{
		protected int[] indices1;
		protected int[] indices2;
		protected int[] indices3;
		protected int[] indices4;
		protected int[] indices5;
		protected int[] indices6;
		protected int[] indices7;
		protected int[] indices8;
		protected ComponentDataArray<T1> array1;
		protected ComponentDataArray<T2> array2;
		protected ComponentDataArray<T3> array3;
		protected ComponentDataArray<T4> array4;
		protected ComponentDataArray<T5> array5;
		protected ComponentDataArray<T6> array6;
		protected ComponentDataArray<T7> array7;
		protected ComponentDataArray<T8> array8;
		
		public SystemBase() { SetTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)); }
		public override void RegistComponentTypeToFactory(Factory factory)
		{
			factory.Second_RegistComponentType<T1>();
			factory.Second_RegistComponentType<T2>();
			factory.Second_RegistComponentType<T3>();
			factory.Second_RegistComponentType<T4>();
			factory.Second_RegistComponentType<T5>();
			factory.Second_RegistComponentType<T6>();
			factory.Second_RegistComponentType<T7>();
			factory.Second_RegistComponentType<T8>();
		}
		public override void ArraySet()
		{
			indices1 = Entities.GetIndices(0);
			array1 = CDAGroup.GetArray<T1>();
			indices2 = Entities.GetIndices(1);
			array2 = CDAGroup.GetArray<T2>();
			indices3 = Entities.GetIndices(2);
			array3 = CDAGroup.GetArray<T3>();
			indices4 = Entities.GetIndices(3);
			array4 = CDAGroup.GetArray<T4>();
			indices5 = Entities.GetIndices(4);
			array5 = CDAGroup.GetArray<T5>();
			indices6 = Entities.GetIndices(5);
			array6 = CDAGroup.GetArray<T6>();
			indices7 = Entities.GetIndices(6);
			array7 = CDAGroup.GetArray<T7>();
			indices8 = Entities.GetIndices(7);
			array8 = CDAGroup.GetArray<T8>();
			
			Length = indices1.Length;
		}
		public override void ArrayDispose()
		{
			indices1 = null;
			array1 = null;
			indices2 = null;
			array2 = null;
			indices3 = null;
			array3 = null;
			indices4 = null;
			array4 = null;
			indices5 = null;
			array5 = null;
			indices6 = null;
			array6 = null;
			indices7 = null;
			array7 = null;
			indices8 = null;
			array8 = null;
		}
		
		//	ForEach
		public delegate void Method(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8);
		protected Method method;
		protected void ForEach(Method m) { method = m; }
		public override void Run()
		{
			for(int i=0; i<Length; i++)
			{
				method(ref array1.datas[indices1[i]],ref array2.datas[indices2[i]],ref array3.datas[indices3[i]],ref array4.datas[indices4[i]],ref array5.datas[indices5[i]],ref array6.datas[indices6[i]],ref array7.datas[indices7[i]],ref array8.datas[indices8[i]]);
			}
		}
	}
}