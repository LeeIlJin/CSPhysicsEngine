using System;
using System.Collections.Generic;

namespace Game.System
{
	public sealed class Rigidbody : ECS.SystemBase<Component.Transform, Component.Collider, Component.Rigidbody>
	{
		private struct CollisionResolveNode
		{
			public int AIndex;
			public int BIndex;
			
			public float AMassInv;
			public float BMassInv;
		}
		
		public Rigidbody()
		{
			
		}
		
		public override void Run()
		{
			int result_case = 0;
			for(int i=0; i<Length; i++)
			{
				array2[indices2[i]].results.Clear();
			}
			
			
			/*
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					int result = System.Util.TestInteractionCollider(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]], indices2[i], ref array1.datas[indices1[j]], ref array2.datas[indices2[j]], indices2[j]);
					if(result == 1)
					{
						//Collision!
					}
					
					result_case += result;
				}
			}
			*/
			
			//	Rigidbody vs Rigidbody
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					int result = Game.System.Util.TestInteractionCollider(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]], indices2[i], ref array1.datas[indices1[j]], ref array2.datas[indices2[j]], indices2[j]);
					if(result == 1)
					{
						//Collision!
					}
				}
			}
			
			
		}
		
		public override void OnCreate() {}
		public override void OnBegin() {}
		public override void OnEnable() {}
		public override void OnDisable() {}
		public override void OnEnd() {}
		public override void OnDispose() {}
	}
}