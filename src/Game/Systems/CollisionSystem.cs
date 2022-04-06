using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.System
{
	public sealed class Collision : ECS.SystemBase<Component.Transform, Component.Collider>
	{
		public Collision()
		{
			
		}
		
		public override void Run()
		{
			int result_case = 0;
			for(int i=0; i<Length; i++)
			{
				array2[indices2[i]].results.Clear();
			}
			
			
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					if(array2.datas[indices2[i]].have_rigidbody == true && array2.datas[indices2[j]].have_rigidbody == true)
						continue;
					
					int result = Game.System.Util.TestInteractionCollider(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]], indices2[i], ref array1.datas[indices1[j]], ref array2.datas[indices2[j]], indices2[j]);
					if(result == 1)
					{
						//Collision!
					}
					
					result_case += result;
				}
			}
			
			Console.WriteLine("Collision Result Case : {0}", result_case);
		}
		
		public override void OnCreate() {}
		public override void OnBegin() {}
		public override void OnEnable() {}
		public override void OnDisable() {}
		public override void OnEnd() {}
		public override void OnDispose() {}
	}
}