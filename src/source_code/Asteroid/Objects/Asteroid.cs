using Asteroid.Effects;
using System.Windows;
using System.Windows.Media;

namespace Asteroid.Objects
{
	public class Asteroid : GameObject
	{
		System.Random Seed { get; set; }
		public double Rotation { get; set; }
		public string Size { get; set; }
		public int Thickness { get; set; }
		public int DestroyedTimer { get; set; }

		public Asteroid(MainWindow parent, int pointValue, int thickness, Point center, string size, int type, int randomSeed) : base(parent, center, pointValue, new System.Collections.Generic.List<Ordnances.Ordnance>(), new System.Collections.Generic.List<Effects.Effects>())
		{
			Geometry = new Shapes.Draw(Brushes.SlateGray, Brushes.Black, thickness).Lines;
			Geometry.Points = new Shapes.Shapes(Center, null, size + type).Points;

			Size = size;
			Thickness = thickness;
			Explosion = (.5, Brushes.LightBlue, Colors.LightBlue);

			Seed = new System.Random(randomSeed);
			Point trajectory = new Point(Center.X + 1, Center.Y - 1);
			trajectory = new RotateTransform(Seed.Next(360), Center.X, Center.Y).Transform(trajectory);
			OffsetX = (trajectory.X - Center.X) * Seed.Next(4, 10) / 5;
			OffsetY = (trajectory.Y - Center.Y) * Seed.Next(4, 10) / 5;
			Rotation = Seed.Next(-1, 2) / Seed.Next(1, 5);
		}

		public override void Move()
		{
			if(!IsDestroyed)
			{
				RotationPoints(Rotation);
				TranslationPoints();
			}

			Effects.RemoveAll(effect => { return !effect.Display() && RemoveFromDisplay(effect); });
			DestroyedTimeoutCooldown();
		}

		public override void TeleportObject(Point teleportOffset) => TeleportPoints(teleportOffset);

		public override void DestroyedTimeoutCooldown()
		{
			if (IsDestroyed)
				if (DestroyedTimer-- <= 0)
					RemoveFromGame = true;
		}

		public override void Destroy()
		{
			IsDestroyed = true;
			for(int i = 0; i < Geometry.Points.Count - 1; i++)
			{
				System.Windows.Shapes.Polyline segment = GenerateExplosionGeometry(i, i + 1, Explosion);
				Effects.Effects line = new ExplodeEffect(new Point(Center.X, Center.Y), Geometry.Points[i], 15 + i, .01, segment);
				Effects.Add(line);
				AddToDisplay(line);
			}
			Geometry.Visibility = Visibility.Hidden;
			DestroyedTimer = 200;

			Seed = new System.Random();
			if (Size != "Small")
				for (int i = Seed.Next(0, 2); i < 4; i++)
				{
					int type = Seed.Next(0, 3);
					Asteroid asteroid = new Asteroid(Parent, Size == "Large" ? 50 : 100, Thickness - 1, new Point(Center.X + 20, Center.Y - 20), Size == "Large" ? "Medium" : "Small", type,
						type == 0 ? Seed.Next() : type == 1 ? Seed.Next(48000) * Seed.Next(48000) : Seed.Next(2000000000) / Seed.Next(1, 2000000000));
					Parent.Objects.Add(asteroid);
					Parent.canvas.Children.Add(asteroid.Geometry);
				}
		}
	}
}