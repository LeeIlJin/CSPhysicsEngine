using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Module
{
	public sealed class GDIRenderModule : SystemModuleForRender
	{
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
		
		private Resource.IPolygon poly;
		private Resource.IColor col;
		private void Test()
		{
			this.RenderColorPolygon(poly,col,new Vector2(200,200),new Vector2(2.0f,2.0f),30.0f);
		}
		
		public override void OnCreate(LoopOrder loop_order)
		{
			FormModule fm = Hull.Window as FormModule;
			if(fm == null)
				Hull.Exit();
			
			alphaColor = Color.FromArgb(0.0f,0.0f,0.0f,0.0f);
			
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
		}
		public override void OnEnd(){}
		public override void OnDispose()
		{
			windowGraphics.Dispose();
			drawGraphics.Dispose();
			drawBackBuffer.Dispose();
		}
		
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
		public override void RenderColorPolygon(Resource.IPolygon p, Resource.IColor color, Vector2 pos, Vector2 scale)
		{
			PolygonWrap pw = p as PolygonWrap;
			if(pw == null)
				return;
			ColorWrap cw = color as ColorWrap;
			if(cw == null)
				return;
			
			pw.RenderWorld(drawGraphics,pos,scale);
		}
		public override void RenderColorPolygon(Resource.IPolygon p, Resource.IColor color, Vector2 pos, Vector2 scale, float angle)
		{
			PolygonWrap pw = p as PolygonWrap;
			if(pw == null)
				return;
			ColorWrap cw = color as ColorWrap;
			if(cw == null)
				return;
			
			pw.RenderLocal(color.brush,angle);
			pw.RenderWorld(drawGraphics,pos,scale);
		}
		public override void RenderColorCircle(Resource.ICircle c, Resource.IColor color, Vector2 pos, Vector2 scale)
		{
			CircleWrap pw = p as CircleWrap;
			if(pw == null)
				return;
			ColorWrap cw = color as ColorWrap;
			if(cw == null)
				return;
			
			scale.MaxClamp();
			pw.RenderWorld(drawGraphics,pos,scale);
		}
		
		//	Wraps =====================================================================
		private abstract class ShapeWrapper : IDispose
		{
			public Graphics graphics;
			public Image image;
			public float bigRadius;
			public float size;
			
			public abstract void RenderLocal(Brush brush, float angle);
			public void RenderWorld(Graphics g, Vector2 pos, Vector2 scale)
			{
				g.DrawImage(image,pos.x - bigRadius * scale.x, pos.y - bigRadius * scale.x, scale.x * size, scale.y * size);
			}
			
			public void Dispose()
			{
				graphics.Dispose();
				image.Dispose();
			}
		}

		private class PolygonWrap : ShapeWrapper, Resource.IPolygon
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
				bigRadius = max;
				size = max * 2.0f;
				
				vertices = new PointF[args.Length];
				for(int i=0; i<vertices.Length; i++)
				{
					vertices = new PointF(args[i].x + bigRadius,bigRadius - args[i].y);
				}
				
				image = (Image)new Bitmap((int)bigRadius * 2, (int)bigRadius * 2);
				graphics = Graphics.FromImage(image);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}
			
			public override void RenderLocal(Brush brush, float angle)
			{
				Matrix transform = new Matrix();
    			transform.RotateAt(angle, new PointF(bigRadius,bigRadius));
				graphics.Transform = transform;
				graphics.Clear(Color.FromArgb(0,0,0,0));
				graphics.FillPolygon(brush,vertices);
			}
			
			public int Length(){ return vertices.Length; }
			public Vector2 Vertex(int i){ return new Vector2(vertices[i].X,vertices[i].Y); }
		}
		private class CircleWrap : ShapeWrapper, Resource.ICircle
		{
			public PointF center;
			
			internal CircleWrap(float radius)
			{
				bigRadius = radius;
				size = radius * 2.0f;
				
				image = (Image)new Bitmap((int)size, (int)size);
				graphics = Graphics.FromImage(image);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}
			
			public override void RenderLocal(Brush brush, float angle)
			{
				graphics.Clear(Color.FromArgb(0,0,0,0));
				graphics.FillEllipse(brush,0.0f,0.0f,size,size);
			}
			
			public float Radius(){ return bigRadius; }
		}
		
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
			
			void SetA(byte v){ brush.Color = Color.FromArgb(v,R(),G(),B()); }
			void SetR(byte v){ brush.Color = Color.FromArgb(A(),v,G(),B()); }
			void SetG(byte v){ brush.Color = Color.FromArgb(A(),R(),v,B()); }
			void SetB(byte v){ brush.Color = Color.FromArgb(A(),R(),G(),v); }
			
			void SetARGB(byte a, byte r, byte g, byte b){ brush.Color = Color.FromArgb(a,r,g,b); }
		}
	}
}