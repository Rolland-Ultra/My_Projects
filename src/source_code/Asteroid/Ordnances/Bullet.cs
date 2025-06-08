using System.Windows;
using System.Windows.Media;

namespace Asteroid.Ordnances
{
	public class Bullet : Ordnance
	{
		public double Speed { get; set; }
		public double OffsetX { get; set; }
		public double OffsetY { get; set; }
		public bool AsteroidCollision { get; set; }
		public bool PlayerShipCollision { get; set; }
		public bool SaucerCollision { get; set; }

		public Bullet(Color color, Point center, Point trajectory, double speed, int lifetime = 200) : base(center, trajectory, lifetime, color)
		{
			Speed = speed;
			Geometry = GenerateGeometry(color, center);
			OffsetX = (Trajectory.X - Center.X) * Speed;
			OffsetY = (Trajectory.Y - Center.Y) * Speed;
		}

		private System.Windows.Shapes.Polyline GenerateGeometry(Color color, Point center)
		{
			return new System.Windows.Shapes.Polyline
			{
				Effect = new Effects.FX(color).Glow,
				Stroke = Brushes.White,
				Fill = Brushes.White,
				StrokeThickness = 5,
				StrokeEndLineCap = PenLineCap.Round,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeLineJoin = PenLineJoin.Round,
				Points = new PointCollection()
				{
					new Point(Center.X, Center.Y),
					new Point(Center.X - .5, Center.Y - .5),
				}
			};
		}

		public override bool UsesPointCollision() => true;

		public override bool Move()
		{
			if (Lifetime <= 0)
				return false;

			TranslateTransform translation = new TranslateTransform(OffsetX, OffsetY);
			PointCollection pointCollection = new PointCollection();
			foreach (Point vert in Geometry.Points)
				pointCollection.Add(translation.Transform(vert));
			Geometry.Points = pointCollection;
			Lifetime--;
			return true;
		}

		public override Point CollisionPoint() => Geometry.Points[0];
	}
}