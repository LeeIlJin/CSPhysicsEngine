using System;
using System.Drawing;

namespace Game.System
{
	public sealed class ColorPolygon : ECS.SystemBase<Component.Transform, Component.ColorPolygon>
	{
		private readonly Module.Draw Draw;
		private WorldCamera camera;
		
		public ColorPolygon(Module.Draw _draw, WorldCamera _camera)
		{
			this.Draw = _draw;
			this.camera = _camera;
		}
		
		public override void OnCreate()
		{
			ForEach((ref Component.Transform transform, ref Component.ColorPolygon resource) =>
			{
				Vector2 pos, sca;
				pos = camera.WorldToRenderPosition(transform.position);
				sca = camera.WorldToRenderScale(transform.scale);
				//transform.angle += 1.75f;
				if(transform.angle >= 360.0f)
					transform.angle = 0.0f;
					
				//renderer.RenderColorPolygon(resource.polygon, resource.color, pos, sca, transform.angle);
				
				PointF[] points = new PointF[resource.points.Length];
				for(int i=0; i<resource.points.Length; i++)
				{
					points[i] = UMath.Transform(resource.points[i], pos, sca, transform.angle);
				}
				
				Draw.Graphics.FillPolygon(resource.brush, points);
			});
		}
		
		public override void OnBegin() {}
		public override void OnEnable() {}
		public override void OnDisable() {}
		public override void OnEnd() {}
		public override void OnDispose() {}
	}
}
