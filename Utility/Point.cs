namespace Utility
{
    public struct Point : MapPoint
    {
        public float x, y;
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float getX()
        {
            return x;
        }

        public float getY()
        {
            return y;
        }
    }
}