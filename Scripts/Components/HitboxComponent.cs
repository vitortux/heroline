using Godot;

namespace Heroline.Scripts.Components;

public partial class HitboxComponent : Area2D
{
    [Export]
    public float Damage { set; get; }

}
