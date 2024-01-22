using Godot;
using System;

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
            Vector2 shakeOffset = Offset;
            shakeOffset.X = Mathf.Lerp(shakeOffset.X, shakeDirection.X, 0.2f);
            shakeOffset.Y = Mathf.Lerp(shakeOffset.Y, shakeDirection.Y, 0.2f);
            Offset = shakeOffset;
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
