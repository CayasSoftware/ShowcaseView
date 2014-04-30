using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace SharpShowcaseView.Drawing
{
    public interface ITextDrawer
    {
        void Draw(Canvas canvas, bool hasPositionChanged);

        void SetDetails(string details);

        void SetTitle(string title);

        void CalculateTextPosition(int canvasW, int canvasH, ShowcaseView showcaseView);

        void SetTitleStyling(Context context, int styleId);

        void SetDetailStyling(Context context, int styleId);
    }
}