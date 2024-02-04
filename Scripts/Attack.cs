using Godot;
using System;

public partial class Attack : Node2D {

    [Export] public int damage = 1;
    [Export] public float knockbackForce = 100f;
    [Export] public float knockbackDurationSeconds = 0.1f;
    //[Export] public float stunDurationSeconds = 1f;
    [Export] public float dashForce = 50f;
    [Export] public float dashDurationSeconds = 0.1f;

    public Vector2 position;

}

