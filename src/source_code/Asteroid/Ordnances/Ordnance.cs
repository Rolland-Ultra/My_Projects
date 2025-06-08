using System.Windows;

namespace Asteroid.Ordnances
{
	public abstract class Ordnance
	{
		public Point Center { get; set; }
		public Point Trajectory { get; set; }
		public System.Windows.Shapes.Polyline Geometry { get; set; }
		public System.Windows.Media.Color OrdnanceColor { get; set; }
		public int Lifetime { get; set; }
		public int GridSquare { get; set; }

		public Ordnance(Point center, Point trajectory, int lifetime, System.Windows.Media.Color color)
		{
			Center = center;
			Trajectory = trajectory;
			Lifetime = lifetime;
			OrdnanceColor = color;
		}

		public abstract bool UsesPointCollision();
		public abstract bool Move();
		public abstract Point CollisionPoint();
	}
}