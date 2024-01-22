using Godot;
using System;
using System.Linq;

public partial class Player : Character {

    public Godot.Collections.Array<string> bodiesHit = new Godot.Collections.Array<string>();
    public PlayerCamera camera;

    public override void _Ready() {
        InitializeBoxes();
        InitializeAnimation();
        camera = GetNode<PlayerCamera>("Camera2D");
    }

    public override void _Process(double delta) {
    }
    public override void _PhysicsProcess(double delta) {
        MoveAndSlide();
    }

    public void OnAttackBoxBodyEntered(Node2D body) {
            GD.Print(body.Name);
        if (body is Enemy enemy && !bodiesHit.Contains<string>(body.Name)) {
            bodiesHit.Add(enemy.Name);

            attackManager.preparedAttack.position = GlobalPosition;
            enemy.health.TakeDamage(attackManager.preparedAttack);

            camera.Shake();
        }
    }

    public override void Death() {
        base.Death();
    }
}