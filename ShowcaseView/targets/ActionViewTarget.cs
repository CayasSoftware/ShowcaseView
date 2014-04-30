using Android.App;
using Android.Views;
using Android.Graphics;

using SharpShowcaseView.Actionbar;
using SharpShowcaseView.Actionbar.Reflection;

namespace SharpShowcaseView.Targets
{
    public class ActionViewTarget : ITarget
    {
        Activity mActivity;
        Type mType;
        ActionBarViewWrapper mActionBarWrapper;
        BaseReflector mReflector;

        public ActionViewTarget(Activity activity, Type type)
        {
            mActivity = activity;
            mType = type;
        }

        void SetUp()
        {
            mReflector = BaseReflector.GetReflectorForActivity(mActivity);
            IViewParent p = mReflector.GetActionBarView(); //ActionBarView
            mActionBarWrapper = new ActionBarViewWrapper((View)p);
        }

        public Point GetPoint()
        {
            ITarget target = null;
            SetUp();

            switch (mType)
            {
                case Type.SPINNER:
                    target = new ViewTarget(mActionBarWrapper.GetSpinnerView());
                    break;

                case Type.HOME:
                    target = new ViewTarget(mReflector.GetHomeButton());
                    break;

                case Type.OVERFLOW:
                    target = new ViewTarget(mActionBarWrapper.GetOverflowView());
                    break;

                case Type.TITLE:
                    target = new ViewTarget(mActionBarWrapper.GetTitleView());
                    break;
            }

            return target == null ? null : target.GetPoint();
        }

        public enum Type
        {
            SPINNER,
            HOME,
            TITLE,
            OVERFLOW
        }
    }
}