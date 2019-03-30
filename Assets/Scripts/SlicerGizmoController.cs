using System.Collections.Generic;
using Assets.Scripts.Slicer;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts{
	public class SlicerGizmoController:MonoBehaviour{
		private Vector3[] dots;
		public void DrawDots(List<Vector3> dots){
			this.dots = dots.ToArray();
		}

		private void OnDrawGizmos(){

			Gizmos.color = Color.green;
			if (dots != null && dots.Length > 0){
				for (int i = 1; i < dots.Length; i++){
					Gizmos.DrawLine(dots[i-1],dots[i]);
					Gizmos.color = Color.black;
					Gizmos.DrawSphere(dots[i],0.01f);
					Gizmos.color = Color.green;
					
				}

			}


		}

		public GameObject[] DoSlice(Vector3 pos, Quaternion rot, ISlicer slicer){
			MeshInfo[] meshes = slicer.Slice(pos,rot);
			Mesh Above = meshes[0].GetMesh();
			Mesh Below = meshes[1].GetMesh();

			MeshUtility.Optimize(Above);
			MeshUtility.Optimize(Below);

			GameObject cloneAbove = Instantiate(gameObject,transform.position,transform.rotation);
			GameObject cloneBelow = Instantiate(gameObject,transform.position,transform.rotation);

			cloneAbove.GetComponent<MeshFilter>().sharedMesh = Above;
			cloneBelow.GetComponent<MeshFilter>().sharedMesh = Below;
			return new GameObject[]{cloneAbove,cloneBelow};

		}
	}
}