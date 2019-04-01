using System.Collections.Generic;
using System.Linq;
using Assets.SlicingEngine;
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

			mAbove.triangles = CheckSize(mAbove.triangles);
			mBelow.triangles = CheckSize(mBelow.triangles);


			mAbove.triangles[1].AddRange(newTrianglesAbove);
			mBelow.triangles[1].AddRange(newTrianglesBelow);


			Vector3 newNormalAbove = rot*Vector3.down;
			Vector3 newNormalBelow = rot*Vector3.up;

			mAbove.normals.AddRange(Enumerable.Repeat(newNormalAbove, newCount +1).ToArray());
			mBelow.normals.AddRange(Enumerable.Repeat(newNormalBelow, newCount +1).ToArray());


			IEnumerable<Vector2> collection = Enumerable.Repeat(Vector2.zero,newCount+1);
			mAbove.uv.AddRange(collection);
			mBelow.uv.AddRange(collection);

	}

		private List<int>[] CheckSize(List<int>[] toUpdate){
			if (toUpdate.Length <= 1){
				List<int> old = toUpdate[0];
				toUpdate = new List<int>[2];
				toUpdate[0] = old;
				toUpdate[1] = new List<int>();

			}

			return toUpdate;
		}

		private static int[] TriangelsRecalculation(MeshInfo mash, int newCount){
			List<int> newTriangles = new List<int>();
			int shift = mash.vertices.Count;
			int max = mash.vertices.Count + newCount;

			List<int> other= new List<int>();
			for (int j = 0; j < mash.triangles.Length; j++){
				for (int i = 0; i < mash.triangles[j].Count / 3; i++){
					int index = mash.triangles[j].Count - (i * 3);
					int A = mash.triangles[j][index - 1];
					int B = mash.triangles[j][index - 2];
					int C = mash.triangles[j][index - 3];
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