using Godot;
using System;
using System.Linq;

public partial class Player : Character {

    //public Godot.Collections.Array<Enemy> _attackedHitBoxes = new Godot.Collections.Array<Enemy>();
    public PlayerCamera camera;

    public override void _Ready() {
        InitializeBodyCollider();
        InitializeAnimation();
        camera = GetNode<PlayerCamera>("Camera2D");
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndSlide();
    }

    public override void Death() {
        base.Death();
    }
}