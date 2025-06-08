using System.Windows;
using System.Windows.Media;

namespace Asteroid.Shapes
{
	class Draw
	{
		public System.Windows.Shapes.Polyline Lines { get; set; }

		public Draw(Brush stroke, Brush fill, int thickness)
		{
			Lines = new System.Windows.Shapes.Polyline()
			{
				Stroke = stroke,
				Fill = fill,
				StrokeThickness = thickness,
				StrokeLineJoin = PenLineJoin.Round,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round
			};
		}
	}

	class Shapes
	{
		public PointCollection Points { get; set; }

		public Shapes(Point center, bool? isPlayerSpaceship, string AsteroidType = null)
		{
			double x = center.X, y = center.Y;

			if (isPlayerSpaceship == true)
				Points = new PointCollection()
				{
					new Point(x-10,y+10),
					new Point(x,y-20),
					new Point(x+10,y+10),
					new Point(x+8,y+5),
					new Point(x-8,y+5),
					new Point(x-10,y+10)
				};
			else if (isPlayerSpaceship == false)
				Points = new PointCollection()
				{
					new Point(x+30,y),
					new Point(x-30,y),
					new Point(x-20,y+10),
					new Point(x+20,y+10),
					new Point(x+30,y),
					new Point(x+20,y-10),
					new Point(x-10,y-10),
					new Point(x-5,y-20),
					new Point(x+5,y-20),
					new Point(x+10,y-10),
					new Point(x-20,y-10),
					new Point(x-30,y)
				};
			else
				switch (AsteroidType)
				{
					case "Large0":
						Points = new PointCollection()
						{
							new Point(x-100,y),
							new Point(x-60,y-40),
							new Point(x-60,y-80),
							new Point(x,y-100),
							new Point(x+20,y-80),
							new Point(x+80,y-60),
							new Point(x+100,y),
							new Point(x+60,y+40),
							new Point(x+60,y+80),
							new Point(x,y+100),
							new Point(x-60,y+80),
							new Point(x-100,y)
						};
						break;
					case "Large1":
						Points = new PointCollection()
						{
							new Point(x-100,y),
							new Point(x-60,y-20),
							new Point(x-80,y-60),
							new Point(x,y-100),
							new Point(x+80,y-80),
							new Point(x+100,y),
							new Point(x+60,y+80),
							new Point(x,y+100),
							new Point(x-80,y+80),
							new Point(x-80,y+20),
							new Point(x-100,y)
						};
						break;
					case "Large2":
						Points = new PointCollection()
						{
							new Point(x+100,y),
							new Point(x+100,y+40),
							new Point(x+60,y+80),
							new Point(x,y+100),
							new Point(x-80,y+80),
							new Point(x-100,y+20),
							new Point(x-100,y-40),
							new Point(x-60,y-80),
							new Point(x,y-100),
							new Point(x+40,y-40),
							new Point(x+100,y)
						};
						break;
					case "Medium0":
						Points = new PointCollection()
						{
							new Point(x-40,y),
							new Point(x-20,y-20),
							new Point(x-20,y-40),
							new Point(x+20,y-20),
							new Point(x+40,y+20),
							new Point(x,y+40),
							new Point(x-40,y)
						};
						break;
					case "Medium1":
						Points = new PointCollection()
						{
							new Point(x+40,y),
							new Point(x,y+40),
							new Point(x-40,y+20),
							new Point(x-20,y-20),
							new Point(x,y-20),
							new Point(x,y-40),
							new Point(x+40,y)
						};
						break;
					case "Medium2":
						Points = new PointCollection()
						{
							new Point(x+40,y),
							new Point(x+20,y+40),
							new Point(x-20,y+40),
							new Point(x-40,y-20),
							new Point(x+20,y-40),
							new Point(x+40,y)
						};
						break;
					case "Small0":
						Points = new PointCollection()
						{
							new Point(x-10,y),
							new Point(x-10,y-10),
							new Point(x,y-20),
							new Point(x+20,y-10),
							new Point(x+10,y+10),
							new Point(x,y+20),
							new Point(x-20,y+10),
							new Point(x-10,y)
						};
						break;
					case "Small1":
						Points = new PointCollection()
						{
							new Point(x-20,y),
							new Point(x-20,y-10),
							new Point(x,y-20),
							new Point(x+20,y),
							new Point(x+10,y+20),
							new Point(x-10,y+10),
							new Point(x-20,y)
						};
						break;
					case "Small2":
						Points = new PointCollection()
						{
							new Point(x-20,y),
							new Point(x,y-20),
							new Point(x+20,y),
							new Point(x+10,y+10),
							new Point(x+10,y+20),
							new Point(x-10,y+10),
							new Point(x-20,y)
						};
						break;
					default: goto case "Large0";
			}
		}
	}
}