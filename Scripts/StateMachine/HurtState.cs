using Godot;
using System;

public partial class HurtState : State {

    public override void Enter() {
        character.animationPlayer.Play("Hurt");

        //character.velocity.X = character.motionManager.pushVelocity;
        character.Velocity = new Vector2(character.motionManager.pushVelocity, 0f);
    }
    public override void Exit() {}

    public override void Update(double delta) {}
    public override void PhysicsUpdate(double delta) {
        if (character.health.hurt) {
            EmitSignal(SignalName.Transitioned, this, GetParent<StateMachine>().initialState.Name);
        }
    }
}
