using System;

namespace Game.Component
{
	public struct Rigidbody : ECS.IComponentData
	{
		private int friends_index;
		public int collider_index{ get{ return friends_index; } }
		
		public Vector2 velocity;
		public float angular_velocity;

		public float gravity_factor;
		public float drag_factor;
		
		public bool fix_angle;
		
		public Rigidbody Gravity(float factor){ gravity_factor = factor * 9.81f; return this; }
		public Rigidbody Drag(float factor){ drag_factor = factor; return this; }
		
		
		public static Rigidbody Create()
		{
			return new Rigidbody
			{
				
			};
		}
		
		public void DeepCopy(){}
		public void SetFriend(int index){ friends_index = index; }
	}
}