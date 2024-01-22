using Godot;
using System;

public abstract partial class Character : CharacterBody2D {

    [Export] public HealthManager health;
    [Export] public AttackManager attackManager;

    [Signal] public delegate void CallTransitionedEventHandler(string stateName);

    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public Vector2 velocity = Vector2.Zero;
    public float horizontalDirection;

    public CollisionShape2D hitbox;

    public Area2D attackBox;
    protected CollisionShape2D attackBoxShape;

    public AnimationPlayer animationPlayer;
    public AnimationPlayer effectAnimationPlayer;

    public void InitializeBoxes() {
        hitbox = GetNode<CollisionShape2D>("Hitbox");
        attackBox = GetNode<Area2D>("AttackBox");
        attackBoxShape = attackBox.GetNode<CollisionShape2D>("CollisionShape2D");
    }
    public void InitializeAnimation() {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        effectAnimationPlayer = GetNode<AnimationPlayer>("EffectAnimationPlayer");
    }

    public void FaceDirection(float direction) {
        if (direction < 0) {
            GetNode<Sprite2D>("Sprite2D").FlipH = true;

            Vector2 position = attackBoxShape.Position;
            attackBoxShape.Position = new Vector2(-Mathf.Abs(position.X), position.Y);
        } else if (direction > 0) {
            GetNode<Sprite2D>("Sprite2D").FlipH = false;

            Vector2 position = attackBoxShape.Position;
            attackBoxShape.Position = new Vector2(Mathf.Abs(position.X), position.Y);
        }
    }

    public virtual void Death() {
    }
}
