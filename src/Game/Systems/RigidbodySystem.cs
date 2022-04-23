using System;
using System.Collections.Generic;

// https://github.com/tutsplus/ImpulseEngine


namespace Game.System
{
	public sealed class Rigidbody : ECS.SystemBase<Component.Transform, Component.Collider, Component.Rigidbody>
	{
		private struct CollisionResolveInfo
		{
			public int ATransform;
			public int BTransform;
			
			public int ACollider;
			public int BCollider;
			
			public int ARigidbody;
			public int BRigidbody;
		}
		
		private struct CollisionApplyInfo
		{
			public int Transform;
			public int Rigidbody;
			public Vector2 LinearVelocity;
			public float AngularVelocity;
		}
		
		private List<CollisionResolveInfo> collisionResolveInfos;
		private List<CollisionApplyInfo> collisionApplyInfos;
		
		private float positional_correction_percent;
		private float positional_correction_slop;
		
		private TickTimer timer;
		private int testCase;
		
		public Rigidbody(int test_case = 3)
		{
			collisionResolveInfos = new List<CollisionResolveInfo>();
			collisionApplyInfos = new List<CollisionApplyInfo>();
			
			positional_correction_percent = 0.2f;
			positional_correction_slop = 0.01f;
			
			timer = new TickTimer();
			testCase = test_case;
			
			timer.Begin();
		}
		
		public override void Run()
		{
			for(int test = 0; test<testCase; test++)
			{
				timer.End();
				float delta = timer.tick_s;
				
				//	=========================================================================
				//	Initialize And PreApply
				//	=========================================================================
				for(int i=0; i<Length; i++)
				{
					array2[indices2[i]].results.Clear();
					
					//	Gravity Apply
					array3.datas[indices3[i]].velocity.y -= array3.datas[indices3[i]].mass_inv * array3.datas[indices3[i]].gravity_factor * delta;
					
					//	Drag Apply
					array3.datas[indices3[i]].velocity += Mechanics.CalcDrag(array3.datas[indices3[i]].drag_factor, array3.datas[indices3[i]].velocity) * delta;
					
					//	Angular Drag Apply
					array3.datas[indices3[i]].angular_velocity += Mechanics.CalcAngularDrag(array3.datas[indices3[i]].angular_drag_factor, array3.datas[indices3[i]].angular_velocity) * delta;
					
					//	Velocity Apply
					array1.datas[indices1[i]].position += array3.datas[indices3[i]].velocity * delta;
					
					//	Angular Velocity Apply
					array1.datas[indices1[i]].radian = UMath.RepeatRadian(array1.datas[indices1[i]].radian + array3.datas[indices3[i]].angular_velocity * delta);
					
					
					
					Game.System.Util.TransformCollider(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]], Vector2.Zero, 0.0f);
					
				}
				
				
				//	=========================================================================
				//	Collider vs Rigidbody
				//	=========================================================================
				collisionResolveInfos.Clear();
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
									ATransform = indices1[j],
									BTransform = -1,
									ACollider = indices2[j],
									BCollider = i,
									ARigidbody = indices3[j],
									BRigidbody = -1
								}
							);
						}
					}
				}
				
				
				
				//	=========================================================================
				//	Rigidbody vs Rigidbody
				//	=========================================================================
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
									ATransform = indices1[i],
									BTransform = indices1[j],
									ACollider = indices2[i],
									BCollider = indices2[j],
									ARigidbody = indices3[i],
									BRigidbody = indices3[j]
								}
							);
						}
					}
				}
				
				
				//	=========================================================================
				//	Calc Collision Resolve
				//	=========================================================================
				collisionApplyInfos.Clear();
				for(int i=0; i<collisionResolveInfos.Count; i++)
				{
					CollisionResolveInfo node = collisionResolveInfos[i];
					
					float a_mass_inv, b_mass_inv;
					float a_angular_velocity, b_angular_velocity;
					Vector2 a_velocity, b_velocity;
					
					float e = (array2.datas[node.ACollider].material.Bounciness + array2.datas[node.BCollider].material.Bounciness) / 2.0f;
					
					a_mass_inv = array3.datas[node.ARigidbody].mass_inv;
					a_angular_velocity = array3.datas[node.ARigidbody].angular_velocity;
					a_velocity = array3.datas[node.ARigidbody].velocity;
					
					if(node.BRigidbody < 0) //	B Is Not Rigidbody
					{
						b_mass_inv = 1.0f;
						b_angular_velocity = 0.0f;
						b_velocity = Vector2.Zero;
					}
					else //	B Is Rigidbody
					{
						b_mass_inv = array3.datas[node.BRigidbody].mass_inv;
						b_angular_velocity = array3.datas[node.BRigidbody].angular_velocity;
						b_velocity = array3.datas[node.BRigidbody].velocity;
					}
					
					
					//	=========================================================================
					//	Solve Results
					//	=========================================================================
					List<Component.CollisionResult> results = array2.datas[node.ACollider].results;
					for(int j=0; j<results.Count; j++)
					{
						Vector2 normal = results[j].normal.normalize;	// A's smashed face(edge)'s normal vector
						
						//	Calculate correction position after smash
						Vector2 correction = normal * ((UMath.Max(results[j].depth - positional_correction_slop, 0.0f) / (a_mass_inv + b_mass_inv)) * positional_correction_percent);
						
						//	Apply collection to position
						array1.datas[node.ATransform].position += correction.negate * a_mass_inv;
						if(node.BTransform >= 0)
						{
							array1.datas[node.BTransform].position += correction * b_mass_inv;
						}
						
						//	Can't find any contact point
						if(results[j].contact_points == null)
							continue;
						
						//	Solve Each Contact Points ---------
						for(int k=0; k<results[j].contact_points.Length; k++)
						{
							float contact_count = (float)results[j].contact_points.Length;
							
							Vector2 relative_ap = results[j].contact_points[k].point - array2.datas[node.ACollider].transformed_center;
							Vector2 relative_bp = results[j].contact_points[k].point - array2.datas[node.BCollider].transformed_center;
							
							float a_inertia_inv = Mechanics.CalcInverseInertiaAtContactPoint(a_mass_inv, relative_ap);
							float b_inertia_inv = Mechanics.CalcInverseInertiaAtContactPoint(b_mass_inv, relative_bp);
							
							Vector2 a_total_velocity = Mechanics.CalcTotalVelocityAtContactPoint(a_velocity, a_angular_velocity, relative_ap);
							Vector2 b_total_velocity = Mechanics.CalcTotalVelocityAtContactPoint(b_velocity, b_angular_velocity, relative_bp);
							
							Vector2 velocity_ab = b_total_velocity - a_total_velocity;
							
							//	This is not Smashing!
							if(Mechanics.GetRelativeStateOfBodies(velocity_ab, normal) != Mechanics.RelativeState.Smashing)
								continue;
							
							//	Calculate impulse scalar
							float impulse = Mechanics.CalcImpulseScalar(e, a_mass_inv, a_inertia_inv, b_mass_inv, b_inertia_inv, normal, velocity_ab, relative_ap, relative_bp, contact_count);
							
							//	Calculate friction vector
							Vector2 friction = Mechanics.CalcFriction(e, impulse, a_mass_inv, array2.datas[node.ACollider].material.StaticFriction, array2.datas[node.ACollider].material.DynamicFriction, b_mass_inv, array2.datas[node.ACollider].material.StaticFriction, array2.datas[node.ACollider].material.DynamicFriction, normal, velocity_ab, contact_count);
							
							Vector2 impulse_vec = normal * impulse;
							
							collisionApplyInfos.Add
							(
								new CollisionApplyInfo
								{
									Transform = node.ATransform,
									Rigidbody = node.ARigidbody,
									LinearVelocity = Mechanics.CalcLinearVelocity(impulse_vec.negate, a_mass_inv) + Mechanics.CalcLinearVelocity(friction.negate, a_mass_inv),
									AngularVelocity = Mechanics.CalcAngularVelocity(impulse_vec.negate, a_inertia_inv, relative_ap) + Mechanics.CalcAngularVelocity(friction.negate, a_inertia_inv, relative_ap)
								}
							);
							
							if(node.BRigidbody >= 0)
							{
								collisionApplyInfos.Add
								(
									new CollisionApplyInfo
									{
										Transform = node.BTransform,
										Rigidbody = node.BRigidbody,
										LinearVelocity = Mechanics.CalcLinearVelocity(impulse_vec, b_mass_inv) + Mechanics.CalcLinearVelocity(friction, b_mass_inv),
										AngularVelocity = Mechanics.CalcAngularVelocity(impulse_vec, b_inertia_inv, relative_bp) + Mechanics.CalcAngularVelocity(friction, b_inertia_inv, relative_bp)
									}
								);
							}
						}
					}
				}
				
				
				
				//	=========================================================================
				//	Apply Resolve Result
				//	=========================================================================
				for(int i=0; i<collisionApplyInfos.Count; i++)
				{
					// Apply it!
					array3.datas[collisionApplyInfos[i].Rigidbody].velocity += collisionApplyInfos[i].LinearVelocity;
					array3.datas[collisionApplyInfos[i].Rigidbody].angular_velocity += collisionApplyInfos[i].AngularVelocity;
				}
				
				
				timer.Begin();
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