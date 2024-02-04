using Godot;
using System;

public partial class MotionManager : Node2D {

    public float pushVelocity = 0f;
    public float knockbackVelocity = 0f;
    public float attackDashVelocity = 0f;

    private Timer _knockbackTimer;
    private Timer _attackDashTimer;

    private Character _character;

    public override void _Ready() {
        _character = GetParent<Character>();

        _knockbackTimer = GetNode<Timer>("KnockbackTimer");
        _attackDashTimer = GetNode<Timer>("AttackDashTimer");
    }

    public override void _Process(double delta) {
    }

    public override void _PhysicsProcess(double delta) {

    }
    public void Knockback(Attack attack) {
        if (attack.knockbackForce > 0) {
            knockbackVelocity = Mathf.Sign(_character.GlobalPosition.X - attack.position.X) * attack.knockbackForce;
            pushVelocity += knockbackVelocity;

            _knockbackTimer.WaitTime = attack.knockbackDurationSeconds;
            _knockbackTimer.Start();
        }
    }
    public void OnKnockbackTimerTimeout() {
        pushVelocity -= knockbackVelocity;
        knockbackVelocity = 0f;
    }

    public void AttackDash(Attack attack) {
        if (attack.dashForce > 0) {
            attackDashVelocity = Mathf.Sign(_character.horizontalDirection) * attack.dashForce;
            pushVelocity += attackDashVelocity;

            _attackDashTimer.WaitTime = attack.dashDurationSeconds;
            _attackDashTimer.Start();
        }
    }
    public void OnAttackDashTimerTimeout() {
        pushVelocity -= attackDashVelocity;
        attackDashVelocity = 0f;
    }

}
