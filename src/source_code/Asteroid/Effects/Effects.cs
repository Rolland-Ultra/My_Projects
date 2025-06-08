using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Asteroid.Effects
{
	public abstract class Effects
	{
		public Shape Geometry { get; set; }
		public int Lifetime { get; set; }

		public Effects(int lifetime) => Lifetime = lifetime;

		public Effects(Polyline geometry, int lifetime)
		{
			Geometry = geometry;
			Lifetime = lifetime;
		}

		public Effects(Point firstPoint, Point secondPoint, int lifetime, Color color) =>
			Geometry = new Polyline
			{
				Points = new PointCollection() { firstPoint, secondPoint },
				Stroke = Brushes.White,
				StrokeThickness = 3,
				Effect = new FX(color).Glow,
			};

		public abstract bool Display();
	}

	class ExplodeEffect : Effects
	{
		public double Speed { get; set; }
		public double OffsetX { get; set; }
		public double OffsetY { get; set; }

		public ExplodeEffect(Point center, Point trajectory, int lifetime, double speed, Polyline line) : base(line, lifetime)
		{
			Speed = speed;
			OffsetX = (trajectory.X - center.X) * Speed;
			OffsetY = (trajectory.Y - center.Y) * Speed;
		}

		public override bool Display()
		{
			if (Lifetime <= 0) return false;

			var translation = new TranslateTransform(OffsetX, OffsetY);
			Polyline polyline = (Polyline)Geometry;
			PointCollection pointCollection = new PointCollection();
			foreach (Point vert in polyline.Points)
				pointCollection.Add(translation.Transform(vert));
			polyline.Points = pointCollection;
			Geometry = polyline;
			Lifetime--;
			return true;
		}
	}

	public class FX
	{
		public System.Windows.Media.Effects.DropShadowEffect Glow { get; set; }

		public FX(Color color) =>
			Glow = new System.Windows.Media.Effects.DropShadowEffect()
			{
				Color = color,
				ShadowDepth = 0,
				Direction = 0,
				BlurRadius = 10
			};
	}
}