using System;

namespace Resource
{
	public interface IWorld
	{
		int WorldID();
		
		void OnCreate();
		void OnBegin();
		void OnEnd();
		void OnDispose();
		
		void OnEnable();
		void OnDisable();
		
		void ChangeWorld(Module.WorldManager wm);
		void FrameBegin();
		void Physics();
		void Update();
		void Render();
		void UIRender();
		void FrameEnd();
	}
}