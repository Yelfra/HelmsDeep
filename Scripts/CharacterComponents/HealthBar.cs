using Godot;
using System;

public partial class HealthBar : Area2D {
    [Export] uint _healthPointColumns = 5;
    [Export] int _healthPointColour;
    [Export] float _fallRiseDuration = 0.3f;

    public int currentLayer = 0;

    private HealthManager _health;
    private Character _character;

    private HBoxContainer _topRow;
    private int _topRowNumberOfElements;
    private HBoxContainer _bottomRow;

    private Area2D _topGhostCollider;

    private float _layerHeight;

    private Tween tween;

    public override void _Ready() {
        _health = GetParent<HealthManager>();
        _character = _health.GetParent<Character>();

        _topGhostCollider = GetNode<Area2D>("TopGhostCollider");

        _topRow = GetNode<HBoxContainer>("TopRow");
        _bottomRow = GetNode<HBoxContainer>("BottomRow");
    }

    public void InstantiateHPGrid() {
        _topRowNumberOfElements = _health.maxHealth < _healthPointColumns ? (int)_health.maxHealth : Mathf.CeilToInt((float)_health.maxHealth / 2);
       
        PackedScene healthPointScene = GD.Load<PackedScene>("res://Scenes/health_point.tscn");
        Vector2 textureSize = Vector2.Zero;

        for (int i = 0; i < _health.maxHealth; i++) {
            Control healthPoint = (Control)healthPointScene.Instantiate();

            Sprite2D healthPointSprite = healthPoint.GetNode<Sprite2D>("Sprite2D");
            healthPointSprite.Frame = _healthPointColour;

            if (i < _topRowNumberOfElements) {
                _topRow.AddChild(healthPoint);
            } else {
                _bottomRow.AddChild(healthPoint);
            }

            if (textureSize == Vector2.Zero) {
                textureSize = healthPointSprite.Texture.GetSize();
                textureSize.X /= healthPointSprite.Hframes;
                textureSize.Y /= healthPointSprite.Vframes;
            }
        }

        const int margin = 0;
        const int numberOfRows = 2;
        const int ghostHeight = 4;

        float width = (_topRowNumberOfElements * (textureSize.X - 1) + 1) + margin;
        float height = (numberOfRows * textureSize.Y - 1) + margin;

        RectangleShape2D rectangleShape = new RectangleShape2D();
        rectangleShape.Size = new Vector2(width, height);
        RectangleShape2D ghostRectangleShape = new RectangleShape2D();
        ghostRectangleShape.Size = new Vector2(width, ghostHeight);

        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        collisionShape.Shape = rectangleShape;
        CollisionShape2D topGhostCollisionShape = _topGhostCollider.GetNode<CollisionShape2D>("CollisionShape2D");
        topGhostCollisionShape.Shape = ghostRectangleShape;

        _layerHeight = 11;
    }

    public override void _PhysicsProcess(double delta) {
        while (currentLayer > 0 && (!_topGhostCollider.HasOverlappingAreas())) {
            MoveUp();
        }

        foreach (HealthBar otherHealthBar in GetOverlappingAreas()) {
            if (currentLayer != otherHealthBar.currentLayer) {
                continue;
            }
            if (_character is Enemy) {
                bool isOtherHealthBarCloserToPlayer = (GetDistanceFromPlayer() > otherHealthBar.GetDistanceFromPlayer());
                if (isOtherHealthBarCloserToPlayer) {
                    MoveDown();
                }
            }
        }
    }

    public void MoveUp() {
        if (currentLayer <= 0) {
            currentLayer = 0;
            return;
        }
        MoveToY(--currentLayer * _layerHeight);
    }
    public void MoveDown() {
        MoveToY(++currentLayer * _layerHeight);
    }
    private void MoveToY(float Y) {
        tween = CreateTween()
                    .SetEase(Tween.EaseType.Out)
                    .SetTrans(Tween.TransitionType.Expo);
        tween.TweenProperty(this, "position", new Vector2(0, Y), _fallRiseDuration);
        //tween.TweenCallback(Callable.From(() => ResolveCollision()));
    }

    public void LoseHitPoints(int damage) {
        for (int i = _health.currentHealth - 1; i > 0 && i > _health.currentHealth - damage - 1; i--) {
            if (i >= _topRowNumberOfElements) {
                _bottomRow.GetChild(i -_topRowNumberOfElements)
                    .GetNode<AnimationPlayer>("AnimationPlayer")
                    .Play("HealthPoint-Loss-" + _healthPointColour.ToString());
            } else {
                _topRow.GetChild(i)
                        .GetNode<AnimationPlayer>("AnimationPlayer")
                        .Play("HealthPoint-Loss-" + _healthPointColour.ToString());
            }
        }
    }

    public float GetDistanceFromPlayer() {
        if (_character is Enemy enemy) {
            return Mathf.Abs(enemy.player.GlobalPosition.X - GlobalPosition.X);
        }
        return 0;
    }
}