using Godot;

namespace Heroline;

public enum PlayerState
{
	Idle,
	Run,
	Jump,
	Down
}

public partial class Player : CharacterBody2D
{
	[Export] private int _speed = 130;
	[Export] private int _acceleration = 15;
	[Export] private int _jumpVelocity = -300;
	[Export] private int _gravity = 980;
	[Export] private float _downGravity = 1.5f;
	[Export] private int _extraJumps = 1;
	private PlayerState _currentState = PlayerState.Idle;
	private int _extraJumpsCount = 0;
	private AnimatedSprite2D _animations;
	private Timer _jumpBuffer;
	private Timer _coyoteTimer;

	public override void _Ready()
	{
		_animations = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_jumpBuffer = GetNode<Timer>("JumpBuffer");
		_coyoteTimer = GetNode<Timer>("CoyoteTimer");
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		UpdateMovement(delta);
		SetCurrentState();
		MoveAndSlide();
	}

	private void HandleInput()
	{
		if (Input.IsActionJustPressed("jump"))
		{
			_jumpBuffer.Start();
		}

		float direction = Input.GetAxis("move_left", "move_right");

		if (direction == 0)
		{
			Velocity = new Vector2(
				Mathf.MoveToward(Velocity.X, 0, _acceleration),
				Velocity.Y
			);
		}
		else
		{
			Velocity = new Vector2(
				Mathf.MoveToward(Velocity.X, direction * _speed, _acceleration),
				Velocity.Y
			);
		}
	}

	private void UpdateMovement(double delta)
	{
		if ((IsOnFloor() || _coyoteTimer.TimeLeft > 0) && _jumpBuffer.TimeLeft > 0)
		{
			Velocity = new Vector2(Velocity.X, _jumpVelocity);
			_currentState = PlayerState.Jump;
			_jumpBuffer.Stop();
			_coyoteTimer.Stop();
		}
		else if (_jumpBuffer.TimeLeft > 0 && _extraJumpsCount < _extraJumps)
		{
			Velocity = new Vector2(Velocity.X, _jumpVelocity);
			_currentState = PlayerState.Jump;
			_extraJumpsCount++;
			_jumpBuffer.Stop();
		}

		if (_currentState == PlayerState.Jump)
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * (float)delta);
		}
		else
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * _downGravity * (float)delta);
		}
	}

	private void SetAnimation()
	{
		if (Velocity.X != 0)
		{
			_animations.Scale = new Vector2(Mathf.Sign(Velocity.X), _animations.Scale.Y);
		}

		// switch (_currentState)
		// {
		// 	case PlayerState.Idle:
		// 		_animations.Play("Idle");
		// 		break;

		// 	case PlayerState.Run:
		// 		_animations.Play("Run");
		// 		break;

		// 	case PlayerState.Jump:
		// 		_animations.Play("Jump");
		// 		break;

		// 	case PlayerState.Down:
		// 		_animations.Play("Down");
		// 		break;
		// }
	}

	private void SetCurrentState()
	{
		switch (_currentState)
		{
			case PlayerState.Idle when Velocity.X != 0:
				_currentState = PlayerState.Run;
				break;

			case PlayerState.Run:
				if (Velocity.X == 0)
				{
					_currentState = PlayerState.Idle;
				}
				else if (!IsOnFloor() && Velocity.Y > 0)
				{
					_currentState = PlayerState.Down;
					_coyoteTimer.Start();
				}
				break;

			case PlayerState.Jump when Velocity.Y > 0:
				_currentState = PlayerState.Down;
				break;

			case PlayerState.Down when IsOnFloor():
				_extraJumpsCount = 0;

				if (Velocity.X == 0)
				{
					_currentState = PlayerState.Idle;
				}
				else
				{
					_currentState = PlayerState.Run;
				}
				break;
		}
	}
}
