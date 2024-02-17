using Godot;
using System;

public partial class HealthManager : Node2D {
    [Export] public uint maxHealth = 10;
    public int currentHealth;

    [Export] float hurtDurationSeconds = 0.1f;
    [Export] private uint _healthPointColumns = 5;
    [Export] State hurtState;

    public bool hurt = false;
    public bool dead = false;
    public bool invulnerable = false;

    public float invulnerableTime = 0f;

    private Character _character;
    private Timer _hurtTimer;
    private GridContainer _healthPointGrid;

    private Godot.Collections.Array<Control> _healthPoints = new Godot.Collections.Array<Control>();

    public override void _Ready() {
        currentHealth = (int)maxHealth;

        _character = GetParent<Character>();

        InstantiateHPGrid();

        _hurtTimer = GetNode<Timer>("HurtTimer");
        _hurtTimer.WaitTime = hurtDurationSeconds;
    }

    public override void _Process(double delta) {
        if (invulnerable) {
            invulnerableTime += (float)delta;
        }
    }

    public override void _PhysicsProcess(double delta) {
        // Status effects - Bleeding, on fire etc.
    }

    public bool TakeDamage(Attack attack) {
        // Cannot take damage during hurt time.
        if (hurt || dead || invulnerable) {
            return false;
        }

        // Hurt Flash
        _character.effectAnimationPlayer.Play("HurtFlash");

        // Sum total damage
        int totalDamage = attack.damage + attack.bonusDamage;

        // Update Health GUI
        for (int i = currentHealth - 1; i > 0 && i > currentHealth - totalDamage - 1; i--) {
            _healthPoints[i].GetNode<AnimationPlayer>("AnimationPlayer").Play("HealthPoint-Loss");
        }

        // Update Health
        currentHealth -= totalDamage;

        // Death
        if (currentHealth <= 0) {
            dead = true;
            _healthPointGrid.QueueFree();
            _character.Death();
            return true;
        }

        // Apply Knockback
        if (_character.motionManager != null) {
            _character.motionManager.Knockback(attack);
        }

        // If character can be hurt, enter hurt state.
        if (hurtDurationSeconds > 0f) {
            _hurtTimer.Start();
            hurt = true;
            if (hurtState != null)
                _character.EmitSignal(Character.SignalName.CallTransitioned, hurtState.Name);
        }
        return true;
    }

    public void OnHurtTimerTimeout() {
        hurt = false;
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
