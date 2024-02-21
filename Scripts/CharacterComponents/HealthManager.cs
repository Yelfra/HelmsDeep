using Godot;
using System;

public partial class HealthManager : Node2D {
    [Export] public uint maxHealth = 10;
    public int currentHealth;

    [Export] float hurtDurationSeconds = 0.1f;
    [Export] State hurtState;

    private HealthBar _healthBar;

    public bool hurt = false;
    public bool dead = false;
    public bool invulnerable = false;

    public float invulnerableTime = 0f;

    private Character _character;
    private Timer _hurtTimer;


    public override void _Ready() {
        currentHealth = (int)maxHealth;

        _character = GetParent<Character>();
        _healthBar = GetNode<HealthBar>("HealthBar");
        if (_healthBar != null) {
            _healthBar.InstantiateHPGrid();
        }

        _hurtTimer = GetNode<Timer>("HurtTimer");
        _hurtTimer.WaitTime = hurtDurationSeconds;
    }

    public override void _Process(double delta) {
        if (invulnerable) {
            invulnerableTime += (float)delta;
        }
        if (Input.IsActionJustPressed("block") && _character is Player && currentHealth < maxHealth) {
            _healthBar.GainHitPoints(1);
            currentHealth += 1;
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
        if (_healthBar != null) {
            _healthBar.LoseHitPoints(totalDamage);
        }

        // Update Health
        currentHealth -= totalDamage;

        // Death
        if (currentHealth <= 0) {
            dead = true;
            if (_healthBar != null) {
                _healthBar.QueueFree();
            }
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
}
