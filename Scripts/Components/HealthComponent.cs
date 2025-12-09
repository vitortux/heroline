using Godot;

namespace Heroline.Scripts.Components;

public partial class HealthComponent : Node
{
	[Signal]
	public delegate void HealthChangedEventHandler(float diff);

	[Signal]
	public delegate void MaxHealthChangedEventHandler(float diff);

	[Signal]
	public delegate void HealthDepletedEventHandler();

	[Export]
	public float MaxHealth
	{
		get => _maxHealth;
		private set
		{
			if (value != _maxHealth)
			{
				float diff = value - _maxHealth;
				_maxHealth = value;
				EmitSignal(nameof(MaxHealthChanged), diff);

				if (CurrentHealth > MaxHealth)
					CurrentHealth = MaxHealth;
			}
		}
	}

	public float CurrentHealth
	{
		get => _currentHealth;
		private set
		{
			float clampedValue = Mathf.Clamp(value, 0, MaxHealth);
			float diff = clampedValue - _currentHealth;
			_currentHealth = clampedValue;
			EmitSignal(nameof(HealthChanged), diff);

			if (_currentHealth == 0)
				EmitSignal(nameof(HealthDepleted));
		}
	}

	private float _maxHealth;
	private float _currentHealth;

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		GD.Print($"Took {damage} damage. Current health: {CurrentHealth}/{MaxHealth}");
	}
}
