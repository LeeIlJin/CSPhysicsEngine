using System;

namespace Game
{
	public sealed class RenderColorPolygonSystem : ECS.SystemBase<Game.Transform, Game.ColorPolygon>
	{
		private readonly Module.RenderBase renderer;
		private WorldCamera camera;
		
		public RenderColorPolygonSystem(Module.RenderBase _renderer, WorldCamera _camera)
		{
			this.renderer = _renderer;
			this.camera = _camera;
		}
		
		public override void OnCreate()
		{
			ForEach((ref Game.Transform transform, ref Game.ColorPolygon resource) =>
			{
				Vector2 pos, sca;
				pos = camera.WorldToRenderPosition(transform.position);
				sca = camera.WorldToRenderScale(transform.scale);
				
				Console.WriteLine("System - Render : Pos({0}) , Sca({1})",pos.ToString(),sca.ToString());
				
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
	