using Asteroid.Effects;
using Asteroid.Ordnances;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Asteroid.Objects
{
	public abstract class GameObject
	{
		public Point Center { get; set; }
		public Point Trajectory { get; set; }
		public double OffsetX { get; set; }
		public double OffsetY { get; set; }
		public bool RemoveFromGame { get; set; }
		public MainWindow Parent { get; set; }
		public List<Ordnance> Ordnances { get; set; }
		public List<Effects.Effects> Effects { get; set; }
		public System.Windows.Shapes.Polyline Geometry { get; set; }
		public bool IsDestroyed { get; set; }
		public int PointValue { get; set; }
		public (double Thickness, Brush LineColor, Color GlowColor) Explosion { get; set; }

		public GameObject(MainWindow parent, Point center, int pointValue, List<Ordnance> ordnances, List<Effects.Effects> effects)
		{
			Parent = parent;
			Center = center;
			PointValue = pointValue;
			Ordnances = ordnances;
			Effects = effects;
		}

		public void AddToDisplay(Effects.Effects effect) => Parent.canvas.Children.Add(effect.Geometry);
		public bool RemoveFromDisplay(Effects.Effects effect) { Parent.canvas.Children.Remove(effect.Geometry); return true; }
		public void AddToDisplay(Ordnance ordnance) => Parent.canvas.Children.Add(ordnance.Geometry);
		public bool RemoveFromDisplay(Ordnance ordnance) { Parent.canvas.Children.Remove(ordnance.Geometry); return true; }
		public abstract void Move();
		public abstract void DestroyedTimeoutCooldown();
		public abstract void Destroy();
		public System.Windows.Shapes.Polyline GenerateExplosionGeometry(int i1, int i2, (double lineThickness, Brush lineColor, Color glowColor) explosion) =>
			new System.Windows.Shapes.Polyline()
			{
				Points = new PointCollection() {
				new Point(Geometry.Points[i1].X, Geometry.Points[i1].Y),
				new Point(Geometry.Points[i2].X, Geometry.Points[i2].Y)
				},
				Stroke = explosion.lineColor,
				StrokeThickness = explosion.lineThickness,
				Effect = new FX(explosion.glowColor).Glow
			};

		public TranslateTransform TranslationPoints()
		{
			var translated = new TranslateTransform(OffsetX, OffsetY);
			var pointCollection = new PointCollection();
			foreach (Point vert in Geometry.Points)
				pointCollection.Add(translated.Transform(vert));
			Geometry.Points = pointCollection;
			Center = translated.Transform(Center);
			return translated;
		}

		public RotateTransform RotationPoints(double rotation)
		{
			var rotated = new RotateTransform(rotation, Center.X, Center.Y);
			var pointCollection = new PointCollection();
			foreach (Point vert in Geometry.Points)
				pointCollection.Add(rotated.Transform(vert));
			Geometry.Points = pointCollection;
			return rotated;
		}

		public abstract void TeleportObject(Point teleportOffset);
		public Transform TeleportPoints(Point teleportOffset)
		{
			var teleport = new TranslateTransform(teleportOffset.X, teleportOffset.Y);
			var pointCollection = new PointCollection();
			foreach (Point vert in Geometry.Points)
				pointCollection.Add(teleport.Transform(vert));
			Geometry.Points = pointCollection;
			Center = teleport.Transform(Center);
			return teleport;
		}

		public int GetPointValue() => PointValue;
	}
}