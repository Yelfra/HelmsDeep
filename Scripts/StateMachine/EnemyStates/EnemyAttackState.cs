using Godot;
using System;

public partial class EnemyAttackState : State {

    private bool _attackInMotion = false;

    public override void Enter() {
        character.attackManager.PrepareAttack("Cleave");
        character.animationPlayer.Play("Attack");

        character.Velocity = Vector2.Zero;
    }
    public override void Exit() {
        AttackEnd();
    }

    public override void Update(double delta) {
    }
    public override void PhysicsUpdate(double delta) {
        float xVelocity = character.motionManager.pushVelocity;
        character.Velocity = new Vector2(xVelocity, 0f);
    }

    public void AttackStart() {
        _attackInMotion = true;
        character.attackManager.StartAttack("Top");
        character.motionManager.AttackDash(character.attackManager.preparedAttack);
    }
    public void AttackEnd() {
        if (!_attackInMotion) {
            return;
        }
        character.attackManager.EndAttack("Top");
        _attackInMotion = false;
    }

    public void AttackAnimationEnd() {
        EmitSignal(SignalName.Transitioned, this, "EnemyPursueState");
    }
}
