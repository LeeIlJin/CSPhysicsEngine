using System;
using System.Collections.Generic;

namespace Game.System
{
	public sealed class Rigidbody : ECS.SystemBase<Component.Transform, Component.Collider, Component.Rigidbody>
	{
		private struct CollisionResolveInfo
		{
			public int ACollider;
			public int BCollider;
			
			public int ARigidbody;
			public int BRigidbody;
		}
		
		private struct CollisionApplyInfo
		{
			public int Rigidbody;
			public Vector2 LinearVelocity;
			public float AngularVelocity;
		}
		
		private List<CollisionResolveInfo> collisionResolveInfos;
		private List<CollisionApplyInfo> collisionApplyInfos;
		
		public delegate float FloatOut();
		private FloatOut DeltaTime;
		public int TestCase;
		
		public Rigidbody(FloatOut delta_time_function, int testcase)
		{
			collisionResolveInfos = new List<CollisionResolveInfo>();
			collisionApplyInfos = new List<CollisionApplyInfo>();
			DeltaTime = delta_time_function;
			TestCase = testcase;
		}
		
		public override void Run()
		{
			collisionResolveInfos.Clear();
			for(int i=0; i<Length; i++)
			{
				array2[indices2[i]].results.Clear();
				
				//	Gravity Apply
				array3.datas[indices3[i]].velocity.y -= array3.datas[indices3[i]].gravity_factor * DeltaTime() * 0.001f;
				
				array1.datas[indices1[i]].position = array1.datas[indices1[i]].position + array3.datas[indices3[i]].velocity * DeltaTime() * 0.5f;
				array1.datas[indices1[i]].angle = array1.datas[indices1[i]].angle + array3.datas[indices3[i]].angular_velocity * DeltaTime() * 0.5f;
				
				Game.System.Util.TransformCollider(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]]);
			}
			
			
			//	Collider vs Rigidbody
			for(int i=0; i<array2.datas.Length; i++)
			{
				if(array2.datas[i].have_rigidbody)
					continue;
				
				for(int j=0; j<Length; j++)
				{
					int result = Game.System.Util.TestInteractionCollider(ref array2.datas[indices2[j]], indices2[j], ref array2.datas[i], i);
					if(result == 1)
					{
						//Collision!
						if(array2.datas[i].trigger == true)
							continue;
						
						collisionResolveInfos.Add
						(
							new CollisionResolveInfo
							{
								ACollider = indices2[j],
								BCollider = i,
								ARigidbody = indices3[j],
								BRigidbody = -1
							}
						);
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
						collisionResolveInfos.Add
						(
							new CollisionResolveInfo
							{
								ACollider = indices2[i],
								BCollider = indices2[j],
								ARigidbody = indices3[i],
								BRigidbody = indices3[j]
							}
						);
					}
				}
			}
			
			
			//	Calc Collision Resolve
			collisionApplyInfos.Clear();
			for(int i=0; i<collisionResolveInfos.Count; i++)
			{
				CollisionResolveInfo node = collisionResolveInfos[i];
				
				float a_mass_inv, b_mass_inv;
				float a_inertia_inv, b_inertia_inv;
				Vector2 velocity_ab;	// B Velocity - A Velocity
				
				float e = (array2.datas[node.ACollider].material.Bounciness + array2.datas[node.BCollider].material.Bounciness) / 2.0f;
				
				
				a_mass_inv = array3.datas[node.ARigidbody].mass_inv;
				
				if(node.BRigidbody < 0)
				{
					b_mass_inv = 1.0f;
					velocity_ab = array3.datas[node.ARigidbody].velocity.negate;
				} // B Is Not Rigidbody
				else
				{
					b_mass_inv = array3.datas[node.BRigidbody].mass_inv;
					velocity_ab = array3.datas[node.BRigidbody].velocity - array3.datas[node.ARigidbody].velocity;
				} // B Is Rigidbody
				
				
				//=================================================================================
				
				List<Component.CollisionResult> results = array2.datas[node.ACollider].results;
				for(int j=0; j<results.Count; j++)
				{
					Vector2 relative_ap;	// Contact Point - A Center Position
					Vector2 relative_bp;	// Contact Point - B Center Position
					
					if(results[j].contact_points == null)
						continue;
					
					for(int k=0; k<results[j].contact_points.Length; k++)
					{
						
						relative_ap = results[j].contact_points[k].point - array2.datas[node.ACollider].transformed_center;
						relative_bp = results[j].contact_points[k].point - array2.datas[node.BCollider].transformed_center;
						
						a_inertia_inv = a_mass_inv / UMath.Pow2(relative_ap.length);
						b_inertia_inv = b_mass_inv / UMath.Pow2(relative_bp.length);
						
						/*
						float CalcImpulseScalar(float e, float aInv_mass, float aInv_inertia, float bInv_mass, float bInv_inertia,
										  Vector2 normal, Vector2 velocity_AToB, Vector2 r_AToP, Vector2 r_BToP)
						*/
						
						float impulse = Mechanics.CalcImpulseScalar(e, a_mass_inv, a_inertia_inv, b_mass_inv, b_inertia_inv, results[j].normal, velocity_ab, relative_ap, relative_bp);
						
						//	Vector2 CalcLinearVelocity(float impulse_scalar, float inv_mass, Vector2 normal)
						Vector2 linear_velocity = Mechanics.CalcLinearVelocity(impulse, a_mass_inv, results[j].normal);
						//	float CalcAngularVelocity(float impulse_scalar, float inv_inertia, Vector2 r_CToP, Vector2 normal)
						float angular_velocity = Mechanics.CalcAngularVelocity(impulse, a_inertia_inv, relative_ap, results[j].normal);
						
						collisionApplyInfos.Add
						(
							new CollisionApplyInfo
							{
								Rigidbody = node.ARigidbody,
								LinearVelocity = linear_velocity,
								AngularVelocity = angular_velocity
							}
						);
						
						if(node.BRigidbody >= 0)
						{
							linear_velocity = Mechanics.CalcLinearVelocity(impulse, b_mass_inv, results[j].normal.negate);
							angular_velocity = Mechanics.CalcAngularVelocity(impulse, b_inertia_inv, relative_bp, results[j].normal.negate);
							
							collisionApplyInfos.Add
							(
								new CollisionApplyInfo
								{
									Rigidbody = node.BRigidbody,
									LinearVelocity = linear_velocity,
									AngularVelocity = angular_velocity
								}
							);
						}
						
						
					}
				}
			}
			
			
			
			//	Apply Resolve Result
			for(int i=0; i<collisionApplyInfos.Count; i++)
			{
				// Apply it!
				array3.datas[collisionApplyInfos[i].Rigidbody].velocity = array3.datas[collisionApplyInfos[i].Rigidbody].velocity + collisionApplyInfos[i].LinearVelocity * DeltaTime();
				array3.datas[collisionApplyInfos[i].Rigidbody].angular_velocity = array3.datas[collisionApplyInfos[i].Rigidbody].angular_velocity + collisionApplyInfos[i].AngularVelocity * DeltaTime();
				
				Console.WriteLine("Apply Linear : {0} : {1}",collisionApplyInfos[i].LinearVelocity.x, collisionApplyInfos[i].LinearVelocity.y);
				
				Console.WriteLine("Apply Angular : {0}",collisionApplyInfos[i].AngularVelocity);
			}
			
			for(int i=0; i<Length; i++)
			{
				array1.datas[indices1[i]].position = array1.datas[indices1[i]].position + array3.datas[indices3[i]].velocity * DeltaTime() * 0.5f;
				array1.datas[indices1[i]].angle = array1.datas[indices1[i]].angle + array3.datas[indices3[i]].angular_velocity * DeltaTime() * 0.5f;
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