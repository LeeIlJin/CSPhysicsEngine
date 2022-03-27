using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Module
{
	public sealed class Draw
	{
		//	Field ===============================================================================
		private Graphics windowGraphics;
		private Graphics drawGraphics;
		private Image drawBackBuffer;
		private Color drawBackColor;
		
		public Graphics Graphics{ get{ return drawGraphics; } }
		
		private void BeforeRender()
		{
			drawGraphics.Clear(drawBackColor);
		}
		
		private void AfterRender()
		{
			windowGraphics.DrawImage(drawBackBuffer,Point.Empty);
		}
		
		public void Initialize(LoopOrder loop_order, Form form)
		{
			drawBackColor = form.BackColor;
			drawBackBuffer = (Image)new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
			windowGraphics = form.CreateGraphics();
			drawGraphics = Graphics.FromImage(drawBackBuffer);
			
			loop_order.Add(this.BeforeRender,50);	// 71 ~ 89 = Order For Render
			loop_order.Add(this.AfterRender,80);
		}
	
		public void Dispose()
		{
			windowGraphics.Dispose();
			drawGraphics.Dispose();
			drawBackBuffer.Dispose();
		}
	}
}
