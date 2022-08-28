using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Rounds;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Collections;
using UnityEngine;

#nullable enable

namespace Weapons.Ranged.Magazines {
	public sealed class DetachableMagazine : MonoBehaviour {
		[ShowInInspector,
		 ProgressBar(0, "Capacity", Segmented = true, ColorGetter = "AmmoColor", DrawValueLabel = true)]
		public int Ammo => Contents.Count;
		public int Capacity;
		// ReSharper disable once UnusedMember.Local
		private Color AmmoColor => Contents.FirstOrDefault()?.Color ?? Color.gray;
		[SerializeField, Sirenix.OdinInspector.ReadOnly] private List<Round> _contents;
		public List<Round> Contents => _contents;
		public Rigidbody RigidBody => gameObject.GetComponent<Rigidbody>();
		public Collider Collider => gameObject.GetComponent<Collider>();

		public DetachableMagazine() { _contents = new List<Round>(); }

		public void LoadRounds(List<Round> Rounds) {
			if (Ammo + Rounds.Count > Capacity) {
				Debug.LogException(
					new ArgumentException($"$Onii-chan! It's too much! {Ammo} + {Rounds.Count} > {Capacity}"));
				return;
			}

			_contents.AddRange(Rounds);
			foreach (Round round in Rounds) { round.gameObject.transform.SetParent(gameObject.transform); }
		}

		public Round Feed() {
			if (Contents.First() == null) {
				var up = new NullReferenceException();
				Debug.LogException(up);
				throw up;
			}

			Round firstRound = Contents.First();
			Contents.RemoveAt(0);
			return firstRound;
		}

		[HorizontalGroup("MaterializeRounds")] private Cartridge.Strengths  testRoundStrength;
		[HorizontalGroup("MaterializeRounds")] private Cartridge testRoundCartridge;
		[HorizontalGroup("MaterializeRounds")] private Bullet           testRoundBullet;
		[HorizontalGroup("MaterializeRounds")] private int              testRoundAmount;
		[Button, ShowIf("@testRoundAmount > 0"), HorizontalGroup("MaterializeRounds")] private void MaterializeRounds() {
			int         roundsToMake = testRoundAmount;
			List<Round> rounds       = new List<Round>();
			while (roundsToMake > 0) {
				Round      newRound    = Round.HandLoad(testRoundStrength, testRoundCartridge, testRoundBullet);
				rounds.Add(newRound);
				newRound.ToggleHammerspace(true);
				roundsToMake--;
			}

			LoadRounds(rounds);
		}
	}
}