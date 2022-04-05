using System;
using System.Collections.Generic;

namespace Game.System
{
	public static class Util
	{
		public static int TestInteractionCollider(ref Component.Transform tA, ref Component.Collider cA, int iA, ref Component.Transform tB, ref Component.Collider cB, int iB)
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
			
			
			if(AtargetB == false && BtargetA == false)
				return 0;
			
			Vector2 centerA = cA.center + tA.position;
			Vector2 centerB = cB.center + tB.position;
			
			if((cA.radius * tA.size) + (cB.radius * tB.size) < Vector2.Distance(centerA, centerB))
				return 0;
			
			//	Interaction Functions
			//	List<Vector2> GJK(Shape a, Shape b)
			//	Edge EPA(Shape a, Shape b, List<Vector2> simplex)
			//	List<ClipPoint> Clip(Polygon a, Polygon b, Edge epa_edge)
			//	List<ClipPoint> Clip(Polygon a, Circle b, Edge epa_edge)
			//	List<ClipPoint> Clip(Circle a, Circle b, Edge epa_edge)
			
			Interaction.Shape shapeA, shapeB;
			if(cA.isPolygon){ shapeA = new Interaction.Polygon(centerA, GetColliderVertices(tA, cA)); }
			else{ shapeA = new Interaction.Circle(centerA, cA.radius * tA.size); }
			
			if(cB.isPolygon){ shapeB = new Interaction.Polygon(centerB, GetColliderVertices(tB, cB)); }
			else{ shapeB = new Interaction.Circle(centerB, cB.radius * tB.size); }
			
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
		
		
		public static Vector2[] GetColliderVertices(Component.Transform transform, Component.Collider collider)
		{
			Vector2[] result = new Vector2[collider.vertices.Length];
			for(int i=0; i<result.Length; i++)
			{
				result[i] = UMath.Transform(collider.vertices[i], transform.position, transform.scale, -transform.angle);
			}
			return result;
		}
	}
}