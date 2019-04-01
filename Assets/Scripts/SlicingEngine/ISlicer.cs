using UnityEngine;

namespace Assets.Scripts.Slicer{
	public interface ISlicer{
		 Vector3[] GetSliceVertices(Vector3 pos, Quaternion rot);
		MeshInfo[] Slice(Vector3 pos, Quaternion rot);

	}
}