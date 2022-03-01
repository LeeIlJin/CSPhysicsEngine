using System;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private ECS.Manager ecs_manager;
		private WorldCamera camera;
		
		private System.ColorShape system_colorShape;
		
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 10.0f, Window.Width(), Window.Height());
			system_colorShape = new System.ColorShape(camera);
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(system_colorShape);
			
			
			ECS.Archetype at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorShape));
			
			
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
					Component.ColorShape.DefaultPolygon(Render).Color(255,col[0],col[1],col[2])
				);
				factory.CreateEntity(at);
			}
			
			ecs_manager = new ECS.Manager(factory);
			factory.Dispose();
			
			Render.CreateColorPolygon
			(
				255,255,0,0,
				new Vector2(-1.0f,-1.0f),
				new Vector2(-1.0f,1.0f),
				new Vector2(1.0f,1.0f),
				new Vector2(1.0f,-1.0f)
			);
			
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
			system_colorShape.Run();
		}
		public override void OnUpdateAfterRender(){}
		
		public override void OnFrameEnd(){}
	}
}