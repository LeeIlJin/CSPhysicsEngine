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
		private System.Rigidbody system_rigidbody;
		
		private ECS.Entity player;
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate()
		{
			camera = new WorldCamera(Vector2.Zero, 50.0f, Window.Width(), Window.Height());
			system_colorPolygon = new System.ColorPolygon(Draw, camera);
			system_colorCircle = new System.ColorCircle(Draw, camera);
			system_collision = new System.Collision();
			system_rigidbody = new System.Rigidbody(1);
			
			Input.AddKeys(Keys.Up, Keys.Down, Keys.Right, Keys.Left, Keys.Q, Keys.W);
		}
		public override void OnBegin()
		{
			ECS.Factory factory = new ECS.Factory();
			factory.First_AddSystems(system_colorPolygon, system_colorCircle, system_collision, system_rigidbody);
			factory.SetNotifyComponent(typeof(Component.Collider), typeof(Component.Rigidbody));
			
			ECS.Archetype at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorPolygon),typeof(Component.Collider),typeof(Component.Rigidbody));
			
			Vector2 position, scale;
			Vector2[] vertices;
			
			for(int i=0; i<20; i++)
			{
				//Byte[] col = URandom.Bytes(3);
				
				position = URandom.Vector2(-3.0f,3.0f);
				//Vector2 scale = new Vector2(1.0f,1.0f);
				//float angle = URandom.Float(360.0f);
				
				//Vector2 position = new Vector2(i * 0.5f - 1.0f, 0.3f);
				scale = new Vector2(1.5f,1.0f);
				float angle = 5.0f;
				
				vertices = new Vector2[4];
				vertices[0] = new Vector2(-0.5f, -0.5f);
				vertices[1] = new Vector2(-0.5f, 0.5f);
				vertices[2] = new Vector2(0.5f, 0.5f);
				vertices[3] = new Vector2(0.5f, -0.5f);
				
				factory.SetComponentModels
				(
					at,
					Component.Transform.Create(position,scale,angle),
					Component.ColorPolygon.Create(255,0,255,0,vertices),
					Component.Collider.Polygon(vertices),
					Component.Rigidbody.Create()
				);
				factory.CreateEntity(at);
			}
			player = factory.CreateEntity(at);
			
			at = new ECS.Archetype(typeof(Component.Transform),typeof(Component.ColorPolygon),typeof(Component.Collider));
			position = new Vector2(0.0f,-8.5f);
			scale = new Vector2(200.0f,10.0f);
			
			vertices = new Vector2[4];
			vertices[0] = new Vector2(-0.5f, -0.5f);
			vertices[1] = new Vector2(-0.5f, 0.5f);
			vertices[2] = new Vector2(0.5f, 0.5f);
			vertices[3] = new Vector2(0.5f, -0.5f);
			
			factory.SetComponentModels
			(
				at,
				Component.Transform.Create(position,scale,0.0f),
				Component.ColorPolygon.Create(255,255,0,255,vertices),
				Component.Collider.Polygon(vertices)
			);
			factory.CreateEntity(at);
			
			
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
		
		public override void OnUpdateBeforeInput(){ system_collision.Run(); system_rigidbody.Run(); }
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
				transform.radian += anglespeed * Time.Delta();
			if(Input.IsKeyPress(Keys.W))
				transform.radian -= anglespeed * Time.Delta();
			
			//transform.angle = UMath.ClampAngle(transform.angle);
			
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