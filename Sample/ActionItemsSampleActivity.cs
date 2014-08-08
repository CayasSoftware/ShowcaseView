using Android.App;
using Android.OS;
using Android.Views;
using SharpShowcaseView;
using SharpShowcaseView.Targets;

namespace Sample
{
    [Activity(Label = "ActionItemSampleActivity")]            
    public class ActionItemsSampleActivity : Activity
    {
        ShowcaseView sv;
        ShowcaseView.ConfigOptions mOptions = new ShowcaseView.ConfigOptions();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sample);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            mOptions.Block = false;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);

            var target = new ActionViewTarget(this, ActionViewTarget.Type.OVERFLOW);
            sv = ShowcaseView.InsertShowcaseView(target, this, Resource.String.showcase_simple_title, Resource.String.showcase_simple_message, mOptions);
            sv.Activated = true;

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    sv.SetShowcase(new ActionViewTarget(this, ActionViewTarget.Type.HOME), true);
                    break;
                case Resource.Id.menu_item1:
                    sv.SetShowcase(new ActionItemTarget(this, Resource.Id.menu_item1), true);
                    break;
                case Resource.Id.menu_item2:
                    sv.SetShowcase(new ActionViewTarget(this, ActionViewTarget.Type.TITLE), true);
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}