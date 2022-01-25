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
			
			loop_order.Add(this.BeforeRender,50);	// 71 ~ 89 = Order For Render
			loop_order.Add(this.AfterRender,80);
		}
		public override void OnBegin()
		{
			
		}
		public override void OnEnd()
		{
			
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
		public override Resource.ICircle CreateCircle(float radius)
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
			ColorWrap c = color as ColorWrap;
			if(c == null)
				return;
			
			pw.Scaling(scale);
			pw.Translate(pos);
			drawGraphics.FillPolygon(c.brush, pw.output);
		}
		public override void RenderColorPolygon(Resource.IPolygon polygon, Resource.IColor color, Vector2 pos, Vector2 scale, float angle)
		{
			PolygonWrap pw = polygon as PolygonWrap;
			if(pw == null)
				return;
			ColorWrap c = color as ColorWrap;
			if(c == null)
				return;
			
			pw.Scaling(scale);
			pw.Rotate(angle);
			pw.Translate(pos);
			drawGraphics.FillPolygon(c.brush, pw.output);
		}
		public override void RenderColorCircle(Resource.ICircle circle, Resource.IColor color, Vector2 pos, Vector2 scale)
		{
			CircleWrap cw = circle as CircleWrap;
			if(cw == null)
				return;
			ColorWrap c = color as ColorWrap;
			if(c == null)
				return;
			
			cw.Scaling(scale);
			cw.Translate(pos);
			drawGraphics.FillEllipse(c.brush, cw.x, cw.y, cw.width, cw.height);
		}
		
		
		
		//	Wraps =====================================================================
		// IPolygon ====================================
		private class PolygonWrap : Resource.IPolygon
		{
			public Vector2[] vertices;
			public PointF[] output;
			
			internal PolygonWrap(Vector2[] args)
			{
				vertices = (Vector2[])args.Clone();
				output = new PointF[vertices.Length];
				for(int i=0; i<output.Length; i++)
					output[i] = new PointF(0.0f,0.0f);
			}
			
			public void Scaling(Vector2 scale)
			{
				for(int i=0; i<output.Length; i++)
				{
					output[i].X = vertices[i].x * scale.x;
					output[i].Y = vertices[i].y * scale.y;
				}
			}
			
			public void Rotate(float angle)
			{
				float rad = angle * UMath.D2R;
				for(int i=0; i<output.Length; i++)
				{
					PointF temp = output[i];
					output[i].X = temp.X * (float)Math.Cos(rad) - temp.Y * (float)Math.Sin(rad);
					output[i].Y = temp.X * (float)Math.Sin(rad) + temp.Y * (float)Math.Cos(rad);
				}
			}
			
			public void Translate(Vector2 position)
			{
				for(int i=0; i<output.Length; i++)
				{
					output[i].X += position.x;
					output[i].Y += position.y;
				}
			}
			
			public int VertexCount(){ return vertices.Length; }
			public Vector2 Vertex(int i){ return vertices[i]; }
			public Resource.IPolygon Copy(){ return new PolygonWrap(this.vertices); }
			public void Dispose(){ vertices = null; output = null; }
		}
		
		//	ICircle =====================================
		private class CircleWrap : Resource.ICircle
		{
			public float radius;
			
			public float x, y;
			public float width, height;
			
			internal CircleWrap(float r)
			{
				this.radius = r;
				
				width = radius;
				height = radius;
				x = 0.0f;
				y = 0.0f;
				
			}
			
			public void Scaling(Vector2 scale)
			{
				width = radius * scale.x;
				height = radius * scale.y;
			}
			
			public void Translate(Vector2 position)
			{
				x = position.x;
				y = position.y;
			}
			
			public float Radius(){ return radius; }
			public Resource.ICircle Copy(){ return new CircleWrap(this.radius); }
			public void Dispose(){ }
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
			public Resource.IColor Copy(){ return new ColorWrap(brush.Color.A,brush.Color.R,brush.Color.G,brush.Color.B); }
			public void Dispose(){ brush.Dispose(); }
		}
	}
}