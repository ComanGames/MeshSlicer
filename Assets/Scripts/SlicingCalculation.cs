using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts{
	public class SlicingCalculation{
		private Mesh mesh;

		public SlicingCalculation(Mesh mesh){
			this.mesh = mesh;
		}

		public Mesh[] Slice(Vector3 pos, Quaternion rot){

			Triangle[] triangle = GetTriangles();


			//Sorting vertices
			Vector3[] v = mesh.vertices;
			int[] t = mesh.triangles;
			Vector3[] n = mesh.normals;
			Vector2[] u = mesh.uv;

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

			List<Vector2> uAbove;
			List<Vector2> uBelow;
			GetUvBelowAhdAbove(sorted, u, out uAbove, out uBelow);

			//Getting t in between
			Triangle[] inBetween;
			inBetween = GetIntersectedTriangels(triangle, sorted);




			foreach (Triangle tr in inBetween){
				bool tUp = IsTriangleUp(tr,sorted);

				Vector3[] newNormals;
				Vector2[] newUVs;
				Vector3[] newVertices = GetTriangleCutVertices(pos, rot, tr,out newNormals,out newUVs);

				vAbove.AddRange(newVertices);

				vBelow.AddRange(newVertices);
				int aboveCount = vAbove.Count;
				int belowCount = vBelow.Count;

				int[] newTU = GetNewTrianglesUp(tr,sorted);
				int[] newTD = GetReverse(newTU);
				if (tUp){
					tAbove.AddRange(GetUpTriangles(newTU, aboveCount));
					tBelow.AddRange(GetDownTriangles(newTD, belowCount));
				}
				else{
					tAbove.AddRange(GetDownTriangles(newTU, aboveCount));
					tBelow.AddRange(GetUpTriangles(newTD, belowCount));
				}

				nAbove.AddRange(newNormals);
				nBelow.AddRange(newNormals);

				uAbove.AddRange(newUVs);
				uBelow.AddRange(newUVs);
			}

			//Mesh Above
			Mesh mAbove = new Mesh();
			mAbove.vertices = vAbove.ToArray();
			mAbove.triangles = tAbove.ToArray();
			mAbove.normals = nAbove.ToArray();
			mAbove.uv = uAbove.ToArray();
			//Mesh Above
			Mesh mBelow = new Mesh();
			mBelow.vertices = vBelow.ToArray();
			mBelow.triangles = tBelow.ToArray();
			mBelow.normals = nBelow.ToArray();
			mBelow.uv = uBelow.ToArray();
			return new[]{mAbove,mBelow};
		}

		public Vector3[] GetMashCutVertices(Vector3 pos, Quaternion rot){

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


			List<Vector3> dots = new List<Vector3>();

			
			foreach (Triangle tr in inBetween){
				dots.AddRange(GetTriangleCutVertices(pos, rot, tr));
			}

			return dots.ToArray();
		}

		private void GetUvBelowAhdAbove(int[] sorted, Vector2[] u, out List<Vector2> uAbove, out List<Vector2> uBelow){
			uAbove = new List<Vector2>();
			uBelow = new List<Vector2>();
			for (int i = 0; i < sorted.Length; i++)
			{
				if (sorted[i] >= 0)
					uAbove.Add(u[i]);
				else
					uBelow.Add(u[i]);
			}
		}

		private int[] GetReverse(int[] newTu){
			int[] r = new int[newTu.Length];
			for (int i = 0; i < newTu.Length; i++){
				r[i] = (newTu[i] + 1) * -1;
			}

			return r;
		}

		private  int[] GetDownTriangles(int[] newT, int aboveCount){
			List<int> t = new List<int>();
			int indexOfDown = 0;
			for (int i = 0; i < newT.Length; i++){
				if (newT[i] < 0)
					indexOfDown = i;
			}

			switch (indexOfDown){
				case 0:
					t.Add(newT[1]); //B
					t.Add(newT[2]); //C
					t.Add(aboveCount - 2); //AB

					t.Add(aboveCount - 2); //AB
					t.Add(newT[2]);
					t.Add(aboveCount - 1); //AC
					break;
				case 1:
					t.Add(newT[2]); //C
					t.Add(newT[0]); //A
					t.Add(aboveCount - 2); //BC

					t.Add(aboveCount - 2); //BC
					t.Add(newT[0]); //A
					t.Add(aboveCount - 1); //BA
					break;
				case 2:
					t.Add(newT[0]); //A
					t.Add(newT[1]); //B
					t.Add(aboveCount - 2); //AC

					t.Add(aboveCount - 2); //AC
					t.Add(newT[1]); //B
					t.Add(aboveCount - 1); //BC
					break;
			}

			return t.ToArray();
		}

		private int[] GetUpTriangles(int[] newT, int aboveCount){
			int[] triangle = new int[3];
			foreach (int i in newT){
				if (i >= 0)
					triangle[0]=i;
			}

			triangle[1] = (aboveCount - 2);
			triangle[2] = (aboveCount - 1);
			return triangle;
		}

		private int[] GetNewTrianglesUp(Triangle tr,int[]sorted){
			List<int> result = new List<int>();
			foreach (int i in tr.GetVertices()){
					result.Add(sorted[i]);
			}

			return result.ToArray();
		}

		private bool IsTriangleUp(Triangle t, int[] s){
			int index = 0;
			foreach (int v in t.GetVertices()){
				if (s[v] == 0)
					index++;
				else
					index += s[v] / (Math.Abs(s[v]));
			}

			return index == -1;

		}

		private void GetNormalsBelowAhdAbove(int[] sorted, Vector3[] n, out List<Vector3> nAbove, out List<Vector3> nBelow){
			nAbove = new List<Vector3>();
			nBelow = new List<Vector3>();
			for (int i = 0; i < sorted.Length; i++)
			{
				if (sorted[i] >= 0)
					nAbove.Add(n[i]);
				else
					nBelow.Add(n[i]);
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
			for (int i = 0; i < v.Length; i++){
				if (sorted[i] >= 0)
					vAbove.Add(v[i]);
				else
					vBelow.Add(v[i]);
			}
		}

		private static Triangle[] GetIntersectedTriangels(Triangle[] triangle, int[] sorted){
			List<Triangle> tIntersection;
			tIntersection = new List<Triangle>();
			for (int i = 0; i < triangle.Length; i++){
				byte above = 0;
				byte below = 0;

				foreach (int k in triangle[i].GetVertices()){
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
				if (IsVerticesAbove(v[i], pos, rot)){
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

			Vector3[] v3;
			Vector2[] v2;
			return GetTriangleCutVertices(point, rot, t, out v3, out v2);
		}

		private Vector3[] GetTriangleCutVertices(Vector3 point, Quaternion rot, Triangle t,out Vector3[] normals, out Vector2[] uvs){
			List<Vector3> dots = new List<Vector3>();
			List<Vector3> normalsL = new List<Vector3>();
			List<Vector2> uvsL = new List<Vector2>();

				Vector3 AB; Vector3 BC;  Vector3 AC;
				Vector3 ABn; Vector3 BCn;  Vector3 ACn;
				Vector2 ABu; Vector2 BCu;  Vector2 ACu;
			bool IsAB = IsEdgeIntersected(t.EdgeAB, point, rot, out AB,out ABn,out ABu);
			bool IsBC = IsEdgeIntersected(t.EdgeBC, point, rot, out BC,out BCn,out BCu);
			bool IsAC = IsEdgeIntersected(t.EdgeAC, point, rot, out AC,out ACn,out ACu);
			if (!IsAB){
				dots.Add(AC);
				dots.Add(BC);
				
				normalsL.Add(ACn);
				normalsL.Add(BCn);

				uvsL.Add(ACu);
				uvsL.Add(BCu);

			}
			if (!IsBC){
				dots.Add(AB);
				dots.Add(AC);

				normalsL.Add(ABn);
				normalsL.Add(ACn);

				uvsL.Add(ABu);
				uvsL.Add(ACu);
			}
			if (!IsAC){
				dots.Add(BC);
				dots.Add(AB);

				normalsL.Add(BCn);
				normalsL.Add(ABn);

				uvsL.Add(BCu);
				uvsL.Add(ABu);
			}

			normals = normalsL.ToArray();
			uvs = uvsL.ToArray();
			return dots.ToArray();
		}
		private bool IsEdgeIntersected(Edge edge, Vector3 pos, Quaternion rot, out Vector3 intersectionPoint, out Vector3 normal,out Vector2 uv){
			Vector3 A = (rot * (mesh.vertices[edge.VerticeA]))+pos; 
			Vector3 B = (rot * (mesh.vertices[edge.VerticeB]))+pos;

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
				intersectionPoint = (Quaternion.Inverse(rot)*(new Vector3(newX,0,newZ)-pos));

				float lerp = InverseLerp(A,B,p);
				normal = Vector3.Lerp(mesh.normals[edge.VerticeA], mesh.normals[edge.VerticeB], lerp);
				uv = Vector3.Lerp(mesh.uv[edge.VerticeA], mesh.uv[edge.VerticeB], lerp);

				return true;


			}

			normal = default;
			uv = default;

			return false;
		}
		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 ab = b - a;
			Vector3 av = value - a;
			return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
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

		private bool IsVerticesAbove(Vector3 point, Vector3 pos, Quaternion rot){

			 Vector3 p = (rot * point ) +pos;
			return p.y > 0;
		}

	}
}