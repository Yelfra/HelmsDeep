using Godot;
using System;

public partial class EnemyAttackState : State {

    public override void Enter() {
        character.attackManager.PrepareAttack("Cleave");
        character.animationPlayer.Play("Attack");

        character.Velocity = Vector2.Zero;
    }
    public override void Exit() {
        character.attackManager.EndAttack();
    }

    public override void Update(double delta) {
    }
    public override void PhysicsUpdate(double delta) {
        float xVelocity = character.motionManager.pushVelocity;
        character.Velocity = new Vector2(xVelocity, 0f);
    }

    public void AttackStart() {
        character.attackManager.StartAttack();
        character.motionManager.AttackDash(character.attackManager.preparedAttack);
    }
    public void AttackEnd() {
        character.attackManager.EndAttack();
    }

    public void AttackAnimationEnd() {
        EmitSignal(SignalName.Transitioned, this, "EnemyPursueState");
    }
}
