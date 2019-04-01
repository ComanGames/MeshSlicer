using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Slicer;
using Assets.SlicingEngine;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts{
	public class SlicerWindow:EditorWindow{
		public MeshFilter Renderer;
		public Material InternalMaterial;
		public SlicerType TypeOfSlicer = 0;
		private Vector3 pos;
		private Vector3 rot = new Vector3(180,180,180);
		private long LastTimeLapse = 0;

		[MenuItem("Window/MeshSlicer")]
		public static void Init(){
			SlicerWindow window = EditorWindow.GetWindow<SlicerWindow>();
			window.Show();
		}
		public void OnGUI(){
			TitleText();
			GetObject();
			GetSlicerType();
			GetPositionOfSlice();
			GetRotationOfSlice();
			GetMaterial();
			DoSlice();
			ShowData();
		}

		private void GetSlicerType(){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(" Type of slicer:", GUILayout.Width(100));
			TypeOfSlicer = (SlicerType)EditorGUILayout.EnumPopup(TypeOfSlicer,GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();
		}

		private void GetMaterial(){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(" Position:", GUILayout.Width(80));
			InternalMaterial = (Material) EditorGUILayout.ObjectField(InternalMaterial, typeof(Material), true);
			if (InternalMaterial == null){
				InternalMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
				InternalMaterial.color = Color.red;
			}

			EditorGUILayout.EndHorizontal();
		}

		private void ShowData(){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Time that was taken:", GUILayout.Width(250));
			GUIStyle style = new GUIStyle();
			style.fontSize = 15;
			style.normal.textColor = Color.green;
			EditorGUILayout.LabelField(LastTimeLapse.ToString());

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}


		private void GetObject(){
			GameObject active = Selection.activeGameObject;
			if (active == null)
				return;
			MeshFilter selected = active.GetComponent<MeshFilter>();
			if (selected != null)
				Renderer = selected;

			

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Object to slice:", GUILayout.Width(200));
			Renderer = (MeshFilter) EditorGUILayout.ObjectField(Renderer, typeof(MeshFilter), true);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			Mesh mesh = new Mesh();
		}

		private void GetPositionOfSlice(){

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(" Position:", GUILayout.Width(80));
			pos.y = EditorGUILayout.Slider(pos.y, -1, 1);
			EditorGUILayout.EndHorizontal();
		}

		private void GetRotationOfSlice(){
			EditorGUILayout.LabelField("Rotation:", GUILayout.Width(80));
			EditorGUI.indentLevel += 3;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("X:", GUILayout.Width(80));
			rot.x = EditorGUILayout.Slider(rot.x, 0, 360);
			EditorGUILayout.EndHorizontal();
;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Z:", GUILayout.Width(80));
			rot.z = EditorGUILayout.Slider(rot.z, 0, 360);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 3;
		}

		private void TitleText(){
			this.titleContent.text = "Mesh Slicer";

			EditorGUILayout.Space();
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = Color.white;
			EditorGUILayout.LabelField("Mesh Slicing settings", style);
			EditorGUILayout.Space();
		}

		private void DoSlice(){
			if (GUILayout.Button("Slice")){
				SlicerGizmoController controller = GetController();

			if (controller == null)
				return;

				Stopwatch stopwatch = Stopwatch.StartNew();

				ISlicer slicer = GetSlicer(Renderer.sharedMesh);
				GameObject[] objects=  controller.DoSlice(pos,Quaternion.Euler(rot),(ISlicer)slicer,InternalMaterial);
				
				Undo.DestroyObjectImmediate(controller.gameObject);
				Undo.RegisterCreatedObjectUndo(objects[0], "Create object");
				Undo.RegisterCreatedObjectUndo(objects[1], "Create object");
				SceneView.RepaintAll();
				Selection.activeGameObject = objects[0];
				LastTimeLapse = stopwatch.ElapsedMilliseconds;
			}
		}

		private ISlicer GetSlicer(Mesh mesh){
					MeshInfo info = new MeshInfo(mesh);
			switch (TypeOfSlicer){
				case SlicerType.Base:
					return  new BaseSlicer(info);
				case SlicerType.Close:
					return  new ClosedSlicer(info);
					
			}
			
			return null;
		}

		private void OnInspectorUpdate(){


			SlicerGizmoController controller = GetController();
			if (controller == null)
				return;


			ISlicer slice = new ClosedSlicer(new MeshInfo(Renderer.sharedMesh));

			Vector3[] vertices = slice.GetSliceVertices(pos,Quaternion.Euler(rot));
			for (int i = 0; i < vertices.Length; i++){
				vertices[i] = Renderer.transform.rotation * vertices[i];

			}
			List<Vector3> dots = new List<Vector3>();
			for (int i = 0; i < vertices.Length; i++){
				dots.Add(vertices[i]+Renderer.transform.position);
			}

			controller.DrawDots(dots);
			SceneView.RepaintAll();
		}

		private  SlicerGizmoController GetController(){
			if (Renderer == null)
				return null;

			SlicerGizmoController controller = Renderer.gameObject.GetComponent<SlicerGizmoController>();
			if (controller == null)
				controller = Renderer.gameObject.AddComponent<SlicerGizmoController>();
			controller.hideFlags = HideFlags.HideInInspector;
			return controller;
		}
	}

}