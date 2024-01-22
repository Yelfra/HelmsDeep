using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StateMachine : Node2D {

    [Export] Character character;
    [Export] public State initialState;
    public State currentState;
    private Dictionary<string, State> _states;

    public override async void _Ready() {
        await ToSignal(character, "ready");

        character.CallTransitioned += OnCallTransition;

        _states = new Dictionary<string, State>();
        foreach (Node2D child in GetChildren()) {
            if (child is State childState) {
                _states[childState.Name] = childState;
                childState.Transitioned += OnTransition;
                childState.character = character;

            }
        }

        if (initialState != null) {
            initialState.Enter();
            currentState = initialState;
        }
    }

    public override void _Process(double delta) {
        if (currentState != null) {
            currentState.Update(delta);
        }
    }
    public override void _PhysicsProcess(double delta) {
        if (currentState != null) {
            currentState.PhysicsUpdate(delta);
        }
    }

    public void OnTransition(State oldState, string newStateName) {
        if (oldState != currentState) {
            return;
        }
        State newState = _states[newStateName];
        if (newState == null) {
            return;
        }

        if (currentState != null) {
            currentState.Exit();
        }
        newState.Enter();
        currentState = newState;
    }
    public void OnCallTransition(string stateName) {
        OnTransition(currentState, stateName);
    }
}
