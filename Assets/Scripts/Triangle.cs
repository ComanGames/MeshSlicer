using System;

namespace Assets.Scripts{
	public class Triangle:IEquatable<Triangle>{
		public int A;
		public int B;
		public int C;

		public Edge[] Edges; //AC

		public Triangle(int a, int b, int c){
			this.A = a;
			this.B = b;
			this.C = c;
			Edges = new Edge[3];

			Edges[0] = new Edge(A,B);
			Edges[1] = new Edge(B,C);
			Edges[2] = new Edge(A,C);
		}

		public int[] GetVertices(){
			return new[]{A, B, C};
		}
		public bool Equals(Triangle other){
			return A == other.A && B == other.B && C == other.C;
		}
	}
}