using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Module
{
	public sealed class GDIRender : Module.RenderBase
	{
		//	Field ===============================================================================
		private Graphics windowGraphics;
		private Graphics drawGraphics;
		private Image drawBackBuffer;
		private Color drawBackColor;
		
		private void BeforeRender()
		{
			drawGraphics.Clear(drawBackColor);
		}
		
		private void AfterRender()
		{
			windowGraphics.DrawImage(drawBackBuffer,Point.Empty);
		}
		
		//	Test
		private Resource.IPolygon poly;
		private Resource.IColor col;
		private Resource.IColor col2;
		private float testangle;
		private void Test()
		{
			this.RenderColorPolygon(poly,col,new Vector2(200,200),new Vector2(2.0f,2.0f),testangle);
			testangle += 30.0f * Hull.Time.Delta();
			if(testangle > 360.0f)
				testangle = 0.0f;
				
			this.RenderColorPolygon(poly,col2,new Vector2(250,250),new Vector2(3.0f,3.0f),testangle);
		}
		
		//	Base's ==============================================================================
		public override void OnCreate(LoopOrder loop_order)
		{
			WindowForm fm = Hull.Window as WindowForm;
			if(fm == null)
				Hull.Exit();
			
			drawBackColor = fm.Form.BackColor;
			drawBackBuffer = (Image)new Bitmap(fm.Form.ClientSize.Width, fm.Form.ClientSize.Height);
			windowGraphics = fm.Form.CreateGraphics();
			drawGraphics = Graphics.FromImage(drawBackBuffer);
			
			loop_order.Add(this.BeforeRender,70);	// 71 ~ 89 = Order For Render
			loop_order.Add(this.AfterRender,90);
			
			loop_order.Add(this.Test,75);
		}
		public override void OnBegin()
		{
			poly = this.CreatePolygon(new Vector2(-30.0f,-30.0f),new Vector2(30.0f,-30.0f),new Vector2(30.0f,40.0f),new Vector2(-30.0f,30.0f));
			col = this.CreateColor(255,255,0,0);
			col2 = this.CreateColor(255,0,255,0);
			testangle = 0.0f;
		}
		public override void OnEnd()
		{
			poly.Dispose();
			col.Dispose();
			col2.Dispose();
		}
		public override void OnDispose()
		{
			windowGraphics.Dispose();
			drawGraphics.Dispose();
			drawBackBuffer.Dispose();
		}
		
		//	Render's ============================================================================
		
		//	Create Resource ===========================================================
		public override Resource.IPolygon CreatePolygon(params Vector2[] args)
		{
			return new PolygonWrap(args);
		}
		public override Resource.ICircle CraeteCircle(float radius)
		{
			return new CircleWrap(radius);
		}
		public override Resource.IColor CreateColor(byte a, byte r, byte g, byte b)
		{
			return new ColorWrap(a,r,g,b);
		}
		
		//	Render Method =============================================================
		public override void RenderColorPolygon(Resource.IPolygon polygon, Resource.IColor color, Vector2 pos, Vector2 scale)
		{
			PolygonWrap pw = polygon as PolygonWrap;
			if(pw == null)
				return;
			ColorWrap cw = color as ColorWrap;
			if(cw == null)
				return;
			
			pw.RenderLocal(cw.brush);
			pw.RenderWorld(drawGraphics,pos,scale);
		}
		public override void RenderColorPolygon(Resource.IPolygon polygon, Resource.IColor color, Vector2 pos, Vector2 scale, float angle)
		{
			PolygonWrap pw = polygon as PolygonWrap;
			if(pw == null)
				return;
			ColorWrap cw = color as ColorWrap;
			if(cw == null)
				return;
			
			pw.RenderLocal(cw.brush,angle);
			pw.RenderWorld(drawGraphics,pos,scale);
		}
		public override void RenderColorCircle(Resource.ICircle circle, Resource.IColor color, Vector2 pos, Vector2 scale)
		{
			CircleWrap ciw = circle as CircleWrap;
			if(ciw == null)
				return;
			ColorWrap cw = color as ColorWrap;
			if(cw == null)
				return;
			
			scale.MaxClamp();
			ciw.RenderLocal(cw.brush);
			ciw.RenderWorld(drawGraphics,pos,scale);
		}
		
		
		
		//	Wraps =====================================================================
		private abstract class ShapeWrap
		{
			protected Graphics graphics;
			protected Image image;
			protected float half_size;
			protected float size;
			
			public abstract void RenderLocal(Brush brush);
			public virtual void RenderLocal(Brush brush, float angle){}
			public void RenderWorld(Graphics g, Vector2 pos, Vector2 scale)
			{
				g.DrawImage(image,pos.x - half_size * scale.x, pos.y - half_size * scale.x, scale.x * size, scale.y * size);
			}
			
			protected void ShapeWrapDispose()
			{
				graphics.Dispose();
				image.Dispose();
			}
			
			protected static Color zero_color = Color.FromArgb(0,0,0,0);
		}
		
		// IPolygon ====================================
		private class PolygonWrap : ShapeWrap, Resource.IPolygon
		{
			public PointF[] vertices;
			internal PolygonWrap(Vector2[] args)
			{
				float max = float.MinValue;
				foreach(Vector2 v in args)
				{
					float dis = v.length;
					if(max < dis)
					{
						max = dis;
					}
				}
				half_size = max;
				size = max * 2.0f;
				
				vertices = new PointF[args.Length];
				for(int i=0; i<vertices.Length; i++)
				{
					vertices[i] = new PointF(args[i].x + half_size,half_size - args[i].y);
				}
				
				image = (Image)new Bitmap((int)size, (int)size);
				graphics = Graphics.FromImage(image);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}
			public override void RenderLocal(Brush brush)
			{
				graphics.Clear(zero_color);
				graphics.FillPolygon(brush,vertices);
			}
			public override void RenderLocal(Brush brush, float angle)
			{
				Matrix transform = new Matrix();
    			transform.RotateAt(angle, new PointF(half_size,half_size));
				graphics.Transform = transform;
				graphics.Clear(zero_color);
				graphics.FillPolygon(brush,vertices);
			}
			public int VertexCount(){ return vertices.Length; }
			public Vector2 Vertex(int i){ return new Vector2(vertices[i].X,vertices[i].Y); }
			public void Dispose(){ ShapeWrapDispose(); vertices = null; }
		}
		
		//	ICircle =====================================
		private class CircleWrap : ShapeWrap, Resource.ICircle
		{
			internal CircleWrap(float radius)
			{
				half_size = radius;
				size = radius * 2.0f;
				
				image = (Image)new Bitmap((int)size, (int)size);
				graphics = Graphics.FromImage(image);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}
			public override void RenderLocal(Brush brush)
			{
				graphics.Clear(zero_color);
				graphics.FillEllipse(brush,0.0f,0.0f,size,size);
			}
			public float Radius(){ return half_size; }
			public void Dispose(){ ShapeWrapDispose(); }
		}
		
		//	IColor ======================================
		private class ColorWrap : Resource.IColor
		{
			public SolidBrush brush;
			internal ColorWrap(byte a, byte r, byte g, byte b)
			{
				brush = new SolidBrush(Color.FromArgb(a,r,g,b));
			}
			public byte A(){ return brush.Color.A; }
			public byte R(){ return brush.Color.R; }
			public byte G(){ return brush.Color.G; }
			public byte B(){ return brush.Color.B; }
			public void SetA(byte v){ brush.Color = Color.FromArgb(v,R(),G(),B()); }
			public void SetR(byte v){ brush.Color = Color.FromArgb(A(),v,G(),B()); }
			public void SetG(byte v){ brush.Color = Color.FromArgb(A(),R(),v,B()); }
			public void SetB(byte v){ brush.Color = Color.FromArgb(A(),R(),G(),v); }
			public void SetARGB(byte a, byte r, byte g, byte b){ brush.Color = Color.FromArgb(a,r,g,b); }
			public void Dispose(){ brush.Dispose(); }
		}
	}
}