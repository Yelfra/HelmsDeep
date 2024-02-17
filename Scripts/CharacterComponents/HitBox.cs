using Godot;
using System;

public partial class HitBox : Area2D {

    [Export] public Character character;
    [Export] public int bonusDamage;

    public bool attackBlocked = false;

    public bool RelayAttack(Attack attack) {
        if (attackBlocked) {
            return false;
        }

        attack.bonusDamage = bonusDamage;
        
        bool isHit = character.health.TakeDamage(attack);
        return isHit;
    }
}
