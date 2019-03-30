using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Slicer{
	public class ClosedSlicer:BaseSlicer,ISlicer{
		public ClosedSlicer(MeshInfo meshInfo) : base(meshInfo){ }
		protected override void PostProcessing(MeshInfo mAbove, MeshInfo mBelow, int newCount,Quaternion rot){
			if (newCount == 0)
				return;

			int startIndex = mAbove.vertices.Count - newCount;
			int startIndexBelow = mBelow.vertices.Count - newCount;
			Vector3[] newVerticesAbove = mAbove.vertices.Skip(startIndex).ToArray();
			Vector3[] newVerticesBelow = mBelow.vertices.Skip(startIndexBelow).ToArray();

			Vector3 avrage = GetAverage(newVerticesAbove);

			int[] newTrianglesAbove;
			int[] newTrianglesBelow;

			newTrianglesAbove = TriangelsRecalculation(mAbove, newCount);
			newTrianglesBelow = TriangelsRecalculation(mBelow, newCount);

			mAbove.vertices.AddRange(newVerticesAbove);
			mAbove.vertices.Add(avrage);

			mBelow.vertices.AddRange(newVerticesBelow);
			mBelow.vertices.Add(avrage);

			mAbove.triangles.AddRange(newTrianglesAbove);
			mBelow.triangles.AddRange(newTrianglesBelow);


			Vector3 newNormalAbove = rot*Vector3.up;
			Vector3 newNormalBelow = rot*Vector3.down;

			mAbove.normals.AddRange(Enumerable.Repeat(newNormalAbove, newCount +1).ToArray());
			mBelow.normals.AddRange(Enumerable.Repeat(newNormalBelow, newCount +1).ToArray());


			IEnumerable<Vector2> collection = Enumerable.Repeat(Vector2.zero,newCount+1);
			mAbove.uv.AddRange(collection);
			mBelow.uv.AddRange(collection);

	}

		private static int[] TriangelsRecalculation(MeshInfo mAbove, int newCount){
			List<int> newTriangles = new List<int>();
			int shift = mAbove.vertices.Count;
			int max = mAbove.vertices.Count + newCount;

			List<int> other= new List<int>();
			for (int i = 0; i < mAbove.triangles.Count / 3; i++){
				int index = mAbove.triangles.Count - (i * 3);
				int A = mAbove.triangles[index - 1];
				int B = mAbove.triangles[index - 2];
				int C = mAbove.triangles[index - 3];
				int[] temp = new int[]{A, B, C};

				int m = 0;
				foreach (int i1 in temp){
					if (i1 < shift - newCount){
						m++;
					}
				}

				if (m == 1){
					foreach (int i1 in temp){
						if (i1 < shift - newCount)
							newTriangles.Add(max);
						else
							newTriangles.Add(i1 + newCount);
					}
				}
			}

			return newTriangles.ToArray();	
		}

		private Vector3 GetAverage(Vector3[] newVertices){
			float x = 0;
			float y = 0;
			float z = 0;
			int c = newVertices.Length;
			for (int i = 0; i < newVertices.Length; i++){
				x += newVertices[i].x;
				y += newVertices[i].y;
				z += newVertices[i].z;
			}

			return new Vector3(x/c,y/c,z/c);
		}
	}
}