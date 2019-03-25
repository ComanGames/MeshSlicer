using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts{
	public class SlicingCalculation{
		public Mesh[] Slice(Mesh mesh, Vector3 point, Quaternion rot,bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy(mesh);

			Triangle[] triangle = GetTriangles(mesh);


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

		public Vector3[] GetCutVertices(Mesh mesh, Vector3 point, Quaternion rot, bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy(mesh);

			Triangle[] triangle = GetTriangles(mesh);

			List<Triangle> tIntersection = new List<Triangle>();

			for (int i = 0; i < triangle.Length; i++)
			{
				Triangle t = triangle[i];
				if (!IsTriangleAbove(t, point, rot)&&IsTriangleBelow(t,point,rot))
					tIntersection.Add(t);
			}

			List<Vector3> edge = new List<Vector3>();
			LinkedList<Triangle> intersectionLinkedList = NewMethod(tIntersection);


			return edge.ToArray();

		}

		private static LinkedList<Triangle> NewMethod(List<Triangle> input){
			LinkedList<Triangle> outlineLinked = new LinkedList<Triangle>();
			outlineLinked.AddFirst(input[0]);
			//We just add one more Item
			for (int i = 1; i < input.Count; i++){
				if (input[i].HaveSameEdge(input[0]) && !input[i].Equals(input[0])){
					outlineLinked.AddAfter(outlineLinked.First, input[i]);
					break;
				}
			}

			LinkedListNode<Triangle> current = outlineLinked.First.Next;

			for (int i = 0; i < input.Count; i++){
				if (!input[i].Equals(current.Value)
				    && !input.Equals(current.Previous.Value) 
				    && input[i].HaveSameEdge(current.Value))
				{
					current= outlineLinked.AddAfter(current, input[i]);
				}

			}

			return outlineLinked;
		}

		private bool IsTriangleAbove(Triangle triangle, Vector3 pos, Quaternion rot){
			Vector3 A = triangle.Vertices[triangle.A];
			Vector3 B = triangle.Vertices[triangle.A];
			Vector3 C = triangle.Vertices[triangle.A];

			return IsVertAbove(A, pos, rot)
			       && IsVertAbove(B, pos, rot)
			       && IsVertAbove(C, pos, rot);
		}
		private bool IsTriangleBelow(Triangle triangle, Vector3 pos, Quaternion rot){

			Vector3 A = triangle.Vertices[triangle.A];
			Vector3 B = triangle.Vertices[triangle.A];
			Vector3 C = triangle.Vertices[triangle.A];

			return !IsVertAbove(A, pos, rot)
			       && !IsVertAbove(B, pos, rot)
			       && !IsVertAbove(C, pos, rot);
		}

		private static Triangle[] GetTriangles(Mesh mesh){
			Triangle[] triangle = new Triangle[mesh.triangles.Length / 3];
			for (int i = 0; i < triangle.Length; i++){
				int a = mesh.triangles[(i * 3)];
				int b = mesh.triangles[(i * 3) + 1];
				int c = mesh.triangles[(i * 3) + 2];
				triangle[i] = new Triangle(a, b, c,mesh.vertices);
			}

			return triangle;
		}

		private bool IsVertAbove(Vector3 point, Vector3 pos, Quaternion rot){

			 Vector3 p = rot * (point - pos) + pos;
			return p.y > 0;
		}

		private bool[,] GenerateAdjancy(Mesh mesh){
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