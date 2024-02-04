using Godot;
using System;

public partial class EnemyPursueState : State {

    [Export] int nearestDistance = 25;

    private CharacterBody2D _player;

    public override void Enter() {
        character.animationPlayer.Play("Run");

        _player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
    }

    public override void Exit() {

    }

    public override void Update(double delta) {

    }
    public override void PhysicsUpdate(double delta) {
        PursuePlayer();
    }

    private void PursuePlayer() {
        character.horizontalDirection = _player.GlobalPosition.X - character.GlobalPosition.X;
        character.FaceDirection(character.horizontalDirection);

        if (Mathf.Abs(character.horizontalDirection) > nearestDistance) {
            character.velocity.X = Mathf.Sign(character.horizontalDirection) * ((Enemy)character).moveSpeed + character.motionManager.pushVelocity;
            character.Velocity = character.velocity;
        } else {
            EmitSignal(SignalName.Transitioned, this, "EnemyAttackState");
        }
    }
}
