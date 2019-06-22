using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Htw.Cave.Legacy
{
	/// <summary>
	/// Adds support for the old bimber matrix equalization.
	/// </summary>
	[AddComponentMenu("Htw.Cave/Legacy/Legacy Bimber Equalization")]
	[RequireComponent(typeof(Camera))]
	public class LegacyBimberEqualization : MonoBehaviour
	{
		[SerializeField]
		private Matrix4x4 equalization;
		public Matrix4x4 Equalization
		{
			get => this.equalization;
			set => this.equalization = value;
		}

		[SerializeField]
		private bool equalize;
		public bool Equalize
		{
			get => this.equalize;
			set => this.equalize = value;
		}

		private Camera cam;

		public void Awake()
		{
			this.cam = base.GetComponent<Camera>();
		}

		public void Reset()
		{
			this.equalization = Matrix4x4.identity;
			this.equalize = true;
		}

		public void OnPreCull()
		{
			if(!this.equalize)
				return;

#if UNITY_EDITOR
			this.cam.projectionMatrix = this.equalization * this.cam.projectionMatrix;
#else
			Matrix4x4 left = this.cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			this.cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, this.equalization * left);

			Matrix4x4 right = this.cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			this.cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, this.equalization * right);
#endif
		}
	}
}
