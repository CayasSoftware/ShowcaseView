using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SharpShowcaseView;

using SharpShowcaseView.Targets;

namespace Sample
{
    public class ShowcaseFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.fragment_layout, container);

            var button = layout.FindViewById<Button>(Resource.Id.buttonFragments);
            button.Click+= delegate
            {
                Toast.MakeText(Activity, Resource.String.it_does_work, ToastLength.Long).Show();
            };

            return layout;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            //SetContentView() needs to be called in the Activity first.
            //That's why it has to be in OnActivityCreated().
            var configOptions = new ShowcaseView.ConfigOptions();
            configOptions.HideOnClickOutside = true;

            ShowcaseView.InsertShowcaseView(new ViewTarget(Resource.Id.buttonFragments, Activity), Activity, Resource.String.showcase_fragment_title, Resource.String.showcase_fragment_message, configOptions);
        }
    }
}