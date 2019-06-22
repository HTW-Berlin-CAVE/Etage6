using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Htw.Cave.Joycons;
using Htw.Cave.Kinect;

namespace Etage6
{
	[AddComponentMenu("Etage6/Shoot Controls")]
	public sealed class ShootControls : MonoBehaviour
	{
		private const int maxAmmunitionInScene = 20;

		[SerializeField]
		private Ammunition ammunitionPrefab;
		public Ammunition AmmunitionPrefab
		{
			get => this.ammunitionPrefab;
			set => this.ammunitionPrefab = value;
		}

		[SerializeField]
		private Transform crosshairPrefab;
		public Transform CrosshairPrefab
		{
			get => this.crosshairPrefab;
			set => this.crosshairPrefab = value;
		}

		[SerializeField]
		private float force;
		public float Force
		{
			get => this.force;
			set => this.force = value;
		}

		private KinectActor actor;
		private Queue<Ammunition> ammunitionQueue;
		private Transform crosshair;

		public void Awake()
		{
			this.ammunitionQueue = new Queue<Ammunition>();
			this.crosshair = Instantiate<Transform>(this.crosshairPrefab);
		}

		public void Start()
		{
			this.actor = KinectPlayArea.Actor;
		}

		public void Update()
		{
			if(JoyconInput.GetButton("Bumper R"))
				Aim();

			if(JoyconInput.GetButtonUp("Bumper R"))
				this.crosshair.gameObject.SetActive(false);

			if(JoyconInput.GetButtonDown("Trigger R"))
				Shoot();
		}

		public void Shoot()
		{
			Ammunition ammunition;

			if(this.ammunitionQueue.Count > maxAmmunitionInScene)
				ammunition = this.ammunitionQueue.Dequeue();
			else
				ammunition = Instantiate<Ammunition>(this.ammunitionPrefab);

			if(this.actor.IsTracked)
				ammunition.Fire(this.actor.GetRightHandPosition(), this.actor.GetRightWristDirection(), this.force);
			else
				ammunition.Fire(transform.position + Vector3.up + transform.right * 0.5f, transform.forward, this.force);

			this.ammunitionQueue.Enqueue(ammunition);
		}

		private void Aim()
		{
			this.crosshair.gameObject.SetActive(true);

			Vector3 origin = transform.position + Vector3.up + transform.right * 0.5f;
			Vector3 direction = transform.forward;

			if(this.actor.IsTracked)
			{
				origin = this.actor.GetRightHandPosition();
				direction = this.actor.GetRightWristDirection();
			}

			RaycastHit hit;

			if(Physics.Raycast(origin, direction, out hit, 100f))
			{
				crosshair.up = -transform.forward;
				crosshair.position = hit.point + hit.normal * 0.01f;
			} else {
				this.crosshair.gameObject.SetActive(false);
			}
		}
	}
}
