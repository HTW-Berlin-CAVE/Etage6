using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Htw.Cave.Projectors
{
	public static class ProjectorEditorUtils
	{
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.Active)]
		public static void ProjectorGizmos(ProjectorMount mount, GizmoType type)
		{
			ProjectorCamera[] cameras = mount.Cameras;

			for(int i = cameras.Length - 1; i >= 0; --i)
			{
				ProjectorCamera camera = cameras[i];

				if(camera.Configuration == null)
					continue;

				Vector3[] plane = camera.TransformPlanePoints();

				Gizmos.color = new Color(0.55f, 0.71f, 0f);

				Gizmos.DrawLine(camera.transform.position, plane[0]);
				Gizmos.DrawLine(camera.transform.position, plane[1]);
				Gizmos.DrawLine(camera.transform.position, plane[2]);
				Gizmos.DrawLine(camera.transform.position, plane[3]);

				Gizmos.DrawLine(plane[0], plane[1]);
				Gizmos.DrawLine(plane[1], plane[2]);
				Gizmos.DrawLine(plane[2], plane[3]);
				Gizmos.DrawLine(plane[3], plane[0]);
			}
		}
	}
}
