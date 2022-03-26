using System;
using System.Windows.Forms;
using System.Drawing;

namespace System
{
	public sealed class Window
	{
		public struct Desc
		{
			public Point Location;
			public Size Size;
			public string Text;
			public Color BackColor;
			public System.Windows.Forms.FormBorderStyle FormBorderStyle;
			
			public static Desc Default = new Desc
			{
				Location = new Point(10,10),
				Size = new Size(1280,720),
				Text = "WindowFormDefault",
				BackColor = Color.White,
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
			};
		}
		
		//	Field ===============================================================================
		public readonly Form Form;
		
		public Window(Desc desc)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Form = new FormWrap(desc);
		}
		
		public void Initialize(LoopOrder loop_order)
		{
			loop_order.Add(Application.DoEvents,20);
		}
		
		//	Window's ============================================================================
		public int Width(){ return Form.ClientSize.Width; }
		public int Height(){ return Form.ClientSize.Height; }
		
		public bool IsCreated(){ return Form.Created; }
		
		public void Show()
		{
			Form.Show();
		}
		public void Exit()
		{
			Application.Exit();
		}
		
		//	Wrapping! ===========================================================================
		private class FormWrap : Form
		{
			public FormWrap(Desc desc)
			{
				this.Location = desc.Location;
				this.ClientSize = desc.Size;
				this.Text = desc.Text;
				this.BackColor = desc.BackColor;
				this.FormBorderStyle = desc.FormBorderStyle;
				
				this.MaximizeBox = false;
				this.DoubleBuffered = true;
			}
		}
	}
}