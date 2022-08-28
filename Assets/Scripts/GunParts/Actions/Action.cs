using System;
using DefaultNamespace;
using Interfaces;
using Rounds;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;
using Weapons.Ranged.Magazines;
using static Round;
using Random = Unity.Mathematics.Random;

#nullable  enable

namespace GunParts.Actions {
	[RequireComponent(typeof(Receiver)), ExecuteAlways, DisallowMultipleComponent, HideMonoScript] public abstract class Action : GunPart {
		private AudioClip cockSound;

		[TabGroup("1", "Stats")]                  public  Cartridge           Cartridge;
		[TabGroup("1", "Stats")]                  public  Cartridge.Strengths Strength;
		[ShowInInspector, TabGroup("1", "Debug")] private MonoBehaviour?      ThingInChamber;
		private                                           Magazine            Magazine     => gameObject.GetComponent<Magazine>();
		protected                                         Transform           EjectionPort => gameObject.transform.Find(EJECTION_PORT_NAME);
		private                                           Transform           Opening      => gameObject.transform.Find(OPENING_NAME);

		private const string EJECTION_PORT_NAME = "Ejection Port";
		private const string OPENING_NAME       = "Opening";

		public bool CanLoad(Round Round) {
			if (Cartridge != Round.Cartridge) return false;
			if (Strength == Cartridge.Strengths.Subsonic && Round.Strength != Cartridge.Strengths.Subsonic) return false;
			if (Strength != Cartridge.Strengths.HighPower && Round.Strength == Cartridge.Strengths.HighPower) return false;
			return true;
		}

		public void Awake() {
			if (gameObject.transform.Find(EJECTION_PORT_NAME) == null) {
				var newEjectionPort = new GameObject(EJECTION_PORT_NAME);
				newEjectionPort.transform.SetParent(transform);
			}
			if (gameObject.transform.Find(OPENING_NAME) == null) {
				var newOpening = new GameObject(OPENING_NAME);
				newOpening.transform.SetParent(transform);
			}
		}
		[ResponsiveButtonGroup("1/Debug/Functions"), HideInEditorMode] protected virtual void StrikeHammer() {
			if (ThingInChamber is not Round roundInChamber) {
				Debug.LogWarning("Hammer struck on either an empty chamber or casing.");
				return;
			}
			
			Casing casing = roundInChamber.Casing;
			GetComponent<AudioSource>().PlayOneShot(GetComponent<Receiver>().FireSound);
			roundInChamber.transform.SetParent(null, true);
			Vector3 newRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			roundInChamber.transform.eulerAngles = newRotation;
			roundInChamber.GetComponent<IHammerspaceable>().ToggleHammerspace(false);
			setAngle();
			roundInChamber.StrikePrimer(GetComponent<Barrel>().velocity);
			Receiver.Barrel.PassBullet();
			ThingInChamber = casing;
			ThingInChamber.GetComponent<IHammerspaceable>().ToggleHammerspace(true);
		}
		[Button] private void setAngle() {
			//ThingInChamber.transform.Rotate(Vector3.up, );
		}
		
		[ResponsiveButtonGroup("1/Debug/Functions")] protected virtual void ChamberRound() {
			if (ThingInChamber != null) {
				Debug.LogException(new Exception("Double feed!"));
				return;
			}

			if (Magazine.Empty) return;
			GetComponent<AudioSource>().PlayOneShot(cockSound);
			Round fuckThis = Magazine.Feed();
			fuckThis.eOnFire.AddListener(() => GetComponent<Receiver>().ApplyRecoil(fuckThis));
			fuckThis.gameObject.transform.SetParent(transform);
			fuckThis.gameObject.transform.position = Opening.position;
			ThingInChamber                         = fuckThis;
		}
		[ResponsiveButtonGroup("1/Debug/Functions")] protected void Eject() {
			if (ThingInChamber == null) return;
			ThingInChamber.GetComponent<IHammerspaceable>().ToggleHammerspace(false);
			ThingInChamber.transform.position = EjectionPort.position;
			ThingInChamber                    = null;
		}
	}
}