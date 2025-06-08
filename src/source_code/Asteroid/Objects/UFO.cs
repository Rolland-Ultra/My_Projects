using Asteroid.Effects;
using Asteroid.Ordnances;
using System.Windows;
using System.Windows.Media;

namespace Asteroid.Objects
{
	public class UFO : GameObject
	{
		public PlayerShip Enemy { get; set; }
		public bool CanFire { get; set; }
		public double Speed { get; set; }
		public int DestroyedTimer { get; set; }
		public int FireCooldown { get; set; }

		public UFO(MainWindow parent, int thickness, int pointValue, Point center, double speed, PlayerShip player) : base(parent, center, pointValue, new System.Collections.Generic.List<Ordnance>(), new System.Collections.Generic.List<Effects.Effects>())
		{
			Geometry = new Shapes.Draw(Brushes.Purple, Brushes.Transparent, thickness).Lines;
			Geometry.Points = new Shapes.Shapes(center, false).Points;
			Geometry.Effect = new FX(Colors.MediumPurple).Glow;
			Explosion = (3, Brushes.White, Colors.MediumPurple);

			Enemy = player;
			CanFire = true;
			Speed = speed;
		}

		public override void Move()
		{
			Fire();
			TranslateShip();

			Effects.RemoveAll(effect => { return !effect.Display() && RemoveFromDisplay(effect); });
			Ordnances.RemoveAll(ordnance => { return !ordnance.Move() && RemoveFromDisplay(ordnance); });

			DestroyedTimeoutCooldown();
			WeaponCooldown();
		}

		public override void DestroyedTimeoutCooldown()
		{
			if (IsDestroyed)
				if (DestroyedTimer-- <= 0)
					RemoveFromGame = true;
		}

		private void TranslateShip()
		{ 
			if (!IsDestroyed)
				if (Center.X > -50)
				{
					OffsetX = -.1 * Speed;
					OffsetY = 0;
					TranslationPoints();
				}
				else
					Destroy();
		}

		public override void TeleportObject(Point teleportOffset) => TeleportPoints(teleportOffset);

		private void Fire()
		{
			if (FireCooldown <= 0) CanFire = true;
			if (!IsDestroyed && CanFire)
			{
				Ordnance ordnance = new Bullet(Colors.MediumPurple, new Point(Center.X, Center.Y + 10), new Point(Enemy.Center.X, Enemy.Center.Y), .01);
				Ordnances.Add(ordnance);
				AddToDisplay(ordnance);

				CanFire = false;
				FireCooldown = 60;
			}
		}

		private void WeaponCooldown() { if (!CanFire) FireCooldown--; }

		public override void Destroy()
		{
			IsDestroyed = true;
			for(int i = 0; i < Geometry.Points.Count - 1; i++)
			{
				System.Windows.Shapes.Polyline segment = GenerateExplosionGeometry(i, i + 1, Explosion);
				Effects.Effects line = new ExplodeEffect(new Point(Center.X, Center.Y), Geometry.Points[i], 50 + i, .01, segment);
				Effects.Add(line);
				AddToDisplay(line);
			}
			Geometry.Visibility = Visibility.Hidden;
			DestroyedTimer = 200;
		}
	}
}