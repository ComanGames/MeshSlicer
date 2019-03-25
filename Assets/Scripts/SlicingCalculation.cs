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

		public Vector3[] GetCutVertices(Vector3 point, Quaternion rot, bool[,] adjacency = null){

			if (adjacency == null)
				adjacency = GenerateAdjancy();

			Triangle[] triangle = GetTriangles();

			List<Triangle> tIntersection = new List<Triangle>();

			for (int i = 0; i < triangle.Length; i++)
			{
				Triangle t = triangle[i];
				if (!IsTriangleAbove(t, point, rot)&&!IsTriangleBelow(t,point,rot))
					tIntersection.Add(t);
			}
			Debug.Log(tIntersection.Count);

			List<Vector3> edge = new List<Vector3>();
			
			LinkedList<Triangle> tLinked = GetLinkedList(tIntersection);
			Debug.Log("Linked list count"+ tLinked.Count);


			//Get First Vetices
			Vector3 dot ;
//				= GetDotFromEdge(tLinked, tLinked.First.Value, tLinked.Last.Value);
//			edge.Add(dot);

			LinkedListNode<Triangle> current = tLinked.First;
			for (int i = 0; i < tLinked.Count-1; i++){
				dot = GetDotFromEdge(tLinked, current.Value, current.Next.Value);
				current = current.Next;
				edge.Add(dot);
			}

			return edge.ToArray();
		}

		private Vector3 GetDotFromEdge(LinkedList<Triangle> tLinked, Triangle triangleA, Triangle triangleB){
			int normalA;
			int normalB;
			GetSameEdge(triangleA,triangleB, out normalA, out normalB);
			Vector3 vertA = mesh.vertices[normalA];
			Vector3 vertB = mesh.vertices[normalB];

			Vector3 dot = UnityEngine.Vector3.Lerp(vertA, vertB, 0.5f);
			return dot;
		}

		public  LinkedList<Triangle> GetLinkedList(List<Triangle> input){


			LinkedList<Triangle> outlineLinked = new LinkedList<Triangle>();
			outlineLinked.AddFirst(input[0]);
			//We just add one more Item
			Debug.Log(input.Count);
			LinkedListNode<Triangle> current = null;

			for (int i = 1; i < input.Count; i++){
				if (HaveSameEdge(input[i],input[0]) && !input[i].Equals(input[0])){
					current = outlineLinked.AddAfter(outlineLinked.First, input[i]);
					break;
				}
			}

			for (int j = 0; j < input.Count; j++){
				
				
			for (int i = 0; i < input.Count; i++){
				if (!input[i].Equals(current.Value)
				    && !input[i].Equals(current.Previous.Value) 
				    && HaveSameEdge(input[i],current.Value))
				{
					current= outlineLinked.AddAfter(current, input[i]);
				}

			}
			}


			return outlineLinked;
		}

		private bool HaveSameEdge(Triangle triangleA, Triangle triangleB){
			int normalA = 0;
			int normalB = 0;
			GetSameEdge(triangleA, triangleB, out normalA, out normalB);
			return normalA != -1 && normalB != -1;
		}


		private void GetSameEdge(Triangle triangleA, Triangle triangleB, out int normalA, out int normalB){
			normalA = -1;
			normalB = -1;

			Vector3 aa = mesh.vertices[triangleA.A];
			Vector3 ab = mesh.vertices[triangleA.B];
			Vector3 ac = mesh.vertices[triangleA.C];

			Vector3 ba = mesh.vertices[triangleB.A];
			Vector3 bb = mesh.vertices[triangleB.B];
			Vector3 bc = mesh.vertices[triangleB.C];

			Vector3[] a = new[]{aa, ab, ac};
			Vector3[] b = new[]{ba, bb, bc};

			int count = 0;
			for (int i = 0; i < a.Length; i++){
				for (int j = 0; j < b.Length; j++){

					if (a[i] == b[j]){
						if(count==0)
							triangleA=a[i]
					}

					
				}
				
			}

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