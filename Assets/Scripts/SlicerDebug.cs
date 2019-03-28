using Assets.Scripts.Slicer;
using UnityEngine;

namespace Assets.Scripts{
	[RequireComponent(typeof(Mesh))]
	public class SlicerDebug : MonoBehaviour{

		public Mesh mesh;
		public Vector3 Angle;
		public Vector3 Pos;

		private void OnDrawGizmosSelected()
		{
		
			ISlicer slice = new ClosedSlicer(new MeshInfo(mesh));

			Vector3[] vertices = slice.GetSliceVertices(Pos,Quaternion.Euler(Angle));
			for (int i = 0; i < vertices.Length; i++){
				Gizmos.DrawSphere(vertices[i]+transform.position,0.01f);
			}


			Mesh testMesh = slice.Slice(Pos,Quaternion.Euler(Angle))[0].GetMesh();
			foreach (Vector3 v in testMesh.vertices){
				Gizmos.DrawCube(v,Vector3.one*0.01f);
			}

			GetComponent<MeshFilter>().mesh = testMesh;



		}

	}
}
