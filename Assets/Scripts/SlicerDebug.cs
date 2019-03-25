using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public class SlicerDebug : MonoBehaviour
{


	private void OnDrawGizmosSelected()
	{
		Mesh mesh = (GetComponent<MeshFilter>()).mesh;
		
		SlicingCalculation slice = new SlicingCalculation(mesh);
		Vector3[] vertices = slice.GetCutVertices(Vector3.up,Quaternion.Euler(45,0,0));
		Debug.Log(vertices.Length);
		for (int i = 0; i < vertices.Length; i++){
			Gizmos.DrawSphere(vertices[i]+transform.position,0.01f);
		}


	}

}
