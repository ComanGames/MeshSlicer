using UnityEngine;

namespace Assets.Scripts.Slicer{
	public class MeshInfo
	{
		public Vector3[] vertices;
		public int[] triangles;
		public Vector3[] normals;
		public Vector2[] uv;
		public string name;

		public MeshInfo(){ }

		public MeshInfo(Mesh mesh){
			vertices = mesh.vertices;
			triangles = mesh.triangles;
			normals = mesh.normals;
			uv = mesh.uv;
			name = mesh.name;
		}

		public Mesh GetMesh(){
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.normals = normals;
			mesh.uv = uv;
			mesh.name = name;
			return mesh;

		}


	}
}