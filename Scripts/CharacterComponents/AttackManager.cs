using Godot;
using System;
using System.Collections.Generic;

public partial class AttackManager : Node2D {

    public Attack preparedAttack;
    public Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();
    public Dictionary<string, Area2D> attackBoxes = new Dictionary<string, Area2D>();

    private Godot.Collections.Array<HitBox> _attackedHitBoxes = new Godot.Collections.Array<HitBox>();

    public override void _Ready() {
        foreach (Area2D attackBox in GetNode<Node2D>("AttackBoxes").GetChildren()) {
            attackBoxes[attackBox.Name] = attackBox;
        }

        attacks = new Dictionary<string, Attack>();
        foreach (Node2D child in GetChildren()) {
            if (child is Attack childAttack) {
                attacks[childAttack.Name] = childAttack;
                PrepareAttack(childAttack.Name);
            }
        }
    }

    public void PrepareAttack(string attackName) {
        preparedAttack = attacks[attackName];
    }

    public void OnAttackBoxAreaEntered(Area2D area) {
        if (area is HitBox hitBox && !_attackedHitBoxes.Contains(hitBox)) {
            _attackedHitBoxes.Add(hitBox);

            preparedAttack.position = GlobalPosition;

            if (hitBox.RelayAttack(preparedAttack)) {
                if (GetParent() is Player playerAgressor) {
                    playerAgressor.camera.AddTrauma(0.2f); // Shake(preparedAttack.damage);
                } else if (hitBox.character is Player playerVictim) {
                    playerVictim.camera.AddTrauma(0.2f); // Shake(preparedAttack.damage);
                }
            }
        }
    }

    public void StartAttack(string attackBoxName) {
        attackBoxes[attackBoxName].Monitoring = true;
    }
    public void EndAttack(string attackBoxName) {
        attackBoxes[attackBoxName].Monitoring = false;
        _attackedHitBoxes.Clear();
    }
    public void EndAttack() {
        foreach (Area2D attackBox in attackBoxes.Values) {
            attackBox.Monitoring = false;
        }
        _attackedHitBoxes.Clear();
    }

    public void RepositionAttackBoxes(float direction) {
        foreach (Area2D attackBox in attackBoxes.Values) {
            CollisionShape2D attackBoxShape = attackBox.GetNode<CollisionShape2D>("CollisionShape2D");
            Vector2 position = attackBoxShape.Position;
            attackBoxShape.Position = new Vector2(Mathf.Sign(direction) * Mathf.Abs(position.X), position.Y);
        }
    }
}
