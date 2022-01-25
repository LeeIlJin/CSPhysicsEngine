using System;

namespace Game
{
	public class WorldCamera
	{
		public Vector2 position;
		public float width;
		public float inv_right{ get; private set; }
		public float inv_top{ get; private set; }
		
		public readonly float screen_width_half;
		public readonly float screen_height_half;
		
		public WorldCamera(Vector2 initPosition, float camera_width, int screen_width, int screen_height)
		{
			this.position = initPosition;
			this.width = camera_width;
			
			this.screen_width_half = (float)screen_width * 0.5f;
			this.screen_height_half = (float)screen_height * 0.5f;
			
			float aspect = screen_width_half / screen_height_half;
			this.inv_right = 0.5f / width;
			this.inv_top = aspect * inv_right;
		}
		
		public void UpdateCamera()
		{
			float aspect = screen_width_half / screen_height_half;
			this.inv_right = 0.5f / width;
			this.inv_top = aspect * inv_right;
		}
		
		public Vector2 WorldToRenderPosition(Vector2 p)
		{
			p -= position;
			p.x *= inv_right;
			p.y *= inv_top;
			
			p.x = p.x + screen_width_half;
			p.y = -p.y + screen_height_half;
			
			return p;
		}
		
		public Vector2 WorldToRenderScale(Vector2 s)
		{
			s.x *= inv_right;
			s.y *= inv_top;
			
			return s;
		}
		
		public Vector2 RenderToWorldPosition(Vector2 p)
		{
			p.x = p.x - screen_width_half;
			p.y = -p.y + screen_height_half;
			
			p.x /= inv_right;
			p.y /= inv_top;
			
			p += position;
			return p;
		}
	}
}