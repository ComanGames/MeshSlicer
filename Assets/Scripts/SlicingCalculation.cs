using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts{
	public class SlicingCalculation{
		private Mesh mesh;

		public SlicingCalculation(Mesh mesh){
			this.mesh = mesh;
		}

		public Mesh[] Slice( Vector3 point, Quaternion rot,bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy();

			Triangle[] triangle = GetTriangles();


			//Sorting vertices
			List<Vector3> vAbove = new List<Vector3>(); 
			List<Vector3> vBelow = new List<Vector3>();

			for (int i = 0; i < mesh.vertices.Length; i++){
				Vector3 v = mesh.vertices[i];
				if(IsVertAbove(v,point,rot))
					vAbove.Add(v);
				else
					vBelow.Add(v);
			}


			List<Triangle> tAbove = new List<Triangle>();
			List<Triangle> tBelow = new List<Triangle>();
			List<Triangle> tIntersection = new List<Triangle>();

			for (int i = 0; i < triangle.Length; i++){
				Triangle t =triangle[i];
				if(IsTriangleAbove(t,point,rot))
					tAbove.Add(t);
				else if(IsTriangleBelow(t,point,rot))
					tBelow.Add(t);
				else
					tIntersection.Add(t);
			}

			Mesh result = new Mesh();

			 throw new NotImplementedException();
		}

		public Vector3[] GetMashCutVertices(Vector3 point, Quaternion rot, bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy();

			Triangle[] triangle = GetTriangles();

			var tIntersection = IntersectedTriangles(point, rot, triangle);

			List<Vector3> dots = new List<Vector3>();

			Vector3[] v = mesh.vertices;
			foreach (Triangle t in tIntersection){
				dots.AddRange(GetTriangleCutVertices(point, rot, t));
			}

			return dots.ToArray();
		}

		private Vector3[] GetTriangleCutVertices(Vector3 point, Quaternion rot, Triangle t){
			List<Vector3> dots = new List<Vector3>();
			for (int i = 0; i < t.Edges.Length; i++){
				Vector3 intersectionPoint;
				if (IsEdgeIntsected(t.Edges[i], point, rot, out intersectionPoint))
					dots.Add(intersectionPoint);
			}

			return dots.ToArray();
		}

		private List<Triangle> IntersectedTriangles(Vector3 point, Quaternion rot, Triangle[] triangle){
			List<Triangle> tIntersection = new List<Triangle>();

			for (int i = 0; i < triangle.Length; i++){
				Triangle t = triangle[i];
				if (!IsTriangleAbove(t, point, rot) && !IsTriangleBelow(t, point, rot))
					tIntersection.Add(t);
			}

			return tIntersection;
		}

		private bool IsEdgeIntsected(Edge edge, Vector3 pos, Quaternion rot, out Vector3 intersectionPoint){
			Vector3 A = rot * (mesh.vertices[edge.VerticeA] - pos) + pos; 
			Vector3 B = rot * (mesh.vertices[edge.VerticeB] - pos) + pos;

			bool AB = A.y >= 0 && B.y <= 0;
			bool BA = B.y >= 0 && A.y <= 0;
			intersectionPoint = default;
			if (AB||BA){
				float newXStep = ((B.x - A.x) / (B.y - A.y));
				float newX =((-1*A.y)*newXStep) +A.x;

				float newZStep = ((B.z - A.z) / (B.y - A.y));
				float newZ =((-1*A.y)*newZStep) +A.z;
				Vector3 p =new Vector3(newX,0,newZ);

				intersectionPoint = p;
				intersectionPoint = Quaternion.Inverse(rot)*(new Vector3(newX,0,newZ)-pos) + pos;

				return true;


			}

			return false;
		}


		private bool HaveSameEdge(Triangle triangleA, Triangle triangleB){
			return GetSameEdge(triangleA,triangleB) != null;
		}


		private Edge GetSameEdge(Triangle triangleA, Triangle triangleB){
			Edge[] edgesA = triangleA.Edges;
			Edge[] edgesB = triangleB.Edges;

			for (int i = 0; i < edgesA.Length; i++){
				for (int j = 0; j < edgesB.Length; j++){
					if (edgesA[i].Equals(edgesB[j])){
						edgesB[j] = edgesA[i];
						return edgesA[i];
					}
				}
				
			}
			return null;
		}

		private bool IsTriangleAbove(Triangle triangle, Vector3 pos, Quaternion rot){
			Vector3 A = mesh.vertices[triangle.A];
			Vector3 B = mesh.vertices[triangle.B];
			Vector3 C = mesh.vertices[triangle.C];

			return IsVertAbove(A, pos, rot)
			       && IsVertAbove(B, pos, rot)
			       && IsVertAbove(C, pos, rot);
		}
		private bool IsTriangleBelow(Triangle triangle, Vector3 pos, Quaternion rot){

			Vector3 A = mesh.vertices[triangle.A];
			Vector3 B = mesh.vertices[triangle.B];
			Vector3 C = mesh.vertices[triangle.C];

			return !IsVertAbove(A, pos, rot)
			       && !IsVertAbove(B, pos, rot)
			       && !IsVertAbove(C, pos, rot);
		}

		private Triangle[] GetTriangles(){
			Triangle[] triangle = new Triangle[mesh.triangles.Length / 3];
			for (int i = 0; i < triangle.Length; i++){
				int a = mesh.triangles[(i * 3)];
				int b = mesh.triangles[(i * 3) + 1];
				int c = mesh.triangles[(i * 3) + 2];
				triangle[i] = new Triangle(a, b, c);
			}

			return triangle;
		}

		private bool IsVertAbove(Vector3 point, Vector3 pos, Quaternion rot){

			 Vector3 p = rot * (point - pos) + pos;
			return p.y > 0;
		}

		private bool[,] GenerateAdjancy(){
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