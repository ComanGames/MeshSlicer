using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Slicer{
	public class ClosedSlicer:BaseSlicer,ISlicer{
		public ClosedSlicer(MeshInfo meshInfo) : base(meshInfo){ }
		protected override void PostProcessing(MeshInfo mAbove, MeshInfo mBelow, int newCount){
			if (newCount == 0)
				return;

			int startIndex = mAbove.vertices.Count - newCount;
			Vector3[] newVertices = mAbove.vertices.Skip(startIndex).ToArray();

			Vector3 avrage = GetAverage(newVertices);

			int[] newTriangles;

			newTriangles = TriangelsRecalculation(mAbove, newCount);

			mAbove.vertices.AddRange(newVertices);
			mAbove.vertices.Add(avrage);

			mAbove.triangles.AddRange(newTriangles);

			int count = Math.Max(0, mAbove.normals.Count() - newCount);
			Vector3 newNormal = GetAverage(mAbove.normals.Skip(count).ToArray());

			Vector3[] newNormals = Enumerable.Repeat(newNormal, newCount + 1).ToArray();
			mAbove.normals.AddRange(newNormals);


			IEnumerable<Vector2> collection = Enumerable.Repeat(Vector2.zero,newCount+1);
			mAbove.uv.AddRange(collection);

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