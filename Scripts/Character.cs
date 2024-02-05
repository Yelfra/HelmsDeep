using Godot;
using System;

//_player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
public abstract partial class Character : CharacterBody2D {
    [Export] public HealthManager health;
    [Export] public AttackManager attackManager;
    [Export] public MotionManager motionManager;

    [Signal] public delegate void CallTransitionedEventHandler(string stateName);

    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public Vector2 velocity = Vector2.Zero;
    public float horizontalDirection = 0f;
    public int facingDirection = 1;

    public CollisionShape2D hitbox;

    public AnimationPlayer animationPlayer;
    public AnimationPlayer effectAnimationPlayer;

    public void InitializeBoxes() {
        hitbox = GetNode<CollisionShape2D>("Hitbox");
    }
    public void InitializeAnimation() {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        effectAnimationPlayer = GetNode<AnimationPlayer>("EffectAnimationPlayer");
    }

    public void FaceDirection(float direction) {
        if (direction < 0) {
            facingDirection = -1;
            GetNode<Sprite2D>("Sprite2D").FlipH = true;
        } else if (direction > 0) {
            facingDirection = 1;
            GetNode<Sprite2D>("Sprite2D").FlipH = false;
        }

        // Reposition AttackBox Left/Right
        if (attackManager != null) {
            attackManager.RepositionAttackBox(facingDirection);
        }
    }

    public virtual void Death() {
    }
}
