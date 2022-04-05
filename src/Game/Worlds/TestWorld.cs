using System;
using System.Windows.Forms;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private ECS.Manager ecs_manager;
		private WorldCamera camera;
		
		private System.ColorPolygon system_colorPolygon;
		private System.ColorCircle system_colorCircle;
		private System.Collision system_collision;
		
		private ECS.Entity player;
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 10.0f, Window.Width(), Window.Height());
			system_colorPolygon = new System.ColorPolygon(Draw, camera);
			system_colorCircle = new System.ColorCircle(Draw, camera);
			system_collision = new System.Collision();
			
			Input.AddKeys(Keys.Up, Keys.Down, Keys.Right, Keys.Left, Keys.Q, Keys.W);
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(system_colorPolygon, system_colorCircle, system_collision);
			
			ECS.Archetype at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorPolygon),typeof(Component.Collider));
			
			
			for(int i=0; i<8; i++)
			{
				//Byte[] col = URandom.Bytes(3);
				
				Vector2 position = URandom.Vector2(-3.0f,3.0f);
				//Vector2 scale = new Vector2(1.0f,1.0f);
				//float angle = URandom.Float(360.0f);
				
				//Vector2 position = new Vector2(i * 0.5f - 1.0f, 0.3f);
				Vector2 scale = new Vector2(1.0f,1.0f);
				float angle = 5.0f;
				
				Vector2[] vertices = new Vector2[3];
				vertices[0] = new Vector2(-0.5f, -0.5f);
				vertices[1] = new Vector2(-0.5f, 0.5f);
				vertices[2] = new Vector2(0.5f, -0.5f);
				
				factory.SetComponentModels
				(
					at,
					Component.Transform.Create(position,scale,angle),
					Component.ColorPolygon.Default().ARGB(255,0,0,0),
					Component.Collider.Polygon(vertices)
				);
				factory.CreateEntity(at);
			}
			player = factory.CreateEntity(at);
			
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
		public override void OnFrameBegin()
		{
			Window.Form.Text = string.Format("delta : {0}s ({1}ms) / {2}FPS / global : {3}s",Time.Delta(), Time.DeltaMs(), Time.Fps(), Time.Global());
		}
		
		public override void OnUpdateBeforeInput(){ system_collision.Run(); }
		public override void OnUpdateAfterInput()
		{
			Component.Transform transform = player.GetComponent<Component.Transform>(ecs_manager);
			float speed = 3.0f;
			float anglespeed = 15.0f;
			
			if(Input.IsKeyPress(Keys.Right))
				transform.position.x += speed * Time.Delta();
			if(Input.IsKeyPress(Keys.Left))
				transform.position.x -= speed * Time.Delta();
			if(Input.IsKeyPress(Keys.Up))
				transform.position.y += speed * Time.Delta();
			if(Input.IsKeyPress(Keys.Down))
				transform.position.y -= speed * Time.Delta();
			
			if(Input.IsKeyPress(Keys.Q))
				transform.angle += anglespeed * Time.Delta();
			if(Input.IsKeyPress(Keys.W))
				transform.angle -= anglespeed * Time.Delta();
			
			transform.angle = UMath.ClampAngle(transform.angle);
			
			player.SetComponent<Component.Transform>(ecs_manager, transform);
		}
		public override void OnUpdateBeforeRender(){ camera.UpdateCamera(); }
		public override void OnRender()
		{
			system_colorPolygon.Run();
			system_colorCircle.Run();
		}
		public override void OnUpdateAfterRender(){}
		
		public override void OnFrameEnd(){}
	}
}