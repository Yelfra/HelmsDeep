using Godot;
using System;

public partial class PlayerCamera : Camera2D {

    [Export] private float _defaultShakeIntensity = 5f;
    [Export] private float _shakeFadeSpeed = 40f;

    private float _shakeIntensity;

    public override void _Process(double delta) {
        if (_shakeIntensity > 0) {
            _shakeIntensity = (float)Mathf.Lerp(_shakeIntensity, 0, _shakeFadeSpeed * delta);
            Offset = RandomOffset();
        }
    }

    public void Shake(float shakeFactor) {
        _shakeIntensity = _defaultShakeIntensity * shakeFactor;
    }
    public void Shake() {
        _shakeIntensity = _defaultShakeIntensity;
    }

    private Vector2 RandomOffset() {
        Vector2 offset;
        offset.X = (float)GD.RandRange(-_shakeIntensity, _shakeIntensity);
        offset.Y = (float)GD.RandRange(-_shakeIntensity, _shakeIntensity);

        return offset;
    }
}

/*
public partial class PlayerCamera : Camera2D {

    private Timer _shakeTimer;
    private float _shakePeriod = 0f;
    [Export] private float _shakeTimeSeconds = 0.1f;

    public override void _Ready() {
        _shakeTimer = GetNode<Timer>("ShakeTimer");
    }

    public override void _Process(double delta) {
        if (!_shakeTimer.IsStopped()) {
            _shakePeriod += (float)delta;

            Vector2 shakeDirection = new Vector2(Mathf.Sin(_shakePeriod) * 10, Mathf.Sin(_shakePeriod) * 20);
            Vector2 offset = Offset;
            offset.X = Mathf.Lerp(offset.X, shakeDirection.X, 0.2f);
            offset.Y = Mathf.Lerp(offset.Y, shakeDirection.Y, 0.2f);
            Offset = offset;
        } else {
            _shakePeriod = 0f;
            Offset = Vector2.Zero;
        }
    }

    public void Shake(float durationSeconds) {
        _shakeTimer.WaitTime = durationSeconds;
        _shakeTimer.Start();
    }
    public void Shake() {
        _shakeTimer.WaitTime = _shakeTimeSeconds;
        _shakeTimer.Start();
    }
}

 */