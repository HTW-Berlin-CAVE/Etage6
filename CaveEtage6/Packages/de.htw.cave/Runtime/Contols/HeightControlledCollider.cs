using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Htw.Cave.Joycons;

namespace Htw.Cave.Controls
{
	/// <summary>
	/// Updates the collider height in respect to the local height.
	/// </summary>
	[AddComponentMenu("Htw.Cave/Controls/Height Controlled Collider")]
	[RequireComponent(typeof(CapsuleCollider))]
	public sealed class HeightControlledCollider : MonoBehaviour
	{
		private const float Spacing = 0.1f;

		[SerializeField]
		private Transform head;
		public Transform Head
		{
			get => this.head;
			set
			{
				this.head = value;
				CenterCollider();
			}
		}

		private CapsuleCollider capsule;

		public void Awake()
		{
			this.capsule = base.GetComponent<CapsuleCollider>();
		}

		public void FixedUpdate()
		{
			CenterCollider();
		}

		public void Reset()
		{
			this.capsule = base.GetComponent<CapsuleCollider>();
			this.capsule.radius = 0.3f;
		}

		public void CenterCollider()
		{
			Vector3 position = this.head.localPosition;

			this.capsule.center = new Vector3(position.x, position.y * 0.5f + (0.5f * Spacing), position.z);
			this.capsule.height = position.y + Spacing;
		}
	}
}
