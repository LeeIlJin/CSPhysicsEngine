using System;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private ECS.Manager ecs_manager;
		private WorldCamera camera;
		
		private System.ColorPolygon system_colorPolygon;
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 10.0f, Window.Width(), Window.Height());
			system_colorPolygon = new System.ColorPolygon(Draw, camera);
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(system_colorPolygon);
			
			
			ECS.Archetype at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorPolygon));
			
			
			for(int i=0; i<2; i++)
			{
				//Byte[] col = URandom.Bytes(3);
				
				Vector2 position = URandom.Vector2(-3.0f,3.0f);
				//Vector2 scale = new Vector2(1.0f,1.0f);
				//float angle = URandom.Float(360.0f);
				
				//Vector2 position = new Vector2(i * 0.5f - 1.0f, 0.3f);
				Vector2 scale = new Vector2(3.0f,2.0f);
				float angle = 0.0f;
				
				Vector2[] vertices = new Vector2[3];
				vertices[0] = new Vector2(-0.5f, -0.5f);
				vertices[1] = new Vector2(-0.5f, 0.5f);
				vertices[2] = new Vector2(0.5f, -0.5f);
				
				factory.SetComponentModels
				(
					at,
					Component.Transform.Create(position,scale,angle),
					Component.ColorPolygon.Default().ARGB(255,0,0,0)
				);
				factory.CreateEntity(at);
			}
			
			ecs_manager = new ECS.Manager(factory);
			factory.Dispose();
		}
		public override void OnEnd()
		{
			
		}
		public override void OnDispose()
		{
			ecs_manager.Dispose();
		}
		
		
		//	Loop Events
		public override void OnFrameBegin(){}
		
		public override void OnUpdateBeforeInput(){}
		public override void OnUpdateAfterInput(){}
		public override void OnUpdateBeforeRender(){ camera.UpdateCamera(); }
		public override void OnRender()
		{
			system_colorPolygon.Run();
		}
		public override void OnUpdateAfterRender(){}
		
		public override void OnFrameEnd(){}
	}
}