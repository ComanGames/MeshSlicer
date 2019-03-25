using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public class SlicerDebug : MonoBehaviour
{

	private void Start(){
		Mesh mesh = (GetComponent<MeshFilter>()).mesh;
		
		SlicingCalculation slice = new SlicingCalculation(mesh);
		Vector3[] vertices = slice.GetCutVertices(Vector3.zero,Quaternion.identity);
		for (int i = 0; i < vertices.Length; i++){
			Gizmos.DrawSphere(vertices[i],1f);
		}

	}

}
