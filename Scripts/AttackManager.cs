using Godot;
using System;
using System.Collections.Generic;

public partial class AttackManager : Node2D {

    public Attack preparedAttack;
    public Dictionary<string, Attack> attacks;

    public override void _Ready() {
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
}
