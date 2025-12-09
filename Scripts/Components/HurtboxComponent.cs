using Godot;

namespace Heroline.Scripts.Components;

public partial class HurtboxComponent : Area2D
{
    [Signal]
    public delegate void ReceivedDamageEventHandler(float damage);

    [Export]
    public HealthComponent HealthComponent;

    public override void _Ready()
    {
        Connect("area_entered", new Callable(this, nameof(_OnAreaEntered)));
    }

    private void _OnAreaEntered(HitboxComponent hitbox)
    {
        if (hitbox == null)
            return;

        HealthComponent.TakeDamage(hitbox.Damage);
        EmitSignal(nameof(ReceivedDamage), hitbox.Damage);
    }
}
