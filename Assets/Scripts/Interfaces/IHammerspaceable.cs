namespace Interfaces {
	public interface IHammerspaceable {
		public bool HammerspaceState              { get; }
		public void ToggleHammerspace(bool State) { }
	}
}