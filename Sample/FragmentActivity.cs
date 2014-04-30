using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace Sample
{
    [Activity(Label = "ShowcaseFragmentActivity")]            
    public class ShowcaseFragmentActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.fragment_activity_layout);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                enableUp();
            }
        }

        private void enableUp()
        {
            ActionBar.SetDisplayHomeAsUpEnabled(true);
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
    }
}