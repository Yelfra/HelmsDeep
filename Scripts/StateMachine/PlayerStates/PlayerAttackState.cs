using Godot;
using System;
using System.Linq;

public partial class PlayerAttackState : State {

    [Export] float moveSpeed = 25f;
    [Export] private float _heavyChargeDurationSeconds = 1f;
    [Export] private float _chargeDurationSeconds = 0.3f;
    [Export] AttackManager attackManager;

    private float _chargeTime = 0f;

    private bool _attackFacingCamera = true; // Switching between facing/not facing camera.
    //private bool _attackQueued = false; // Attack to be executed after the current attack ends.
    private bool _attackInMotion = false; // Current attack is in motion.
    private bool _chargingAttack = false; // Attack is currently being charged.

    public string currentAttackBoxName = "Middle";
    private string _previousAttackBoxName = "Middle";

    private string _animationAttackHead1Charge = "AttackHead-1-Charge";
    private string _animationAttackHead1 = "AttackHead-1";
    private string _animationAttackHead2Charge = "AttackHead-2-Charge";
    private string _animationAttackHead2 = "AttackHead-2";

    private string _animationAttackTorso1Charge = "AttackTorso-1-Charge";
    private string _animationAttackTorso1 = "AttackTorso-1";

    private string _animationAttackLegs1Charge = "AttackLegs-1-Charge";
    private string _animationAttackLegs1 = "AttackLegs-1";
    private string _animationAttackLegs2Charge = "AttackLegs-2-Charge";
    private string _animationAttackLegs2 = "AttackLegs-2";

    public override void Enter() {
        _chargingAttack = true;
        PlayChargeAttackAnimation();
    }
    public override void Exit() {
        AttackEnd();
        _chargingAttack = false;
        _chargeTime = 0f;
    }

    public override void Update(double delta) {
        // Aim Attack
        if (!_attackInMotion) {
            if (Input.IsActionPressed("aim_up")) {
                currentAttackBoxName = "Top";
            } else if (Input.IsActionPressed("aim_down")) {
                currentAttackBoxName = "Bottom";
            } else {
                currentAttackBoxName = "Middle";
            }
        }

        // Release Attack
        if (Input.IsActionJustReleased("attack") && _chargingAttack) {
            LaunchAttack();
        }
        // Press Attack
        if (Input.IsActionPressed("attack") && !_attackInMotion) {
            _chargeTime += (float)delta;

            bool attackBoxChanged = (currentAttackBoxName != _previousAttackBoxName);
            if (!_chargingAttack || attackBoxChanged) {
                _chargingAttack = true;
                _previousAttackBoxName = currentAttackBoxName;

                PlayChargeAttackAnimation();
            }
            if (_chargeTime >= _heavyChargeDurationSeconds) { // Executed during heavy charge.
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

    private void QuickAttack() {
        //GD.Print("Quick Attack!");
        attackManager.PrepareAttack("Quick");
    }
    private void ChargedAttack() {
        //GD.Print("Charged Attack!");
        attackManager.PrepareAttack("Charged");
    }
    private void HeavyChargedAttack() {
        //GD.Print("Heavy Charged Attack!");
        attackManager.PrepareAttack("HeavyCharged");
    }
    private void LaunchAttack() {
        if (_chargeTime >= _heavyChargeDurationSeconds) {
            HeavyChargedAttack();
        } else if (_chargeTime >= _chargeDurationSeconds) {
            ChargedAttack();
        } else {
            QuickAttack();
        }
        _chargeTime = 0f;

        PlayAttackAnimation();

        character.motionManager.AttackDash(attackManager.preparedAttack);

        _attackFacingCamera = !_attackFacingCamera;
        _attackInMotion = true;
        _chargingAttack = false;
    }

    public void AttackStart() {
        attackManager.StartAttack(currentAttackBoxName);
    }
    public void AttackEnd() {
        if (!_attackInMotion) {
            return;
        }
        attackManager.EndAttack(_previousAttackBoxName);
        _attackInMotion = false;
    }

    public void AttackAnimationEnd() {
        EmitSignal(SignalName.Transitioned, this, "PlayerMoveState"); // On animation end, exit attack state.
    }

    private void PlayAttackAnimation() {
        string attackAnimation = "";

        switch (currentAttackBoxName) {
            case "Top": {
                if (_attackFacingCamera) {
                    attackAnimation = _animationAttackHead1;
                } else {
                    attackAnimation = _animationAttackHead2;
                }
                break;
            }
            case "Bottom": {
                if (_attackFacingCamera) {
                    attackAnimation = _animationAttackLegs1;
                } else {
                    attackAnimation = _animationAttackLegs2;
                }
                break;
            }
            default: {
                //if (_attackFacingCamera) {
                attackAnimation = _animationAttackTorso1;
                //} else {
                //attackAnimation = "Attack-2-Swing";
                //}
                break;
            }
        }

        character.animationPlayer.Play(attackAnimation);
    }
    private void PlayChargeAttackAnimation() {
        string attackAnimation = "";

        switch (currentAttackBoxName) {
            case "Top": {
                if (_attackFacingCamera) {
                    attackAnimation = _animationAttackHead1Charge;
                } else {
                    attackAnimation = _animationAttackHead2Charge;
                }
                break;
            }
            case "Bottom": {
                if (_attackFacingCamera) {
                    attackAnimation = _animationAttackLegs1Charge;
                } else {
                    attackAnimation = _animationAttackLegs2Charge;
                }
                break;
            }
            default: {
                //if (_attackFacingCamera) {
                attackAnimation = _animationAttackTorso1Charge;
                //} else {
                //attackAnimation = "Attack-2-Swing";
                //}
                break;
            }
        }

        character.animationPlayer.Play(attackAnimation);
    }
}
