using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day15WPF
{
    public class Elf : Unit
    {
        public Elf()
        {
            UnitType = 'E';
        }
    }

    public class Goblin : Unit
    {
        public Goblin()
        {
            UnitType = 'G';
        }
    }

    public class Unit : IComparable
    {
        public char UnitType { get; set; }
        public int HP { get; set; }
        public int AttackPower { get; set; }
        public Position Position { get; set; }

        public bool Alive() { return (HP > 0); }

        public Unit(int attackPower = 3)
        {
            HP = 200;
            AttackPower = attackPower;
        }

        public bool InRange(Unit otherUnit)
        {
            return Position.IsAdjacent(otherUnit.Position);
        }

        public int CompareTo(object obj)
        {
            return this.Position.CompareTo(((Unit)obj).Position);
        }
    }

    public class Position : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position()
        {
        }

        public bool IsAdjacent(Position otherPosition)
        {
            return (Math.Abs(Y - otherPosition.Y) + Math.Abs(X - otherPosition.X) == 1);
        }

        public int CompareTo(object obj)
        {
            if (this.Y.CompareTo(((Position)obj).Y) != 0)
                return this.Y.CompareTo(((Position)obj).Y);
            else if (this.X.CompareTo(((Position)obj).X) != 0)
                return this.X.CompareTo(((Position)obj).X);
            else
                return 0;
        }
    }
}
