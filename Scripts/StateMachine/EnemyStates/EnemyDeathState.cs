using Godot;
using System;

public partial class EnemyDeathState : State {
    public override void Enter() {
        character.Velocity = Vector2.Zero;

        character.animationPlayer.Play("Death");
        ((Enemy)character).headParticle.Emitting = true;

        character.hitbox.SetDeferred("disabled", true);
    }
    public override void Exit() {
        character.QueueFree();
    }

    public override void Update(double delta) {
    }
    public override void PhysicsUpdate(double delta) {
    }
}
