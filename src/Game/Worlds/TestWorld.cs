using System;

namespace Game
{
	public class TestWorld : Module.WorldNode
	{
		private Resource.IPolygon poly;
		private Resource.IColor col;
		private Resource.IColor col2;
		private float testangle;
		private void Test()
		{
			Render.RenderColorPolygon(poly,col,new Vector2(200,200),new Vector2(2.0f,2.0f),testangle);
			testangle += 30.0f * Time.Delta();
			if(testangle > 360.0f)
				testangle = 0.0f;
				
			Render.RenderColorPolygon(poly,col2,new Vector2(250,250),new Vector2(3.0f,3.0f),testangle);
			
			Vector2 pos;
			for(int i=0; i<50; i++)
			{
				pos = new Vector2(250 + i * 20, 250 + i * 10);
				Render.RenderColorPolygon(poly,col2,pos,new Vector2(0.5f,0.5f),testangle + i);
			}
		}
		
		public override void OnEnable(){}
		public override void OnDisable(){}
		
		public override void OnCreate(){}
		public override void OnBegin()
		{
			poly = Render.CreatePolygon(new Vector2(-30.0f,-30.0f),new Vector2(30.0f,-30.0f),new Vector2(30.0f,40.0f),new Vector2(-30.0f,30.0f));
			col = Render.CreateColor(255,255,0,0);
			col2 = Render.CreateColor(255,0,255,0);
			testangle = 0.0f;
		}
		public override void OnEnd()
		{
			poly.Dispose();
			col.Dispose();
			col2.Dispose();
		}
		public override void OnDispose(){}
		
		
		//	Loop Events
		public override void OnFrameBegin(){}
		
		public override void OnUpdateBeforeInput(){}
		public override void OnUpdateAfterInput(){}
		
		public override void OnRender(){ Test(); }
		public override void OnUpdateAfterRender(){}
		
		public override void OnFrameEnd(){}
	}
}