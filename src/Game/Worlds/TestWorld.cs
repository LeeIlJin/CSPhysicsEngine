using System;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private ECS.Manager ecs_manager;
		private WorldCamera camera;
		
		private System.ColorPolygon system_colorPolygon;
		
		
		private Resource.IPolygon center;
		private Resource.IColor center_color;
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 10.0f, Window.Width(), Window.Height());
			system_colorPolygon = new System.ColorPolygon(Render, camera);
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(system_colorPolygon);
			
			
			ECS.Archetype at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorPolygon));
			
			
			for(int i=0; i<2500; i++)
			{
				Byte[] col = URandom.Bytes(3);
				
				Vector2 position = URandom.Vector2(-3.0f,3.0f);
				Vector2 scale = new Vector2(1.0f,1.0f);
				float angle = URandom.Float(360.0f);
				
				factory.SetComponentModels
				(
					at,
					Component.Transform.Create(position,scale,angle),
					Component.ColorPolygon.Default(Render).Color(255,col[0],col[1],col[2])
				);
				factory.CreateEntity(at);
			}
			
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
			system_colorPolygon.Run();
			
			Render.RenderColorPolygon(center,center_color,new Vector2(Window.Width() * 0.5f, Window.Height() * 0.5f),new Vector2(3.0f,3.0f));
		}
		public override void OnUpdateAfterRender(){}
		
		public override void OnFrameEnd(){}
	}
}