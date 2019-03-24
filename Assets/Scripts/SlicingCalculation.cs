using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts{
	public class SlicingCalculation{
		public Mesh[] Slice(Mesh mesh, Vector3 point, Quaternion rot,bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy(mesh);
			Triangle[] triangle = GetTriangles(mesh);

			List<Vector3> vAbove = new List<Vector3>(); 
			List<Vector3> vBelow = new List<Vector3>();

			for (int i = 0; i < mesh.vertices.Length; i++){
				Vector3 vert = mesh.vertices[i];
				if(IsAbove(vert,point,rot))
					vAbove.Add(vert);
				else
					vBelow.Add(vert);

			}

			 throw new NotImplementedException();
		}

		private static Triangle[] GetTriangles(Mesh mesh){
			Triangle[] triangle = new Triangle[mesh.triangles.Length / 3];
			for (int i = 0; i < triangle.Length; i++){
				int a = mesh.triangles[(i * 3)];
				int b = mesh.triangles[(i * 3) + 1];
				int c = mesh.triangles[(i * 3) + 2];
				triangle[i] = new Triangle(a, b, c);
			}

			return triangle;
		}

		private bool IsAbove(Vector3 vert, Vector3 point, Quaternion rot){

			 Vector3 p = rot * (vert - point) + point;
			return p.y > 0;
		}

		private bool[,] GenerateAdjancy(Mesh mesh){
			int m = mesh.vertices.Length;
			bool[,] P = new bool[m,m];
			for (int i = 0; i < mesh.triangles.Length; i+=3){

				int a = mesh.triangles[i];
				int b = mesh.triangles[i + 1];
				int c = mesh.triangles[i + 2];

				P[a,b]=P[b,a]=
					P[b,c]=P[c,b]=
						P[a,c]=P[c,a]= true;
			}
			return P;
		}
	}
}