using System;
using System.Collections.Generic;

namespace Game.System
{
	public sealed class Collision : ECS.SystemBase<Component.Transform, Component.Collider>
	{
		public override void Run()
		{
			int result_case = 0;
			for(int i=0; i<Length; i++)
				array2[indices2[i]].results.Clear();
			
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					result_case += TestInteraction(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]], indices2[i], ref array1.datas[indices1[j]], ref array2.datas[indices2[j]], indices2[j]);
				}
			}
			
			Console.WriteLine("Collision Result Case : {0}",result_case);
		}
		
		private static int TestInteraction(ref Component.Transform tA, ref Component.Collider cA, int iA, ref Component.Transform tB, ref Component.Collider cB, int iB)
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
			
			//	Interaction Functions
			//	List<Vector2> GJK(Shape a, Shape b)
			//	Edge EPA(Shape a, Shape b, List<Vector2> simplex)
			//	List<ClipPoint> Clip(Polygon a, Polygon b, Edge epa_edge)
			//	List<ClipPoint> Clip(Polygon a, Circle b, Edge epa_edge)
			//	List<ClipPoint> Clip(Circle a, Circle b, Edge epa_edge)
			
			Interaction.Shape shapeA, shapeB;
			if(cA.isPolygon){ shapeA = new Interaction.Polygon(centerA, GetVertices(tA, cA)); }
			else{ shapeA = new Interaction.Circle(centerA, cA.radius); }
			
			if(cB.isPolygon){ shapeB = new Interaction.Polygon(centerB, GetVertices(tB, cB)); }
			else{ shapeB = new Interaction.Circle(centerB, cB.radius); }
			
			List<Vector2> simplex = Interaction.GJK(shapeA, shapeB);
			if(simplex == null)
				return 0;
			
			Interaction.Edge edge = Interaction.EPA(shapeA, shapeB, simplex);
			if(edge.fail)
				return 0;
			
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
			
			
			if(AtargetB)
			{
				cA.results.Add(new Component.CollisionResult(iB, edge.normal, edge.depth, clipPoints, false));
			}
			if(BtargetA)
			{
				cB.results.Add(new Component.CollisionResult(iA, edge.normal, edge.depth, clipPoints, true));
			}
			return 1;
		}
		
		private static Vector2[] GetVertices(Component.Transform transform, Component.Collider collider)
		{
			Vector2[] result = new Vector2[collider.vertices.Length];
			for(int i=0; i<result.Length; i++)
			{
				result[i] = (collider.vertices[i] * transform.scale) + transform.position;
			}
			return result;
		}
		
		public override void OnCreate() {}
		public override void OnBegin() {}
		public override void OnEnable() {}
		public override void OnDisable() {}
		public override void OnEnd() {}
		public override void OnDispose() {}
	}
}