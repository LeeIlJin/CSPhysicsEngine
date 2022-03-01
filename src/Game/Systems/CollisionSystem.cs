using System;
using System.Collections.Generic;

namespace Game.System
{
	public sealed class Collision : ECS.SystemBase<Component.Transform, Component.Collider, Component.ColorPolygon>
	{
		public override void Run()
		{
			int result_case = 0;
			for(int i=0; i<Length; i++)
			{
				array2[indices2[i]].results.Clear();
				array3.datas[indices3[i]].color.SetR(255);
				array3.datas[indices3[i]].color.SetG(0);
			}
			
			
			for(int i=0; i<Length-1; i++)
			{
				for(int j=i+1; j<Length; j++)
				{
					int result = TestInteraction(ref array1.datas[indices1[i]], ref array2.datas[indices2[i]], indices2[i], ref array1.datas[indices1[j]], ref array2.datas[indices2[j]], indices2[j]);
					if(result == 1)
					{
						array3.datas[indices3[i]].color.SetR(0);
						array3.datas[indices3[j]].color.SetG(255);
					}

					result_case += result;
				}
			}
			
			Console.WriteLine("Collision Result Case : {0}", result_case);
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
			
			if((cA.radius * tA.size) + (cB.radius * tB.size) < Vector2.Distance(centerA, centerB))
				return 0;
			
			//	Interaction Functions
			//	List<Vector2> GJK(Shape a, Shape b)
			//	Edge EPA(Shape a, Shape b, List<Vector2> simplex)
			//	List<ClipPoint> Clip(Polygon a, Polygon b, Edge epa_edge)
			//	List<ClipPoint> Clip(Polygon a, Circle b, Edge epa_edge)
			//	List<ClipPoint> Clip(Circle a, Circle b, Edge epa_edge)
			
			Interaction.Shape shapeA, shapeB;
			if(cA.isPolygon){ shapeA = new Interaction.Polygon(centerA, GetVertices(tA, cA)); }
			else{ shapeA = new Interaction.Circle(centerA, cA.radius * tA.size); }
			
			if(cB.isPolygon){ shapeB = new Interaction.Polygon(centerB, GetVertices(tB, cB)); }
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
				cA.results.Add(new Component.CollisionResult(iB, edge.normal, edge.depth, clipPoints, false));
			}
			if(BtargetA)
			{
				cB.results.Add(new Component.CollisionResult(iA, edge.normal, edge.depth, clipPoints, true));
			}
			
			return 1;
		}
		
		//	Todo:
		//	Render 되는 도형과 가상공간좌표 상 도형의 위치가 매칭이 안되고 있을 가능성 있음
		//	특정 각도에서 도형의 충돌 처리가 불안정함
		private static Vector2[] GetVertices(Component.Transform transform, Component.Collider collider)
		{
			Vector2[] result = new Vector2[collider.vertices.Length];
			float rad = transform.angle * UMath.D2R;
			for(int i=0; i<result.Length; i++)
			{
				Vector2 temp = collider.vertices[i] * transform.scale;
				result[i].x = temp.x * (float)Math.Cos(rad) - temp.y * (float)Math.Sin(rad);
				result[i].y = temp.x * (float)Math.Sin(rad) + temp.y * (float)Math.Cos(rad);
				result[i] += transform.position;
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