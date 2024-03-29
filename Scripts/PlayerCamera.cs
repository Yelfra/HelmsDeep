using Godot;
using System;

public partial class PlayerCamera : Camera2D {

    [Export(PropertyHint.Range, "0,1")] float decay = 0.8f;
    [Export(PropertyHint.Range, "0,1")] float maxRoll = 0.1f;
    [Export] Vector2 maxOffset = new Vector2(2, 1);

    private float _trauma = 0f; // [0, 1]
    private int _traumaExponent = 2; // 2 || 3

    //private FastNoiseLite _noise = new FastNoiseLite();
    //private int _noiseY = 0;

    public override void _Ready() {
        //_noise.Seed = GD.RandRange(0, 500);
        //_noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        //_noise.FractalOctaves = 2;
    }

    public override void _Process(double delta) {
        if (_trauma > 0f) {
            _trauma = Mathf.Max(_trauma - decay * (float)delta, 0f);
            Shake();
        }
        if (Input.IsKeyPressed(Key.P)) {
            //AddTrauma(0.1f);
            _trauma = 0.3f;
        }
    }

    private void Shake() {
        float amount = Mathf.Pow(_trauma, _traumaExponent);
        //_noiseY += 1;
        Rotation = maxRoll * amount * GD.RandRange(-1, 1); // maxRoll * amount * _noise.GetNoise2D(_noise.Seed, _noiseY);

        float maxOffsetX = maxOffset.X * amount * GD.RandRange(-1, 1); // maxOffset.X * amount * _noise.GetNoise2D(_noise.Seed * 2, _noiseY);
        float maxOffsetY = maxOffset.Y * amount * GD.RandRange(-1, 1); // maxOffset.Y * amount * _noise.GetNoise2D(_noise.Seed * 3, _noiseY);
        Offset = new Vector2(maxOffsetX, maxOffsetY);
    }

    public void AddTrauma(float amount) {
        _trauma = Mathf.Min(_trauma + amount, 1f);
    }
}