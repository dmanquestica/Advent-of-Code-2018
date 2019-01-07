using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day13WPF
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y);
        }
    }

    public class Cart
    {
        public int ID { get; set; }
        public Direction Direction { get; set; }
        public Position Position { get; set; }
        public bool Crashed { get; set; }
        public int TimesAtIntersection { get; set; }

        public Cart()
        {
            Crashed = false;
            TimesAtIntersection = 0;
        }

        public void TurnRight()
        {
            if (Direction == Direction.North)
                Direction = Direction.East;
            else if (Direction == Direction.South)
                Direction = Direction.West;
            else if (Direction == Direction.West)
                Direction = Direction.North;
            else
                Direction = Direction.South;
        }

        public void TurnLeft()
        {
            if (Direction == Direction.North)
                Direction = Direction.West;
            else if (Direction == Direction.South)
                Direction = Direction.East;
            else if (Direction == Direction.West)
                Direction = Direction.South;
            else
                Direction = Direction.North;
        }

        public void IntersectionMove(ref char[,] Tracks)
        {
            switch (TimesAtIntersection % 3)
            {
                case 0:
                    TurnLeft();
                    break;
                case 1:
                    break;
                case 2:
                    TurnRight();
                    break;
            }
            TimesAtIntersection++;
        }

        public void Move(ref char[,] Tracks)
        {
            if (Direction == Direction.North || Direction == Direction.South)
            {
                var positionToCheck = Direction == Direction.North ? Tracks[Position.Y - 1, Position.X] : Tracks[Position.Y + 1, Position.X];
                Position.Y -= Direction == Direction.North ? 1 : -1;
                switch (positionToCheck)
                {
                    case '|':
                        break;
                    case '\\':
                        TurnLeft();
                        break;
                    case '/':
                        TurnRight();
                        break;
                    case '+':
                        IntersectionMove(ref Tracks);
                        break;
                    default:
                        throw new Exception("You're off the track");
                }
            }
            else if (Direction == Direction.East || Direction == Direction.West)
            {
                var positionToCheck = Direction == Direction.East ? Tracks[Position.Y, Position.X + 1] : Tracks[Position.Y, Position.X - 1];
                Position.X += Direction == Direction.East ? 1 : -1;
                switch (positionToCheck)
                {
                    case '-':
                        break;
                    case '\\':
                        TurnRight();
                        break;
                    case '/':
                        TurnLeft();
                        break;
                    case '+':
                        IntersectionMove(ref Tracks);
                        break;
                    default:
                        throw new Exception("You're off the track");
                }
            }
        }
    }

    public enum Direction
    {
        North = 0,
        South = 1,
        West = 2,
        East = 3,
    }
}
