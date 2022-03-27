using System;

namespace Game.Component
{
	public struct Rigidbody : ECS.IComponentData
	{
		private int friends_index;
		public int collider_index{ get{ return friends_index; } }
		
		
		
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