using Android.Graphics;

namespace SharpShowcaseView.Targets
{
    public class PointTarget : ITarget
    {
        Point mPoint;

        public PointTarget(Point point)
        {
            mPoint = point;
        }

        public PointTarget(int x, int y) {
            mPoint = new Point(x, y);
        }
            
        public Point GetPoint()
        {
            return mPoint;
        }
    }
}