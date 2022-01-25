using System;

namespace Game
{
	public struct ColorPolygon : ECS.IComponentData
	{
		public Resource.IPolygon polygon;
		public Resource.IColor color;
		
		public ColorPolygon Color(byte a, byte r, byte g, byte b){ color.SetARGB(a,r,g,b); return this; }
		
		public static ColorPolygon Set(Module.RenderBase renderModule, params Vector2[] args)
		{
			return new ColorPolygon()
			{
				polygon = renderModule.CreatePolygon(args),
				color = renderModule.CreateColor(255,0,255,0)
			};
		}
		
		public void DeepCopy(){ polygon = polygon.Copy(); color = color.Copy(); }
		public void SetFriend(int index){}
	}
	
	public struct ColorCircle : ECS.IComponentData
	{
		public Resource.ICircle circle;
		public Resource.IColor color;
		
		public ColorCircle Color(byte a, byte r, byte g, byte b){ color.SetARGB(a,r,g,b); return this; }
		
		public static ColorCircle Set(Module.RenderBase renderModule, float radius)
		{
			return new ColorCircle()
			{
				circle = renderModule.CreateCircle(radius),
				color = renderModule.CreateColor(255,0,255,0)
			};
		}
		
		public void DeepCopy(){ circle = circle.Copy(); color = color.Copy(); }
		public void SetFriend(int index){}
	}
}