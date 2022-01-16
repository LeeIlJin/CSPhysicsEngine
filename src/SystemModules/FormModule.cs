using System;
using System.Windows.Forms;
using System.Drawing;

namespace Module
{
	public sealed class FormModule : SystemModuleForWindow
	{
		public readonly Form Form;
		
		public FormModule(Desc desc)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Form = new FormWrap(desc);
		}
		
		private void ChangeTitle()
		{
			Form.Text = string.Format("delta : {0} ({1}) / {2} / global : {3}",Hull.Time.StrDelta(),Hull.Time.StrDeltaMs(),Hull.Time.StrFps(),Hull.Time.StrGlobal());
		}
		
		//	Module's
		public override void OnCreate(LoopOrder loop_order)
		{
			loop_order.Add(ChangeTitle,1);
			loop_order.Add(Application.DoEvents,5);
		}
		public override void OnBegin(){}
		public override void OnEnd(){}
		public override void OnDispose(){}
		
		//	Window's
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
		
		
		//	Resources
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