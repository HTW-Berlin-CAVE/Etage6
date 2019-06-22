using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Etage6
{
	[AddComponentMenu("Etage6/Door Manager")]
	public sealed class DoorManager : MonoBehaviour
	{
		public static DoorManager Instance { get; private set; }

		public static void Add(Door door)
		{
			Instance.doors.Add(door);
		}

		[SerializeField]
		private Transform doorTarget;
		public Transform DoorTarget
		{
			get => this.doorTarget;
			set => this.doorTarget = value;
		}

		[SerializeField]
		private float timeToOpen;
		public float TimeToOpen
		{
			get => this.timeToOpen;
			set => this.timeToOpen = value;
		}

		[SerializeField]
		private float timeToClose;
		public float TimeToClose
		{
			get => this.timeToClose;
			set => this.timeToClose = value;
		}

		[SerializeField]
		private float triggerDistance;
		public float TriggerDistance
		{
			get => this.triggerDistance;
			set => this.triggerDistance = value;
		}

		[SerializeField]
		private AudioClip openSound;
		public AudioClip OpenSound
		{
			get => this.openSound;
			set => this.openSound = value;
		}

		[SerializeField]
		private AudioClip closeSound;
		public AudioClip CloseSound
		{
			get => this.closeSound;
			set => this.closeSound = value;
		}

		private List<Door> doors;
		private float sqrDistance;

		public void Awake()
		{
			if(Instance != null && Instance != this)
				Destroy(this);

			Instance = this;

			this.doors = new List<Door>();
			this.sqrDistance = this.triggerDistance * this.triggerDistance;
		}

		public void Update()
		{
			for(int i = 0; i < this.doors.Count; ++i)
			{
				bool inRange = doors[i].InSqrRange(this.doorTarget.position, this.sqrDistance);

				if(!doors[i].IsOpen && inRange)
					doors[i].Open();
				else if(doors[i].IsOpen && !inRange)
					doors[i].Close();
			}
		}
	}
}
