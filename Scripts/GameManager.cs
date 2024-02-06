using Godot;
using System;

public partial class GameManager : Node2D {

    public Node currentScene;

    public override void _Ready() {
        currentScene = GetTree().CurrentScene;

        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
    
}
