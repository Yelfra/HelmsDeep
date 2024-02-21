using Godot;
using System;

public partial class PlayerHealthBar : HealthBar {

    [Export(PropertyHint.Range, "0,1")] float decay = 0.8f;
    [Export(PropertyHint.Range, "0,1")] float maxRoll = 0.1f;
    [Export] Vector2 maxOffset = new Vector2(2, 1);

    private float _trauma = 0f; // [0, 1]
    private int _traumaExponent = 2; // 2 || 3

    private Vector2 startingPosition;
    //private FastNoiseLite _noise = new FastNoiseLite();
    //private int _noiseY = 0;

    public override void _Ready() {
        base._Ready();
        startingPosition = healthPointDisplay.Position;

        //_noise.Seed = GD.RandRange(0, 500);
        //_noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        //_noise.FractalOctaves = 2;
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta) {
        if ((float)health.currentHealth / health.maxHealth < 0.2f) {
            _trauma = 0.1f;
            Shake();
        } else {
            _trauma = 0f;
        }
    }

    private void Shake() {
        float amount = Mathf.Pow(_trauma, _traumaExponent);
        //_noiseY += 1;
        healthPointDisplay.Rotation = maxRoll * amount * GD.RandRange(-1, 1); // maxRoll * amount * _noise.GetNoise2D(_noise.Seed, _noiseY);

        float maxOffsetX = maxOffset.X * amount * GD.RandRange(-1, 1); // maxOffset.X * amount * _noise.GetNoise2D(_noise.Seed * 2, _noiseY);
        float maxOffsetY = maxOffset.Y * amount * GD.RandRange(-1, 1); // maxOffset.Y * amount * _noise.GetNoise2D(_noise.Seed * 3, _noiseY);
        healthPointDisplay.Position = startingPosition + new Vector2(maxOffsetX, maxOffsetY);
    }
}
