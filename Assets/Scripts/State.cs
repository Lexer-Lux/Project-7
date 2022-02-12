using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// FIXME: That weird error where you sometimes get multiple vision display lines at once.
public abstract class State : object {
	public readonly Enemy owner;
    public abstract Color color { get; }
    public virtual Type previousState { get; }
    public virtual Type nextState { get; }
    public State(Enemy owner = null) {
        this.owner = owner;
    }
	public virtual void Enter() {}
	public virtual void Update() {
		// * Make it so that the enemy line of sight will start again when going from Patrollling -> Searching or Searching -> Attacking, and also the reverse.
		if (previousState != null && !owner.Eyes.seeingPlayer && owner.Eyes.LOSlength == 0) {
            owner.EnterState(previousState);
            owner.Eyes.LOSlength = owner.Eyes.relativePositionOfPlayer.magnitude;
        } else if (nextState != null && owner.Eyes.seeingPlayer && owner.Eyes.playerIdentified) {
            owner.EnterState(nextState);
            owner.Eyes.LOSlength = 0;
        }
	}
	public virtual void Exit() { }
}
public class Patrolling : State {
	override public Color color { get => new Color(0.81176470588f, 0f, 0.75294117647f, 1f); }
    override public Type nextState { get => owner.states.searching; }
    public Patrolling(Enemy owner = null): base(owner) {}
}
public class Searching : State {
	override public Color color { get => new Color(0.23529411764f, 0.75686274509f, 1f, 1f); }
	override public Type previousState { get => owner.states.patrolling; }
	override public Type nextState { get => owner.states.attacking; }
    public Searching(Enemy owner = null): base(owner) {}
}
public class Attacking : State {
	override public Color color { get => new Color(1f, 0f, 0.2431372549f, 1f); }
	override public Type previousState { get => owner.states.searching; }
    public Attacking(Enemy owner = null): base(owner) {}
}
public class Watching : Patrolling { //Patrolling but for people who don't move. They just look at each patrol point instead of travelling to them.
	public Watching(Enemy owner = null): base(owner) {}
}
public class Shooting : Attacking { //Attacking, but for people who don't move.
	public Shooting(Enemy owner = null): base(owner) {}
}
