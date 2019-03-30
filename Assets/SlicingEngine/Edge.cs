using System;
using System.Collections.Generic;

namespace Assets.Scripts.Slicer{
	public class Edge:IEquatable<Edge>{
		private List<Triangle> Triangles;
		public int VerticeA ;
		public int VerticeB ;
		public Edge(int verticeA, int verticeB,Triangle t=null){
			Triangles= new List<Triangle>();
			VerticeA = verticeA;
			VerticeB = verticeB;
			if(t!=null)
				Triangles.Add(t);
		}

		public void AddTriangle(Triangle t){
			Triangles.Add(t);
		}

		public Triangle[] GeTriangles(){
			return Triangles.ToArray();
		}


		public bool Equals(Edge other){
			return 
				(VerticeA==other.VerticeA&&VerticeB==other.VerticeB)||
				(VerticeA==other.VerticeB&&VerticeB==other.VerticeA);
		}
	}
}