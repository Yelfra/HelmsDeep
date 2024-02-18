using Godot;
using System;

public partial class AttackHighlighter : Node2D {

    [Export] PlayerAttackState attackState;

    private HitBox _currentHitBox;
    private HitBox _previousHitBox;

    Godot.Collections.Array<RayCast2D> highlightRays = new Godot.Collections.Array<RayCast2D>();

    public override void _Ready() {
        foreach (RayCast2D highlightRay in GetChildren()) {
            highlightRays.Add(highlightRay);
        }
    }

    public override void _Process(double delta) {
        RayCast2D highlightRay = GetNode<RayCast2D>(attackState.currentAttackBoxName.ToString());
        if (highlightRay == null) {
            return;
        }

        _currentHitBox = (HitBox)highlightRay.GetCollider();

        bool hitBoxExited = (_currentHitBox == null && _previousHitBox != null);
        if (hitBoxExited) {
            _previousHitBox.UnHighlight();
        }
        bool hitBoxDifferent = (_previousHitBox != null && _previousHitBox != _currentHitBox);
        if (hitBoxDifferent) {
            _previousHitBox.UnHighlight();
        }

        if (_currentHitBox != null) {
            if (Input.IsActionPressed("attack")) {
                _currentHitBox.Highlight();
            } else {
                _currentHitBox.UnHighlight();
            }

            _previousHitBox = _currentHitBox;
        }
    }

    public void RepositionRays(float direction) {
        foreach (RayCast2D highlightRay in highlightRays) {
            Vector2 targetPosition = highlightRay.TargetPosition;
            highlightRay.TargetPosition = new Vector2(Mathf.Sign(direction) * Mathf.Abs(targetPosition.X), targetPosition.Y);
        }
    }
}
