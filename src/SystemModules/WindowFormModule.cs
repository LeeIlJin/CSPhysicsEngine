using System;
using System.Windows.Forms;
using System.Drawing;

namespace Module
{
	public sealed class WindowForm : Module.WindowBase
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
		
		public WindowForm(Desc desc)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Form = new FormWrap(desc);
		}
		
		private void ChangeTitle()
		{
			Form.Text = string.Format("delta : {0}s ({1}ms) / {2}FPS / global : {3}s",Hull.Time.Delta(),Hull.Time.DeltaMs(),Hull.Time.Fps(),Hull.Time.Global());
		}
		
		//	Base's ==============================================================================
		public override void OnCreate(LoopOrder loop_order)
		{
			loop_order.Add(ChangeTitle,1);
			loop_order.Add(Application.DoEvents,20);
		}
		public override void OnBegin(){}
		public override void OnEnd(){}
		public override void OnDispose(){}
		
		//	Window's ============================================================================
		public override int Width(){ return Form.ClientSize.Width; }
		public override int Height(){ return Form.ClientSize.Height; }
		
		public override bool IsCreated(){ return Form.Created; }
		
		public override void Show()
		{
			Form.Show();
		}
		public override void Exit()
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