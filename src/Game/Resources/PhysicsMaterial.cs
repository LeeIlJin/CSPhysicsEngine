using System;

namespace Game.Resource
{
	public struct PhysicsMaterial
	{
		public float Bounciness;
		public float StaticFriction;
		public float DynamicFriction;
		
		public PhysicsMaterial(float b, float s, float d)
		{
			Bounciness = b;
			StaticFriction = s;
			DynamicFriction = d;
		}
		
		public static PhysicsMaterial Default()
		{
			return new PhysicsMaterial(0.0f,0.6f,0.6f);
		}
	}
}