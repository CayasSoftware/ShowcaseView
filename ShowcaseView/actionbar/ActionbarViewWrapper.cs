using System;
using Android.Views;
using Java.Lang.Reflect;
using Android.Util;

namespace SharpShowcaseView.Actionbar
{
    /// <summary>
    /// Class which wraps round the many implementations of ActionBarView and allows finding of Action items.
    /// </summary>
    public class ActionBarViewWrapper
    {
        IViewParent mActionBarView;
        //        private View mActionBarView;
        Java.Lang.Class mActionBarViewClass;
        Java.Lang.Class mAbsActionBarViewClass;
        //        public ActionBarViewWrapper(IViewParent actionBarView)

        public ActionBarViewWrapper(View actionBarView)
        {
            if (!actionBarView.Class.Name.Contains("ActionBarView"))
            {
                String previousP = actionBarView.Class.Name;
                actionBarView = (View)actionBarView.Parent;
                String throwP = actionBarView.Class.Name;

                if (!actionBarView.Class.Name.Contains("ActionBarView"))
                {
                    throw new Java.Lang.IllegalStateException("Cannot find ActionBarView for " + "Activity, instead found " + previousP + " and " + throwP);
                }
            }

            mActionBarView = (IViewParent)actionBarView;
            mActionBarViewClass = actionBarView.Class;
            mAbsActionBarViewClass = actionBarView.Class.Superclass;
        }

        /// <summary>
        /// Gets the spinner view.
        /// </summary>
        /// <returns>Return the view which represents the spinner on the ActionBar, or null if there isn't one.</returns>
        public View GetSpinnerView()
        {
            try
            {
                Field spinnerField = mActionBarViewClass.GetDeclaredField("mSpinner");
                spinnerField.Accessible = true;

                return (View)spinnerField.Get((Java.Lang.Object)mActionBarView);
            }
            catch (Java.Lang.NoSuchFieldException e)
            {
                Log.Error("TAG", "Failed to find actionbar spinner", e);
            }
            catch (Java.Lang.IllegalAccessException e)
            {
                Log.Error("TAG", "Failed to access actionbar spinner", e);
            }

            return null;
        }

        /// <summary>
        /// Gets the title view.
        /// </summary>
        /// <returns>Return the view which represents the title on the ActionBar, or null if there isn't one.</returns>
        public View GetTitleView()
        {
            try
            {
                Field mTitleViewField = mActionBarViewClass.GetDeclaredField("mTitleView");
                mTitleViewField.Accessible = true;

                return (View)mTitleViewField.Get((Java.Lang.Object)mActionBarView);
            }
            catch (Java.Lang.NoSuchFieldException e)
            {
                Log.Error("TAG", "Failed to find actionbar title", e);
            }
            catch (Java.Lang.IllegalAccessException e)
            {
                Log.Error("TAG", "Failed to access actionbar title", e);
            }

            return null;
        }

        /// <summary>
        /// Gets the overflow view.
        /// </summary>
        /// <returns>Return the view which represents the overflow action item on the ActionBar, or null if there isn't one.</returns>
        public View GetOverflowView()
        {
            try
            {
                Field actionMenuPresenterField = mAbsActionBarViewClass.GetDeclaredField("mActionMenuPresenter");
                actionMenuPresenterField.Accessible = true;

                var actionMenuPresenter = actionMenuPresenterField.Get((Java.Lang.Object)mActionBarView);
                Field overflowButtonField = actionMenuPresenter.Class.GetDeclaredField("mOverflowButton");
                overflowButtonField.Accessible = true;

                return (View)overflowButtonField.Get(actionMenuPresenter);
            }
            catch (Java.Lang.IllegalAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (Java.Lang.NoSuchFieldException e)
            {
                e.PrintStackTrace();
            }
            return null;
        }

        public View GetActionItem(int actionItemId)
        {
            try
            {
                Field actionMenuPresenterField = mAbsActionBarViewClass.GetDeclaredField("mActionMenuPresenter");
                actionMenuPresenterField.Accessible = true;

                var actionMenuPresenter = actionMenuPresenterField.Get((Java.Lang.Object)mActionBarView);

                Field menuViewField = actionMenuPresenter.Class.Superclass.GetDeclaredField("mMenuView");
                menuViewField.Accessible = true;

                var menuView = menuViewField.Get(actionMenuPresenter);

                Field mChField;
                if (menuView.Class.ToString().Contains("com.actionbarsherlock"))
                {
                    // There are thousands of superclasses to traverse up
                    // Have to get superclasses because mChildren is private
                    mChField = menuView.Class.Superclass.Superclass.Superclass.Superclass.GetDeclaredField("mChildren");
                }
                else if (menuView.Class.ToString().Contains("android.support.v7"))
                {
                    mChField = menuView.Class.Superclass.Superclass.Superclass.GetDeclaredField("mChildren");
                }
                else
                {
                    mChField = menuView.Class.Superclass.Superclass.GetDeclaredField("mChildren");
                }

                mChField.Accessible = true;
                var mChs = (Java.Lang.Object[])mChField.Get(menuView);

                foreach (Object mCh in mChs)
                {
                    if (mCh != null)
                    {
                        var v = (View)mCh;

                        if (v.Id == actionItemId)
                        {
                            return v;
                        }
                    }
                }
            }
            catch (Java.Lang.IllegalAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (Java.Lang.NoSuchFieldException e)
            {
                e.PrintStackTrace();
            }
            return null;
        }
    }
}