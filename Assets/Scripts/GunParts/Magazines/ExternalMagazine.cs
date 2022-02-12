using System;
using System.Collections.Generic;
using GunParts;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
#nullable enable

namespace Weapons.Ranged.Magazines {
	public sealed class ExternalMagazine : Magazine {
		public                             List<int>           Levels = new List<int>() {5, 10, 20, 30};
		[SerializeField] private           int                 Level;
		[SerializeField] private           DetachableMagazine? mag;
		public override                    int                 Capacity => mag?.Capacity ?? 0;

		[OdinSerialize, ShowInInspector, TabGroup("1", "Size Upgrade", Order = 3)] private Upgrade capacityUpgrade;
		public ExternalMagazine() { capacityUpgrade = new Upgrade(this); }

		/// <summary> Creates a new empty magazine of the type appropriate to this gun's ammo capacity upgrade level. </summary> 
		public GameObject GenerateEmptyMagazine() {
			GameObject? returnValue = null;
			switch (Level) {
				case 0: returnValue = Resources.Load("Magazines/Single-Stack Magazine") as GameObject;
					break;
				case 1: returnValue = Resources.Load("Magazines/Dual-Stack Magazine") as GameObject;
					break;
				case 2: returnValue = Resources.Load("Magazines/Drum Magazine") as GameObject;
					break;
				case 3: returnValue = Resources.Load("Magazines/Double Drum Magazine") as GameObject;
					break;
			}

			if (returnValue == null) {
				var up = new ArgumentOutOfRangeException();
				Debug.LogException(new ArgumentOutOfRangeException());
				throw up;
			}
			return returnValue;
		}

		protected override List<Round>        Contents => mag?.Contents ?? null;
		
		/// <summary> The only way you should be setting Contents on a gun with detachable mags is through RemoveMag and InsertMag.. </summary>
		[Button, TabGroup("1", "Debug")] public void EjectMag() {
			if (mag == null) {
				Debug.LogWarning("Hit the mag eject with no mag inserted.");
				return;
			}

			mag.Collider.enabled      = true;
			mag.RigidBody.isKinematic = false;
			mag.transform.SetParent(null, true);
			mag = null;
		}

		/// <summary> The only way you should be setting Contents on a gun with detachable mags is through EjectMag and InsertMag.. </summary>
		[Button, TabGroup("1", "Debug")] public void InsertMag(DetachableMagazine _mag) {
			if (mag != null) {
				Debug.LogException(new Exception("Tactical reload failed. Eject your mag first."));
				return;
			}

			mag                       = _mag;
			mag.Collider.enabled      = false;
			mag.RigidBody.isKinematic = true;
			mag.gameObject.transform.SetParent(magFeed.transform);
			mag.transform.position = magFeed.transform.position;
		}

		public override Round Feed() => mag!.Feed();

		[Button] private void materializeMagazine() {
			var newMag = GenerateEmptyMagazine();
			newMag.transform.position = transform.position;
			Selection.activeObject    = newMag.gameObject;
		}
	}
}