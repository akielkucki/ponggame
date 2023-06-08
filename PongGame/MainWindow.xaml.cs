using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.Windows.Shapes.Path;

namespace PongGame
{

    public partial class MainWindow : Window
    {
        private const double PaddleSpeed = 10;
        private double ballXSpeed = 4;
        private double ballYSpeed = 2;
        private int player1Score = 0;
        private int player2Score = 0;
        private DispatcherTimer gameLoopTimer;
        private Queue<Point> ballTrailPoints = new Queue<Point>();
        private const int BallTrailLength = 10; // Adjust this value to control the trail length
        private List<Line> ballTrailLines = new List<Line>();
        private dynamic file;

        public MainWindow()
        {
            InitializeComponent();

            // Set up game loop timer
            double multiplier = 1;
            double millis = 16.666;
            gameLoopTimer = new DispatcherTimer();
            gameLoopTimer.Tick += GameLoopTimer_Tick;
            gameLoopTimer.Interval = TimeSpan.FromMilliseconds(millis / multiplier);
            gameLoopTimer.Start();
            ball.RadiusX = 10;
            ball.RadiusY = 10;
            ResizeMode = ResizeMode.NoResize;

        }

        private void GameLoopTimer_Tick(object sender, EventArgs e)
        {
            // Start control section for Player 1
            if (Keyboard.IsKeyDown(Key.W))
            {
                Canvas.SetTop(paddle1, Canvas.GetTop(paddle1) - PaddleSpeed / 1.5);
            }
            else if (Keyboard.IsKeyDown(Key.S))
            {
                Canvas.SetTop(paddle1, Canvas.GetTop(paddle1) + PaddleSpeed / 1.5);
            }
            // End control section for Player 1

            // Start control section for Player 2
            if (Keyboard.IsKeyDown(Key.Up))
            {
                Canvas.SetTop(paddle2, Canvas.GetTop(paddle2) - PaddleSpeed / 1.5);
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {
                Canvas.SetTop(paddle2, Canvas.GetTop(paddle2) + PaddleSpeed / 1.5);
            }

            // Ensure paddle2 stays within the bounds of the canvas
            if (Canvas.GetTop(paddle2) <= 0)
            {
                Canvas.SetTop(paddle2, 0);
            }
            else if (Canvas.GetTop(paddle2) >= canvas.ActualHeight - paddle2.ActualHeight)
            {
                Canvas.SetTop(paddle2, canvas.ActualHeight - paddle2.ActualHeight);
            }
            // End control section for Player 2

            // Move the ball
            Canvas.SetLeft(ball, Canvas.GetLeft(ball) + ballXSpeed);
            Canvas.SetTop(ball, Canvas.GetTop(ball) + ballYSpeed);
// Update the ball trail
            ballTrailPoints.Enqueue(new Point(Canvas.GetLeft(ball), Canvas.GetTop(ball)));
            if (ballTrailPoints.Count > BallTrailLength)
            {
                ballTrailPoints.Dequeue();
            }

            UpdateBallTrail();


            // Check for collisions with walls
            if (Canvas.GetTop(ball) < 0 || Canvas.GetTop(ball) > canvas.ActualHeight - 10 - ball.ActualHeight)
            {
                ballYSpeed = -ballYSpeed;
            }

            // Check for collisions with paddles
            if (Canvas.GetLeft(ball) < Canvas.GetLeft(paddle1) + paddle1.ActualWidth &&
                Canvas.GetTop(ball) + ball.ActualHeight > Canvas.GetTop(paddle1) &&
                Canvas.GetTop(ball) < Canvas.GetTop(paddle1) + paddle1.ActualHeight)
            {
                leftPaddleCollisionEvent();
                ballXSpeed = Math.Abs(ballXSpeed);
            }

            if (Canvas.GetLeft(ball) >= Canvas.GetLeft(paddle2) - 10 &&
                Canvas.GetTop(ball) + ball.Height >= Canvas.GetTop(paddle2) &&
                Canvas.GetTop(ball) <= Canvas.GetTop(paddle2) + paddle2.ActualHeight)
            {
                rightPaddleCollisionEvent();
                ballXSpeed = -Math.Abs(ballXSpeed);
            }


            // Check for scoring
            if (Canvas.GetLeft(ball) < 0)
            {
                player2Score++;
                UpdateScore();
                ResetBall();
            }
            else if (Canvas.GetLeft(ball) + ball.ActualWidth > canvas.ActualWidth)
            {
                player1Score++;
                UpdateScore();
                ResetBall();
            }
        }

        private void rightPaddleCollisionEvent()
        {
            try
            {
                using (SoundPlayer player =
                       new SoundPlayer("C:\\Users\\alexk\\RiderProjects\\PongGame\\PongGame\\assets\\pickupCoin.wav"))
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void leftPaddleCollisionEvent()
        {
            rightPaddleCollisionEvent();
        }


        private void UpdateScore()
        {
            scoreLabel.Text = $"{player1Score} - {player2Score}";
        }

        private void ResetBall()
        {
            Canvas.SetLeft(ball, canvas.ActualWidth / 2 - ball.ActualWidth / 2);
            Canvas.SetTop(ball, canvas.ActualHeight / 2 - ball.ActualHeight / 2);
            ballXSpeed = -ballXSpeed;
            ballYSpeed = 2;
        }

        private void UpdateBallTrail()
        {
            // Remove old trails
            foreach (var line in ballTrailLines)
            {
                canvas.Children.Remove(line);
            }

            ballTrailLines.Clear();

            if (ballTrailPoints.Count > 1)
            {
                double opacityStep = 1.0 / BallTrailLength;
                double currentOpacity = opacityStep;

                for (int i = 1; i < ballTrailPoints.Count; i++)
                {
                    Point prevPoint = ballTrailPoints.ElementAt(i - 1);
                    Point currentPoint = ballTrailPoints.ElementAt(i);

                    // Calculate the center coordinates for each point in the trail
                    double prevCenterX = prevPoint.X + ball.ActualWidth / 2;
                    double prevCenterY = prevPoint.Y + ball.ActualHeight / 2;
                    double currentCenterX = currentPoint.X + ball.ActualWidth / 2;
                    double currentCenterY = currentPoint.Y + ball.ActualHeight / 2;

                    Line line = new Line
                    {
                        // Set the line coordinates based on the center coordinates
                        X1 = prevCenterX,
                        Y1 = prevCenterY,
                        X2 = currentCenterX,
                        Y2 = currentCenterY,
                        Stroke = Brushes.White, // adjust color as needed
                        StrokeThickness = 4, // adjust thickness as needed
                        Opacity = currentOpacity
                    };

                    canvas.Children.Add(line);
                    ballTrailLines.Add(line);
                    currentOpacity += opacityStep;
                }
            }
        }
    }
}


