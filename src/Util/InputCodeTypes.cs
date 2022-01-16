using System;

namespace Key
{
	public enum Code
	{
		Right,Left,Up,Down,Space,
		A,B,C,D,E,F,G,H,I,J,K,L,N,M,O,P,Q,R,S,T,U,V,W,X,Y,Z,
		Num1,Num2,Num3,Num4,Num5,Num6,Num7,Num8,Num9,Num0,NumMinus,NumPlus,
		Pad1,Pad2,Pad3,Pad4,Pad5,Pad6,Pad7,Pad8,Pad9,Pad0,PadMinus,PadPlus,
		LeftCtrl,LeftAlt,LeftShift,RightCtrl,RightAlt,RightShift,
		Tab
	};
	
	public static class Info
	{
		public static int GetCodeCount(){ return System.Enum.GetValues(typeof(Key.Code)).Length; }
	}
}

namespace Mouse
{
	public enum Code
	{
		Left,Middle,Right
	}
	
	public static class Info
	{
		public static int GetCodeCount(){ return System.Enum.GetValues(typeof(Mouse.Code)).Length; }
	}
}