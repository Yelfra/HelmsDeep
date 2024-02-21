using Godot;
using System;

public partial class Enemy : Character {

    [Export] public float moveSpeed = 50f;

    public CpuParticles2D headParticle;
    public CharacterBody2D player;

    public override void _Ready() {
        InitializeBodyCollider();
        InitializeAnimation();
        headParticle = GetNode<CpuParticles2D>("HeadParticle");
        player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndSlide();
    }

    public override void Death() {
        EmitSignal(SignalName.CallTransitioned, "EnemyDeathState");
        base.Death();
    }
}
