using Godot;
using System;

public abstract partial class State : Node2D {

    public Character character;

    [Signal] public delegate void TransitionedEventHandler(State state, string stateName);
    public override void _Ready() {}

    public abstract void Enter();
    public abstract void Exit();


    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}
