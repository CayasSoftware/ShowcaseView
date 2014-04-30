using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Util;

namespace SharpShowcaseView.Drawing
{
    public class ClingDrawer : IClingDrawer
    {
        Paint mEraser;
        Drawable mShowcaseDrawable;
        Rect mShowcaseRect;

        Color showcaseColor;

        public Color ShowcaseColor
        {
            get
            {
                return showcaseColor;
            }
            set
            {
                if (showcaseColor != value)
                {
                    showcaseColor = value;
                    mShowcaseDrawable.SetColorFilter(showcaseColor, PorterDuff.Mode.Multiply);
                }
            }
        }

        public ClingDrawer(Resources resources, Color showcaseColor)
        {
            this.showcaseColor = showcaseColor;

            PorterDuffXfermode mBlender = new PorterDuffXfermode(PorterDuff.Mode.Clear);
            mEraser = new Paint();
            mEraser.Color = Color.White;
            mEraser.Alpha = 0;
            mEraser.SetXfermode(mBlender);
            mEraser.AntiAlias = true;

            mShowcaseDrawable = resources.GetDrawable(Resource.Drawable.cling_bleached);
            mShowcaseDrawable.SetColorFilter(showcaseColor, PorterDuff.Mode.Multiply);
        }
            
        public void DrawShowcase(Canvas canvas, float x, float y, float scaleMultiplier, float radius)
        {
            Matrix mm = new Matrix();
            mm.PostScale(scaleMultiplier, scaleMultiplier, x, y);
            canvas.Matrix = mm;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb)
            {
                canvas.DrawCircle(x, y, radius, mEraser);
            }

            mShowcaseDrawable.SetBounds(mShowcaseRect.Left, mShowcaseRect.Top, mShowcaseRect.Right, mShowcaseRect.Bottom);
            mShowcaseDrawable.Draw(canvas);

            canvas.Matrix = new Matrix();
        }

        public int GetShowcaseWidth()
        {
            return mShowcaseDrawable.IntrinsicWidth;
        }


        public int GetShowcaseHeight()
        {
            return mShowcaseDrawable.IntrinsicHeight;
        }

        /**
     * Creates a {@link android.graphics.Rect} which represents the area the showcase covers. Used
     * to calculate where best to place the text
     *
     * @return true if voidedArea has changed, false otherwise.
     */
        public bool CalculateShowcaseRect(float x, float y)
        {
            if (mShowcaseRect == null) {
                mShowcaseRect = new Rect();
            }

            int cx = (int) x, cy = (int) y;
            int dw = GetShowcaseWidth();
            int dh = GetShowcaseHeight();

            if (mShowcaseRect.Left == cx - dw / 2) {
                return false;
            }

            Log.Debug("ShowcaseView", "Recalculated");

            mShowcaseRect.Left = cx - dw / 2;
            mShowcaseRect.Top = cy - dh / 2;
            mShowcaseRect.Right = cx + dw / 2;
            mShowcaseRect.Bottom = cy + dh / 2;

            return true;
        }

        public Rect GetShowcaseRect()
        {
            return mShowcaseRect;
        }
    }
}