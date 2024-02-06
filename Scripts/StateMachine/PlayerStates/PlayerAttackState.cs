using Godot;
using System;

public partial class PlayerAttackState : State {

    [Export] float moveSpeed = 25f;
    [Export] private float _chargeFillTimeSeconds = 1f;
    [Export] private float _chargeStartTimeSeconds = 0.2f;

    private float _chargeTime = 0f;

    private bool _attackSwitch = false; // Switching between first and second attack animation.
    //private bool _attackQueued = false; // Attack to be executed after the current attack ends.
    private bool _attackInMotion = false; // Current attack is in motion.
    private bool _chargingAttack = false; // Attack is currently being charged.

    public override void _Ready() {
    }

    public override void Enter() {
    }
    public override void Exit() {
        character.attackManager.EndAttack();
        _attackInMotion = false;
        _chargingAttack = false;
        _chargeTime = 0f;
    }

    public override void Update(double delta) {
        // Release Attack
        if (Input.IsActionJustReleased("attack") && _chargingAttack) {
            _chargingAttack = false;
            if (_chargeTime >= _chargeStartTimeSeconds) {
                ChargedAttack();
            } else {
                QuickAttack();
            }
        }
        // Press Attack
        if (Input.IsActionPressed("attack") && !_attackInMotion) {
            _chargeTime += (float)delta;
            if (!_chargingAttack) { // Executed only once per charge.
                _chargingAttack = true;
                PlayChargeAttackAnimation();
            }
            if (_chargeTime >= _chargeFillTimeSeconds) { // Executed during charge.
                //((Player)character).camera.Shake(0.05f);
            }
        }

        // Block
        if (Input.IsActionJustPressed("block")) {
            EmitSignal(SignalName.Transitioned, this, "PlayerBlockState");
        }

        // Movement
        character.horizontalDirection = Input.GetAxis("move_left", "move_right");
    }
    public override void PhysicsUpdate(double delta) {
        float pushVelocity = character.motionManager.pushVelocity;
        float xVelocity = _chargingAttack ? pushVelocity : character.horizontalDirection * moveSpeed + pushVelocity;
        character.Velocity = new Vector2(xVelocity, 0f);

        if (!_attackInMotion && character.horizontalDirection != 0) { // Cannot change direction during swing.
            character.FaceDirection(character.horizontalDirection);

            if (!_chargingAttack) { // Moving once the attack is over transitions to move state.
                EmitSignal(SignalName.Transitioned, this, "PlayerMoveState");
            }
        }
    }

    private void ChargedAttack() {
        if (_chargeTime >= _chargeFillTimeSeconds) {
            //GD.Print("Heavy Charged Attack!");
            character.attackManager.PrepareAttack("HeavyCharged");
        } else {
            //GD.Print("Charged Attack!");
            character.attackManager.PrepareAttack("Charged");
        }
        _chargeTime = 0f;
        DefaultAttack();
    }
    private void QuickAttack() {
        //GD.Print("Quick Attack!");
        character.attackManager.PrepareAttack("Quick");
        DefaultAttack();
    }
    private void DefaultAttack() {
        PlayAttackAnimation();

        character.motionManager.AttackDash(character.attackManager.preparedAttack);

        _attackSwitch = !_attackSwitch;
        _attackInMotion = true;
    }

    public void AttackStart() {
        character.attackManager.StartAttack();
    }
    public void AttackEnd() {
        character.attackManager.EndAttack();
        _attackInMotion = false;
    }

    public void AttackAnimationEnd() {
        EmitSignal(SignalName.Transitioned, this, "PlayerMoveState"); // On animation end, exit attack state.
    }

    private void PlayAttackAnimation() {
        if (_attackSwitch) {
            character.animationPlayer.Play("Attack-1-Swing");
        } else {
            character.animationPlayer.Play("Attack-2-Swing");
        }
    }
    private void PlayChargeAttackAnimation() {
        if (_attackSwitch) {
            character.animationPlayer.Play("Attack-1-Hold");
        } else {
            character.animationPlayer.Play("Attack-2-Hold");
        }
    }
}
