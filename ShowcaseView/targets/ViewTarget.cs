using Android.App;
using Android.Views;
using Android.Graphics;

namespace SharpShowcaseView.Targets
{
    public class ViewTarget : ITarget
    {
        readonly View mView;

        public ViewTarget(View view)
        {
            mView = view;
        }

        public ViewTarget(int viewId, Activity activity)
        {
            mView = activity.FindViewById(viewId);
        }
            
        public Point GetPoint()
        {
            if (mView == null)
                return null;

            var location = new int[2];
            mView.GetLocationInWindow(location);
            int x = location[0] + mView.Width / 2;
            int y = location[1] + mView.Height / 2;

            return new Point(x, y);
        }
    }
}