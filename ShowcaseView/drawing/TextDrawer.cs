using Android.Content;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using SharpShowcaseView.Utils;

namespace SharpShowcaseView.Drawing
{
    public class TextDrawer : ITextDrawer
    {
        int PADDING = 24;
        int ACTIONBAR_PADDING = 66;

        TextPaint mPaintTitle;
        TextPaint mPaintDetail;

        SpannableString mTitle, mDetails;
        float mDensityScale;
        IShowcaseAreaCalculator mCalculator;
        float[] mBestTextPosition = new float[3];
        DynamicLayout mDynamicTitleLayout;
        DynamicLayout mDynamicDetailLayout;
        TextAppearanceSpan mTitleSpan;
        TextAppearanceSpan mDetailSpan;

        public TextDrawer(float densityScale, IShowcaseAreaCalculator calculator)
        {
            mDensityScale = densityScale;
            mCalculator = calculator;

            mPaintTitle = new TextPaint();
            mPaintTitle.AntiAlias = true;

            mPaintDetail = new TextPaint();
            mPaintDetail.AntiAlias = true;
        }

        public void Draw(Canvas canvas, bool hasPositionChanged)
        {
            if (ShouldDrawText())
            {
                float[] textPosition = GetBestTextPosition();

                if (!TextUtils.IsEmpty(mTitle))
                {
                    canvas.Save();
                    if (hasPositionChanged)
                    {
                        mDynamicTitleLayout = new DynamicLayout(mTitle, mPaintTitle, (int) textPosition[2], Layout.Alignment.AlignNormal, 1.0f, 1.0f, true);
                    }

                    canvas.Translate(textPosition[0], textPosition[1]);
                    mDynamicTitleLayout.Draw(canvas);
                    canvas.Restore();
                }

                if (!TextUtils.IsEmpty(mDetails))
                {
                    canvas.Save();
                    if (hasPositionChanged)
                    {
                        mDynamicDetailLayout = new DynamicLayout(mDetails, mPaintDetail, (int) textPosition[2], Layout.Alignment.AlignNormal, 1.2f, 1.0f, true);
                    }

                    canvas.Translate(textPosition[0], textPosition[1] + mDynamicTitleLayout.Height);
                    mDynamicDetailLayout.Draw(canvas);
                    canvas.Restore();
                }
            }
        }

        public void SetDetails(string details)
        {
            if (details != null)
            {
                SpannableString ssbDetail = new SpannableString(details);
                ssbDetail.SetSpan(mDetailSpan, 0, ssbDetail.Length(), 0);
                mDetails = ssbDetail;
            }
        }
            
        public void SetTitle(string title)
        {
            if (title != null) {
                SpannableString ssbTitle = new SpannableString(title);
                ssbTitle.SetSpan(mTitleSpan, 0, ssbTitle.Length(), 0);
                mTitle = ssbTitle;
            }
        }

        /**
     * Calculates the best place to position text
     *
     * @param canvasW width of the screen
     * @param canvasH height of the screen
     */

        public void CalculateTextPosition(int canvasW, int canvasH, ShowcaseView showcaseView)
        {
            Rect showcase = showcaseView.HasShowcaseView() ? mCalculator.GetShowcaseRect() : new Rect();

            int[] areas = new int[4]; //left, top, right, bottom
            areas[0] = showcase.Left * canvasH;
            areas[1] = showcase.Top * canvasW;
            areas[2] = (canvasW - showcase.Right) * canvasH;
            areas[3] = (canvasH - showcase.Bottom) * canvasW;

            int largest = 0;
            for(int i = 1; i < areas.Length; i++)
            {
                if(areas[i] > areas[largest])
                    largest = i;
            }

            // Position text in largest area
            switch(largest)
            {
                case 0:
                    mBestTextPosition[0] = PADDING * mDensityScale;
                    mBestTextPosition[1] = PADDING * mDensityScale;
                    mBestTextPosition[2] = showcase.Left - 2 * PADDING * mDensityScale;
                    break;
                case 1:
                    mBestTextPosition[0] = PADDING * mDensityScale;
                    mBestTextPosition[1] = (PADDING + ACTIONBAR_PADDING) * mDensityScale;
                    mBestTextPosition[2] = canvasW - 2 * PADDING * mDensityScale;
                    break;
                case 2:
                    mBestTextPosition[0] = showcase.Right + PADDING * mDensityScale;
                    mBestTextPosition[1] = PADDING * mDensityScale;
                    mBestTextPosition[2] = (canvasW - showcase.Right) - 2 * PADDING * mDensityScale;
                    break;
                case 3:
                    mBestTextPosition[0] = PADDING * mDensityScale;
                    mBestTextPosition[1] = showcase.Bottom + PADDING * mDensityScale;
                    mBestTextPosition[2] = canvasW - 2 * PADDING * mDensityScale;
                    break;
            }

            if(showcaseView.ConfigurationOptions.CenterText)
            {
                // Center text vertically or horizontally
                switch(largest) {
                    case 0:
                    case 2:
                        mBestTextPosition[1] += canvasH / 4;
                        break;
                    case 1:
                    case 3:
                        mBestTextPosition[2] /= 2;
                        mBestTextPosition[0] += canvasW / 4;
                        break;
                } 
            }
            else
            {
                // As text is not centered add actionbar padding if the text is left or right
                switch(largest) {
                    case 0:
                    case 2:
                        mBestTextPosition[1] += ACTIONBAR_PADDING * mDensityScale;
                        break;
                }
            }
        }
            
        public void SetTitleStyling(Context context, int styleId)
        {
            mTitleSpan = new TextAppearanceSpan(context, styleId);
        }

        public void SetDetailStyling(Context context, int styleId)
        {
            mDetailSpan = new TextAppearanceSpan(context, styleId);
        }

        public float[] GetBestTextPosition()
        {
            return mBestTextPosition;
        }

        public bool ShouldDrawText()
        {
            return !TextUtils.IsEmpty(mTitle) || !TextUtils.IsEmpty(mDetails);
        }
    }
}