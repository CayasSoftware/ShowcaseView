using Android.App;
using Android.OS;

using SharpShowcaseView;

namespace Sample
{
    [Activity(Label = "MemoryTesting")]            
    public class MemoryManagementTesting : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var itemViewProperties = new ShowcaseViews.ItemViewProperties(Resource.Id.buttonBlocked, Resource.String.showcase_like_title, Resource.String.showcase_like_message);

            var showcaseViews = new ShowcaseViews(this);
            showcaseViews.AddView(itemViewProperties).AddView(itemViewProperties).AddView(itemViewProperties).AddView(itemViewProperties).AddView(itemViewProperties).AddView(itemViewProperties).Show();
        }
    }
}