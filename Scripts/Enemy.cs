using Godot;
using System;

public partial class Enemy : Character {

    [Export] public float moveSpeed = 50f;
    public CpuParticles2D headParticle;

    public override void _Ready() {
        InitializeBoxes();
        InitializeAnimation();
        headParticle = GetNode<CpuParticles2D>("HeadParticle");
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndSlide();
    }

    public override void Death() {
        EmitSignal(SignalName.CallTransitioned, "EnemyDeathState");
        base.Death();
    }
}
