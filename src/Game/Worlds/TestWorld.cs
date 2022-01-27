using System;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private ECS.Manager ecs_manager;
		private WorldCamera camera;
		
		private RenderColorPolygonSystem render_colorPolygonSystem;
		
		
		private Resource.IPolygon center;
		private Resource.IColor center_color;
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 10.0f, Window.Width(), Window.Height());
			render_colorPolygonSystem = new RenderColorPolygonSystem(Render, camera);
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(render_colorPolygonSystem);
			
			ECS.Archetype at = new ECS.Archetype(typeof(Game.Transform),typeof(Game.ColorPolygon));
			factory.SetComponentModels
			(
				at,
				Game.Transform.Set(new Vector2(0.5f,0.0f),Vector2.One,0.0f),
				Game.ColorPolygon.Default(Render)
			);
			factory.CreateEntity(at);
			
			ecs_manager = new ECS.Manager(factory);
			factory.Dispose();
			
			center = Render.CreatePolygon
			(
				new Vector2(-1.0f,-1.0f),
				new Vector2(-1.0f,1.0f),
				new Vector2(1.0f,1.0f),
				new Vector2(1.0f,-1.0f)
			);
			
			center_color = Render.CreateColor(255,255,0,0);
		}
		public override void OnEnd()
		{
			
		}
		public override void OnDispose()
		{
			ecs_manager.Dispose();
			center.Dispose();
			center_color.Dispose();
		}
		
		
		//	Loop Events
		public override void OnFrameBegin(){}
		
		public override void OnUpdateBeforeInput(){}
		public override void OnUpdateAfterInput(){}
		public override void OnUpdateBeforeRender(){ camera.UpdateCamera(); }
		public override void OnRender()
		{
			Console.WriteLine("Render");
			render_colorPolygonSystem.Run();
			
			Render.RenderColorPolygon(center,center_color,new Vector2(Window.Width() * 0.5f, Window.Height() * 0.5f),new Vector2(3.0f,3.0f));
		}
		public override void OnUpdateAfterRender(){}
		
		public override void OnFrameEnd(){}
	}
}