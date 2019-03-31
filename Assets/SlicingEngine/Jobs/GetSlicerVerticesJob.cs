using Assets.Scripts;
using Assets.Scripts.Slicer;
using UnityEngine;

namespace Assets.SlicingEngine{
	public class GetSlicerVerticesJob{
		public ISlicer Slicer;
		public Vector3 Pos;
		public Quaternion Rot;
		public Vector3[] ResultVector3s;

		public GetSlicerVerticesJob(ISlicer slicer, Vector3 pos, Quaternion rot)
		{
			Slicer = slicer;
			Pos = pos;
			Rot = rot;

		}

		public void Execute()
		{
			ResultVector3s = Slicer.GetSliceVertices(Pos, Rot);
		}
	}
}