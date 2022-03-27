using System;

namespace Game.Component
{
	public struct Transform : ECS.IComponentData
	{
		public Vector2 position;
		public Vector2 scale;
		public float angle;
		
		public float size{ get{ return UMath.Max(scale.x,scale.y); } }
		
		public void Angle(float _angle){ angle = UMath.Repeat(_angle,0.0f,360.0f); }
		
		public static Transform Create(Vector2 _position, Vector2 _scale, float _angle)
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
			return Create(Vector2.Zero, Vector2.One, 0.0f);
		}
		
		public void DeepCopy(){}
		public void SetFriend(int index){}
		
	}
}