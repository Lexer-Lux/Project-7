using System;
using System.Collections.Generic;
using BodyParts.Systems;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI {
	public class MyStupidConvolutedBar : MonoBehaviour {
		// TODO: Text box where the blood volume is displayed. 1 HP = 100 mL.
		[SerializeField, ShowInInspector]                          private byte             barsPerSegment = 4; // Number of bars each segment will hold.
		[SerializeField, ShowInInspector]                          private byte             valuePerBar = 3;
		[SerializeField, ShowInInspector, Required, AssetSelector] private GameObject       barPrefab;
		[SerializeField, ShowInInspector]                          private List<GameObject> bars;

		[Required, SceneObjectsOnly, ShowInInspector, HideInPrefabAssets]
		public SystemWithStat System{
			get => _system;
			set {
				//if (_system != null) { _system.eMaxValueChanged.RemoveListener(generateBars); }
				_system = value;
				if (value == null) return;
				//_system.eMaxValueChanged.AddListener(generateBars);
				generateBars();
			}
		}
		[SerializeField, HideInInspector] private SystemWithStat _system;

		[Button] private void clear() {
			// Delete all children of children named #0, #1, or #2.
			foreach (Transform child in transform) {
				if (!child.name.StartsWith("#")) continue;
				foreach (Transform grandchild in child) { Destroy(grandchild.gameObject); }
			}
		}
		[Button] private void generateBars() {
			clear();
			//TODO: If I haven't created enough # numbered gameobjects to hold all the bars, they'll overflow into the base of the hierarchy.
			// Create one bar for each MaxValue and set its parent as the correct segment. Name them #0, #1, #2, etc.
			bars = new List<GameObject>();
			for (byte i = 0; i < System.Level; i++) {
				GameObject newBar = Instantiate(barPrefab, transform.Find($"#{i / barsPerSegment}"));
				newBar.name = $"#{i}";
				bars.Add(newBar);
			}
		}
		[Button] private void updateMaxValue() {
			//Loop through each bar and get the ProgressBar component. Distribute System.Maxvalue across the ProgressBars.
			ushort temp = 0;
			for (byte i = 0; i < bars.Count; i++) {
				ProgressBar bar = bars[i].GetComponent<ProgressBar>();
				temp         += valuePerBar;
				if (temp <= System.MaxValue) {
					bar.UpdateBar01(1);
				} else {
					bar.UpdateBar01((float) (temp - System.MaxValue)/valuePerBar);
					//bar.UpdateBar01((float) (System.MaxValue - (temp - valuePerBar)) / valuePerBar);
				}

			}
			
		}

		[Button] private void updateCurrentValue() {
			//Loop through each bar and get the MMBar component. Distribute System.Value across the ProgressBars.
			ushort temp = 0;
			for (byte i = 0; i < bars.Count; i++) {
				MMProgressBar bar = bars[i].GetComponent<MMProgressBar>();
				temp += valuePerBar;
				if (temp <= System.Stat) {
					bar.UpdateBar01(1);
				} else {
					bar.UpdateBar01((float) (temp - System.MaxValue)/valuePerBar);
					//bar.UpdateBar01((float) (System.MaxValue - (temp - valuePerBar)) / valuePerBar);
				}

			}
			
		}
	}
}