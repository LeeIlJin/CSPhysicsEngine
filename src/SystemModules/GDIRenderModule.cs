using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Module
{
	public sealed class GDIRender : Module.RenderBase
	{
		//	Field ===============================================================================
		private Graphics windowGraphics;
		private Graphics drawGraphics;
		private Image drawBackBuffer;
		private Color drawBackColor;
		
		private List<ColorPolygonWrap> polygons;
		private List<ColorCircleWrap> circles;
		
		private void BeforeRender()
		{
			drawGraphics.Clear(drawBackColor);
		}
		
		private void ListRender()
		{
			for(int i=0; i<polygons.Count; i++)
			{
				if(!polygons[i].visible)
					continue;
				polygons[i].CalcOutput();
				drawGraphics.FillPolygon(polygons[i].brush, polygons[i].output);
			}
			
			for(int i=0; i<circles.Count; i++)
			{
				if(!circles[i].visible)
					continue;
				circles[i].CalcOutput();
				drawGraphics.FillEllipse(circles[i].brush, circles[i].position.x, circles[i].position.y, circles[i].scale.x, circles[i].scale.y);
			}
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
			
			polygons = new List<ColorPolygonWrap>();
			circles = new List<ColorCircleWrap>();
			
			loop_order.Add(this.BeforeRender,50);	// 51 ~ 79 = Order For Render
			loop_order.Add(this.ListRender,79);
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
			for(int i=0; i<polygons.Count; i++)
			{
				polygons[i].Dispose();
			}
			for(int i=0; i<circles.Count; i++)
			{
				circles[i].Dispose();
			}
			
			polygons.Clear();
			circles.Clear();
			
			windowGraphics.Dispose();
			drawGraphics.Dispose();
			drawBackBuffer.Dispose();
		}
		
		//	Render's ============================================================================
		
		//	Create Resource ===========================================================
		public override Resource.IColorPolygon CreateColorPolygon(byte a, byte r, byte g, byte b, params Vector2[] args)
		{
			ColorPolygonWrap newone = new ColorPolygonWrap(args,a,r,g,b);
			newone.owner = this;
			polygons.Add(newone);
			return newone;
		}
		public override Resource.IColorCircle CreateColorCircle(byte a, byte r, byte g, byte b, float radius)
		{
			ColorCircleWrap newone = new ColorCircleWrap(radius,a,r,g,b);
			newone.owner = this;
			circles.Add(newone);
			return newone;
		}
		
		
		//	Wraps =====================================================================
		// IColorPolygon ====================================
		private class ColorPolygonWrap : Resource.IColorPolygon
		{
			public GDIRender owner;
			
			public bool visible;
			
			public bool position_changed;
			public Vector2 position;
			public bool scale_changed;
			public Vector2 scale;
			public bool angle_changed;
			public float angle;
			
			public Vector2[] vertices;
			public PointF[] output;
			
			public SolidBrush brush;
			
			internal ColorPolygonWrap(Vector2[] args, byte a, byte r, byte g, byte b)
			{
				visible = true;
				
				position_changed = false;
				scale_changed = false;
				angle_changed = false;
				
				position = Vector2.Zero;
				scale = Vector2.One;
				angle = 0.0f;
				
				vertices = (Vector2[])args.Clone();
				output = new PointF[vertices.Length];
				
				brush = new SolidBrush(Color.FromArgb(a,r,g,b));
			}
			
			public void CalcOutput()
			{	
				if(scale_changed)
				{
					for(int i=0; i<output.Length; i++)
					{
						output[i].X = vertices[i].x * scale.x;
						output[i].Y = -vertices[i].y * scale.y;
					}
					scale_changed = false;
				}
				else
				{
					for(int i=0; i<output.Length; i++)
					{
						output[i].X = vertices[i].x;
						output[i].Y = -vertices[i].y;
					}
				}
				
				if(angle_changed)
				{
					float rad = angle * UMath.D2R;
					for(int i=0; i<output.Length; i++)
					{
						PointF temp = output[i];
						output[i].X = temp.X * (float)Math.Cos(rad) - temp.Y * (float)Math.Sin(rad);
						output[i].Y = temp.X * (float)Math.Sin(rad) + temp.Y * (float)Math.Cos(rad);
					}
					angle_changed = false;
				}
				
				if(position_changed)
				{
					for(int i=0; i<output.Length; i++)
					{
						output[i].X += position.x;
						output[i].Y += position.y;
					}
					position_changed = false;
				}
			}
			
			public void Visible(bool b){ visible = b; }
			public bool IsVisible(){ return visible; }
			
			public void SetPositionThisFrame(Vector2 _position){ position_changed = true; position = _position; }
			public void SetScaleThisFrame(Vector2 _scale){ scale_changed = true; scale = _scale; }
			public void SetAngleThisFrame(float _angle){ angle_changed = true; angle = _angle; }
			
			public byte A(){ return brush.Color.A; }
			public byte R(){ return brush.Color.R; }
			public byte G(){ return brush.Color.G; }
			public byte B(){ return brush.Color.B; }
			public void SetA(byte v){ brush.Color = Color.FromArgb(v,R(),G(),B()); }
			public void SetR(byte v){ brush.Color = Color.FromArgb(A(),v,G(),B()); }
			public void SetG(byte v){ brush.Color = Color.FromArgb(A(),R(),v,B()); }
			public void SetB(byte v){ brush.Color = Color.FromArgb(A(),R(),G(),v); }
			public void SetARGB(byte a, byte r, byte g, byte b){ brush.Color = Color.FromArgb(a,r,g,b); }
			
			public int VertexCount(){ return vertices.Length; }
			public Vector2 Vertex(int i){ return vertices[i]; }
			
			public Resource.IColorShape Copy(){ return owner.CreateColorPolygon(brush.Color.A,brush.Color.R,brush.Color.G,brush.Color.B,this.vertices); }
			public void Dispose(){ vertices = null; output = null; brush.Dispose(); }
		}
		
		//	ICircle =====================================
		private class ColorCircleWrap : Resource.IColorCircle
		{
			public GDIRender owner;
			
			public bool visible;
			
			public bool position_changed;
			public Vector2 position;
			public bool scale_changed;
			public Vector2 scale;
			
			public float radius;
			
			public SolidBrush brush;
			
			internal ColorCircleWrap(float _radius, byte a, byte r, byte g, byte b)
			{
				visible = true;
				
				position_changed = false;
				scale_changed = false;
				
				position = Vector2.Zero;
				scale = Vector2.One;
				
				this.radius = _radius;
				brush = new SolidBrush(Color.FromArgb(a,r,g,b));
			}
			
			public void CalcOutput()
			{
				if(position_changed)
				{
					position_changed = false;
				}
				else
				{
					position = Vector2.Zero;
				}
				
				if(scale_changed)
				{
					scale = scale * radius;
					scale_changed = false;
				}
				else
				{
					scale = new Vector2(radius, radius);
				}
			}
			
			public void Visible(bool b){ visible = b; }
			public bool IsVisible(){ return visible; }
			
			public void SetPositionThisFrame(Vector2 _position){ position_changed = true; position = _position; }
			public void SetScaleThisFrame(Vector2 _scale){ scale_changed = true; scale = _scale; }
			public void SetAngleThisFrame(float _angle){ return; }
			
			public byte A(){ return brush.Color.A; }
			public byte R(){ return brush.Color.R; }
			public byte G(){ return brush.Color.G; }
			public byte B(){ return brush.Color.B; }
			public void SetA(byte v){ brush.Color = Color.FromArgb(v,R(),G(),B()); }
			public void SetR(byte v){ brush.Color = Color.FromArgb(A(),v,G(),B()); }
			public void SetG(byte v){ brush.Color = Color.FromArgb(A(),R(),v,B()); }
			public void SetB(byte v){ brush.Color = Color.FromArgb(A(),R(),G(),v); }
			public void SetARGB(byte a, byte r, byte g, byte b){ brush.Color = Color.FromArgb(a,r,g,b); }
			
			public float Radius(){ return radius; }
			public Resource.IColorShape Copy(){ return owner.CreateColorCircle(brush.Color.A,brush.Color.R,brush.Color.G,brush.Color.B,this.radius); }
			public void Dispose(){ brush.Dispose(); }
		}
	}
}