using Godot;
using System;

//_player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
public abstract partial class Character : CharacterBody2D {
    [Export] public HealthManager health;
    [Export] public AttackManager attackManager;
    [Export] public MotionManager motionManager;

    // Player only
    [Export] public AttackHighlighter attackHighlighter;

    [Signal] public delegate void CallTransitionedEventHandler(string stateName);

    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public float horizontalDirection = 0f;
    public int facingDirection = 1;

    public CollisionShape2D bodyCollider;

    public AnimationPlayer animationPlayer;
    public AnimationPlayer effectAnimationPlayer;

    public void InitializeBodyCollider() {
        bodyCollider = GetNode<CollisionShape2D>("BodyCollider");
    }
    public void InitializeAnimation() {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        effectAnimationPlayer = GetNode<AnimationPlayer>("EffectAnimationPlayer");
    }

    public void FaceDirection(float direction) {
        if (direction < 0) {
            facingDirection = -1;
            foreach (Sprite2D sprite in GetNode<Node2D>("Sprite").GetChildren()) {
                sprite.FlipH = true;
            }
        } else if (direction > 0) {
            facingDirection = 1;
            foreach (Sprite2D sprite in GetNode<Node2D>("Sprite").GetChildren()) {
                sprite.FlipH = false;
            }
        }

        if (attackManager != null) {
            attackManager.RepositionAttackBoxes(facingDirection);
        }
        if (attackHighlighter != null) {
            attackHighlighter.RepositionRays(facingDirection);
        }
    }

    public virtual void Death() {
    }
}
