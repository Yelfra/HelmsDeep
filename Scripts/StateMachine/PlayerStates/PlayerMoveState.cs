using Godot;
using System;

public partial class PlayerMoveState : State {

    [Export] public float walkSpeed = 75.0f;
    [Export] public float runSpeed = 150.0f;

    private bool _isRunning;

    private enum subState {
        IDLE, WALK, RUN
    }
    private subState _currentSubState = subState.IDLE;
    private float _currentSpeed;

    public override void Enter() {
        Idle();
    }
    public override void Exit() {}

    public override void Update(double delta) {
        if (Input.IsActionPressed("attack")) {
            EmitSignal(SignalName.Transitioned, this, "PlayerAttackState");
        }

        character.horizontalDirection = Input.GetAxis("move_left", "move_right");
        _isRunning = Input.IsActionPressed("run");

        if (character.horizontalDirection == 0) {
            if (_currentSubState != subState.IDLE)
                Idle();
        } else if (!_isRunning) {
            if (_currentSubState != subState.WALK)
                Walk();
        } else {
            if (_currentSubState != subState.RUN)
                Run();
        }
    }
    public override void PhysicsUpdate(double delta) {
        character.velocity.X = character.horizontalDirection * _currentSpeed + character.motionManager.pushVelocity;
        character.Velocity = character.velocity;
        character.FaceDirection(character.horizontalDirection);
    }

    private void Idle() {
        character.animationPlayer.Play("Idle");
        _currentSpeed = 0f;
        _currentSubState = subState.IDLE;
    }
    private void Walk() {
        character.animationPlayer.Play("Walk");
        _currentSpeed = walkSpeed;
        _currentSubState = subState.WALK;
    }
    private void Run() {
        character.animationPlayer.Play("Run");
        _currentSpeed = runSpeed;
        _currentSubState = subState.RUN;
    }
}
