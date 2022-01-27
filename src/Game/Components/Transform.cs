using System;

namespace Game
{
	public struct Transform : ECS.IComponentData
	{
		public Vector2 position;
		public Vector2 scale;
		public float angle;
		
		public float size{ get{ return UMath.Max(scale.x,scale.y); } }
		
		public static Transform Set(Vector2 _position, Vector2 _scale, float _angle)
		{
			return new Transform
			{
				position = _position,
				scale = _scale,
				angle = _angle
			};
		}
		
		public static Transform Default()
		{
			return new Transform
			{
				 position = Vector2.Zero,
				 scale = Vector2.One,
				 angle = 0.0f
			};
		}
		
		public void DeepCopy(){}
		public void SetFriend(int index){}
	}
}