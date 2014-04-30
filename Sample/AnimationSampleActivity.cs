using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using SharpShowcaseView;
using SharpShowcaseView.Targets;

namespace Sample
{
    [Activity(Label = "AnimationSampleActivity")]            
    public class AnimationSampleActivity : Activity
    {
        private ShowcaseView showcaseView;
        private int counter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_animation);
            counter = 0;

            var textView1 = FindViewById<TextView>(Resource.Id.animation_textView);
            var textView2 = FindViewById<TextView>(Resource.Id.animation_textView2);
            var textView3 = FindViewById<TextView>(Resource.Id.animation_textView3);

            showcaseView = ShowcaseView.InsertShowcaseView(new ViewTarget(FindViewById(Resource.Id.animation_textView)), this);
            showcaseView.OverrideButtonClick((s,e) =>
            {
                switch (counter)
                {
                    case 0:
                        showcaseView.SetShowcase(new ViewTarget(textView2), true);
                        break;

                    case 1:
                        showcaseView.SetShowcase(new ViewTarget(textView3), true);
                        break;

                    case 2:
                        showcaseView.SetShowcase(null);
                        showcaseView.SetText("Look ma!", "You don't always need a target to showcase");

                        SetAlpha(0.4f, new View[]{textView1, textView2, textView3});
                        break;

                    case 3:
                        showcaseView.Hide();
                        SetAlpha(1.0f, new View[]{textView1, textView2, textView3});
                        break;
                }
                counter++;
            });

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                enableUp();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        void enableUp()
        {
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        static void SetAlpha(float alpha, View[] views)
        {
            if (SampleActivity.IsHoneycombOrAbove())
            {
                foreach (View view in views)
                {
                    view.Alpha = alpha;
                }
            }
        }
    }
}