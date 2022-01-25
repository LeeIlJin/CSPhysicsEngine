using System;

namespace Game
{
	public struct PolygonMesh : ECS.IComponentData
	{
		public Resource.IPolygon resource;
		
		public void DeepCopy(){ resource = resource.Copy(); }
		public void SetFriend(int index){}
	}
}