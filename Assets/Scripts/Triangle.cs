using System;

namespace Assets.Scripts{
	public class Triangle:IEquatable<Triangle>{
		public int A;
		public int B;
		public int C;

		public Edge EdgeAB;
		public Edge EdgeBC;
		public Edge EdgeAC;

		public Triangle(int a, int b, int c){
			this.A = a;
			this.B = b;
			this.C = c;

			EdgeAB = new Edge(A,B);
			EdgeBC = new Edge(B,C);
			EdgeAC = new Edge(A,C);
		}

		public int[] GetVertices(){
			return new[]{A, B, C};
		}
		public Edge[] GetEdges(){
			return new[]{EdgeAB,EdgeBC,EdgeAC};
		}
		public bool Equals(Triangle other){
			return A == other.A && B == other.B && C == other.C;
		}
	}
}