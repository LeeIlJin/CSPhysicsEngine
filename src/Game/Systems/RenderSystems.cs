using System;

namespace Game.System
{
	public sealed class ColorShape : ECS.SystemBase<Component.Transform, Component.ColorShape>
	{
		private WorldCamera camera;
		
		public ColorShape(WorldCamera _camera)
		{
			this.camera = _camera;
		}
		
		public override void OnCreate()
		{
			ForEach((ref Component.Transform transform, ref Component.ColorShape resource) =>
			{
				Vector2 pos, sca;
				pos = camera.WorldToRenderPosition(transform.position);
				sca = camera.WorldToRenderScale(transform.scale);
				transform.angle += 1.75f;
				if(transform.angle >= 360.0f)
					transform.angle = 0.0f;
				
				resource.resource.SetPositionThisFrame(pos);
				resource.resource.SetScaleThisFrame(sca);
				resource.resource.SetAngleThisFrame(transform.angle);
			});
		}
		
		public override void OnBegin() {}
		public override void OnEnable() {}
		public override void OnDisable() {}
		public override void OnEnd() {}
		public override void OnDispose() {}
	}
}
	