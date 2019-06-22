using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Etage6
{
	[AddComponentMenu("Etage6/Door")]
	[RequireComponent(typeof(AudioSource))]
	public class Door : MonoBehaviour
	{
		[SerializeField]
		private float openAngle;
		public float OpenAngle
		{
			get => this.openAngle;
			set => this.openAngle = value;
		}

		public bool IsOpen { get; private set; }

		private AudioSource audioSource;
		private float initialAngle;
		private float angleAcc;
		private bool shouldClose;

		public void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
			this.initialAngle = transform.rotation.eulerAngles.y;
			this.IsOpen = false;
		}

		public void Start()
		{
			DoorManager.Add(this);
			base.enabled = false;
		}

		public void Update()
		{
			MoveAngle();
		}

		public void Reset()
		{
			this.openAngle = 90f;
		}

		public void Open()
		{
			if(base.enabled)
				return;

			this.angleAcc = 0f;
			this.audioSource.clip = DoorManager.Instance.OpenSound;
			this.audioSource.Play();
			this.IsOpen = true;
			base.enabled = true;
		}

		public void Close()
		{
			if(base.enabled)
				return;

			this.angleAcc = 0f;
			this.audioSource.clip = DoorManager.Instance.CloseSound;
			this.audioSource.Play();
			this.IsOpen = false;
			base.enabled = true;
		}

		public bool InSqrRange(Vector3 position, float range)
		{
			return (transform.position - position).sqrMagnitude < range;
		}

		private void MoveAngle()
		{
			this.angleAcc += Time.deltaTime / DoorManager.Instance.TimeToOpen;
			float angle = 0f;

			if(this.IsOpen)
				angle = Mathf.Lerp(this.initialAngle, this.openAngle, this.angleAcc);
			else
				angle = Mathf.Lerp(this.openAngle, this.initialAngle, this.angleAcc);

			float targetAngle = this.IsOpen ? this.openAngle : this.initialAngle;

			if(this.angleAcc > 1f)
			{
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetAngle, transform.localEulerAngles.z);
				base.enabled = false;
			} else {
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
			}
		}
	}
}
