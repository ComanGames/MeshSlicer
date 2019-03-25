using System;
using UnityEngine;

namespace Assets.Scripts{
	public struct Triangle:IEquatable<Triangle>{
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

		public int[] GerVertices(){
			return new[]{A, B, C};
		}

	
		public bool HaveSameEdge(Triangle other){
			int count = 0;
			int[] first = GerVertices();
			int[] second = GerVertices();
			for (int i = 0; i < first.Length; i++){
				for (int j = 0; j < second.Length; j++){

					if (first[i] == second[j])
						count++;
				}
			}

			return count==2;

		}


		public bool Equals(Triangle other){
			return A == other.A && B == other.B && C == other.C;
		}
	}
}