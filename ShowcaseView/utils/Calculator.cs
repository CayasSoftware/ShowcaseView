using Android.Views;
using Android.Graphics;

namespace SharpShowcaseView.Utils
{
    /// <summary>
    /// Calculates various items for use with ShowcaseView.
    /// </summary>
    public static class Calculator {

        public static Point GetShowcasePointFromView(View view, ShowcaseView.ConfigOptions options)
        {
            var result = new Point();

            if (options.Insert == ShowcaseView.INSERTTOVIEW)
            {
                result.X = view.Left + view.Width / 2;
                result.Y = view.Top + view.Height / 2;
            }
            else
            {
                var coordinates = new int[2];

                view.GetLocationInWindow(coordinates);
                result.X = coordinates[0] + view.Width / 2;
                result.Y = coordinates[1] + view.Height / 2;
            }
            return result;
        }
    }
}