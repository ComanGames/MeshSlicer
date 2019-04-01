using Assets.Scripts.Slicer;
using Assets.SlicingEngine;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts{
	public struct SlicingJob:IJob{

		public Vector3 Pos;
		public Quaternion Rot;
		public InfoForMashJob InpuMeshData;
		public InfoForMashJob ResultAbove;
		public InfoForMashJob ResultBelow;
		public SlicingJob(MeshInfo mesh, Vector3 pos, Quaternion rot){
			Pos = pos;
			Rot = rot;
			InpuMeshData =  new InfoForMashJob(mesh);
			ResultAbove = new InfoForMashJob(mesh);
			ResultBelow = new InfoForMashJob(mesh);
			}

		public void Execute(){
			ISlicer slicer = new ClosedSlicer(InpuMeshData.GetMesh());
			MeshInfo[] meshes = slicer.Slice(Pos, Rot);
			ResultAbove = new InfoForMashJob(meshes[0]);
			ResultBelow = new InfoForMashJob(meshes[1]);
		}
	}
}