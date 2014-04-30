using Android.App;
using Android.Views;

namespace SharpShowcaseView.Actionbar.Reflection
{
    public class AppCompatReflector : BaseReflector
    {
        Activity mActivity;

        public AppCompatReflector(Activity activity)
        {
            mActivity = activity;
        }

        public override View GetHomeButton()
        {
            View homeButton = mActivity.FindViewById(Android.Resource.Id.Home);

            if (homeButton != null)
            {
                return homeButton;
            }

            int homeId = mActivity.Resources.GetIdentifier("home", "id", mActivity.PackageName);
            homeButton = mActivity.FindViewById(homeId);

            if (homeButton == null)
            {
                throw new Java.Lang.RuntimeException("insertShowcaseViewWithType cannot be used when the theme " + "has no ActionBar");
            }
            return homeButton;
        }

        public override void ShowcaseActionItem(int itemId)
        {

        }
    }
}