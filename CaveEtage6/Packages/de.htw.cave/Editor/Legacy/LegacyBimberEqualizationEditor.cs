using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using Htw.Cave.Utilites;

namespace Htw.Cave.Legacy
{
	[CustomEditor(typeof(LegacyBimberEqualization))]
	public class LegacyBimberEqualizationEditor : Editor
	{
		private LegacyBimberEqualization me;

		public void OnEnable()
		{
			this.me = (LegacyBimberEqualization)base.target;;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			serializedObject.Update();

			DrawDefaultInspector();

			EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if(GUILayout.Button("Import"))
			{
				string path = EditorUtility.OpenFilePanel("Import Equalization Matrix", Application.dataPath, "txt");

				if(!string.IsNullOrEmpty(path))
				{
					this.me.Equalization = ConvertFromFile(path);
				}
			}

			EditorGUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
		}

		private Matrix4x4 ConvertFromFile(string path)
		{
			Matrix4x4 mat = Matrix4x4.identity;

			using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open)))
			{
				for(int i = 0; i < 16; ++i)
					mat[i] = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
			}

			return mat;
		}
	}
}
