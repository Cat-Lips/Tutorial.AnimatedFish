using F00F;
using Godot;

namespace Fish3D;

[Tool]
public partial class Fish : Node3D
{
    [Export] public FishConfig Config { get; set => this.Set(ref field, value ?? new(), () => Config.Init(this)); }

    public override void _Ready()
        => Config ??= new();
}
