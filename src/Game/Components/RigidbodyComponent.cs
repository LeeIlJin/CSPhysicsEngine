using System;

namespace Game.Component
{
	public struct Rigidbody : ECS.IComponentData
	{
		public Vector2 velocity;
		public float angular_velocity;
		
		public float mass_inv;
		public float gravity_factor;
		public float drag_factor;
		public float angular_drag_factor;
		
		public bool fix_angle;
		
		public Rigidbody Mass(float factor){ factor = 1.0f / UMath.Max(factor,0.0001f); return this;}
		public Rigidbody Gravity(float factor){ gravity_factor = factor * 9.81f; return this; }
		public Rigidbody Drag(float factor){ drag_factor = factor; return this; }
		public Rigidbody AngularDrag(float factor){ angular_drag_factor = factor; return this; }
		public Rigidbody FixAngle(bool b){ fix_angle = b; return this; }
		
		
		public static Rigidbody Create()
		{
			return new Rigidbody
			{
				velocity = new Vector2(0.0f,0.0f),
				angular_velocity = 0.0f,
				mass_inv = 1.0f,
				gravity_factor = 9.81f,
				drag_factor = 0.1f,
				angular_drag_factor = 0.1f,
				fix_angle = false
			};
		}
		
		public void DeepCopy(){}
		public void Notify(){}
	}
}