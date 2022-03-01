using System;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private ECS.Manager ecs_manager;
		private WorldCamera camera;
		
		private System.ColorPolygon system_colorPolygon;
		private System.Collision system_collision;
		
		private Resource.IPolygon center;
		private Resource.IColor center_color;
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 10.0f, Window.Width(), Window.Height());
			system_colorPolygon = new System.ColorPolygon(Render, camera);
			system_collision = new System.Collision();
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(system_colorPolygon, system_collision);
			
			
			ECS.Archetype at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorPolygon),typeof(Component.Collider));
			
			
			for(int i=0; i<10; i++)
			{
				Byte[] col = URandom.Bytes(3);
				
				Vector2 position = URandom.Vector2(-3.0f,3.0f);
				//Vector2 scale = new Vector2(1.0f,1.0f);
				//float angle = URandom.Float(360.0f);
				
				//Vector2 position = new Vector2(i * 0.5f - 1.0f, 0.3f);
				Vector2 scale = new Vector2(3.0f,2.0f);
				float angle = 10.0f;
				
				Vector2[] vertices = new Vector2[3];
				vertices[0] = new Vector2(-0.5f, -0.5f);
				vertices[1] = new Vector2(-0.5f, 0.5f);
				vertices[2] = new Vector2(0.5f, -0.5f);
				
				factory.SetComponentModels
				(
					at,
					Component.Transform.Create(position,scale,angle),
					Component.ColorPolygon.Default(Render).Color(255,255,0,0),
					Component.Collider.Polygon(vertices)
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
		
		public override void OnUpdateBeforeInput(){ system_collision.Run(); }
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