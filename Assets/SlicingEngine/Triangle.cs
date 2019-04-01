using System;

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
			Triangle[] triangles = new Triangle[m.triangles.Count / 3];
			for (int i = 0; i < triangles.Length; i++){
				int a = m.triangles[(i * 3)];
				int b = m.triangles[(i * 3) + 1];
				int c = m.triangles[(i * 3) + 2];
				triangles[i] = new Triangle(a, b, c);
			}
			return triangles;
		}
		public Edge[] GetEdges(){
			return new[]{EdgeAB,EdgeBC,EdgeAC};
		}
		public bool Equals(Triangle other){
			return A == other.A && B == other.B && C == other.C;
		}
	}
}