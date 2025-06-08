using Asteroid.Effects;
using Asteroid.Ordnances;
using System.Windows;
using System.Windows.Media;

namespace Asteroid.Objects
{
	public class PlayerShip : GameObject
	{
		public Point ThrustStart { get; set; }
		public Point ThrustEnd { get; set; }
		public bool? IsRotating { get; set; }
		public bool IsApplyingThrust { get; set; }
		public bool IsInvincible { get; set; }
		public bool CanWarp { get; set; }
		public bool CanReappear { get; private set; }
		public bool CanFire { get; set; }
		public double Speed { get; set; }
		public int InvincibilityTimer { get; set; }
		public int DestroyedTimer { get; set; }
		public int FireCooldown { get; set; }
		public int WarpCooldown { get; private set; }
		public int Score { get; set; }
		public int Lives { get; set; }
		private int NextLife { get; set; }
		public int RoundModifier { get; set; }
		public int Health { get; set; }
		public int MaxHealth { get; set; }
		public Point RespawnLocation { get; set; }

		public PlayerShip(MainWindow parent, int thickness, int round, int lives, int health, Point center) : base(parent, center, 0, new System.Collections.Generic.List<Ordnance>(), new System.Collections.Generic.List<Effects.Effects>())
		{
			Geometry = new Shapes.Draw(Brushes.LightBlue, Brushes.Transparent, thickness).Lines;
			Geometry.Points = new Shapes.Shapes(center, true).Points;
			Geometry.Effect = new FX(Colors.LightBlue).Glow;

			Speed = 0;
			IsApplyingThrust = false;
			Trajectory = new Point(Center.X, Center.Y - .1);
			ThrustStart = new Point(Center.X - 5, Center.Y + 10);
			ThrustEnd = new Point(Center.X + 5, Center.Y + 10);
			IsRotating = null;
			Explosion = (3, Brushes.White, Colors.LightBlue);

			CanWarp = true;
			RespawnLocation = center;
			Score = 0;
			RoundModifier = round;
			IsInvincible = false;
			Lives = lives;
			NextLife = 10000;
			Health = health;
			MaxHealth = health;
		}

		public override void Move()
		{
			Thrust();
			Rotate();
			TranslateShip();

			Effects.RemoveAll(effect => { return !effect.Display() && RemoveFromDisplay(effect); });
			Ordnances.RemoveAll(ord => { return !ord.Move() && RemoveFromDisplay(ord); });

			DestroyedTimeoutCooldown();
			WeaponCooldown();
			InvincibilityCooldown();
			WarpAbilityCooldown();
		}

		public override void DestroyedTimeoutCooldown()
		{
			if (IsDestroyed)
				if (DestroyedTimer-- <= 0)
					Respawn();
		}

		private void TranslateShip()
		{
			if (OffsetX < .01 && OffsetX > -.01) OffsetX = 0;
			if (OffsetY < .01 && OffsetY > -.01) OffsetY = 0;

			if (!IsDestroyed)
			{
				var translation = TranslationPoints();
				Trajectory = translation.Transform(Trajectory);
				ThrustStart = translation.Transform(ThrustStart);
				ThrustEnd = translation.Transform(ThrustEnd);

				if (OffsetX > 5) OffsetX = 5.0;
				if (OffsetX < -5) OffsetX = -5.0;
				if (OffsetY > 5) OffsetY = 5.0;
				if (OffsetY < -5) OffsetY = -5.0;
			}

			if (OffsetX != 0 && !IsApplyingThrust)
				OffsetX += OffsetX > 0 ? -.0005 : .0005;
			if (OffsetY != 0 && !IsApplyingThrust)
				OffsetY += OffsetY > 0 ? -.0005 : .0005;
		}

		public override void TeleportObject(Point teleportOffset)
		{
			var teleport = TeleportPoints(teleportOffset);
			Trajectory = teleport.Transform(Trajectory);
			ThrustStart = teleport.Transform(ThrustStart);
			ThrustEnd = teleport.Transform(ThrustEnd);
		}

		private void WeaponCooldown() { if (!CanFire) FireCooldown--; }

		public void Fire()
		{
			if (FireCooldown <= 0) CanFire = true;

			if (!IsDestroyed && CanFire)
			{
				Ordnance ordnance = new Bullet(Colors.LightBlue, Center, Trajectory, 80);
				Ordnances.Add(ordnance);
				Parent.canvas.Children.Add(ordnance.Geometry);
				CanFire = false;
				FireCooldown = 15;
			}
		}

		private void WarpAbilityCooldown() { if (!CanWarp) WarpCooldown--; }

		public void Warp()
		{
			if (WarpCooldown <= 0) CanWarp = true;

			if(!IsDestroyed && CanWarp)
			{
				int seed = new System.Random().Next();
				System.Random r = new System.Random(seed);
				double x = r.Next((int)Parent.ActualWidth);
				double y = r.Next((int)Parent.ActualHeight);

				TeleportObject(new Point(x, y));

				CanWarp = false;
				CanReappear = true;
				WarpCooldown = 200;
			}
		}

		public void Thrust()
		{
			if (!IsDestroyed && IsApplyingThrust)
			{
				OffsetX += Trajectory.X - Center.X;
				OffsetY += Trajectory.Y - Center.Y;
			}
		}

		public void Rotate()
		{
			if (!IsDestroyed && IsRotating != null)
			{
				var rotation = RotationPoints(IsRotating == true ? -5 : 5);
				Trajectory = rotation.Transform(Trajectory);
				ThrustStart = rotation.Transform(ThrustStart);
				ThrustEnd = rotation.Transform(ThrustEnd);
			}
		}

		public void AddPoints(int value)
		{
			if (Score < 2000000000) Score += value * RoundModifier;
			NextLife -= value * RoundModifier;

			if (NextLife <= 0)
			{
				Lives++;
				NextLife = 10000 * (RoundModifier - 1);
			}
		}

		private void InvincibilityCooldown()
		{
			if (IsInvincible)
			{
				if (InvincibilityTimer <= 0)
				{
					IsInvincible = false;
					Geometry.Stroke = Brushes.LightBlue;
				}
				else if (InvincibilityTimer % 15 < 8)
					Geometry.Stroke = Brushes.White;
				else
					Geometry.Stroke = Brushes.LightBlue;

				InvincibilityTimer--;
			}
		}

		public override void Destroy()
		{
			Health--;
			if(Health <= 0)
			{
				IsDestroyed = IsInvincible = true;
				for (byte k = 0; k < 3; k++)
				{
					(int, int) i = k == 0 ? (0, 1) : k == 1 ? (1, 2) : (3, 4);
					(double, double) p = k == 0 ? (-.1, - .1) : k == 1 ? (.1, .1) : (0, .1);
					System.Windows.Shapes.Polyline segment = GenerateExplosionGeometry(i.Item1, i.Item2, Explosion);
					Effects.Effects line = new ExplodeEffect(new Point(Center.X, Center.Y), new Point(Center.X + p.Item1, Center.Y + p.Item2), 100 + k, 1, segment);
					Effects.Add(line);
					AddToDisplay(line);
				}
				Geometry.Visibility = Visibility.Hidden;
				DestroyedTimer = 150;
			}
		}

		public void Respawn()
		{
			if(Lives > 0)
			{
				Health = MaxHealth;
				Lives--;
				OffsetX = 0;
				OffsetY = 0;
				Speed = 0;
				IsApplyingThrust = false;
				CanWarp = true;
				IsDestroyed = false;

				Center = RespawnLocation;
				Trajectory = new Point(Center.X, Center.Y - .1);
				ThrustStart = new Point(Center.X - 5, Center.Y + 10);
				ThrustEnd = new Point(Center.X + 5, Center.Y + 10);
				Geometry.Points = new Shapes.Shapes(Center, true).Points;
				Geometry.Visibility = Visibility.Visible;

				IsInvincible = true;
				InvincibilityTimer = 200;
			}
		}
	}
}