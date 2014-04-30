using Android.App;
using Android.Views;

namespace SharpShowcaseView.Actionbar.Reflection
{
    /// <summary>
    /// Base class which uses reflection to determine how to showcase Action Items and Action Views.
    /// </summary>
    public abstract class BaseReflector
    {
        public abstract View GetHomeButton();

        public abstract void ShowcaseActionItem(int itemId);

        public IViewParent GetActionBarView()
        {
            return GetHomeButton().Parent.Parent;
        }

        public static BaseReflector GetReflectorForActivity(Activity activity)
        {
            return new ActionBarReflector(activity);

//            switch (SearchForActivitySuperClass(activity))
//            {
//                case ActionBarType.STANDARD:
//                    return new ActionBarReflector(activity);
//                case ActionBarType.APPCOMPAT:
//                    return new AppCompatReflector(activity);
//                case ActionBarType.ACTIONBARSHERLOCK:
//                    return new SherlockReflector(activity);
//            }
//            return null;
        }

        //todo: comparision of parent types, than activate switch in GetReflactorForActivity
//        static ActionBarType SearchForActivitySuperClass(Activity activity)
//        {
//            Java.Lang.Class currentLevel = activity.Class;
//
//            while (currentLevel == typeof(Activity))
//            {
//                if (currentLevel.SimpleName.Equals("SherlockActivity"))
//                {
//                    return ActionBarType.ACTIONBARSHERLOCK;
//                }
//
//                if (currentLevel.SimpleName.Equals("ActionBarActivity"))
//                {
//                    return ActionBarType.APPCOMPAT;
//                }
//
//                currentLevel = currentLevel.Superclass;
//            }
//            return ActionBarType.STANDARD;
//        }

        enum ActionBarType
        {
            STANDARD,
            APPCOMPAT,
            ACTIONBARSHERLOCK
        }
    }
}