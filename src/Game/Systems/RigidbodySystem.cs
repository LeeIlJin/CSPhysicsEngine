using System;
using System.Collections.Generic;

namespace Game.System
{
	public sealed class Rigidbody : ECS.SystemBase<Component.Transform, Component.Collider, Component.Rigidbody>
	{
		private struct CollisionResolveNode
		{
			public int ACollider;
			public int BCollider;
			
			public int ARigidbody;
			public int BRigidbody;
		}
		private List<CollisionResolveNode> collisionResolveNodes;
		
		public int TestCase;
		
		public Rigidbody(int testcase)
		{
			collisionResolveNodes = List<CollisionResolveNode>();
			TestCase = testcase;
		}
		
		public override void Run()
		{
			int result_case = 0;
			collisionResolveNodes.Clear();
			for(int i=0; i<Length; i++)
			{
				array2[indices2[i]].results.Clear();
				TransformCollider(ref array1[indices1[i]], ref array2[indices2[i]]);
			}
			
			//	Collider vs Rigidbody
			for(int i=0; i<array2.datas.Length; i++)
			{
				if(array2.datas[i].have_rigidbody)
					continue;
				
				for(int j=0; j<Length; j++)
				{
					int result = Game.System.Util.TestInteractionCollider(ref array2.datas[i], i, ref array2.datas[indices2[j]], indices2[j]);
					if(result == 1)
					{
						//Collision!
					}
				}
			}
			
			//	Rigidbody vs Rigidbody
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					int result = Game.System.Util.TestInteractionCollider(ref array2.datas[indices2[i]], indices2[i], ref array2.datas[indices2[j]], indices2[j]);
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