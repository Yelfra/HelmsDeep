using Godot;
using System;

public partial class HurtState : State {

    public override void Enter() {
        character.animationPlayer.Play("Hurt");

        character.velocity.X = character.health.knockbackVelocity;
        character.Velocity = character.velocity;
    }
    public override void Exit() {}

    public override void Update(double delta) {}
    public override void PhysicsUpdate(double delta) {
        if (character.health.hurt) {
            EmitSignal(SignalName.Transitioned, this, GetParent<StateMachine>().initialState.Name);
        }
    }
}
