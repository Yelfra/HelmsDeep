using Godot;
using System;

public partial class EnemySpawner : Node2D {

    [Export] float minSpawnSeconds = 5f;
    [Export] float maxSpawnSeconds = 10f;
    [Export] int maxSpawnedAtSameTime = 5;

    public Node currentScene;
    public Timer enemySpawnTimer;

    private PackedScene _enemyScene;

    public override void _Ready() {
        currentScene = GetTree().CurrentScene;

        _enemyScene = GD.Load<PackedScene>("res://Scenes/enemy.tscn");

        enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");
        RandomizeSpawnTime();
    }

    public override void _Process(double delta) {
    }

    public void OnEnemySpawnTimerTimeout() {
        if (GetChildCount() > maxSpawnedAtSameTime) {
            return;
        }

        RandomizeSpawnTime();
        AddChild(_enemyScene.Instantiate());
    }

    private void RandomizeSpawnTime() {
        enemySpawnTimer.WaitTime = GD.Randf() % (maxSpawnSeconds - minSpawnSeconds) + minSpawnSeconds;
    }
}
