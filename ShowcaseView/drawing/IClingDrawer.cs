using Android.Graphics;
using SharpShowcaseView.Utils;

namespace SharpShowcaseView.Drawing
{
    interface IClingDrawer : IShowcaseAreaCalculator
    {
        void DrawShowcase(Canvas canvas, float x, float y, float scaleMultiplier, float radius);

        int GetShowcaseWidth();

        int GetShowcaseHeight();
    }
}