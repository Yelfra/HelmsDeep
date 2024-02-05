using Godot;
using System;

public partial class Enemy : Character {

    [Export] public float moveSpeed = 50f;
    public CpuParticles2D headParticle;

    public override void _Ready() {
        InitializeBoxes();
        InitializeAnimation();
        headParticle = GetNode<CpuParticles2D>("HeadParticle");
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndSlide();
    }

    //public void OnAttackBoxBodyEntered(Node2D body) {
    //    if (body is Player player) {
    //        attackBox.SetDeferred("Monitoring", false);

    //        attackManager.preparedAttack.position = GlobalPosition;
    //        player.health.TakeDamage(attackManager.preparedAttack);
    //        player.camera.Shake(attackManager.preparedAttack.damage);
    //    }
    //}

    public override void Death() {
        EmitSignal(SignalName.CallTransitioned, "EnemyDeathState");
        base.Death();
    }
}
