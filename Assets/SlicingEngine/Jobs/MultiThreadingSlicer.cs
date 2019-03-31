using Assets.Scripts;
using Assets.Scripts.Slicer;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Assets.SlicingEngine{
	public class MultiThreadingSlicer:ISlicer{

		private MeshInfo info;

		public MultiThreadingSlicer(MeshInfo info){
			this.info = info;
		}

		public Vector3[] GetSliceVertices(Vector3 pos, Quaternion rot){
			// Create a native array of a single float to store the result. This example waits for the job to complete for illustration purposes
			return null;

		}

		public MeshInfo[] Slice(Vector3 pos, Quaternion rot){
			SlicingJob slicingJob = new SlicingJob(info, pos,rot);
			JobHandle handle =slicingJob.Schedule();
			handle.Complete();
			MeshInfo[] result = new MeshInfo[2];
			result[0] = slicingJob.ResultAbove.GetMesh();
			result[1] = slicingJob.ResultBelow.GetMesh();

			return result;
		}
	}
}