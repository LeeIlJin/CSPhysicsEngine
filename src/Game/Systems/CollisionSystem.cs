using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.System
{
	public sealed class Collision : ECS.SystemBase<Component.Transform, Component.Collider>
	{
		public Collision()
		{
			SetRejectTypes(typeof(Component.Rigidbody));
		}
		
		public override void Run()
		{
			int result_case = 0;
			for(int i=0; i<Length; i++)
			{
				array2.datas[indices2[i]].results.Clear();
				Game.System.Util.TransformCollider(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]]);
			}
			
			
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					int result = Game.System.Util.TestInteractionCollider(ref array2.datas[indices2[i]], indices2[i], ref array2.datas[indices2[j]], indices2[j]);
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