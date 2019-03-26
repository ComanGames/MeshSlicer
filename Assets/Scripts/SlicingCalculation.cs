using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts{
	public class SlicingCalculation{
		private Mesh mesh;

		public SlicingCalculation(Mesh mesh){
			this.mesh = mesh;
		}

		public Mesh[] Slice(Vector3 pos, Quaternion rot, bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy();

			Triangle[] triangle = GetTriangles();


			//Sorting vertices
			Vector3[] v = mesh.vertices;
			int[] t = mesh.triangles;
			Vector3[] n = mesh.normals;
			int[] sorted = SortVertices(v, pos, rot);


			List<Vector3> vAbove;
			List<Vector3> vBelow;
			GetVerticesBelovAndAbove(sorted, v, out vAbove, out vBelow);

			List<int> tAbove;
			List<int> tBelow;
			GetTrianglesBelowAhdAbove(sorted, t, out tAbove, out tBelow);

			List<Vector3> nAbove;
			List<Vector3> nBelow;
			GetNormalsBelowAhdAbove(sorted, n, out nAbove, out nBelow);

			//Getting t in between
			Triangle[] inBetween;
			inBetween = GetIntersectedTriangels(triangle, sorted);




			//Mesh Above
			Mesh mAbove = new Mesh();
			mAbove.vertices = vBelow.ToArray();
			mAbove.triangles = tBelow.ToArray();
			mAbove.normals = nBelow.ToArray();
			for (int i = 0; i < vAbove.Count; i++){
				
			}

			return new[]{mAbove};
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

		private void GetNormalsBelowAhdAbove(int[] sorted, Vector3[] n, out List<Vector3> nAbove, out List<Vector3> nBelow){
			nAbove = new List<Vector3>();
			nBelow = new List<Vector3>();
			for (int i = 0; i < sorted.Length; i++)
			{
				if (sorted[i] >= 0)
					nAbove.Add(n[sorted[i]]);
				else
					nBelow.Add(n[(sorted[i] + 1) * -1]);
			}
		}

		private void GetTrianglesBelowAhdAbove(int[] sorted, int[] t, out List<int> tAbelow, out List<int> tBelow){
			tAbelow = new List<int>();
			tBelow = new List<int>();
			for (int i = 0; i < t.Length/3; i++){
				int a = sorted[t[(i * 3)]];
				int b = sorted[t[(i * 3) + 1]];
				int c = sorted[t[(i * 3) + 2]];
				if(a>=0&&b>=0&&c>=0){
					tAbelow.AddRange(new int[]{a,b,c});
				}
				else if(a<0&&b<0&&c<0)
					tBelow.AddRange(new int[]{-a-1,-b-1,-c-1});
			}
		}


		private static void GetVerticesBelovAndAbove(int[] sorted, Vector3[] v, out List<Vector3> vAbove, out List<Vector3> vBelow){
			vAbove = new List<Vector3>();
			vBelow = new List<Vector3>();
			for (int i = 0; i < sorted.Length; i++){
				if (sorted[i] >= 0)
					vAbove.Add(v[sorted[i]]);
				else
					vBelow.Add(v[(sorted[i] + 1) * -1]);
			}
		}

		private static Triangle[] GetIntersectedTriangels(Triangle[] triangle, int[] sorted){
			List<Triangle> tIntersection;
			tIntersection = new List<Triangle>();
			for (int i = 0; i < triangle.Length; i++){
				byte above = 0;
				byte below = 0;

				foreach (int k in triangle[i].GerVertices()){
					if (sorted[k] >= 0)
						above++;
					else
						below++;
					if (above > 0 && below > 0)
						tIntersection.Add(triangle[i]);
				}
			}

			return tIntersection.ToArray();
		}


		private int[] SortVertices(Vector3[] v, Vector3 pos, Quaternion rot){
			int[] result = new int[v.Length];
			int up = 0;
			int down = -1;
			for (int i = 0; i < v.Length; i++){
				if (IsVertAbove(v[i], pos, rot)){
					result[i] = up;
					up++;
				}
				else{
					result[i] = down;
					down--;
				}
			}

			return result;
		}

		private Vector3[] GetTriangleCutVertices(Vector3 point, Quaternion rot, Triangle t){
			List<Vector3> dots = new List<Vector3>();
			for (int i = 0; i < t.Edges.Length; i++){
				Vector3 intersectionPoint;
				if (IsEdgeIntersected(t.Edges[i], point, rot, out intersectionPoint))
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

		private bool IsEdgeIntersected(Edge edge, Vector3 pos, Quaternion rot, out Vector3 intersectionPoint){
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