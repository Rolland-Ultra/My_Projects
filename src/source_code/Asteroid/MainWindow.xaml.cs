using Asteroid.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Asteroid
{
	public partial class MainWindow : Window
	{
		Random seed;
		public System.Collections.Generic.List<GameObject> Objects;
		PlayerShip playerShip;
		enum GameObjectTypes { Asteroid, PlayerShip, UFO };
		enum AsteroidSize { Large, Medium, Small};

		int playerHealth;
		int playerLives;
		int gameRound;
		double UFOTimer;
		private bool gameStarted;
		System.Windows.Threading.DispatcherTimer timer;

		public MainWindow()
		{
			InitializeComponent();
			Objects = new System.Collections.Generic.List<GameObject>();
			seed = new Random();
			gameStarted = false;
			gameRound = 1;
			playerLives = 3;
			playerHealth = 1;
		}
		private new void MouseEnter(object sender, MouseEventArgs e) =>
			((TextBlock)sender).Foreground = Brushes.LightGray;

		private new void MouseDown(object sender, MouseButtonEventArgs e) =>
			((TextBlock)sender).Foreground = Brushes.White;

		private new void MouseLeave(object sender, MouseEventArgs e) =>
			((TextBlock)sender).Foreground = Brushes.Gray;

		private void DisplayWindow(StackPanel window, bool isDisplayed) =>
			(window.IsEnabled, window.Visibility) = (isDisplayed, isDisplayed ? Visibility.Visible : Visibility.Hidden);

		private void Game_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Main_Menu_Game.Foreground = Brushes.White;
			DisplayWindow(Main_Menu, false);
			canvas.Visibility = Visibility.Visible;
			Cursor = Cursors.None;
			GameStart();
		}

		private void Controls_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Main_Menu_Controls.Foreground = Brushes.White;
			DisplayWindow(Main_Menu, false);
			DisplayWindow(Controls, true);
		}

		private void Controls_Back_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Controls_Back.Foreground = Brushes.White;
			DisplayWindow(Controls, false);
			DisplayWindow(Main_Menu, true);
		}

		private void Resume_MouseUp(object sender, MouseButtonEventArgs e)
		{
			DisplayWindow(Pause_Menu, false);
			canvas.Visibility = Visibility.Visible;
			Cursor = Cursors.None;
			timer.Start();
		}

		private void Quit_To_Menu_MouseUp(object sender, MouseButtonEventArgs e)
		{
			gameStarted = false;
			DisplayWindow(Pause_Menu, false);
			DisplayWindow(Main_Menu, true);
			canvas.Children.RemoveRange(0, canvas.Children.Count);
			GameHUD.Visibility = Visibility.Hidden;
		}

		private void Quit_MouseUp(object sender, MouseButtonEventArgs e) => Close();

		private void GameStart()
		{
			gameStarted = true;
			Objects = new System.Collections.Generic.List<GameObject>();
			SpawnObject(GameObjectTypes.Asteroid);
			SpawnObject(GameObjectTypes.PlayerShip);
			UFOTimer = 2000;
			double x = canvas.ActualWidth / 2;
			System.Windows.Shapes.Polyline HUD = new System.Windows.Shapes.Polyline
			{
				Points = new PointCollection() {
					new Point(x-300,0),
					new Point(x-310,15),
					new Point(x-170,15),
					new Point(x-160,0),
					new Point(x-180,30),
					new Point(x-100,30),
					new Point(x-80,0),
					new Point(x-120,60),
					new Point(x+120,60),
					new Point(x+80,0),
					new Point(x+100,30),
					new Point(x+180,30),
					new Point(x+160,0),
					new Point(x+170,15),
					new Point(x+310,15),
					new Point(x+300,0),
				},
				Stroke = Brushes.LightGray,
				Fill = Brushes.Transparent,
				Effect = new Effects.FX(Colors.LightGray).Glow,
				StrokeThickness = 5
			};

			canvas.Children.Add(HUD);
			Lives.Text = playerShip.Lives.ToString();
			Score.Text = playerShip.Score.ToString();
			GameHUD.IsEnabled = true;
			GameHUD.Visibility = Visibility.Visible;

			timer = new System.Windows.Threading.DispatcherTimer();
			timer.Tick += GameTick;
			timer.Interval = new TimeSpan(100000);
			timer.Start();
		}

		private void GameTick(object sender, EventArgs e)
		{
			MoveElements();
			CollisionTest();
			Lives.Text = playerShip.Lives.ToString();
			Score.Text = playerShip.Score.ToString();
			CheckForRoundClear();
			SpawnUFO();
			if (playerShip.Lives <= 0) EndGame();
		}

		private void SpawnObject(GameObjectTypes gameObjectType)
		{
			GameObject spawnedObject;
			switch (gameObjectType)
			{
				case GameObjectTypes.PlayerShip:
					spawnedObject = new PlayerShip(this, 3, gameRound, playerLives, playerHealth, new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2));
					playerShip = (PlayerShip)spawnedObject;
					break;
				case GameObjectTypes.UFO:
					spawnedObject = new UFO(this, 3, 500, new Point(canvas.ActualWidth, new Random(seed.Next()).Next(20, (int)canvas.ActualHeight - 50)), 20, playerShip);
					break;
				case GameObjectTypes.Asteroid:
				default:
					spawnedObject = new Objects.Asteroid(this, 20, 5, new Point(seed.Next(0, (int)canvas.ActualWidth), seed.Next(0, (int)canvas.ActualHeight)), "Large", seed.Next(0, 3), seed.Next());
					break;
			}
			Objects.Add(spawnedObject);
			canvas.Children.Add(spawnedObject.Geometry);
		}

		private void SpawnUFO()
		{
			if (UFOTimer > 0) UFOTimer--;
			else
			{
				SpawnObject(GameObjectTypes.UFO);
				UFOTimer = 2000 / gameRound;
			}
		}

		private bool FindCollisionWithDetails(GameObject gameObject, System.Windows.Shapes.Shape shape)
		{
			IntersectionDetail collides = gameObject.Geometry.RenderedGeometry.FillContainsWithDetail(shape.RenderedGeometry, 2, ToleranceType.Relative);
			return collides != IntersectionDetail.Empty && collides != IntersectionDetail.NotCalculated;
		}

		private bool FindCollision(GameObject gameObject, Ordnances.Ordnance ordnance) =>
			gameObject.Geometry.RenderedGeometry.FillContains(ordnance.CollisionPoint()) || FindCollisionWithDetails(gameObject, ordnance.Geometry);


		private void CollisionTest()
		{
			var objectsCopy = new System.Collections.Generic.List<GameObject>(Objects);
			foreach (GameObject gameObject in objectsCopy)
				if (!(gameObject is PlayerShip))
				{
					Rect boundingBox = gameObject.Geometry.RenderedGeometry.Bounds;
					boundingBox.Inflate(10, 10);
					bool skipPlayerHitTest = false;
					foreach (Ordnances.Ordnance playerOrdnance in playerShip.Ordnances)
						if (!gameObject.IsDestroyed && playerOrdnance.UsesPointCollision() && FindCollision(gameObject, playerOrdnance))
						{
							skipPlayerHitTest = true;
							gameObject.Destroy();
							playerShip.AddPoints(gameObject.GetPointValue());
							playerOrdnance.Lifetime = 0;
						}
					if (!skipPlayerHitTest && !playerShip.IsInvincible && !playerShip.IsDestroyed)
					{
						foreach (Ordnances.Ordnance enemyOrdnance in gameObject.Ordnances)
							if (enemyOrdnance.UsesPointCollision() && FindCollision(playerShip, enemyOrdnance))
							{
								playerShip.Destroy();
								enemyOrdnance.Lifetime = 0;
							}
						if (!playerShip.IsDestroyed && !gameObject.IsDestroyed && boundingBox.IntersectsWith(playerShip.Geometry.RenderedGeometry.Bounds) && FindCollisionWithDetails(gameObject, playerShip.Geometry))
							playerShip.Destroy();
					}
				}
		}

		private void MoveElements()
		{
			foreach (GameObject _object in Objects)
			{
				_object.Move();
				LoopBorders(_object);
			}
			Objects.RemoveAll(obj => obj.RemoveFromGame == true);
		}

		private void LoopBorders(GameObject gameObject)
		{
			double x = 0, y = 0;
			bool loopBorders = false;

			if (gameObject.Center.X > canvas.ActualWidth + 5)
			{
				x = -(canvas.ActualWidth + 10);
				loopBorders = true;
			}
			else if (gameObject.Center.X < -5)
			{
				x = canvas.ActualWidth + 10;
				loopBorders = true;
			}
			if (gameObject.Center.Y > canvas.ActualHeight + 5)
			{
				y = -(canvas.ActualHeight + 10);
				loopBorders = true;
			}
			else if (gameObject.Center.Y < -5)
			{
				y = canvas.ActualHeight + 10;
				loopBorders = true;
			}
			if (loopBorders) gameObject.TeleportObject(new Point(x, y));
		}

		private void CheckForRoundClear()
		{
			if (Objects.TrueForAll(x => !(x is Objects.Asteroid) && !(x is UFO)))
			{
				if (int.Parse(Score.Text) < 40000)
					gameRound++;
				playerShip.RoundModifier++;
				playerShip.IsInvincible = true;
				playerShip.InvincibilityTimer = 200;
				for (int i = 0; i < gameRound; i++)
					SpawnObject(GameObjectTypes.Asteroid);
			}
		}

		private void EndGame()
		{
			timer.Stop();
			Cursor = Cursors.Cross;
			gameStarted = false;
			canvas.Children.RemoveRange(0, canvas.Children.Count);
			GameHUD.Visibility = Visibility.Hidden;
			DisplayWindow(Main_Menu, true);
		}

		private void canvas_KeyDown(object sender, KeyEventArgs e)
		{
			if(gameStarted)
			{
				if (e.Key == Key.W) playerShip.IsApplyingThrust = true;
				if (e.Key == Key.A) playerShip.IsRotating = true;
				if (e.Key == Key.D) playerShip.IsRotating = false;
			}
		}

		private void canvas_KeyUp(object sender, KeyEventArgs e)
		{
			if (gameStarted)
			{
				switch (e.Key)
				{
					case Key.W:
						playerShip.IsApplyingThrust = false;
						playerShip.Speed = 0;
						break;
					case Key.A:
					case Key.D:
						playerShip.IsRotating = null;
						break;
					case Key.Space:
						playerShip.Fire();
						break;
					case Key.S:
						playerShip.Warp();
						break;
					case Key.Escape:
						timer.Stop();
						Pause_Menu.IsEnabled = true;
						Pause_Menu.Visibility = Visibility.Visible;
						Cursor = Cursors.Cross;
						canvas.Visibility = Visibility.Hidden;
						break;
				}
			}
		}
	}
}