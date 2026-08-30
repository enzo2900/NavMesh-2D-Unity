using System;

namespace Utility
{
    public class Couple<L,R> 
    {

        public L Left;
        
        public R Right;
        public Couple(L left, R right)
        {
            this.Left = left;
            this.Right = right;
            
        }

        public L left()
        {
            return Left;
        }

        public R right()
        {
            return Right;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right); 
        }

        public override bool Equals(object obj)
        {
            if (obj is Couple<L, R> couple)
            {
                return ReferenceEquals(Left, couple.Left);
            }

            return false;
        }
    }
}