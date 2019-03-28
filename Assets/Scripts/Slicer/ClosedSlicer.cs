using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Slicer{
	public class ClosedSlicer:BaseSlicer,ISlicer{
		public ClosedSlicer(MeshInfo meshInfo) : base(meshInfo){ }
		protected override void PostProcessing(MeshInfo mAbove, MeshInfo mBelow, int newCount){
			Vector3[] newVertices = new Vector3[newCount+1];
			int startIndex = mAbove.vertices.Length - newCount;
			for (int i = 0; i < newVertices.Length-1; i++)
				newVertices[i] = mAbove.vertices[startIndex + i];

			Vector3 avrage = GetAverage(newVertices);

			int shift = mAbove.vertices.Length;
			int[]newTriangles = new int[(newCount/2)*3];
			for (int i = 0; i < newTriangles.Length/3; i++){
				int index = i*3;
				newTriangles[index] = shift+newCount;
				newTriangles[index + 1] = shift + i + 1;
				newTriangles[index + 2] = shift + i + 1;
			}

			List<Vector3> vAbove = mAbove.vertices.ToList();
			List<Vector3> vBelow = mBelow.vertices.ToList();

			vAbove.AddRange(newVertices);
			vBelow.AddRange(newVertices);
			vAbove.Add(avrage);
			vBelow.Add(avrage);

			List<Vector3> vAbove = mAbove.vertices.ToList();
			List<Vector3> vBelow = mBelow.vertices.ToList();

			

		}

		private Vector3 GetAverage(Vector3[] newVertices){
			long x = 0;
			long y = 0;
			long z = 0;
			int c = newVertices.Length;
			for (int i = 0; i < newVertices.Length; i++){
				x += (long)newVertices[i].x;
				y += (long)newVertices[i].y;
				z += (long)newVertices[i].z;
			}
			return new Vector3(x/c,y/c,z/c);
		}
	}
}