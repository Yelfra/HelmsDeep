using Godot;
using System;

public partial class EnemyPursueState : State {

    [Export] int maxDistance = 25;
    [Export] int minDistance = 15;

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

        float distance = Mathf.Abs(character.horizontalDirection);

        if (distance > maxDistance) {
            float xVelocity = Mathf.Sign(character.horizontalDirection) * ((Enemy)character).moveSpeed + character.motionManager.pushVelocity;
            character.Velocity = new Vector2(xVelocity, 0f);
        } else if (distance < minDistance) {
            float xVelocity = -Mathf.Sign(character.horizontalDirection) * ((Enemy)character).moveSpeed + character.motionManager.pushVelocity;
            character.Velocity = new Vector2(xVelocity, 0f);
        } else {
            EmitSignal(SignalName.Transitioned, this, "EnemyAttackState");
        }
    }
}
