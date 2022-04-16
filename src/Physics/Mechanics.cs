using System;

/*
	e = Bouncing Const Scalar
	Inv_Mass = 1 / Mass
	Inv_Inertia = 1 / Inertia
	Inv_Inertia = Inv_Mass / (relative_AToP.Length ^ 2)
	
	velocity_AToB = B's Velocity - A's Velocity
	relative_AToP = Contact Point - A's Center
*/



public static class Mechanics
{
	public enum RelativeState
	{
		Leaving,
		Smashing,
		Stop
	}
	
	public static RelativeState GetRelativeStateOfBodies(Vector2 velocity_AToB, Vector2 normal)
	{
		float d = Vector2.Dot(velocity_AToB,normal);
		if(d == 0.0f)
			return RelativeState.Stop;
		else if(d < 0.0f)
			return RelativeState.Smashing;
		// d > 0.0f
		return RelativeState.Leaving;
	}
	
	public static float GetInverseInertia(Vector2 rotate_axis, Vector2 mass_center, float inv_mass)
	{
		float r = Vector2.Distance(rotate_axis, mass_center);
		return inv_mass / (r * r);
	}
	
	/*
	public static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c) //	(A X B) X C
	{
		float ac = Vector2.Dot(a,c); (a.x * c.x) + (a.y * c.y)
		float bc = Vector2.Dot(b,c); (b.x * c.x) + (b.y * c.y)
		return b * ac - a * bc;
	}
	*/
	
	//	Scalar J
	public static float CalcImpulseScalar(float e, float aInv_mass, float aInv_inertia, float bInv_mass, float bInv_inertia,
										  Vector2 normal, Vector2 velocity_AToB, Vector2 r_AToP, Vector2 r_BToP)
	{
		float temp = Vector2.Dot(velocity_AToB, normal);
		if(temp == 0.0f)
			return 0.0f;
		float up = temp * -(1.0f + e);
		
		float down = aInv_mass + bInv_mass;
		
		Vector2 a = r_AToP * aInv_inertia;
		Vector2 b = normal * aInv_inertia;
		Vector2 ai = Vector2.TripleProduct(a,b,r_AToP);
		
		a = r_BToP * bInv_inertia;
		Vector2 bi = Vector2.TripleProduct(a,b,r_BToP);
		
		float d = Vector2.Dot(ai + bi,normal);
		down += d;
		
		return up / down;
	}
	
	public static Vector2 CalcLinearVelocity(float impulse_scalar, float inv_mass, Vector2 normal)
	{
		return normal * (impulse_scalar * inv_mass);
	}
	
	public static float CalcAngularVelocity(float impulse_scalar, float inv_inertia, Vector2 r_CToP, Vector2 normal)
	{
		float d = Vector2.Cross(r_CToP, normal * impulse_scalar);
		return d * inv_inertia;
	}
	
	public static Vector2 CalcFriction(float e, float impulse_scalar, float aInv_mass, float aStatic_friction, float aDynamic_friction, float bInv_mass, float bStatic_friction, float bDynamic_friction, Vector2 normal, Vector2 velocity_AToB)
	{
		Vector2 tangent = velocity_AToB - normal * Vector2.Dot(velocity_AToB,normal);
		tangent.SetNormalize();
		float jt = -(1.0f + e) * Vector2.Dot(velocity_AToB,tangent);
		jt = jt / (aInv_mass + bInv_mass);
		
		float mu = aStatic_friction * aStatic_friction + bStatic_friction * bStatic_friction;
		Vector2 frictionImpulse;
		if(Math.Abs(jt) < impulse_scalar * mu)
			frictionImpulse = tangent * jt;
		else
			frictionImpulse = tangent * (-impulse_scalar * (aDynamic_friction * aDynamic_friction + bDynamic_friction * bDynamic_friction));
		
		return frictionImpulse;
	}
	
	public static Vector2 CalcDrag(float drag_factor, Vector2 velocity, float face = 1.0f, float pop = 1.0f)
	{
		float vsquared = velocity.lengthSquared;
		return velocity.normalize * (pop * vsquared * face * drag_factor * -0.5f);
	}
}