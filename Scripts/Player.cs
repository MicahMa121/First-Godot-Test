using Godot;
//https://docs.godotengine.org/en/stable/getting_started/first_2d_game/04.creating_the_enemy.html
public partial class Player : Area2D
{
	[Export]
    private float _speed = 400;
    private float _angularSpeed = Mathf.Pi;
	private int _health = 10;

	public Vector2 ScreenSize;
    public Player()
    {
    }

    [Signal]
	public delegate void HealthChangedEventHandler(int oldValue, int newValue);
    [Signal]
	public delegate void HitEventHandler();
	public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
    }
    public override void _Process(double delta)
    {
		Vector2 velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X = 1;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X = -1;
		}
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y = -1;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y = 1;
		}
		var animation = GetNode<AnimatedSprite2D>("Animation");
		if (velocity.Length() > 0)
		{
			velocity.Normalized();
			animation.Play();
		}
		else 
		{
			animation.Stop();
		}
		if (velocity.X != 0)	
		{
			animation.Animation = "walk";
			animation.FlipV = false;
			animation.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animation.Animation = "swim";
			animation.FlipV = velocity.Y > 0;
		}
		Position += velocity * (float)delta*_speed;
		Position = new Vector2(Mathf.Clamp(Position.X, 0, ScreenSize.X),
			Mathf.Clamp(Position.Y, 0, ScreenSize.Y));
		
    }

    private void OnButtonPressed()
    {
        SetProcess(!IsProcessing());
    }
	private void OnBodyEntered()
	{
		Hide();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
	public void TakeDamage(int amount)
	{
		int oldHealth = _health;
		_health -= amount;
		EmitSignal(SignalName.HealthChanged, oldHealth, _health);
	}
}
