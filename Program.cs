using System.Media;

abstract public class GameConfig
{
    public const int gameField = 9;
    public const int gameSpeed = 300;
    public const string WAV_PATH = "../../assets/bipka.wav";
}

public class Snake : GameConfig
{
    public char dir { get; set; } = 'R';
    public Segment[] segments { get; set; }
    public Food food { get; set; } = new Food();
    public bool dead { get; set; } = false;

    public Snake(int input_x, int input_y)
    {
        segments = new Segment[3];
        for (int i = 0; i < 3; i++)
        {
            segments[i] = new Segment { x = input_x - (i + 1), y = input_y };
        }
    }
    public void Move()
    {
        for (int i = segments.Length - 1; i > 0; i--)
        {
            segments[i].x = segments[i - 1].x;
            segments[i].y = segments[i - 1].y;
        }
        switch (dir)
        {
            case 'R':
                segments[0].x++;
                break;
            case 'L':
                segments[0].x--;
                break;
            case 'U':
                segments[0].y--;
                break;
            case 'D':
                segments[0].y++;
                break;
        }
    }
    public void KeyPress(Screen screen)
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D:
                    if (dir != 'L') dir = 'R';
                    break;
                case ConsoleKey.A:
                    if (dir != 'R') dir = 'L';
                    break;
                case ConsoleKey.W:
                    if (dir != 'D') dir = 'U';
                    break;
                case ConsoleKey.S:
                    if (dir != 'U') dir = 'D';
                    break;
            }
        }
    }
    public void CheckCollision()
    {
        if (segments[0].x == food.x && segments[0].y == food.y)
        {
            Segment[] newSegments = new Segment[segments.Length + 1];
            for (int i = 0; i < segments.Length; i++)
            {
                newSegments[i] = segments[i];
            }
            newSegments[segments.Length] = new Segment { x = segments[segments.Length - 1].x, y = segments[segments.Length - 1].y };
            segments = newSegments;

            food.GenerateFood(segments);
        }
        if (segments[0].x < 0 || segments[0].x >= gameField || segments[0].y < 0 || segments[0].y >= gameField)
        {
            dead = true;
            Console.WriteLine("Game Over! You hit the wall.");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }
        for (int i = 1; i < segments.Length; i++)
        {
            if (segments[0].x == segments[i].x && segments[0].y == segments[i].y)
            {
                dead = true;
                Console.WriteLine("Game Over! You hit yourself.");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
        }
    }
    public class Segment
    {
        public int x { get; set; }
        public int y { get; set; }
    }
    public class Food : GameConfig
    {
        SoundPlayer player = new SoundPlayer(WAV_PATH);
        public int x { get; set; }
        public int y { get; set; }

        public void GenerateFood(Segment[] segments)
        {
            player.Play();
            Random rand = new Random();
            x = rand.Next(0, gameField);
            y = rand.Next(0, gameField);

            for (int i = 0; i < segments.Length; i++)
            {
                if (x == segments[i].x && y == segments[i].y)
                {
                    GenerateFood(segments);
                    return;
                }
            }
        }

    }

}
public class Screen : GameConfig
{
    char[,] matrix = new char[25, 25];
    int size = 25;

    public Screen()
    {
        int size = GameConfig.gameField;
        char[,] input_matrix = new char[size, size];
        matrix = input_matrix;
        this.size = size;
    }

    public void UpdateScreen(Snake s)
    {

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = ' ';
            }
        }
        matrix[s.segments[0].y, s.segments[0].x] = 'X';
        matrix[s.food.y, s.food.x] = 'F';
        for (int i = 1; i < s.segments.Length; i++)
        {
            matrix[s.segments[i].y, s.segments[i].x] = 'O';
        }
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < (size * 2) + 2; i++)
        {
            Console.Write("#");
        }
        Console.WriteLine();
        for (int i = 0; i < size; i++)
        {
            Console.Write("#");
            for (int j = 0; j < size; j++)
            {
                Console.Write("." + matrix[i, j]);
            }
            Console.Write("#");
            Console.WriteLine();
        }
        for (int i = 0; i < (size * 2) + 2; i++)
        {
            Console.Write("#");
        }
        Console.WriteLine("\n" + s.segments.Length);
    }
}
public class Program : GameConfig
{
    public static void Main(string[] args)
    {
        Screen screen = new Screen();
        Snake snake = new Snake(3, 0);
        snake.food.GenerateFood(snake.segments);
        while (true)
        {
            snake.KeyPress(screen);
            snake.Move();
            snake.CheckCollision();
            screen.UpdateScreen(snake);
            Thread.Sleep(gameSpeed);

            if (snake.segments.Length == (Math.Pow(gameField, 2)))
            {
                Console.WriteLine("You win! You filled the entire game field.");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
        }
    }
}

//хуй