using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Htw.Cave.Projectors
{
	/// <summary>
	/// Gives access to the mounted cameras and updates its position
	/// to the eye position for stereoscopic rendering.
	/// </summary>
	[AddComponentMenu("Htw.Cave/Projectors/Projector Mount")]
	public class ProjectorMount : MonoBehaviour
	{
		[SerializeField]
		protected ProjectorEyes eyes;
		public ProjectorEyes Eyes
		{
			get => this.eyes;
			set => this.eyes = value;
		}

		public ProjectorCamera[] Cameras => base.GetComponentsInChildren<ProjectorCamera>(true);

		public virtual void Update()
		{
			transform.position = this.eyes.Anchor;
		}
	}
}
