# CSPhysicsEngine
  
  
- How to use my ECS DOD  
  
1. Declare and Assign : ECS.Factory = new ECS.Factory();  
  
2. Call Method : ECS.Factory.First_AddSystems(*Assignd system objects*)  
  
4. (Not Required) Call Method : ECS.Factory.Second_RegistComponentType<*Component type*>()  
  
5. Declare and Assign : ECS.Archetype = new ECS.Archetype(*Component types you want to give to the entity (typeof(_))*)  
	If you want shared one, use ECS.SharedArchetype  
  
6. (Not Required But I Recommande) Call Method : ECS.Factory.SetComponentModel(*Component type* , *Assignd Component*)  
	or SetComponentModels(ECS.Archetype / ECS.SharedArchetype , *Assignd Components*)  
  
7. Call Method : ECS.Factory.CreateEntity(ECS.Archetype or ECS.SharedArchetype or Both all)  
	This Method will return int[] , It means index about in component array (Order is the same as the order of types when assigning an Archetype)  
	(and If you use Both all for CreateEntity -> first is ECS.Archetype , second is ECS.SharedArchetype)  
	  
8. Declare and Assign : ECS.Manager = new ECS.Manager(ECS.Factory);  
  
9. You can use the system's Run function, or you can use a custom function with Indices and Arrays  
  
10. Don't forget to dispose the factory  
  
========================================  
↓↓↓↓↓↓↓↓↓↓ >> Example Code << ↓↓↓↓↓↓↓↓↓↓  
  
- OnCreate -  
ECS.Factory factory = new ECS.Factory();  
SystemBase box_color_render_system = new GSBoxColorRenderSystem(Camera);  
  
factory.First_AddSystems(box_color_render_system);  
  
ECS.Archetype at = new ECS.Archetype(typeof(GCTransform),typeof(GCColorBrush),typeof(GCBoxMesh));  
factory.SetComponentModel(typeof(GCBoxMesh),GCBoxMesh.Default);  
  
for(int i=0; i<100; i++)  
{  
	factory.SetComponentModels  
	(  
		at,  
		new GCTransform{ position = new Vector2(x,y), scale = new Vector2(sizex,sizey) },  
		GCColorBrush.RGB(r,g,b)  
	);  
	  
	factory.CreateEntity(at);  
}  
  
ECS.Manager manager = new ECS.Manager(factory);  
factory.Dispose();  
  
- OnRender  
box_color_render_system.Run();  
  
- GSBoxColorRenderSystem  
public class GSBoxColorRenderSystem : ECS.SystemBase<GCTransform,GCBoxMesh,GCColorBrush>  
{  
	private WorldCamera camera;  
	  
	public GSBoxColorRenderSystem(WorldCamera cam)  
	{  
		camera = cam;  
	}  
	public override void OnCreate()  
	{  
		ForEach((ref GCTransform transform,ref GCBoxMesh box,ref GCColorBrush brush) =>  
		{  
			Render Box (box,transform,brush)  
		});  
	}  
}  
