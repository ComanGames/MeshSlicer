using System;
using System.Linq;
using Assets.Scripts.Slicer;
using Unity.Collections;
using UnityEngine;

namespace Assets.SlicingEngine{
	public struct InfoForMashJob{
		

		public NativeArray<Vector3> vertices;
//		public NativeArray<int> triangles;
		public NativeArray<Vector3> normals;
		public NativeArray<Vector2> uv;

		public  InfoForMashJob (MeshInfo mesh){
			vertices = new NativeArray<Vector3>(mesh.vertices.ToArray(),Allocator.TempJob);
//			triangles =  new NativeArray<int>( mesh.triangles.ToArray(), Allocator.TempJob);
			normals = new NativeArray<Vector3>( mesh.normals.ToArray(), Allocator.TempJob);
			uv = new NativeArray<Vector2>( mesh.uv.ToArray(),Allocator.TempJob);
		}

		public MeshInfo GetMesh(){
//			MeshInfo mesh = new MeshInfo();
//			mesh.vertices = vertices.ToList();
////			mesh.triangles = triangles.ToList();
//			if(normals.Length>0)
//				mesh.normals = normals.ToList();
//			if(uv.Length==vertices.Length)
//				mesh.uv = uv.ToList();
//
//			return mesh;
			 throw new NotImplementedException();

		}
	}
}