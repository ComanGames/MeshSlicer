using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Slicer{
	public class MeshInfo{
		public List<Vector3> vertices;
		public List<int>[] triangles;
		public List<Vector3> normals;
		public List<Vector2> uv;
		public string name;

		public MeshInfo(List<Vector3> vertices, List<int>[] triangles, List<Vector3> normals, List<Vector2> uv, string name){
			this.vertices = vertices;
			this.triangles = triangles;
			this.normals = normals;
			this.uv = uv;
			this.name = name;
		}

		public MeshInfo(Mesh mesh){

			vertices = mesh.vertices.ToList();
			normals = mesh.normals.ToList();
			uv = mesh.uv.ToList();
			name = mesh.name;

			triangles = new List<int>[mesh.subMeshCount];
			for (int i = 0; i < triangles.Length; i++)
				triangles[i] = mesh.GetTriangles(i).ToList();
		}

		public Mesh GetMesh(){

			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			
			if(normals.Count>0)
				mesh.normals = normals.ToArray();
			if(uv.Count==mesh.vertices.Length)
				mesh.uv = uv.ToArray();

			mesh.name = name;

			mesh.subMeshCount = triangles.Length;
			for (int i = 0; i < triangles.Length; i++){
				mesh.SetTriangles(triangles[i].ToArray(),i);
			}

			return mesh;
		}


	}
}