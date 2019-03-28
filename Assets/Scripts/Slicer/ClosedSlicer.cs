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

			Vector3[] newVertices = new Vector3[newCount];
			int startIndex = mAbove.vertices.Count - newCount;
			for (int i = 0; i < newVertices.Length-1; i++)
				newVertices[i] = mAbove.vertices[startIndex + i];

			Vector3 avrage = GetAverage(newVertices);

			int shift = mAbove.vertices.Count;
			int[]newTriangles = new int[(newCount/2)*3];
			for (int i = 0; i < (newTriangles.Length / 3)-1; i++){
				int index = i * 3;

				if (i % 2 == 0){
					newTriangles[index] = shift + newCount;
					newTriangles[index + 1] = shift + i * 2;
					newTriangles[index + 2] = shift + i * 2 + 1;
				}
				else{
					newTriangles[index+2] = shift + newCount;
					newTriangles[index + 1] = shift + i * 2;
					newTriangles[index ] = shift + i * 2 + 1;
				}
			}
			
			newTriangles[newTriangles.Length - 1] = shift + newCount;
			newTriangles[newTriangles.Length -2] = shift;
			newTriangles[newTriangles.Length-3 ] = shift +newCount-3;

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