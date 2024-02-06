using Godot;
using System;
using System.Collections.Generic;

public partial class AttackManager : Node2D {

    public Attack preparedAttack;
    public Dictionary<string, Attack> attacks;

    private Godot.Collections.Array<Character> _bodiesHit = new Godot.Collections.Array<Character>();

    private Area2D _attackBox;
    private CollisionShape2D _attackBoxShape;

    public override void _Ready() {
        _attackBox = GetNode<Area2D>("AttackBox");
        _attackBoxShape = _attackBox.GetNode<CollisionShape2D>("CollisionShape2D");

        attacks = new Dictionary<string, Attack>();
        foreach (Node2D child in GetChildren()) {
            if (child is Attack childAttack) {
                attacks[childAttack.Name] = childAttack;
            }
        }
    }

    public void PrepareAttack(string attackName) {
        preparedAttack = attacks[attackName];
    }

    public void OnAttackBoxBodyEntered(Node2D body) {
        if (body is Character character && !_bodiesHit.Contains(character)) {
            _bodiesHit.Add(character);

            preparedAttack.position = GlobalPosition;
            if (character.health.TakeDamage(preparedAttack)) {
                if (GetParent() is Player playerAgressor) {
                    playerAgressor.camera.Shake(preparedAttack.damage);
                } else if (character is Player playerVictim) {
                    playerVictim.camera.Shake(preparedAttack.damage);
                }
            }
        }
    }

    public void StartAttack() {
        _attackBox.Monitoring = true;
    }
    public void EndAttack() {
        _attackBox.Monitoring = false;
        _bodiesHit.Clear();
    }

    public void RepositionAttackBox(float direction) {
        Vector2 position = _attackBoxShape.Position;
        _attackBoxShape.Position = new Vector2(Mathf.Sign(direction) * Mathf.Abs(position.X), position.Y);
    }
}
