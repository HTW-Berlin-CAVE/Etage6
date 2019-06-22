using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Etage6
{
	[AddComponentMenu("Etage6/Ammunition")]
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(AudioSource))]
	public sealed class Ammunition : MonoBehaviour
	{
		private Rigidbody rigid;
		private AudioSource audioSource;

		public void Awake()
		{
			this.rigid = base.GetComponent<Rigidbody>();
			this.audioSource = base.GetComponent<AudioSource>();
		}

		public void Fire(Vector3 origin, Vector3 direction, float force)
		{
			transform.position = origin;
			this.rigid.velocity = Vector3.zero;
			this.rigid.AddForce(direction * force, ForceMode.Impulse);
			this.rigid.AddForce(Vector3.up * 7f, ForceMode.Impulse);
			this.audioSource.Play();
		}
	}
}
