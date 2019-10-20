using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace Snake
{
    

    public partial class MainWindow : Window
    {
        private const int snakeSize = 20;
        private int respawnSnakeX = 0;
        private int respawnSnakeY = 0;
        private int respawnFoodX = 0;
        private int respawnFoodY = 0;
        private const int gameAreaBorderThickness = 3;
        private const int mainWindowHeight = 400;
        private const int mainWindowWidth = 550;
        private const int gameAreaWidth = 400;
        private const int gameAreaHeight = mainWindowHeight-40;
        private int score = 0;
        private Random random = new Random();
        private int pause = 150;
        private Label scoreLabel = new Label();
        private Label highScoreLabel = new Label();

        private object sync = new object();
        private bool gameStarted = false;
        private int highScore = 0;

       

        private Ellipse food = new Ellipse();
        private List<SnakePart> snake = new List<SnakePart>();
        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Width = mainWindowWidth;
            mainWindow.Height = mainWindowHeight;
            gameArea.Width = gameAreaWidth;
            gameArea.Height = gameAreaHeight;

            food.Width = snakeSize;
            food.Height = snakeSize;
            food.Fill = new SolidColorBrush(Colors.Red);
            Rectangle snakeHead = new Rectangle();
            snakeHead.Height = snakeSize;
            snakeHead.Width = snakeSize;
            snakeHead.Fill = new SolidColorBrush(Colors.Black);
            snakeHead.Stroke = Brushes.Yellow;
            snakeHead.StrokeThickness = 2;
            snake.Add(new SnakePart(snakeHead, Direction.NONE));
            gameArea.Children.Add(food);
            gameArea.Children.Add(snake[0].Rect);
            scoreLabel.FontFamily = new FontFamily("Arial");
            scoreLabel.FontSize = 20;
            highScoreLabel.FontFamily = new FontFamily("Arial");
            highScoreLabel.FontSize = 20;

            myBase.Children.Add(scoreLabel);
            myBase.Children.Add(highScoreLabel);

            PrepareToStart();

            Canvas.SetTop(scoreLabel, 10);
            Canvas.SetLeft(scoreLabel, Canvas.GetLeft(gameArea) + gameArea.Width + 20);
            Canvas.SetTop(highScoreLabel, Canvas.GetTop(scoreLabel) + 50);
            Canvas.SetLeft(highScoreLabel, Canvas.GetLeft(scoreLabel));

        }

        private void PrepareToStart()
        {
            respawnFoodX = random.Next(0, (gameAreaWidth - 1) / snakeSize) * snakeSize;
            respawnFoodY = random.Next(0, (gameAreaHeight - 1) / snakeSize) * snakeSize;
            do
            {
                respawnSnakeX = random.Next(0, (gameAreaWidth - 1) / snakeSize) * snakeSize;
                respawnSnakeY = random.Next(0, (gameAreaHeight - 1) / snakeSize) * snakeSize;
            }
            while (respawnSnakeX == respawnFoodX && respawnSnakeY == respawnFoodY);

            
            Canvas.SetLeft(snake[0].Rect, respawnSnakeX);
            Canvas.SetTop(snake[0].Rect, respawnSnakeY);
            Canvas.SetLeft(food, respawnFoodX);
            Canvas.SetTop(food, respawnFoodY);
            Canvas.SetLeft(gameArea, gameAreaBorderThickness);
            Canvas.SetTop(gameArea, gameAreaBorderThickness);            
            scoreLabel.Content = "Score: \n" + 0;
            score = 0;
            highScoreLabel.Content = "Highscore: \n" + highScore;

            if (snake.Count > 1)
            {
                for (int i = 1; i < snake.Count; i++)
                {
                    gameArea.Children.Remove(snake[i].Rect);
                    
                }
                while (snake.Count != 1)
                {
                    snake.Remove(snake[snake.Count-1]);
                }

            }      
            
        }

        private bool CheckForStopGame()
        {
            if (Canvas.GetLeft(snake[0].Rect) < 0 || Canvas.GetLeft(snake[0].Rect) >= gameAreaWidth || Canvas.GetTop(snake[0].Rect) < 0 || Canvas.GetTop(snake[0].Rect) >= gameAreaHeight)
            {
                gameTickTimer.Stop();
                return true;
            }

            if (snake.Count > 1)
            {
                for (int i = 1; i < snake.Count; i++)
                {
                    if (Canvas.GetLeft(snake[0].Rect) == Canvas.GetLeft(snake[i].Rect) && Canvas.GetTop(snake[0].Rect) == Canvas.GetTop(snake[i].Rect))
                    {
                        gameTickTimer.Stop();
                        return true;
                    }
                }
            }

            if (((gameAreaWidth*gameAreaWidth) / Math.Pow(snakeSize, 2)) == snake.Count)
            {
                gameTickTimer.Stop();
                return true;
            }


            return false;
        }
        private void DrawSnake()
        {
            foreach (SnakePart snakePart in snake)
                switch (snakePart.Direct)
                {
                    case (Direction.LEFT):
                        Canvas.SetLeft(snakePart.Rect, Canvas.GetLeft(snakePart.Rect) - snakeSize);
                        break;
                    case (Direction.RIGHT):
                        Canvas.SetLeft(snakePart.Rect, Canvas.GetLeft(snakePart.Rect) + snakeSize);
                        break;
                    case (Direction.UP):
                        Canvas.SetTop(snakePart.Rect, Canvas.GetTop(snakePart.Rect) - snakeSize);
                        break;
                    case (Direction.DOWN):
                        Canvas.SetTop(snakePart.Rect, Canvas.GetTop(snakePart.Rect) + snakeSize);
                        break;
                    default:
                        gameTickTimer.Stop();
                        break;
                }
        }
        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            gameStarted = !CheckForStopGame();
            if (!gameStarted) return;
            DrawSnake();

            for (int i = snake.Count - 1; i > 0; i--)
            {
                snake[i].Direct = snake[i - 1].Direct;
            }

            if (Canvas.GetLeft(snake[0].Rect) == Canvas.GetLeft(food) && Canvas.GetTop(snake[0].Rect) == Canvas.GetTop(food))
            {
                score++;
                scoreLabel.Content = "Score: \n" + score;


                Rectangle rect = new Rectangle();
                rect.Height = snakeSize;
                rect.Width = snakeSize;
                rect.Fill = new SolidColorBrush(Colors.Black);
                int x = 0;
                int y = 0;

                switch (snake[snake.Count - 1].Direct)
                {
                    case (Direction.LEFT):
                        x = (int)Canvas.GetLeft(snake[snake.Count - 1].Rect) + snakeSize;
                        y = (int)Canvas.GetTop(snake[snake.Count - 1].Rect);
                        break;
                    case (Direction.RIGHT):
                        x = (int)Canvas.GetLeft(snake[snake.Count - 1].Rect) - snakeSize;
                        y = (int)Canvas.GetTop(snake[snake.Count - 1].Rect);
                        break;
                    case (Direction.UP):
                        x = (int)Canvas.GetLeft(snake[snake.Count - 1].Rect);
                        y = (int)Canvas.GetTop(snake[snake.Count - 1].Rect) + snakeSize;
                        break;
                    case (Direction.DOWN):
                        x = (int)Canvas.GetLeft(snake[snake.Count - 1].Rect);
                        y = (int)Canvas.GetTop(snake[snake.Count - 1].Rect) - snakeSize;
                        break;
                    default:
                        break;
                }
                rect.Stroke = Brushes.Yellow;
                rect.StrokeThickness = 2;
                snake.Add(new SnakePart(rect, snake[snake.Count - 1].Direct));
                gameArea.Children.Add(snake[snake.Count - 1].Rect);

                Canvas.SetLeft(snake[snake.Count - 1].Rect, x);
                Canvas.SetTop(snake[snake.Count - 1].Rect, y);
                snake[snake.Count - 1].Rect.UpdateLayout();
                    
                bool foodInFreeSpace = false;
                while (!foodInFreeSpace)
                {
                    foodInFreeSpace = true;
                    respawnFoodX = random.Next(0, (gameAreaWidth - 1) / snakeSize) * snakeSize;
                    respawnFoodY = random.Next(0, (gameAreaHeight - 1) / snakeSize) * snakeSize;
                    for (int i = 0; i < snake.Count; i++)
                    {
                        if (Canvas.GetLeft(snake[i].Rect) == respawnFoodX && Canvas.GetTop(snake[i].Rect) == respawnFoodY)
                        {
                            foodInFreeSpace = false;
                            break;
                        }
                    }
                }

                Canvas.SetLeft(food, respawnFoodX);
                Canvas.SetTop(food, respawnFoodY);
                    
            }
            
        }
        private void RectKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                gameTickTimer.Stop();
                gameStarted = false;
                if (score > highScore) highScore = score;
                PrepareToStart();
            }            
            if (!gameStarted && (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Right || e.Key == Key.Left))
            {
                gameTickTimer.Start();
                gameStarted = true;
            }
            if (gameStarted)
            {
                if (snake.Count == 1)
                {
                    switch (e.Key)
                    {
                        case Key.Up:
                            snake[0].Direct = Direction.UP;
                            break;
                        case Key.Down:
                            snake[0].Direct = Direction.DOWN;
                            break;
                        case Key.Right:
                            snake[0].Direct = Direction.RIGHT;
                            break;
                        case Key.Left:
                            snake[0].Direct = Direction.LEFT;
                            break;
                    }
                }
                else
                {
                    if ((e.Key == Key.Up || e.Key == Key.Down) && (snake[0].Direct == Direction.LEFT || snake[0].Direct == Direction.RIGHT))
                    {
                        snake[0].Direct = (e.Key == Key.Up) ? Direction.UP : Direction.DOWN;
                    }
                    else if ((e.Key == Key.Right || e.Key == Key.Left) && (snake[0].Direct == Direction.UP || snake[0].Direct == Direction.DOWN))
                    {
                        snake[0].Direct = (e.Key == Key.Right) ? Direction.RIGHT : Direction.LEFT;
                    }
                }
            }
            
        }
        protected override void OnContentRendered(EventArgs e)
        {
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(pause);
            //gameTickTimer.IsEnabled = true;
            gameTickTimer.Tick += GameTickTimer_Tick;
        }
        
    }

}
