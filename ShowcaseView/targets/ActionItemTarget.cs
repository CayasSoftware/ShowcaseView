using Android.App;
using Android.Views;
using Android.Graphics;

using SharpShowcaseView.Actionbar;
using SharpShowcaseView.Actionbar.Reflection;

namespace SharpShowcaseView.Targets
{
    public class ActionItemTarget : ITarget
    {
        Activity mActivity;
        int mItemId;

        ActionBarViewWrapper mActionBarWrapper;

        public ActionItemTarget(Activity activity, int itemId)
        {
            mActivity = activity;
            mItemId = itemId;
        }
            
        public Point GetPoint()
        {
            SetUp();
            return new ViewTarget(mActionBarWrapper.GetActionItem(mItemId)).GetPoint();
        }

        void SetUp()
        {
            BaseReflector reflector = BaseReflector.GetReflectorForActivity(mActivity);
            IViewParent p = reflector.GetActionBarView(); //ActionBarView
            mActionBarWrapper = new ActionBarViewWrapper((View)p);
        }
    }
}