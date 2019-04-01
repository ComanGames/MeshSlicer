using Assets.Scripts.Slicer;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.SlicerGame{
	[RequireComponent(typeof(MeshCollider))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class Slicable : MonoBehaviour, ISlicable{
		public Material InternalMaterial;
		public float ForceMultiplayer = 100f;
		protected Rigidbody _body;
		protected MeshCollider _collider;
		protected Mesh _mash;
		protected internal Quaternion Right;
		protected internal Quaternion Left;


		// Start is called before the first frame update
		void Start(){
			Left = Quaternion.Euler(-90,0,0);
			Right = Quaternion.Euler(90,0,0);
			_body = GetComponent<Rigidbody>();
			_collider = GetComponent<MeshCollider>();
			_mash = GetComponent<MeshFilter>().mesh;
		}

		// Update is called once per frame
		void Update(){ }

		public void GetCut(Ray ray){
			
			ISlicer slicer= new ClosedSlicer(new MeshInfo(_mash));
			Quaternion rot = Quaternion.LookRotation(ray.direction, Vector3.up);
			var pos = ray.origin - transform.position;
			GameObject[] sliced=  DoSlice(gameObject, pos, rot, slicer, InternalMaterial);
			sliced[0].GetComponent<Rigidbody>().AddForce(Left * ray.direction * ForceMultiplayer);
	 		sliced[1].GetComponent<Rigidbody>().AddForce(Right * ray.direction * ForceMultiplayer);
			Destroy(gameObject);
		}

		public GameObject[] DoSlice(GameObject obj, Vector3 pos, Quaternion rot, ISlicer slicer, Material mat){

			MeshInfo[] meshes = slicer.Slice(pos, rot);
			Mesh Above = meshes[0].GetMesh();
			Mesh Below = meshes[1].GetMesh();

			MeshUtility.Optimize(Above);
			MeshUtility.Optimize(Below);

			GameObject cloneAbove = Instantiate(obj, obj.transform.position, obj.transform.rotation);
			GameObject cloneBelow = Instantiate(obj, obj.transform.position, obj.transform.rotation);

			cloneAbove.GetComponent<MeshFilter>().mesh = Above;
			cloneBelow.GetComponent<MeshFilter>().mesh = Below;

			cloneAbove.GetComponent<MeshCollider>().sharedMesh = Above;
			cloneBelow.GetComponent<MeshCollider>().sharedMesh = Below;

			MeshRenderer above = cloneAbove.GetComponent<MeshRenderer>();
			MeshRenderer below = cloneBelow.GetComponent<MeshRenderer>();

			Material[] original = obj.GetComponent<MeshRenderer>().sharedMaterials;

			SetMaterials(above, original[0], mat);
			SetMaterials(below, original[0], mat);


			return new GameObject[]{cloneAbove, cloneBelow};

		}

		private static void SetMaterials(MeshRenderer target, Material original, Material interl){
			Material[] multiple = new Material[2];
			multiple[0] = original;
			multiple[1] = interl;
			target.sharedMaterials = multiple;

		}
	}
}
