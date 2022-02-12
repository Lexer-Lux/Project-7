using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Rounds;
using Sirenix.OdinInspector;
using UnityEngine;

#nullable enable

namespace Weapons.Ranged.Magazines {
	public sealed class InternalMagazine : Magazine {
		[SerializeField, TabGroup("1", "Stats")] private int _capacity;
		public override                                  int Capacity => _capacity;

		[SerializeField, ReadOnly, TabGroup("1", "Debug")] private List<Round> _contents;
		protected override                                         List<Round> Contents => _contents;

		[TabGroup("1", "Stats")] public bool UsesClips;

		public InternalMagazine() {
			UsesClips = false;
			_contents = new List<Round>();
		}

		// TODO: Double-loader and quad-loader technique.
		public void LoadRounds(List<Round> Rounds) {
			if (Rounds.Count > 1 && !UsesClips) Debug.LogError("T-two at once?!");
			if (Ammo + Rounds.Count > Capacity)
				Debug.LogException(
					new ArgumentException($"$Onii-chan! It's too much! {Ammo} + {Rounds.Count} > {Capacity}"));

			if (Rounds.Any(round => !gun.Action.CanLoad(round))) {
				var up = new ArgumentException("Your gun a splode");
				Debug.LogException(up);
				throw up;
			}

			Contents.AddRange(Rounds);
			foreach (Round round in Rounds) {
				round.gameObject.transform.SetParent(magFeed.transform);
				round.transform.position = magFeed.transform.position;
			}
		}

		/// <summary> Should only be used when the action chambers a round. </summary>
		public override Round Feed() {
			if (Empty) Debug.LogError("When you nut but she keep suckin");
			Round firstRound = Contents.First();
			Contents.RemoveAt(0);
			return firstRound;
		}

		[HorizontalGroup("1/Debug/MaterializeRounds", Order = -1)]
		[HorizontalGroup("1/Debug/MaterializeRounds"), ShowInInspector, HideLabel]
		private Cartridge.Strengths testRoundStrength;
		[HorizontalGroup("1/Debug/MaterializeRounds"), ShowInInspector, HideLabel]
		private Cartridge testRoundCartridge;
		[HorizontalGroup("1/Debug/MaterializeRounds"), ShowInInspector, HideLabel]
		private Bullet testRoundBullet;
		[HorizontalGroup("1/Debug/MaterializeRounds"), ShowInInspector, HideLabel] private int testRoundAmount;
		[Button("Materialize!", ButtonSizes.Small), ShowIf("@testRoundAmount > 0"), HorizontalGroup("1/Debug/MaterializeRounds")]
		private void MaterializeRounds() {
			int roundsToMake = testRoundAmount;

			if (UsesClips) {
				List<Round> rounds = new List<Round>();
				while (roundsToMake > 0) {
					var newRound = generateRound();
					rounds.Add(newRound);
					roundsToMake--;
				}

				LoadRounds(rounds);
			}
			else {
				while (roundsToMake > 0) {
					var         newRound   = generateRound();
					List<Round> roundsList = new List<Round>() {newRound};
					LoadRounds(roundsList);
					roundsToMake--;
				}
			}

			Round generateRound() {
				Round      newRound    = Round.HandLoad(testRoundStrength, testRoundCartridge, testRoundBullet);
				newRound.EnterHammerspace();
				return newRound;
			}
		}
	}
}