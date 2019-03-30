using Assets.Scripts.Slicer;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts{
	public class SlicingJob:IJob{

		public ISlicer Slicer;
		public Vector3 Pos;
		public Quaternion Rot;
		public Mesh Above;
		public Mesh Below;
		public SlicingJob(ISlicer slicer, Vector3 pos, Quaternion rot){
			Slicer = slicer;
			Pos = pos;
			Rot = rot;
		}

		public void Execute(){
			MeshInfo[] meshes = Slicer.Slice(Pos, Rot);
			Above = meshes[0].GetMesh();
			Below = meshes[1].GetMesh();
		}
	}
}