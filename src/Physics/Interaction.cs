using System;
using System.Collections.Generic;

public static class Interaction
{
	//	Resources ============================================================================
	
	public abstract class Shape
	{
		public Vector2 center;
		public abstract Vector2 Support(Vector2 dir);
	}
	
	public class Circle : Shape
	{
		public float radius;
		public Circle(Vector2 c, float r)
		{
			this.center = c;
			this.radius = r;
		}
		public override Vector2 Support(Vector2 dir)
		{
			return center + dir.normalize * radius;
		}
	}
	
	public class Polygon : Shape
	{
		public Vector2[] vertices;
		public Polygon(Vector2 c, Vector2[] vs)
		{
			this.center = c;
			this.vertices = vs;
		}
		public override Vector2 Support(Vector2 dir)
		{
			float furthestDistance = float.MinValue;
			Vector2 furthestVertex = Vector2.Zero;
			
			foreach(Vector2 v in vertices)
			{
				float dis = Vector2.Dot(v,dir);
				if(dis > furthestDistance)
				{
					furthestDistance = dis;
					furthestVertex = v;
				}
			}
			return furthestVertex;
		}
	}
	
	public class Edge
	{
		public float depth;
		public Vector2 normal;
		public int index;
		
		public Edge(){}
		public Edge(bool b){ this.index = b?1:-1; }
		public bool fail{ get{ return this.index == -1; } }
	}
	
	public class ClipPoint
	{
		public Vector2 point;
		public float depth;
		public ClipPoint(Vector2 p){ this.point = p; }
	}
	
	private class ClipEdge
	{
		public Vector2 max;
		public Vector2 v1;
		public Vector2 v2;
		
		public Vector2 edge{ get{ return v2 - v1; } }
		public ClipEdge(Vector2 m, Vector2 a, Vector2 b)
		{
			this.max = m;
			this.v1 = a;
			this.v2 = b;
		}
		public float Dot(Vector2 v)
		{
			return Vector2.Dot(edge,v);
		}
	}
	
	
	//	Methods ==============================================================================
	
	private static Vector2 Support(Shape a, Shape b, Vector2 dir)
	{
		Vector2 av = a.Support(dir);
		Vector2 bv = b.Support(dir.negate);
		
		return av - bv;
	}
	
	private static Edge FindClosestEdge(List<Vector2> simplex)
	{
		Edge closest = new Edge();
		closest.depth = float.MaxValue;
		
		for(int i=0; i<simplex.Count; i++)
		{
			int j = (i + 1 == simplex.Count) ? 0 : i + 1;
			
			Vector2 ab = simplex[j] - simplex[i];
			Vector2 oa = simplex[i];
			
			Vector2 n = Vector2.TripleProduct(ab,oa,ab);
			n.SetNormalize();
			
			float d = Vector2.Dot(n,simplex[i]);
			if(d < closest.depth)
			{
				closest.depth = d;
				closest.normal = n;
				closest.index = j;
				
			}
		}
		return closest;
	}
	
	private static ClipEdge Best(Vector2[] vertices, Vector2 normal)
	{
		int c = vertices.Length;
		float max = float.MinValue;
		int index = 0;
		for(int i=0; i<c; i++)
		{
			float projection = Vector2.Dot(normal,vertices[i]);
			if(projection > max)
			{
				max = projection;
				index = i;
			}
		}
		
		Vector2 v = vertices[index];
		Vector2 v1 = vertices[((index + 1) < c ? index + 1 : 0)];
		Vector2 v0 = vertices[((index - 1) >= 0 ? index - 1 : c - 1)];
		
		Vector2 l = v - v1;
		Vector2 r = v - v0;
		
		l.SetNormalize();
		r.SetNormalize();
		
		if(Vector2.Dot(r,normal) <= Vector2.Dot(l,normal))
			return new ClipEdge(v,v0,v);
		else
			return new ClipEdge(v,v,v1);
	}
	
	private static List<ClipPoint> ClipLine(Vector2 v1, Vector2 v2, Vector2 normal, float o)
	{
		List<ClipPoint> cp = new List<ClipPoint>();
		float d1 = Vector2.Dot(normal,v1) - o;
		float d2 = Vector2.Dot(normal,v2) - o;
		
		if(d1 >= 0.0f) cp.Add(new ClipPoint(v1));
		if(d2 >= 0.0f) cp.Add(new ClipPoint(v2));
		
		if(d1 * d2 < 0.0f)
		{
			Vector2 e = v2 - v1;
			float u = d1 / (d1 - d2);
			e *= u;
			e += v1;
			cp.Add(new ClipPoint(e));
		}
		
		return cp;
	}
	
	
	//	Actualy Use This =====================================================================
	
	public static List<Vector2> GJK(Shape a, Shape b)
	{
		Vector2 sab, sac, sao;
		Vector2 dir = b.center - a.center;
		List<Vector2> simplex = new List<Vector2>(3);
		
		for(int i=0;i<3;i++)
			simplex.Add(Vector2.Zero);
		
		int index = 0;
		
		if ((dir.x == 0.0f) && (dir.y == 0.0f))
			dir.x = 1.0f;
		
		simplex[0] = Support(a,b,dir);
		if(Vector2.Dot(simplex[0],dir) <= 0)
			return null;
		
		dir.SetNegate();
		
		int tryc = 1000;
		while(--tryc >= 0)
		{
			simplex[++index] = Support(a,b,dir);
			if(Vector2.Dot(simplex[index],dir) <= 0)
				return null;
			
			sao = -simplex[index];
			if(index < 2)
			{
				sab = simplex[0] - simplex[1];
				dir = Vector2.TripleProduct(sab,sao,sab);
				if(dir.lengthSquared == 0.0f)
					dir = sab.right;
				continue;
			}
			
			sab = simplex[1] - simplex[2];
			sac = simplex[0] - simplex[2];
			
			Vector2 sacperp = Vector2.TripleProduct(sab,sac,sac);
			if(Vector2.Dot(sacperp,sao) >= 0)
				dir = sacperp;
			else
			{
				Vector2 sabperp = Vector2.TripleProduct(sac,sab,sab);
				if(Vector2.Dot(sabperp,sao) < 0)
					return simplex;
				simplex[0] = simplex[1];
				dir = sabperp;
			}
			simplex[1] = simplex[2];
			index--;
		}
		return null;
	}
	
	public static Edge EPA(Shape a, Shape b, List<Vector2> simplex)
	{
		int tryc = 1000;
		while(--tryc >= 0)
		{
			Edge e = FindClosestEdge(simplex);
			Vector2 p = Support(a,b,e.normal);
			float d = Vector2.Dot(p,e.normal);
			if((d - e.depth) < 0.0001f)
			{
				e.depth = d;
				return e;
			}
			else
			{
				simplex.Insert(e.index,p);
			}
		}
		return new Edge(false);
	}
	
	public static List<ClipPoint> Clip(Polygon a, Polygon b, Edge epa_edge)
	{
		ClipEdge e1 = Best(a.vertices,epa_edge.normal);
		ClipEdge e2 = Best(b.vertices,epa_edge.normal.negate);
		
		ClipEdge refe, ince;
		bool flip = false;
		if(Math.Abs(e1.Dot(epa_edge.normal)) <= Math.Abs(e2.Dot(epa_edge.normal)))
		{
			refe = e1;
			ince = e2;
		}
		else
		{
			refe = e2;
			ince = e1;
			flip = true;
		}
		
		Vector2 refv = refe.edge;
		refv.SetNormalize();
		
		float o = Vector2.Dot(refv,refe.v1);
		List<ClipPoint> cp = ClipLine(ince.v1,ince.v2,refv,o);
		if(cp.Count < 2) return null;
		
		o = Vector2.Dot(refv,refe.v2);
		cp = ClipLine(cp[0].point,cp[1].point,refv.negate,-o);
		if(cp.Count < 2) return null;
		
		Vector2 refNorm = refe.edge.left;
		refNorm.SetNormalize();
		if(flip) refNorm.SetNegate();
		
		float max = Vector2.Dot(refNorm,refe.max);
		
		cp[0].depth = Vector2.Dot(refNorm,cp[0].point) - max;
		cp[1].depth = Vector2.Dot(refNorm,cp[1].point) - max;
		
		if(cp[1].depth < 0.0f)
			cp.RemoveAt(1);
		if(cp[0].depth < 0.0f)
			cp.RemoveAt(0);
		
		if(cp.Count == 0)
			cp = null;
		
		return cp;
	}
	
	public static List<ClipPoint> Clip(Polygon a, Circle b, Edge epa_edge)
	{
		List<ClipPoint> cp = new List<ClipPoint>();
		
		ClipEdge ae = Best(a.vertices,epa_edge.normal);
		Vector2 bep = b.center + (epa_edge.normal * -b.radius);
		
		Vector2 p = bep - ae.max;
		float l = p.length;
		cp.Add(new ClipPoint(ae.max));
		cp[0].depth = l;
		
		return cp;
	}
	
	public static List<ClipPoint> Clip(Circle a, Polygon b, Edge epa_edge)
	{
		return Interaction.Clip(b,a,epa_edge);
	}
	
	public static List<ClipPoint> Clip(Circle a, Circle b, Edge epa_edge)
	{
		List<ClipPoint> cp = new List<ClipPoint>();
		
		float dis = Vector2.Distance(a.center,b.center);
		Vector2 p = a.center + epa_edge.normal * (dis * 0.5f);
		cp.Add(new ClipPoint(p));
		cp[0].depth = epa_edge.depth * 0.5f;
		
		return cp;
	}
}