using UnityEngine;

namespace Assets.Scripts.SlicerGame{
	public class GameObjectSlicer:MonoBehaviour{
		public LayerMask CollisionMask;
		public Vector3 RayOffset;
		public int RayCounts = 10;
		public float RayLength = 5;
		public float Distance = 5;

		private void FixedUpdate()
		{
			var rays = GetRays();
			foreach (Ray ray in rays){
				RaycastHit hit;
				bool isHit = Physics.Raycast(ray, out hit,RayLength,CollisionMask);
				if (isHit){
					ISlicable slicable = hit.transform.GetComponent<ISlicable>();
					if(slicable!=null)
						slicable.GetCut(new Ray(hit.point,ray.direction));
				}
			}
		}

		private void OnDrawGizmos()
		{
			var rays = GetRays();

			Gizmos.color = Color.green;
			foreach (Ray ray in rays)
				Gizmos.DrawRay(ray.origin,ray.direction);

		}

		private Ray[] GetRays(){
			Ray[] rays = new Ray[RayCounts];
			for (int i = 0; i < RayCounts; i++){
				Vector3 pos = ((i * Distance) * Vector3.left) + transform.position + (transform.rotation*RayOffset);
				Vector3 dir =transform.rotation*Vector3.up * RayLength;
				rays[i] = new Ray(pos, dir);
			}

			return rays;
		}
	}
}