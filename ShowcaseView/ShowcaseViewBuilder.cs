using System;
using Android.App;
using Android.Views;
using Android.Graphics;
using SharpShowcaseView.Targets;

namespace SharpShowcaseView
{
    public class ShowcaseViewBuilder
    {
        readonly ShowcaseView showcaseView;

        public ShowcaseViewBuilder(Activity activity)
        {
            this.showcaseView = new ShowcaseView(activity);
        }

        public ShowcaseViewBuilder(ShowcaseView showcaseView)
        {
            this.showcaseView = showcaseView;
        }

        public ShowcaseViewBuilder(Activity activity, int showcaseLayoutViewId)
        {
            this.showcaseView = (ShowcaseView) activity.LayoutInflater.Inflate(showcaseLayoutViewId, null);
        }

        public ShowcaseViewBuilder SetShowcaseNoView()
        {
            showcaseView.SetShowcase(null);
            return this;
        }

        public ShowcaseViewBuilder SetShowcaseView(View view)
        {
            showcaseView.SetShowcase(new ViewTarget(view));
            return this;
        }

        public ShowcaseViewBuilder SetShowcasePosition(int x, int y)
        {
            showcaseView.SetShowcase(new PointTarget(x, y));
            return this;
        }

        public ShowcaseViewBuilder SetShowcaseItem(int itemType, int actionItemId, Activity activity)
        {
            showcaseView.SetShowcaseItem(itemType, actionItemId, activity);
            return this;
        }

        public ShowcaseViewBuilder SetShowcaseIndicatorScale(float scale)
        {
            showcaseView.ScaleMultiplier = scale;
            return this;
        }

        public ShowcaseViewBuilder OverrideButtonClick(View.IOnClickListener listener)
        {
            showcaseView.OverrideButtonClick(listener);
            return this;
        }

        public ShowcaseViewBuilder AnimateGesture(float offsetStartX, float offsetStartY, float offsetEndX, float offsetEndY)
        {
            showcaseView.AnimateGesture(offsetStartX, offsetStartY, offsetEndX, offsetEndY);
            return this;
        }

        public ShowcaseViewBuilder SetText(String titleText, String subText)
        {
            showcaseView.SetText(titleText, subText);
            return this;
        }

        public ShowcaseViewBuilder SetText(int titleText, int subText)
        {
            showcaseView.SetText(titleText, subText);
            return this;
        }

        public ShowcaseViewBuilder PointTo(View view)
        {
            showcaseView.SetShowcase(new ViewTarget(view));
            return this;
        }

        public ShowcaseViewBuilder PointTo(float x, float y)
        {
            showcaseView.PointTo(x, y);
            return this;
        }

        public ShowcaseViewBuilder SetConfigOptions(ShowcaseView.ConfigOptions configOptions) {
            showcaseView.ConfigurationOptions = configOptions;
            return this;
        }

        public ShowcaseView Build()
        {
            return showcaseView;
        }
    }
}