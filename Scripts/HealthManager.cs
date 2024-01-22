using Godot;
using System;

public partial class HealthManager : Node2D {
    [Export] public uint maxHealth = 10;
    public int currentHealth;

    [Export] float hurtDurationSeconds = 0.1f;
    [Export] State hurtState;
    [Export] private uint _healthPointColumns = 5;

    public bool hurt = false;
    public bool dead = false;
    public float knockbackVelocity = 0f;

    private Character _character;
    private Timer _hurtTimer;
    private Timer _knockbackTimer;
    private GridContainer _healthPointGrid;

    private Godot.Collections.Array<Control> _healthPoints = new Godot.Collections.Array<Control>();

    public override void _Ready() {
        currentHealth = (int)maxHealth;

        _character = GetParent<Character>();

        InstantiateHPGrid();
        
        _hurtTimer = GetNode<Timer>("HurtTimer");
        _hurtTimer.WaitTime = hurtDurationSeconds;

        _knockbackTimer = GetNode<Timer>("KnockbackTimer");
    }

    public override void _Process(double delta) {
    }
    public override void _PhysicsProcess(double delta) {
        // Status effects - Bleeding, on fire etc.
    }

    public void TakeDamage(Attack attack) {
        // Cannot take damage during hurt time.
        if (hurt || dead) {
            return;
        }
        // Hurt Flash
        _character.effectAnimationPlayer.Play("HurtFlash");
        // Camera Shake (for player)
        if (_character is Player player)
            player.camera.Shake(player.health.hurtDurationSeconds);

        // Update HealthManager
        for (int i = currentHealth - 1; i > 0 && i > currentHealth - attack.damage - 1; i--) {
            _healthPoints[i].GetNode<AnimationPlayer>("AnimationPlayer").Play("HealthPoint-Loss");
        }
        currentHealth -= attack.damage;

        // Death
        if (currentHealth <= 0) {
            dead = true;
            _healthPointGrid.QueueFree();
            _character.Death();
            return;
        }

        // Apply Knockback
        if (attack.knockbackForce > 0) {
            knockbackVelocity = Mathf.Sign(this.GlobalPosition.X - attack.position.X) * attack.knockbackForce;

            _knockbackTimer.WaitTime = attack.knockbackDurationSeconds;
            _knockbackTimer.Start();
        }

        // If character can be hurt, enter hurt state.
        if (hurtDurationSeconds > 0f) {
            _hurtTimer.Start();
            hurt = true;
            if (hurtState != null)
                _character.EmitSignal(Character.SignalName.CallTransitioned, hurtState.Name);
        }
    }

    public void OnHurtTimerTimeout() {
        hurt = false;
    }
    public void OnKnockbackTimerTimeout() {
        knockbackVelocity = 0f;
    }

    private void InstantiateHPGrid() {
        _healthPointGrid = GetNode<GridContainer>("HealthPointGrid");
        _healthPointGrid.Columns = maxHealth < _healthPointColumns ? (int)maxHealth : (int)_healthPointColumns;

        PackedScene healthPointScene = GD.Load<PackedScene>("res://Scenes/health_point.tscn");

        for (int i = 0; i < maxHealth; i++) {
            Control healthPoint = (Control)healthPointScene.Instantiate();
            _healthPointGrid.AddChild(healthPoint);
            _healthPoints.Add(healthPoint);
        }
    }
}
