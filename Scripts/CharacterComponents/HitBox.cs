using Godot;
using System;

public partial class HitBox : Area2D {

    [Export] public Character character;
    [Export] Sprite2D bodyPart;
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

    public void Highlight() {
        try {
            ((ShaderMaterial)bodyPart.Material).SetShaderParameter("flash_intensity", 0.3);
        } catch (ObjectDisposedException) {
            return;
        }
    }
    public void UnHighlight() {
        try {
            ((ShaderMaterial)bodyPart.Material).SetShaderParameter("flash_intensity", 0);
        } catch (ObjectDisposedException) {
            return;
        }
    }
}
