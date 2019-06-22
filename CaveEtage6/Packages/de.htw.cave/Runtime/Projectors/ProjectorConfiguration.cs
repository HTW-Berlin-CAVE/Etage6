using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Htw.Cave.Math;

namespace Htw.Cave.Projectors
{
	/// <summary>
	/// Defines how a camera will render the stereoscopic image.
	/// </summary>
	[CreateAssetMenu(fileName = "New Projector Configuration", menuName = "Htw.Cave/Projector Configuration", order = 21)]
	public class ProjectorConfiguration : ScriptableObject
	{
		[SerializeField]
		private int displayId;
		public int DisplayId
		{
			get => this.displayId;
			set => this.displayId = value;
		}

		[SerializeField]
		private string displayName;
		public string DisplayName
		{
			get => this.displayName;
			set => this.displayName = value;
		}

		[SerializeField]
		private float width;
		public float Width
		{
			get => this.width;
			set => this.width = value;
		}

		[SerializeField]
		private float height;
		public float Height
		{
			get => this.height;
			set => this.height = value;
		}

		[SerializeField]
		private float fieldOfView;
		public float FieldOfView
		{
			get => this.fieldOfView;
			set => this.fieldOfView = value;
		}

		[SerializeField]
		private bool invertStereo;
		public bool InvertStereo
		{
			get => this.invertStereo;
			set => this.invertStereo = value;
		}
	}
}
