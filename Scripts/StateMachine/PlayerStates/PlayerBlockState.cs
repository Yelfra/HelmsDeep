using Godot;
using System;

public partial class PlayerBlockState : State {

    public override void Enter() {
        character.health.invulnerable = true;
        character.animationPlayer.Play("Block");
    }
    public override void Exit() {
        character.health.invulnerable = false;
    }

    public override void Update(double delta) {
        // Release block
        if (Input.IsActionJustReleased("block")) {
            EmitSignal(SignalName.Transitioned, this, "PlayerMoveState");
        } 
        else if (Input.IsActionJustPressed("attack")) {
            EmitSignal(SignalName.Transitioned, this, "PlayerAttackState");
        }

        // Movement
        character.horizontalDirection = Input.GetAxis("move_left", "move_right");
    }
    public override void PhysicsUpdate(double delta) {
        float xVelocity = character.motionManager.pushVelocity;
        character.Velocity = new Vector2(xVelocity, 0f);
        character.FaceDirection(character.horizontalDirection);
    }
}
