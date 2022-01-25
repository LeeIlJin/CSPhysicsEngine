using System;

namespace Game
{
	public struct Transform : ECS.IComponentData
	{
		public Vector2 position;
		public Vector2 scale;
		public float angle;
		
		public float size{ get{ return UMath.Max(scale.x,scale.y); } }
		
		public void DeepCopy(){ position = Vector2.Zero; scale = Vector2.Zero; angle = 0.0f; }
		public void SetFriend(int index){}
	}
}