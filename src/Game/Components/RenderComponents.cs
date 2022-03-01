using System;

namespace Game.Component
{
	public struct ColorShape : ECS.IComponentData
	{
		public Resource.IColorShape resource;
		
		public ColorShape Color(byte a, byte r, byte g, byte b){ resource.SetARGB(a,r,g,b); return this; }
		
		public static ColorShape Create(Module.RenderBase renderModule, byte a, byte r, byte g, byte b, params Vector2[] args)
		{
			return new ColorShape
			{
				resource = renderModule.CreateColorPolygon(a,r,g,b,args)
			};
		}
		
		public static ColorShape Create(Module.RenderBase renderModule, byte a, byte r, byte g, byte b, float radius)
		{
			return new ColorShape
			{
				resource = renderModule.CreateColorCircle(a,r,g,b,radius)
			};
		}
		
		public static ColorShape DefaultPolygon(Module.RenderBase renderModule)
		{
			Vector2[] vertices = new Vector2[3];
			vertices[0] = new Vector2(-0.5f, -0.5f);
			vertices[1] = new Vector2(-0.5f, 0.5f);
			vertices[2] = new Vector2(0.5f, -0.5f);
			
			return new ColorShape
			{
				resource = renderModule.CreateColorPolygon(255, 48, 101, 172,vertices)
			};
		}
		
		public static ColorShape DefaultCircle(Module.RenderBase renderModule)
		{
			return new ColorShape
			{
				resource = renderModule.CreateColorCircle(255, 48, 101, 172, 0.5f)
			};
		}
		
		public void DeepCopy(){ resource = resource.Copy(); }
		public void SetFriend(int index){}
	}
	
}