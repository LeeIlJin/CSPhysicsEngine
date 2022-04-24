using System;

namespace Game.Component
{
	public struct Rigidbody : ECS.IComponentData
	{
		public Vector2 velocity;
		public float angular_velocity;
		
		public float mass_inv;
		public float inertia_inv;
		public float gravity_factor;
		public float drag_factor;
		public float angular_drag_factor;
		
		public bool fix_angle;
		
		public Rigidbody Density(float factor, Vector2[] vertices)
		{
			float[] mi = Mechanics.GetMassAndInertiaFromDensity(factor, vertices);
			mass_inv = (mi[0] == 0.0f) ? 0.0f : 1.0f / mi[0];
			inertia_inv = (mi[1] == 0.0f) ? 0.0f : 1.0f / mi[1];
			inertia_inv *= UMath.D2R;
			return this;
		}
		
		public Rigidbody Density(float factor, float radius)
		{
			float[] mi = Mechanics.GetMassAndInertiaFromDensity(factor, radius);
			mass_inv = (mi[0] == 0.0f) ? 0.0f : 1.0f / mi[0];
			inertia_inv = (mi[1] == 0.0f) ? 0.0f : 1.0f / mi[1];
			return this;
		}
		
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
				inertia_inv = 1.0f,
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