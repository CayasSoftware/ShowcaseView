using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SharpShowcaseView.Actionbar.Reflection
{
    /// <summary>
    /// Reflector which finds action items in the standard API 11 ActionBar implementation.
    /// </summary>
    public class ActionBarReflector : BaseReflector
    {
        Activity mActivity;

        public ActionBarReflector(Activity activity)
        {
            mActivity = activity;
        }

        public override View GetHomeButton()
        {
            View homeButton = mActivity.FindViewById(Android.Resource.Id.Home);
            if (homeButton == null) {
                throw new Java.Lang.RuntimeException(
                    "insertShowcaseViewWithType cannot be used when the theme " +
                    "has no ActionBar");
            }
            return homeButton;
        }

        public override void ShowcaseActionItem(int itemId)
        {
        }
    }
}