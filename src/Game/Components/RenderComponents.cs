using System;

namespace Game.Component
{
	public struct ColorPolygon : ECS.IComponentData
	{
		public Resource.IPolygon polygon;
		public Resource.IColor color;
		
		public ColorPolygon Color(byte a, byte r, byte g, byte b){ color.SetARGB(a,r,g,b); return this; }
		
		public static ColorPolygon Create(Module.RenderBase renderModule, byte a, byte r, byte g, byte b, params Vector2[] args)
		{
			return new ColorPolygon
			{
				polygon = renderModule.CreatePolygon(args),
				color = renderModule.CreateColor(a,r,g,b)
			};
		}
		
		public static ColorPolygon Default(Module.RenderBase renderModule)
		{
			Vector2[] vertices = new Vector2[3];
			vertices[0] = new Vector2(-0.5f, -0.5f);
			vertices[1] = new Vector2(-0.5f, 0.5f);
			vertices[2] = new Vector2(0.5f, -0.5f);
			
			return Create(renderModule, 255, 48, 101, 172, vertices);
		}
		
		public void DeepCopy(){ polygon = polygon.Copy(); color = color.Copy(); }
		public void SetFriend(int index){}
	}
	
	public struct ColorCircle : ECS.IComponentData
	{
		public Resource.ICircle circle;
		public Resource.IColor color;
		
		public ColorCircle Color(byte a, byte r, byte g, byte b){ color.SetARGB(a,r,g,b); return this; }
		
		public static ColorCircle Create(Module.RenderBase renderModule, byte a, byte r, byte g, byte b, float radius)
		{
			return new ColorCircle
			{
				circle = renderModule.CreateCircle(radius),
				color = renderModule.CreateColor(a,r,g,b)
			};
		}
		
		public static ColorCircle Default(Module.RenderBase renderModule)
		{
			return Create(renderModule, 255, 48, 101, 172, 0.5f);
		}
		
		public void DeepCopy(){ circle = circle.Copy(); color = color.Copy(); }
		public void SetFriend(int index){}
	}
}