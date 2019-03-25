using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts{
	public struct Triangle:IEquatable<Triangle>{
		public int A;
		public int B;
		public int C;


		public Triangle(int a, int b, int c){
			this.A = a;
			this.B = b;
			this.C = c;
		}

		public int[] GerVertices(){
			return new[]{A, B, C};
		}


		public bool Equals(Triangle other){
			return A == other.A && B == other.B && C == other.C;
		}
	}
}