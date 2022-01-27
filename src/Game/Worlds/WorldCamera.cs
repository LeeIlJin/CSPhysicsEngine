using System;

namespace Game
{
	public class WorldCamera
	{
		public Vector2 position;
		
		private float width;
		private float zoom;
		public float xFactor{ get; private set; }
		public float yFactor{ get; private set; }
		
		public readonly float screen_width_half;
		public readonly float screen_height_half;
		public readonly float screen_aspect_ratio;
		
		public void SetWidth(float w){ width = UMath.Max(0.001f,w); }
		public void Zooming(float f){ zoom = UMath.Max(0.001f,f); }
		
		public WorldCamera(Vector2 initPosition, float camera_width, int screen_width, int screen_height)
		{
			this.position = initPosition;
			this.width = camera_width;
			this.zoom = 1.0f;
			
			this.screen_width_half = (float)screen_width * 0.5f;
			this.screen_height_half = (float)screen_height * 0.5f;
			
			screen_aspect_ratio = screen_width_half / screen_height_half;
			this.xFactor = screen_width_half / width;
			this.yFactor = screen_height_half / (width / screen_aspect_ratio);
		}
		
		public void UpdateCamera()
		{
			float zoom_width = width * zoom;
			this.xFactor = 2.0f * screen_width_half / zoom_width;
			this.yFactor = 2.0f * screen_height_half / (zoom_width / screen_aspect_ratio);
		}
		
		public Vector2 WorldToRenderPosition(Vector2 p)
		{
			p -= position;
			p.x *= xFactor;
			p.y *= yFactor;
			
			p.x = p.x + screen_width_half;
			p.y = -p.y + screen_height_half;
			
			return p;
		}
		
		public Vector2 WorldToRenderScale(Vector2 s)
		{
			s.x *= xFactor;
			s.y *= xFactor;
			return s;
		}
		
		public Vector2 RenderToWorldPosition(Vector2 p)
		{
			p.x = p.x - screen_width_half;
			p.y = -p.y + screen_height_half;
			
			p.x /= xFactor;
			p.y /= yFactor;
			
			p += position;
			return p;
		}
	}
}