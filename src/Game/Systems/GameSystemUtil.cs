using System;
using System.Collections.Generic;

namespace Game.System
{
	public static class Util
	{
		public static void TransformCollider(ref Component.Transform t, ref Component.Collider c , Vector2 velocity, float angular_velocity)
		{
			Vector2 pos = t.position + velocity;
			float rad = t.radian + angular_velocity;
			
			c.transformed_vertices = GetColliderVertices(c, pos, t.scale, rad);
			
			c.transformed_center = c.center + t.position;
			
			c.transformed_radius = c.radius * t.size;
		}
		
		public static int TestInteractionCollider(ref Component.Collider cA, int iA, ref Component.Collider cB, int iB)
		{	
			bool AtargetB = false;
			bool BtargetA = false;
			
			
			if(cA.target_layers == null)
				AtargetB = true;
			else
			{
				for(int i=0; i<cA.target_layers.Length; i++)
				{
					if(cA.target_layers[i] == cB.layer)
					{
						AtargetB = true;
						break;
					}
				}
			}
			
			
			if(cB.target_layers == null)
				BtargetA = true;
			else
			{
				for(int i=0; i<cB.target_layers.Length; i++)
				{
					if(cB.target_layers[i] == cA.layer)
					{
						BtargetA = true;
						break;
					}
				}
			}
			
			
			if(AtargetB == false || BtargetA == false)
				return 0;
			
			if(cA.transformed_radius + cB.transformed_radius < Vector2.Distance(cA.transformed_center, cB.transformed_center))
				return 0;
			
			//	Interaction Functions
			//	List<Vector2> GJK(Shape a, Shape b)
			//	Edge EPA(Shape a, Shape b, List<Vector2> simplex)
			//	List<ClipPoint> Clip(Polygon a, Polygon b, Edge epa_edge)
			//	List<ClipPoint> Clip(Polygon a, Circle b, Edge epa_edge)
			//	List<ClipPoint> Clip(Circle a, Circle b, Edge epa_edge)
			
			Interaction.Shape shapeA, shapeB;
			if(cA.isPolygon){ shapeA = new Interaction.Polygon(cA.transformed_center, cA.transformed_vertices); }
			else{ shapeA = new Interaction.Circle(cA.transformed_center, cA.transformed_radius); }
			
			if(cB.isPolygon){ shapeB = new Interaction.Polygon(cB.transformed_center, cB.transformed_vertices); }
			else{ shapeB = new Interaction.Circle(cB.transformed_center, cB.transformed_radius); }
			
			//	GJK
			List<Vector2> simplex = Interaction.GJK(shapeA, shapeB);
			if(simplex == null)
				return 0;
			
			//	EPA
			Interaction.Edge edge = Interaction.EPA(shapeA, shapeB, simplex);
			if(edge.fail)
				return 0;
			
			
			//	Clip
			List<Interaction.ClipPoint> clipPoints;
			if(cA.isPolygon)
			{
				if(cB.isPolygon){ clipPoints = Interaction.Clip((Interaction.Polygon)shapeA, (Interaction.Polygon)shapeB, edge); }
				else{ clipPoints = Interaction.Clip((Interaction.Polygon)shapeA, (Interaction.Circle)shapeB, edge); }
			}
			else
			{
				if(cB.isPolygon){ clipPoints = Interaction.Clip((Interaction.Circle)shapeA, (Interaction.Polygon)shapeB, edge); }
				else{ clipPoints = Interaction.Clip((Interaction.Circle)shapeA, (Interaction.Circle)shapeB, edge); }
			}
			
			
			//	Add Result
			if(AtargetB)
			{
				cA.results.Add(new Component.CollisionResult(iB, edge.normal, edge.depth, clipPoints));
			}
			if(BtargetA)
			{
				cB.results.Add(new Component.CollisionResult(iA, edge.normal.negate, edge.depth, clipPoints));
			}
			
			return 1;
		}
		
		
		public static Vector2[] GetColliderVertices(Component.Collider collider, Vector2 position, Vector2 scale, float angle)
		{
			Vector2[] result = new Vector2[collider.vertices.Length];
			for(int i=0; i<result.Length; i++)
			{
				result[i] = UMath.Transform(collider.vertices[i], position, scale, angle);
			}
			return result;
		}
	}
	
}