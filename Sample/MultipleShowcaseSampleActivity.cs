using Android.App;

using Android.OS;
using Android.Widget;
using SharpShowcaseView;
using Android.Graphics;

namespace Sample
{
    [Activity(Label = "Sample")]
    public class MultipleShowcaseSampleActivity : Activity
    {
        static float SHOWCASE_KITTEN_SCALE = 1.2f;
        static float SHOWCASE_LIKE_SCALE = 0.5f;

        ShowcaseView.ConfigOptions mOptions = new ShowcaseView.ConfigOptions();
        ShowcaseViews mViews;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sample_legacy);

            FindViewById(Resource.Id.buttonLike).Click += delegate
            {
                Toast.MakeText(ApplicationContext, Resource.String.like_message, ToastLength.Short).Show();
            };

            mOptions.Block = false;
            mOptions.HideOnClickOutside = false;

            mViews = new ShowcaseViews(this, new MyShowcaseAcknowledgeListener(this));
            mViews.AddView( new ShowcaseViews.ItemViewProperties(Resource.Id.image, Resource.String.showcase_image_title, Resource.String.showcase_image_message, SHOWCASE_KITTEN_SCALE));
            mViews.AddView( new ShowcaseViews.ItemViewProperties(Resource.Id.buttonLike, Resource.String.showcase_like_title, Resource.String.showcase_like_message, SHOWCASE_LIKE_SCALE));
            mViews.Show();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                enableUp();
            }
        }

        void enableUp()
        {
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        class MyShowcaseAcknowledgeListener : ShowcaseViews.IOnShowcaseAcknowledged
        {
            readonly Activity parent;

            public MyShowcaseAcknowledgeListener(Activity parent)
            {
                this.parent = parent;
            }

            #region IOnShowcaseAcknowledged implementation

            public void OnShowCaseAcknowledged(ShowcaseView showcaseView)
            {
                Toast.MakeText(parent, Resource.String.dismissed_message, ToastLength.Short).Show();
            }

            #endregion
        }
    }
}