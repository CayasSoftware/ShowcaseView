using Android.Graphics;

namespace SharpShowcaseView.Utils
{
    /// <summary>
    /// Responsible for calculating where the Showcase should position itself
    /// </summary>
    public interface IShowcaseAreaCalculator
    {
        bool CalculateShowcaseRect(float showcaseX, float showcaseY);

        Rect GetShowcaseRect();
    }
}