using System;
using System.Collections.Generic;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

namespace Assets.Scripts.Slicer{
	public class BaseSlicer:ISlicer{
		private readonly MeshInfo _meshInfo;
		private readonly Triangle[] _triangles ;

		public BaseSlicer(MeshInfo meshInfo){
			_meshInfo =meshInfo;
			_triangles = Triangle.GetTriangles(meshInfo);
		}

		public MeshInfo[] Slice(Vector3 pos, Quaternion rot){

			//Sorting vertices
			int[] sorted = SortVertices(_meshInfo.vertices, pos, rot);

			GetVerticesBelovAndAbove(sorted, _meshInfo.vertices, out var vAbove, out var vBelow);
			GetTrianglesBelowAhdAbove(sorted, _meshInfo.triangles, out var tAbove, out var tBelow);
			GetNormalsBelowAhdAbove(sorted, _meshInfo.normals, out var nAbove, out var nBelow);
			GetUvBelowAhdAbove(sorted, _meshInfo.uv, out var uAbove, out var uBelow);

			//Getting t in between
			Triangle[] inBetween = GetIntersectedTriangels(_triangles, sorted);

			int newVerticesCount = 0;
			foreach (Triangle tr in inBetween){

				Vector3[] newVertices = GetTriangleCutVertices(pos, rot, tr,out var newNormals,out var newUVs);

				newVerticesCount += newVertices.Length;
				vAbove.AddRange(newVertices);
				vBelow.AddRange(newVertices);

				int aboveCount = vAbove.Count;
				int belowCount = vBelow.Count;

				int[] newTrianglesUp = GetNewTrianglesUp(tr,sorted);
				int[] newTrianglesDown = GetReverse(newTrianglesUp);

				bool tUp = IsTriangleUp(tr,sorted);
				if (tUp){
					tAbove.AddRange(GetUpTriangles(newTrianglesUp, aboveCount));
					tBelow.AddRange(GetDownTriangles(newTrianglesDown, belowCount));
				}
				else{
					tAbove.AddRange(GetDownTriangles(newTrianglesUp, aboveCount));
					tBelow.AddRange(GetUpTriangles(newTrianglesDown, belowCount));
				}

				nAbove.AddRange(newNormals);
				nBelow.AddRange(newNormals);

				uAbove.AddRange(newUVs);
				uBelow.AddRange(newUVs);
			}

			//Mesh Above
			MeshInfo mAbove = AssembleMesh(vAbove, tAbove, nAbove, uAbove);
			MeshInfo mBelow = AssembleMesh(vBelow, tBelow, nBelow, uBelow);

			PostProcessing(mAbove, mBelow,newVerticesCount);
			return new[]{mAbove,mBelow};
		}

		public Vector3[] GetSliceVertices(Vector3 pos, Quaternion rot){

			//Sorting vertices
			int[] sorted = SortVertices(_meshInfo.vertices, pos, rot);

			//Getting t in between
			Triangle[] inBetween = GetIntersectedTriangels(_triangles, sorted);


			List<Vector3> dots = new List<Vector3>();

			
			foreach (Triangle tr in inBetween){
				dots.AddRange(GetTriangleCutVertices(pos, rot, tr));
			}

			return dots.ToArray();
		}

		protected virtual void PostProcessing(MeshInfo mAbove, MeshInfo mBelow,int newCount){
		}

		private static MeshInfo AssembleMesh(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv){
			MeshInfo m = new MeshInfo();
			m.vertices = vertices;
			m.triangles = triangles;
			m.normals = normals;
			m.uv = uv;
			return m;
		}

		private void GetUvBelowAhdAbove(int[] sorted,List< Vector2> u, out List<Vector2> uAbove, out List<Vector2> uBelow){
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

		private void GetNormalsBelowAhdAbove(int[] sorted,List<Vector3> n, out List<Vector3> nAbove, out List<Vector3> nBelow){
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

		private void GetTrianglesBelowAhdAbove(int[] sorted, List<int> t, out List<int> tAbelow, out List<int> tBelow){
			tAbelow = new List<int>();
			tBelow = new List<int>();
			for (int i = 0; i < t.Count/3; i++){
				int a = sorted[t[(i * 3)]];
				int b = sorted[t[(i * 3) + 1]];
				int c = sorted[t[(i * 3) + 2]];
				if(a>=0&&b>=0&&c>=0){
					tAbelow.AddRange(new[]{a,b,c});
				}
				else if(a<0&&b<0&&c<0)
					tBelow.AddRange(new[]{-a-1,-b-1,-c-1});
			}
		}

		private static void GetVerticesBelovAndAbove(int[] sorted, List<Vector3> v, out List<Vector3> vAbove, out List<Vector3> vBelow){
			vAbove = new List<Vector3>();
			vBelow = new List<Vector3>();
			for (int i = 0; i < v.Count; i++){
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
		private int[] SortVertices( List<Vector3> v, Vector3 pos, Quaternion rot){
			int[] result = new int[v.Count];
			int up = 0;
			int down = -1;
			for (int i = 0; i < v.Count; i++){
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
			return GetTriangleCutVertices(point, rot, t, out _, out _);
		}
		private Vector3[] GetTriangleCutVertices(Vector3 point, Quaternion rot, Triangle t,out Vector3[] normals, out Vector2[] uvs){
			List<Vector3> dots = new List<Vector3>();
			List<Vector3> normalsL = new List<Vector3>();
			List<Vector2> uvsL = new List<Vector2>();

				Vector3 AB; Vector3 BC;  Vector3 AC;
				Vector3 ABn; Vector3 BCn;  Vector3 ACn;
				Vector2 ABu; Vector2 BCu;  Vector2 ACu;
			bool isAb = IsEdgeIntersected(t.EdgeAB, point, rot, out AB,out ABn,out ABu);
			bool isBc = IsEdgeIntersected(t.EdgeBC, point, rot, out BC,out BCn,out BCu);
			bool isAc = IsEdgeIntersected(t.EdgeAC, point, rot, out AC,out ACn,out ACu);
			if (!isAb){
				dots.Add(AC);
				dots.Add(BC);
				
				normalsL.Add(ACn);
				normalsL.Add(BCn);

				uvsL.Add(ACu);
				uvsL.Add(BCu);

			}
			if (!isBc){
				dots.Add(AB);
				dots.Add(AC);

				normalsL.Add(ABn);
				normalsL.Add(ACn);

				uvsL.Add(ABu);
				uvsL.Add(ACu);
			}
			if (!isAc){
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
			Vector3 a = (rot * (_meshInfo.vertices[edge.VerticeA]))+pos; 
			Vector3 b = (rot * (_meshInfo.vertices[edge.VerticeB]))+pos;

			bool ab = a.y >= 0 && b.y <= 0;
			bool ba = b.y >= 0 && a.y <= 0;
			intersectionPoint = default;
			if (ab||ba){
				float newXStep = ((b.x - a.x) / (b.y - a.y));
				float newX =((-1*a.y)*newXStep) +a.x;

				float newZStep = ((b.z - a.z) / (b.y - a.y));
				float newZ =((-1*a.y)*newZStep) +a.z;
				Vector3 p =new Vector3(newX,0,newZ);

				intersectionPoint = p;
				intersectionPoint = (Quaternion.Inverse(rot)*(new Vector3(newX,0,newZ)-pos));

				float lerp = InverseLerp(a,b,p);
				normal = Vector3.Lerp(_meshInfo.normals[edge.VerticeA], _meshInfo.normals[edge.VerticeB], lerp);
				uv = Vector3.Lerp(_meshInfo.uv[edge.VerticeA], _meshInfo.uv[edge.VerticeB], lerp);

				return true;


			}

			normal = default;
			uv = default;

			return false;
		}

		private float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 ab = b - a;
			Vector3 av = value - a;
			return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
		}

		private bool IsVerticesAbove(Vector3 point, Vector3 pos, Quaternion rot){

			 Vector3 p = (rot * point ) +pos;
			return p.y > 0;
		}

	}
}