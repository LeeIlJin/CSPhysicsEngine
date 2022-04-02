using System;
using System.Collections.Generic;

namespace Game.Component
{
	public struct CollisionResult
	{
		public readonly Interaction.ClipPoint[] contact_points;
		public readonly int target;
		public readonly Vector2 normal;
		public readonly float depth;
		
		public readonly bool isB;
		public bool isA{ get{ return !isB; } }
		
		public CollisionResult(int _target, Vector2 _normal, float _depth, List<Interaction.ClipPoint> _contact_points, bool _isB)
		{
			target = _target;
			normal = _normal;
			depth = _depth;
			if(_contact_points == null)
				contact_points = null;
			else
				contact_points = _contact_points.ToArray();
			isB = _isB;
		}
	}
	
	public struct Collider : ECS.IComponentData
	{
		public Vector2[] vertices;
		public Vector2 center;
		public float radius;
		public bool trigger;
		
		public int layer;
		public int[] target_layers;
		
		public List<CollisionResult> results;
		
		public bool isPolygon{ get{ return (vertices != null); } }
		public bool isCircle{ get{ return (vertices == null); } }
		
		public Collider Layer(int _layer){ layer = _layer; return this; }
		public Collider TargetLayers(params int[] _layers){ target_layers = _layers; return this; }
		
		public static Collider Create(Vector2 _center, float _radius, params Vector2[] _vertices)
		{
			return new Collider
			{
				center = _center,
				radius = _radius,
				vertices = _vertices,
				layer = 0,
				target_layers = null
			};
		}
		
		public static Collider Polygon(params Vector2[] _vertices)
		{
			Vector2 _center = Vector2.Zero;
			float _radius = 0.0f;
			
			for(int i=0; i<_vertices.Length; i++)
			{
				_center += _vertices[i];
			}
			_center /= _vertices.Length;
			
			float distance = 0.0f;
			for(int i=0; i<_vertices.Length; i++)
			{
				distance = (_vertices[i] - _center).length;
				if(distance > _radius)
					_radius = distance;
			}
			
			return Create(_center, _radius, _vertices);
		}
		
		public static Collider Circle(float _radius)
		{
			return Create(Vector2.Zero, _radius, null);
		}
		
		public void DeepCopy()
		{
			if(vertices != null)
				vertices = (Vector2[])vertices.Clone();
			if(target_layers != null)
				target_layers = (int[])target_layers.Clone();
				
			results = new List<CollisionResult>();
		}
		public void SetFriend(int index){}
	}
}