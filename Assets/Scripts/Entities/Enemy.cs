using System;
using System.Collections;
using System.Collections.Generic;
using Body_Parts;
using Sirenix.OdinInspector;
using UnityEngine;
using static State;

// TODO: Make abstract later.
// TODO: Make into seperate AI package, with states and shit.
// TODO: Jump straight to attacking after the enemy LOS hits you when searching.
public class Enemy : Entity {
    public Eyes                 Eyes => gameObject.GetComponent<Eyes>();
    [Header("AI")]
    public State state;
    public States states = new States() { patrolling = typeof(Patrolling), searching = typeof(Searching), attacking = typeof(Attacking) };
    public class States {
        public Type patrolling;
        public Type searching;
        public Type attacking;
    } //Just holds my AI state types/classes.
        public void EnterState(Type _state) {
        if (state != null) state.Exit();
        var args = new object[] { this }; // TODO: There has to be a better way to do this.
        state = (State)Activator.CreateInstance(_state, args);
        state.Enter();
    }
    protected override void Start() {
        base.Start();
        EnterState(states.patrolling);
    }
    protected override void Update() {
        base.Update();
        state.Update();
    }
}
