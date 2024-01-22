using Godot;
using System;

public partial class EnemyAttackState : State {

    public Timer attackTimer;

    public override void Enter() {
        character.attackManager.PrepareAttack("Cleave");
        character.animationPlayer.Play("Attack");

        attackTimer = GetNode<Timer>("Timer");
        attackTimer.WaitTime = character.animationPlayer.CurrentAnimationLength;
        attackTimer.Start();

        character.Velocity = Vector2.Zero;
    }
    public override void Exit() {
        character.attackBox.Monitoring = false;
        attackTimer.Stop();
    }

    public override void Update(double delta) {}
    public override void PhysicsUpdate(double delta) {}

    public void OnTimerTimeout() {
        EmitSignal(SignalName.Transitioned, this, "EnemyPursueState");
    }

    public void AttackStart() {
        character.attackBox.Monitoring = true;
    }
    public void AttackEnd() {
        character.attackBox.Monitoring = false;
    }
}
