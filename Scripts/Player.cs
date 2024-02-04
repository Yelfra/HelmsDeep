using Godot;
using System;
using System.Linq;

public partial class Player : Character {

    public Godot.Collections.Array<Enemy> bodiesHit = new Godot.Collections.Array<Enemy>();
    public PlayerCamera camera;

    public override void _Ready() {
        InitializeBoxes();
        InitializeAnimation();
        camera = GetNode<PlayerCamera>("Camera2D");
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndSlide();
    }

    public void OnAttackBoxBodyEntered(Node2D body) {
        if (body is Enemy enemy && !bodiesHit.Contains(body)) {
            bodiesHit.Add(enemy);

            attackManager.preparedAttack.position = GlobalPosition;
            enemy.health.TakeDamage(attackManager.preparedAttack);

            camera.Shake(attackManager.preparedAttack.damage);
        }
    }

    public override void Death() {
        base.Death();
    }
}