using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public class SlicerDebug : MonoBehaviour{

	public Mesh mesh;
	public Vector3 Angle;
	public Vector3 Pos;

	private void OnDrawGizmosSelected()
	{
		
		SlicingCalculation slice = new SlicingCalculation(mesh);
		Vector3[] vertices = slice.GetMashCutVertices(Pos,Quaternion.Euler(Angle));
		for (int i = 0; i < vertices.Length; i++){
			Gizmos.DrawSphere(vertices[i]+transform.position,0.01f);
		}


		Mesh testMesh = slice.Slice(Pos,Quaternion.Euler(Angle))[1];
		foreach (Vector3 v in testMesh.vertices){
			Gizmos.DrawCube(v,Vector3.one*0.01f);
		}

		GetComponent<MeshFilter>().mesh = testMesh;



	}

}
