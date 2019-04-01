using System;
using System.Collections.Generic;

namespace Assets.Scripts.Slicer{
	public class Triangle:IEquatable<Triangle>{

		public int SubMashId;
		public int A;
		public int B;
		public int C;

		public Edge EdgeAB;
		public Edge EdgeBC;
		public Edge EdgeAC;

		public Triangle(int a, int b, int c,int mashId){
			this.A = a;
			this.B = b;
			this.C = c;

			EdgeAB = new Edge(A,B);
			EdgeBC = new Edge(B,C);
			EdgeAC = new Edge(A,C);
			SubMashId = mashId;
		}

		public int[] GetVertices(){
			return new[]{A, B, C};
		}
		public static Triangle[] GetTriangles(MeshInfo m){
		List<Triangle> triangles = new List<Triangle>();
			for (int j = 0; j < m.triangles.Length; j++){
					for (int i = 0; i < m.triangles[j].Count/3; i++){
						int a = m.triangles[j][(i * 3)];
						int b = m.triangles[j][(i * 3) + 1];
						int c = m.triangles[j][(i * 3) + 2];
						triangles.Add( new Triangle(a, b, c,j));
					}
				}
			return triangles.ToArray();
		}
		public Edge[] GetEdges(){
			return new[]{EdgeAB,EdgeBC,EdgeAC};
		}
		public bool Equals(Triangle other){
			return A == other.A && B == other.B && C == other.C;
		}
	}
}