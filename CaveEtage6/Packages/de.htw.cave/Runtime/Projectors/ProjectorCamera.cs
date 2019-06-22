using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Htw.Cave.Math;

namespace Htw.Cave.Projectors
{
	/// <summary>
	/// Renders a stereoscopic image with a specialized projection matrix based
	/// on a projection plane.
	/// </summary>
	[AddComponentMenu("Htw.Cave/Projectors/Projector Camera")]
	[RequireComponent(typeof(Camera))]
	public sealed class ProjectorCamera : MonoBehaviour
	{
		[SerializeField]
		private ProjectorConfiguration configuration;
		public ProjectorConfiguration Configuration
		{
			get => this.configuration;
			set => this.configuration = value;
		}

		[SerializeField]
		private ProjectorPlane plane;
		public ProjectorPlane Plane
		{
			get => this.plane;
			set => this.plane = value;
		}

		private Camera cam;
		public Camera Camera
		{
			get => this.cam;
		}

		private ProjectorMount mount;
		private Vector3[] planePoints;
		private Vector3 vr, vu, vn;

		public void Awake()
		{
			if(this.configuration == null)
				throw new UnityException("Missing " + nameof(ProjectorConfiguration) + " in "+ nameof(ProjectorCamera) + " component.");

			if(this.cam == null)
				FindCamera();

			this.mount = base.GetComponentInParent<ProjectorMount>();
			this.vr = this.vu = this.vn = Vector3.zero;
		}

		public void Start()
		{
			UpdatePlanePoints();
		}

		public void FindCamera()
		{
			this.cam = base.GetComponent<Camera>();

			if(this.cam == null)
				this.cam = base.GetComponentInChildren<Camera>();
		}

		public void ActivateCameraDisplay()
		{
			this.cam.rect = new Rect(0f, 0f, 1f, 1f);

#if UNITY_EDITOR
			this.cam.targetDisplay = this.configuration.DisplayId;
#else
			// For some reason Unity shows that there is only 1 display in the editor.
			if(Display.displays.Length > this.configuration.DisplayId)
			{
				Display display = Display.displays[this.configuration.DisplayId];
				display.Activate();
				this.cam.targetDisplay = this.configuration.DisplayId;
			}
#endif
		}

		public void ResizeCameraViewport(int viewports)
		{
			float size = 1f / viewports;

			this.cam.targetDisplay = 0;
			this.cam.rect = new Rect(this.configuration.DisplayId * size, 0f, size, 1f);
		}

		public void SetCameraClipPlanes(float near, float far)
		{
			this.cam.nearClipPlane = near;
			this.cam.farClipPlane = far;
		}

		public void SetCameraStereo(float convergence, float separation)
		{
			this.cam.stereoConvergence = convergence;
			this.cam.stereoSeparation = separation;
		}

		public void UpdateCameraProjection()
		{
			Vector3[] plane = TransformPlanePoints();

			if(transform.hasChanged)
				Projection.HolographicPrecompute(plane[3], plane[2], plane[0], ref vr, ref vu, ref vn);

			ProjectorEyes eyes = this.mount.Eyes;
			eyes.Invert = this.configuration.InvertStereo;

#if UNITY_EDITOR
			Matrix4x4 worldToCameraMat;
			Matrix4x4 projectionMat = Projection.Holographic(plane[3], plane[2], plane[0], vr, vu, vn, eyes.Anchor, this.cam.nearClipPlane, this.cam.farClipPlane, out worldToCameraMat);

			this.cam.projectionMatrix = projectionMat;
			//this.cam.worldToCameraMatrix = worldToCameraMat;
#else
			Matrix4x4 worldToCameraMatLeft;
			Matrix4x4 worldToCameraMatRight;

			Matrix4x4 projectionMatLeft = Projection.Holographic(plane[3], plane[2], plane[0], vr, vu, vn, eyes.Left, this.cam.nearClipPlane, this.cam.farClipPlane, out worldToCameraMatLeft);
			Matrix4x4 projectionMatRight = Projection.Holographic(plane[3], plane[2], plane[0], vr, vu, vn, eyes.Right, this.cam.nearClipPlane, this.cam.farClipPlane, out worldToCameraMatRight);

			this.cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, projectionMatLeft);
			this.cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, projectionMatRight);
			//this.cam.SetStereoViewMatrix(Camera.StereoscopicEye.Left, worldToCameraMatLeft);
			//this.cam.SetStereoViewMatrix(Camera.StereoscopicEye.Right, worldToCameraMatRight);
#endif
		}

		public Vector3[] TransformPlanePoints()
		{
			if(this.plane.HasChanged)
				UpdatePlanePoints();

			return this.planePoints;
		}

		private void UpdatePlanePoints()
		{
			this.planePoints = this.plane.TransformPlane(this.configuration.Width, this.configuration.Height);
		}
	}
}
