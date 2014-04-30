using System;
using Android.Animation;
using Android.Graphics;

namespace SharpShowcaseView.Utils
{
    public static class PointAnimator
    {
        public static Animator OfPoints(Java.Lang.Object objects, String xMethod, String yMethod, Point[] values)
        {
            var set = new AnimatorSet();
            var xValues = new int[values.Length];
            var yValues = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                xValues[i] = values[i].X;
                yValues[i] = values[i].Y;
            }

            ObjectAnimator xAnimator = ObjectAnimator.OfInt(objects, xMethod, xValues);
            ObjectAnimator yAnimator = ObjectAnimator.OfInt(objects, yMethod, yValues);

            set.PlayTogether(xAnimator, yAnimator);
            return set;
        }

        public static Animator OfPoints(ShowcaseView showcaseView, Point[] values)
        {
            return OfPoints(showcaseView, "showcaseX", "showcaseY", values);
        }

        public static Animator OfPoints(ShowcaseView showcaseView, Point point)
        {
            return OfPoints(showcaseView, new Point[]{ point });
        }
    }
}