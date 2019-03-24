using UnityEngine;

namespace Assets.Scripts{
	public struct Triangle{
		public int A;
		public int B;
		public int C;
		public Vector3[] Vertices;


		public Triangle(int a, int b, int c,Vector3[] vertices){
			this.A = a;
			this.B = b;
			this.C = c;
			Vertices = vertices;
		}

	}
}