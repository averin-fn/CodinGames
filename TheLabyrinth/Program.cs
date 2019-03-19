using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    static int R, C, A;
    static char[,] Map = null; // labyrinth map
    static int[,] Values = null; // length to start map values
    static bool IsTimer = false;
    static Point Ctrl = new Point();
    static Stack<Point> Steps = new Stack<Point>();

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        R = int.Parse(inputs[0]); // number of rows.
        C = int.Parse(inputs[1]); // number of columns.
        A = int.Parse(inputs[2]); // number of rounds between the time the alarm countdown is activated and the time the alarm goes off.

        // invert array for Point(X, Y) work
        Map = new char[C, R];
        Values = new int[C, R];

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int KR = int.Parse(inputs[0]); // row where Kirk is located.
            int KC = int.Parse(inputs[1]); // column where Kirk is located.

            Point KP = new Point(KC, KR); // Kirck point

            for (int y = 0; y < R; y++)
            {
                string ROW = Console.ReadLine(); // C of the characters in '#.TC?' (i.e. one line of the ASCII maze).

                for (int x = 0; x < C; x++)
                {
                    Map[x, y] = ROW[x];

                    if (ROW[x] == 'C') // get ctrl point
                    {
                        Ctrl = new Point(x, y);
                    }
                }
            }

            Move(KP);
        }
    }

    static Direction GetDirection(Point from, Point to)
    {
        if (to.Y > from.Y) return Direction.DOWN;
        if (to.Y < from.Y) return Direction.UP;
        if (to.X > from.X) return Direction.RIGHT;
        if (to.X < from.X) return Direction.LEFT;

        throw new Exception(string.Format("Faild to get direction. From: {0} - To: {1}", from, to));
    }

    static bool TryMove(Point from, Point to)
    {
        if (Map[to.X, to.Y] != '#')
        {
            if (Values[to.X, to.Y] == 0 || Values[to.X, to.Y] > Values[from.X, from.Y] + 1)
            {
                // find min around current point and set next step value
                Values[to.X, to.Y] = FindMin(new Point(to.X, to.Y)).min + 1;
                Console.WriteLine(GetDirection(from, new Point(to.X, to.Y))); // move
                Steps.Push(from); // add step
                return true;
            }
        }
        return false;
    }

    static void Move(Point current)
    {
        if (current == Ctrl) IsTimer = true;

        // set start value
        if (Values[current.X, current.Y] == 0)
        {
            Values[current.X, current.Y] = 1;
        }

        if (IsTimer)
        {
            Console.WriteLine(GetDirection(current, FindMin(current).point));
            return;
        }

        // move next if current point length < number of rounds
        if (Values[current.X, current.Y] <= A)
        {
            if (TryMove(current, new Point(current.X + 1, current.Y))) return;
            if (TryMove(current, new Point(current.X - 1, current.Y))) return;
            if (TryMove(current, new Point(current.X, current.Y + 1))) return;
            if (TryMove(current, new Point(current.X, current.Y - 1))) return;
        }

        // move back
        if (Steps.TryPop(out Point p))
        {
            Console.WriteLine(GetDirection(current, p));
            return;
        }

        throw new Exception("Kirck can't move");
    }

    static bool CheckMin(Point point, int? min)
    {
        return (Values[point.X, point.Y] != 0 && (!min.HasValue || Values[point.X, point.Y] < min));
    }

    // find min around the current point
    static (Point point, int min) FindMin(Point current)
    {
        Point p = new Point();
        int? min = null;

        if (CheckMin(new Point(current.X + 1, current.Y), min))
        {
            min = Values[current.X + 1, current.Y];
            p = new Point(current.X + 1, current.Y);
        }
        if (CheckMin(new Point(current.X - 1, current.Y), min))
        {
            min = Values[current.X - 1, current.Y];
            p = new Point(current.X - 1, current.Y);
        }
        if (CheckMin(new Point(current.X, current.Y + 1), min))
        {
            min = Values[current.X, current.Y + 1];
            p = new Point(current.X, current.Y + 1);
        }
        if (CheckMin(new Point(current.X, current.Y - 1), min))
        {
            min = Values[current.X, current.Y - 1];
            p = new Point(current.X, current.Y - 1);
        }

        if (!min.HasValue)
        {
            throw new Exception("min value not found.");
        }

        return (p, min.Value);
    }
}