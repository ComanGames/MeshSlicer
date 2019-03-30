using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Slicer{
	public class MeshInfo
	{
		public List<Vector3> vertices;
		public List<int> triangles;
		public List<Vector3> normals;
		public List<Vector2> uv;
		public string name;

		public MeshInfo(){ }

		public MeshInfo(Mesh mesh){
			vertices = mesh.vertices.ToList();
			triangles = mesh.triangles.ToList();
			normals = mesh.normals.ToList();
			uv = mesh.uv.ToList();
			name = mesh.name;
		}

		public Mesh GetMesh(){
			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			if(normals.Count>0)
				mesh.normals = normals.ToArray();
			if(uv.Count==mesh.vertices.Length)
				mesh.uv = uv.ToArray();

			mesh.name = name;
			return mesh;

		}


	}
}