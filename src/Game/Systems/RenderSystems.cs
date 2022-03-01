using System;

namespace Game.System
{
	public sealed class ColorPolygon : ECS.SystemBase<Component.Transform, Component.ColorPolygon>
	{
		private readonly Module.RenderBase renderer;
		private WorldCamera camera;
		
		public ColorPolygon(Module.RenderBase _renderer, WorldCamera _camera)
		{
			this.renderer = _renderer;
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
					
				renderer.RenderColorPolygon(resource.polygon, resource.color, pos, sca, transform.angle);
			});
		}
		
		public override void OnBegin() {}
		public override void OnEnable() {}
		public override void OnDisable() {}
		public override void OnEnd() {}
		public override void OnDispose() {}
	}
}
	