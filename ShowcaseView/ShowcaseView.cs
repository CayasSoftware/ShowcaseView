using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;

using SharpShowcaseView.Drawing;
using SharpShowcaseView.Targets;
using SharpShowcaseView.Utils;
using SharpShowcaseView.Actionbar.Reflection;
using SharpShowcaseView.Actionbar;
using System.Collections.Generic;
using Java.Interop;

namespace SharpShowcaseView
{
    /// <summary>
    /// A view which allows you to showcase areas of your app with an explanation.
    /// </summary>
    public class ShowcaseView : RelativeLayout, View.IOnClickListener, View.IOnTouchListener
    {
        public static int INSERTTODECOR = 0;
        public static int INSERTTOVIEW = 1;
        public static String PREFSSHOWCASEINTERNAL = "showcase_internal";
        public static int INNERCIRCLERADIUS = 94;

//        public enum ShowcaseItemTypes
//        {
//            ActionHome = 0,
//            Title = 1,
//            Spinner = 2,
//            ActionItem = 3,
//            ActionOverflow = 6
//        }

        public const int ITEMACTIONHOME = 0;
        public const int ITEMTITLE = 1;
        public const int ITEMSPINNER = 2;
        public const int ITEMACTIONITEM = 3;
        public const int ITEMACTIONOVERFLOW = 6;

        static AccelerateDecelerateInterpolator INTERPOLATOR = new AccelerateDecelerateInterpolator();
        int showcaseX = -1;
        int showcaseY = -1;
        float showcaseRadius = -1;
        float metricScale = 1.0f;

        bool isRedundant = false;
        bool hasCustomClickListener = false;

        ConfigOptions mOptions;
        Color mBackgroundColor;
        View mHandy;
        Button mEndButton;
        IOnShowcaseEventListener mEventListener = new NoneOnShowcaseEventListener();
        bool mAlteredText = false;
        String buttonText;
        float scaleMultiplier = 1f;
        ITextDrawer mTextDrawer;
        ClingDrawer mShowcaseDrawer;
        bool mHasNoTarget = false;


        public ConfigOptions ConfigurationOptions
        {
            get
            {
                // Make sure that this method never returns null
                if (mOptions == null)
                    mOptions = new ConfigOptions();

                return mOptions;
            }
            set
            {
                mOptions = value;
            }
        }

        public float ScaleMultiplier
        {
            get
            {
                return scaleMultiplier;
            }
            set
            {
                scaleMultiplier = value;
            }
        }

        public bool IsOneShot
        {
            get;
            set;
        }

        [Export ("setShowcaseX")]
        public void SetShowcaseX(int x)
        {
            SetShowcasePosition(x, showcaseY);
        }

        [Export ("setShowcaseY")]
        public void SetShowcaseY(int y)
        {
            SetShowcasePosition(showcaseX, y);
        }

        [Export ("getShowcaseX")]
        public int GetShowcaseX() {
            return showcaseX;
        }

        [Export ("getShowcaseY")]
        public int GetShowcaseY() {
            return showcaseY;
        }

        public ShowcaseView(Context context) : this(context, null, Resource.Styleable.CustomTheme_showcaseViewStyle)
        {
        }

        public ShowcaseView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            // Get the attributes for the ShowcaseView
            var styled = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.ShowcaseView, Resource.Attribute.showcaseViewStyle, Resource.Style.ShowcaseView);
            mBackgroundColor = styled.GetColor(Resource.Styleable.ShowcaseView_sv_backgroundColor, Color.Argb(128, 80, 80, 80));
            var showcaseColor = styled.GetColor(Resource.Styleable.ShowcaseView_sv_showcaseColor, Color.ParseColor("#33B5E5"));

            int titleTextAppearance = styled.GetResourceId(Resource.Styleable.ShowcaseView_sv_titleTextAppearance, Resource.Style.TextAppearance_ShowcaseView_Title);
            int detailTextAppearance = styled.GetResourceId(Resource.Styleable.ShowcaseView_sv_detailTextAppearance, Resource.Style.TextAppearance_ShowcaseView_Detail);

            buttonText = styled.GetString(Resource.Styleable.ShowcaseView_sv_buttonText);
            styled.Recycle();

            metricScale = Context.Resources.DisplayMetrics.Density;
            mEndButton = (Button)LayoutInflater.From(context).Inflate(Resource.Layout.showcase_button, null);

            mShowcaseDrawer = new ClingDrawer(Resources, showcaseColor);

            // TODO: This isn't ideal, ClingDrawer and Calculator interfaces should be separate
            mTextDrawer = new TextDrawer(metricScale, mShowcaseDrawer);
            mTextDrawer.SetTitleStyling(context, titleTextAppearance);
            mTextDrawer.SetDetailStyling(context, detailTextAppearance);

            var options = new ConfigOptions();
            options.ShowcaseId = Id;

            ConfigurationOptions = options;

            Init();
        }

        void Init()
        {
            SetHardwareAccelerated(true);

            bool hasShot = Context.GetSharedPreferences(PREFSSHOWCASEINTERNAL, FileCreationMode.Private).GetBoolean("hasShot" + ConfigurationOptions.ShowcaseId, false);

            if (hasShot && mOptions.IsOneShot)
            {
                // The showcase has already been shot once, so we don't need to do anything
                Visibility = ViewStates.Gone;
                isRedundant = true;
                return;
            }

            showcaseRadius = metricScale * INNERCIRCLERADIUS;
            SetOnTouchListener(this);

            if (!mOptions.NoButton && mEndButton.Parent == null)
            {
                RelativeLayout.LayoutParams lps = ConfigurationOptions.ButtonLayoutParams;

                if (lps == null)
                {
                    lps = (LayoutParams)GenerateDefaultLayoutParams();
                    lps.AddRule(LayoutRules.AlignParentBottom);
                    lps.AddRule(LayoutRules.AlignParentRight);

                    int margin = (int)(metricScale * 12);
                    lps.SetMargins(margin, margin, margin, margin);
                }
                mEndButton.LayoutParameters = lps;
                mEndButton.Text = buttonText != null ? buttonText : Resources.GetString(Resource.String.ok);

                if (!hasCustomClickListener)
                {
                    mEndButton.SetOnClickListener(this);
                }

                AddView(mEndButton);
            }
        }

        [Obsolete("Use SetShowcase() with the target of null")]
        public void SetShowcaseNoView()
        {
            SetShowcasePosition(1000000, 1000000);
        }

        /// <summary>
        /// Sets the showcase view.
        /// </summary>
        /// <param name="view">The View to showcase.</param>
        [Obsolete("Use SetShowcase with a ViewTarget")]
        public void SetShowcaseView(View view)
        {
            if (isRedundant || view == null)
            {
                isRedundant = true;
                return;
            }
            isRedundant = false;

            view.Post(() =>
            {
                Point viewPoint = Calculator.GetShowcasePointFromView(view, ConfigurationOptions);
                SetShowcasePosition(viewPoint);
                Invalidate();
            });
        }

        [Obsolete("This will soon become private. Use SetShowcase with a PointTarget")]
        public void SetShowcasePosition(Point point)
        {
            SetShowcasePosition(point.X, point.Y);
        }

        /// <summary>
        /// Set a specific position to showcase
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        [Obsolete("Use SetShowcase with a PointTarget")]
        public void SetShowcasePosition(int x, int y)
        {
            if (isRedundant)
            {
                return;
            }
            showcaseX = x;
            showcaseY = y;

            Invalidate();
        }

        public void SetShowcase(ITarget target)
        {
            SetShowcase(target, false);
        }

        public void SetShowcase(ITarget target, bool animate)
        {
            PostDelayed(() =>
            {
                if (target != null && target.GetPoint() != null)
                {
                    var targetPoint = target.GetPoint();
 
                    mHasNoTarget = false;

                    if (animate)
                    {
                        var animator = PointAnimator.OfPoints(this, targetPoint);
                        animator.SetDuration(ConfigurationOptions.FadeInDuration);
                        animator.SetInterpolator(INTERPOLATOR);
                        animator.Start();

                        // as long as the Export attribute is used is this unnessessary
//                        var values = new Point[]{targetPoint};
//
//                        var set = new Android.Animation.AnimatorSet();
//                        set.SetDuration(ConfigurationOptions.FadeInDuration);
//
//                        var xValues = new int[values.Length];
//                        var yValues = new int[values.Length];
//
//                        for (int i = 0; i < values.Length; i++)
//                        {
//                            xValues[i] = values[i].X;
//                            yValues[i] = values[i].Y;
//                        }
//
//
//                        var xAnimator = Android.Animation.ObjectAnimator.OfInt(this, "showcaseX", xValues);
//                        var yAnimator = Android.Animation.ObjectAnimator.OfInt(this, "showcaseY", yValues);
//
//
//                        var xAnimator = Android.Animation.ObjectAnimator.OfInt(xValues);
//                        xAnimator.Update+= delegate(object sender, Android.Animation.ValueAnimator.AnimatorUpdateEventArgs e) {
//                            ShowcaseX = (int) e.Animation.AnimatedValue;
//                            Console.WriteLine("x: " + ShowcaseX);
//                        };
//
//                        var yAnimator = Android.Animation.ObjectAnimator.OfInt(yValues);
//                        yAnimator.Update += delegate(object sender, Android.Animation.ValueAnimator.AnimatorUpdateEventArgs e) {
//                            ShowcaseY = (int)e.Animation.AnimatedValue;
//                            Console.WriteLine("y: " + ShowcaseY);
//                        };
//
//                        set.PlayTogether(new List<Android.Animation.Animator>(){xAnimator, yAnimator});
//
//                        //set.Play(xAnimator).Before(yAnimator);
//
//                        set.SetInterpolator(INTERPOLATOR);
//                        set.Start();
                    }
                    else
                    {
                        SetShowcasePosition(targetPoint);
                    }
                }
                else
                {
                    mHasNoTarget = true;
                    Invalidate();
                }
            }, 100);
        }

        public bool HasShowcaseView()
        {
            return (showcaseX != 1000000 && showcaseY != 1000000) || !mHasNoTarget;
        }


        [Obsolete]
        public void SetShowcaseItem(int itemType, int actionItemId, Activity activity)
        {
            Post(() =>
            {
                var reflector = BaseReflector.GetReflectorForActivity(activity);
                IViewParent p = reflector.GetActionBarView(); //ActionBarView
                var wrapper = new ActionBarViewWrapper((View)p);

                switch (itemType)
                {
                    case ITEMACTIONHOME:
                        SetShowcaseView(reflector.GetHomeButton());
                        break;
                    case ITEMSPINNER:
                        SetShowcaseView(wrapper.GetSpinnerView());
                        break;
                    case ITEMTITLE:
                        SetShowcaseView(wrapper.GetTitleView());
                        break;
                    case ITEMACTIONITEM:
                        SetShowcaseView(wrapper.GetActionItem(actionItemId));
                        break;
                    case ITEMACTIONOVERFLOW:
                        View overflow = wrapper.GetOverflowView();

                        // This check essentially checks if we are on a device with a legacy menu key
                        if (overflow != null)
                        {
                            SetShowcaseView(wrapper.GetOverflowView());
                        }
                        else
                        {
                            SetShowcasePosition(GetLegacyOverflowPoint());
                        }
                        break;
                    default:
                        Log.Error("TAG", "Unknown item type");
                        break;
                }
            });
        }

        /// <summary>
        /// Gets the bottom centre of the screen, where a legacy menu would pop up.
        /// </summary>
        /// <returns>The legacy overflow point.</returns>
        Point GetLegacyOverflowPoint()
        {
            return new Point(Left + Width / 2, Bottom);
        }

        /// <summary>
        /// Override the standard button click event.
        /// </summary>
        /// <param name="listener">Listener to listen to on click events.</param>
        public void OverrideButtonClick(IOnClickListener listener)
        {
            if (isRedundant)
            {
                return;
            }

            if (mEndButton != null)
            {
                mEndButton.SetOnClickListener(listener != null ? listener : this);
            }

            hasCustomClickListener = true;
        }

        /// <summary>
        /// Override the standard button click event.
        /// </summary>
        /// <param name="handler">The click handler.</param>
        public void OverrideButtonClick(EventHandler handler)
        {
            if (isRedundant)
            {
                return;
            }

            if (mEndButton != null)
            {
                mEndButton.Click += handler;
            }

            hasCustomClickListener = true;
        }

        public void PerformButtonClick()
        {
            mEndButton.PerformClick();
        }

        public void SetOnShowcaseEventListener(IOnShowcaseEventListener listener)
        {
            mEventListener = listener != null ? listener : new NoneOnShowcaseEventListener();
        }

        public void SetButtonText(string text)
        {
            if (mEndButton != null)
            {
                mEndButton.Text = text;
            }
        }

        public void SetHardwareAccelerated(bool accelerated)
        {
            if (accelerated)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
                {
                    if (IsHardwareAccelerated)
                    {
                        var hardwarePaint = new Paint();
                        hardwarePaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Overlay));

                        SetLayerType(LayerType.Hardware, hardwarePaint);
                    }
                    else
                    {
                        SetLayerType(LayerType.Software, null);
                    }
                }
                else
                {
                    DrawingCacheEnabled = true;
                }
            }
            else
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
                {
                    SetLayerType(LayerType.Software, null);
                }
                else
                {
                    DrawingCacheEnabled = true;
                }
            }
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if (showcaseX < 0 || showcaseY < 0 || isRedundant)
            {
                base.DispatchDraw(canvas);
                return;
            }

            bool recalculatedCling = mShowcaseDrawer.CalculateShowcaseRect(showcaseX, showcaseY);
            bool recalculateText = recalculatedCling || mAlteredText;
            mAlteredText = false;

            if (Build.VERSION.SdkInt <= BuildVersionCodes.Honeycomb && !mHasNoTarget)
            {
                var path = new Path();
                path.AddCircle(showcaseX, showcaseY, showcaseRadius, Path.Direction.Cw);
                canvas.ClipPath(path, Region.Op.Difference);
            }

            //Draw background color
            canvas.DrawColor(mBackgroundColor);

            // Draw the showcase drawable
            if (!mHasNoTarget)
            {
                mShowcaseDrawer.DrawShowcase(canvas, showcaseX, showcaseY, scaleMultiplier, showcaseRadius);
            }

            // Draw the text on the screen, recalculating its position if necessary
            if (recalculateText)
            {
                mTextDrawer.CalculateTextPosition(canvas.Width, canvas.Height, this);
            }
            mTextDrawer.Draw(canvas, recalculateText);

            base.DispatchDraw(canvas);
        }

        /// <summary>
        /// Adds an animated hand performing a gesture.
        /// All parameters passed to this method are relative to the center of the showcased view.
        /// </summary>
        /// <param name="offsetStartX">X-offset of the start position.</param>
        /// <param name="offsetStartY">Y-offset of the start position.</param>
        /// <param name="offsetEndX">X-offset of the end position.</param>
        /// <param name="offsetEndY">Y-offset of the end position.</param>
        /// <see cref="AnimateGesture(float, float, float, float, bool)"/>
        public void AnimateGesture(float offsetStartX, float offsetStartY, float offsetEndX, float offsetEndY)
        {
            AnimateGesture(offsetStartX, offsetStartY, offsetEndX, offsetEndY, false);
        }

        /// <summary>
        /// Adds an animated hand performing a gesture.
        /// </summary>
        /// <param name="startX">X-coordinate or x-offset of the start position.</param>
        /// <param name="startY">Y-coordinate or x-offset of the start position.</param>
        /// <param name="endX">X-coordinate or x-offset of the end positio.</param>
        /// <param name="endY">Y-coordinate or x-offset of the end position.</param>
        /// <param name="absoluteCoordinates">If set to <c>true</c> this will use absolute coordinates instead of coordinates relative to the center of the showcased view.</param>
        public void AnimateGesture(float startX, float startY, float endX, float endY, bool absoluteCoordinates)
        {
            mHandy = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(Resource.Layout.handy, null);
            AddView(mHandy);

            MoveHand(startX,startY,endX,endY, absoluteCoordinates, (s,e)=>{
                RemoveView(mHandy);
            });
        }

        void MoveHand(float startX, float startY, float endX, float endY, bool absoluteCoordinates, EventHandler animationEndHandler)
        {
            SharpShowcaseView.Animation.AnimationUtils.CreateMovementAnimation(mHandy, absoluteCoordinates ? 0 : showcaseX, absoluteCoordinates ? 0 : showcaseY, startX, startY, endX, endY, animationEndHandler).Start();
        }

        public void OnClick(View view)
        {
            // If the type is set to one-shot, store that it has shot
            if (mOptions.IsOneShot)
            {
                var preferences = Context.GetSharedPreferences(PREFSSHOWCASEINTERNAL, FileCreationMode.Private);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
                {
                    preferences.Edit().PutBoolean("hasShot" + ConfigurationOptions.ShowcaseId, true).Apply();
                }
                else
                {
                    preferences.Edit().PutBoolean("hasShot" + ConfigurationOptions.ShowcaseId, true).Commit();
                }
            }
            Hide();
        }

        public void Hide()
        {
            mEventListener.OnShowcaseViewHide(this);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb && ConfigurationOptions.FadeOutDuration > 0)
            {
                FadeOutShowcase();
            }
            else
            {
                Visibility = ViewStates.Gone;
                mEventListener.OnShowcaseViewDidHide(this);
            }
        }

        void FadeOutShowcase()
        {
            SharpShowcaseView.Animation.AnimationUtils.CreateFadeOutAnimation(this, (s,e) =>
            {
                Visibility = ViewStates.Gone;
                mEventListener.OnShowcaseViewHide(this);

            }).Start();
        }

        public void Show()
        {
            mEventListener.OnShowcaseViewShow(this);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb && ConfigurationOptions.FadeInDuration > 0)
            {
                FadeInShowcase();
            }
            else
            {
                Visibility = ViewStates.Visible;
            }
        }

        void FadeInShowcase()
        {
            SharpShowcaseView.Animation.AnimationUtils.CreateFadeInAnimation(this, ConfigurationOptions.FadeInDuration, (s, e) =>
            {
                Visibility = ViewStates.Visible;
            }).Start();
        }

        public bool OnTouch(View view, MotionEvent motionEvent)
        {
            float xDelta = Math.Abs(motionEvent.RawX - showcaseX);
            float yDelta = Math.Abs(motionEvent.RawY - showcaseY);
            double distanceFromFocus = Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));

            if (MotionEventActions.Up == motionEvent.Action && mOptions.HideOnClickOutside && distanceFromFocus > showcaseRadius)
            {
                Hide();
                return true;
            }

            return mOptions.Block && distanceFromFocus > showcaseRadius;
        }

        [Obsolete("Use ScaleMultiplier property")]
        public void SetShowcaseIndicatorScale(float scaleMultiplier)
        {
            ScaleMultiplier = scaleMultiplier;
        }

        public void SetText(int titleTextResId, int subTextResId)
        {
            String titleText = Context.Resources.GetString(titleTextResId);
            String subText = Context.Resources.GetString(subTextResId);
            SetText(titleText, subText);
        }

        public void SetText(String titleText, String subText)
        {
            mTextDrawer.SetTitle(titleText);
            mTextDrawer.SetDetails(subText);
            mAlteredText = true;
            Invalidate();
        }

        /// <summary>
        /// Get the ghostly gesture hand for custom gestures.
        /// </summary>
        /// <returns>A View representing the ghostly hand.</returns>
        public View GetHand()
        {
            View mHandy = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(Resource.Layout.handy, null);
            AddView(mHandy);
            SharpShowcaseView.Animation.AnimationUtils.Hide(mHandy);

            return mHandy;
        }

        /// <summary>
        /// Point to a specific view.
        /// </summary>
        /// <param name="view">The view to showcase.</param>
        [Obsolete("Use PointTo(target)")]
        public void PointTo(View view)
        {
            float x = SharpShowcaseView.Animation.AnimationUtils.GetX(view) + view.Width / 2;
            float y = SharpShowcaseView.Animation.AnimationUtils.GetY(view) + view.Height / 2;
            PointTo(x, y);
        }

        /// <summary>
        /// Point to a specific point on the screen.
        /// </summary>
        /// <param name="x">The x coordinate to point to.</param>
        /// <param name="y">The y coordinate to point to.</param>
        [Obsolete("Use PointTo(Target)")]
        public void PointTo(float x, float y)
        {
            mHandy = GetHand();
            SharpShowcaseView.Animation.AnimationUtils.CreateMovementAnimation(mHandy, x, y).Start();
        }

        /// <summary>
        /// Point to a specific point on the screen.
        /// </summary>
        /// <param name="target">The target to point to.</param>
        public void PointTo(ITarget target)
        {
            Post(() =>
            {
                mHandy = GetHand();
                Point targetPoint = target.GetPoint();
                SharpShowcaseView.Animation.AnimationUtils.CreateMovementAnimation(mHandy, targetPoint.X, targetPoint.Y).Start();
            });
        }

        /// <summary>
        /// Quick method to insert a ShowcaseView into an Activity
        /// </summary>
        /// <returns>The created ShowcaseView instance.</returns>
        /// <param name="viewToShowcase">View to showcase.</param>
        /// <param name="activity">Activity to insert into.</param>
        /// <param name="title">Text to show as a title. Can be null..</param>
        /// <param name="detailText">More detailed text. Can be null.</param>
        /// <param name="options">A set of options to customise the ShowcaseView.</param>
        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(View viewToShowcase, Activity activity, String title, String detailText, ConfigOptions options)
        {
            var sv = new ShowcaseView(activity);
            if (options != null)
            {
                sv.ConfigurationOptions = options;
            }

            if (sv.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(sv);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(sv);
            }

            sv.SetShowcaseView(viewToShowcase);
            sv.SetText(title, detailText);
            return sv;
        }

        /// <summary>
        /// Quick method to insert a ShowcaseView into an Activity.
        /// </summary>
        /// <returns>The created ShowcaseView instance.</returns>
        /// <param name="viewToShowcase">View to showcase.</param>
        /// <param name="activity">Activity to insert into.</param>
        /// <param name="title">Text to show as a title. Can be null.</param>
        /// <param name="detailText">More detailed text. Can be null.</param>
        /// <param name="options">A set of options to customise the ShowcaseView.</param>
        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(View viewToShowcase, Activity activity, int title, int detailText, ConfigOptions options)
        {
            var showcaseView = new ShowcaseView(activity);

            if (options != null)
            {
                showcaseView.ConfigurationOptions = options;
            }

            if (showcaseView.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(showcaseView);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(showcaseView);
            }

            showcaseView.SetShowcaseView(viewToShowcase);
            showcaseView.SetText(title, detailText);

            return showcaseView;
        }

        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(int showcaseViewId, Activity activity, String title, String detailText, ConfigOptions options)
        {
            View v = activity.FindViewById(showcaseViewId);

            if (v != null)
            {
                return InsertShowcaseView(v, activity, title, detailText, options);
            }

            return null;
        }

        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(int showcaseViewId, Activity activity, int title, int detailText, ConfigOptions options)
        {
            View v = activity.FindViewById(showcaseViewId);
            if (v != null)
            {
                return InsertShowcaseView(v, activity, title, detailText, options);
            }
            return null;
        }

        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(int x, int y, Activity activity, String title, String detailText, ConfigOptions options)
        {
            var showcaseView = new ShowcaseView(activity);

            if (options != null)
            {
                showcaseView.ConfigurationOptions = options;
            }

            if (showcaseView.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(showcaseView);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(showcaseView);
            }

            showcaseView.SetShowcasePosition(x, y);
            showcaseView.SetText(title, detailText);
            return showcaseView;
        }

        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(int x, int y, Activity activity, int title, int detailText, ConfigOptions options)
        {
            var showcaseView = new ShowcaseView(activity);

            if (options != null)
            {
                showcaseView.ConfigurationOptions = options;
            }

            if (showcaseView.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(showcaseView);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(showcaseView);
            }

            showcaseView.SetShowcasePosition(x, y);
            showcaseView.SetText(title, detailText);
            return showcaseView;
        }

        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseView(View showcase, Activity activity)
        {
            return InsertShowcaseView(showcase, activity, null, null, null);
        }

        /// <summary>
        /// Quickly insert a ShowcaseView into an Activity, highlighting an item.
        /// </summary>
        /// <returns>The showcase view with type.</returns>
        /// <param name="type">The type of item to showcase (can be ITEM_ACTION_HOME, ITEM_TITLE_OR_SPINNER, ITEM_ACTION_ITEM or ITEM_ACTION_OVERFLOW).</param>
        /// <param name="itemId">The ID of an Action item to showcase (only required for ITEM_ACTION_ITEM).</param>
        /// <param name="activity">Activity to insert the ShowcaseView into.</param>
        /// <param name="title">Text to show as a title. Can be null.</param>
        /// <param name="detailText">More detailed text. Can be null.</param>
        /// <param name="options">A set of options to customise the ShowcaseView.</param>
        [Obsolete("Use InsertShowcaseView with Target")]
        public static ShowcaseView InsertShowcaseViewWithType(int type, int itemId, Activity activity, String title, String detailText, ConfigOptions options)
        {
            var showcaseView = new ShowcaseView(activity);

            if (options != null)
            {
                showcaseView.ConfigurationOptions = options;
            }

            if (showcaseView.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(showcaseView);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(showcaseView);
            }

            showcaseView.SetShowcaseItem(type, itemId, activity);
            showcaseView.SetText(title, detailText);
            return showcaseView;
        }

        /// <summary>
        /// Quickly insert a ShowcaseView into an Activity, highlighting an item.
        /// </summary>
        /// <returns>The showcase view with type.</returns>
        /// <param name="type">The type of item to showcase (can be ITEM_ACTION_HOME, ITEM_TITLE_OR_SPINNER, ITEM_ACTION_ITEM or ITEM_ACTION_OVERFLOW).</param>
        /// <param name="itemId">The ID of an Action item to showcase (only required for ITEM_ACTION_ITEM).</param>
        /// <param name="activity">Activity to insert the ShowcaseView into.</param>
        /// <param name="title">Text to show as a title. Can be null.</param>
        /// <param name="detailText">More detailed text. Can be null.</param>
        /// <param name="options">A set of options to customise the ShowcaseView.</param>
        [Obsolete]
        public static ShowcaseView InsertShowcaseViewWithType(int type, int itemId, Activity activity, int title, int detailText, ConfigOptions options)
        {
            ShowcaseView sv = new ShowcaseView(activity);
            if (options != null)
            {
                sv.ConfigurationOptions = options;
            }
            if (sv.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(sv);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(sv);
            }
            sv.SetShowcaseItem(type, itemId, activity);
            sv.SetText(title, detailText);
            return sv;
        }

        [Obsolete]
        public static ShowcaseView InsertShowcaseView(int x, int y, Activity activity)
        {
            return InsertShowcaseView(x, y, activity, null, null, null);
        }

        // Internal insert method so all inserts are routed through one method
        static ShowcaseView InsertShowcaseViewInternal(ITarget target, Activity activity, String title, String detail, ConfigOptions options)
        {
            var sv = new ShowcaseView(activity);
            sv.ConfigurationOptions = options;

            if (sv.ConfigurationOptions.Insert == INSERTTODECOR)
            {
                ((ViewGroup)activity.Window.DecorView).AddView(sv);
            }
            else
            {
                ((ViewGroup)activity.FindViewById(Android.Resource.Id.Content)).AddView(sv);
            }

            sv.SetShowcase(target);
            sv.SetText(title, detail);

            return sv;
        }

        public static ShowcaseView InsertShowcaseView(ITarget target, Activity activity)
        {
            return InsertShowcaseViewInternal(target, activity, null, null, null);
        }

        public static ShowcaseView InsertShowcaseView(ITarget target, Activity activity, String title, String detail)
        {
            return InsertShowcaseViewInternal(target, activity, title, detail, null);
        }

        public static ShowcaseView InsertShowcaseView(ITarget target, Activity activity, int title, int detail)
        {
            return InsertShowcaseViewInternal(target, activity, activity.GetString(title), activity.GetString(detail), null);
        }

        public static ShowcaseView InsertShowcaseView(ITarget target, Activity activity, String title, String detail, ConfigOptions options)
        {
            return InsertShowcaseViewInternal(target, activity, title, detail, options);
        }

        public static ShowcaseView InsertShowcaseView(ITarget target, Activity activity, int title, int detail, ConfigOptions options)
        {
            return InsertShowcaseViewInternal(target, activity, activity.GetString(title), activity.GetString(detail), options);
        }


        public class ConfigOptions
        {
            public bool Block { get; set; }
            public bool NoButton { get; set; }
            public bool HideOnClickOutside { get; set; }

            /// <summary>
            /// Does not work with the {@link ShowcaseViews} class as it does not make sense (only with {@link ShowcaseView}).
            /// </summary>
            [Obsolete("Not compatible with Target API")]
            public int Insert = INSERTTODECOR;

            /// <summary>
            /// If you want to use more than one Showcase with the IsOneShot = true
            /// in one Activity, set a unique value for every different Showcase you want to use.
            /// </summary>
            public int ShowcaseId { get; set; }

            /// <summary>
            /// If you want to use more than one Showcase with IsOneShot = true in one
            /// Activity, set a unique ShowcaseId value for every different
            /// Showcase you want to use. If you want to use this in the ShowcaseViews class, you
            /// need to set a custom ShowcaseId for each ShowcaseView.
            /// </summary>
            public bool IsOneShot { get; set; }

            /// <summary>
            /// Default duration for fade in animation. Set to 0 to disable.
            /// </summary>
            public int FadeInDuration { get; set; }

            /// <summary>
            /// Default duration for fade out animation. Set to 0 to disable.
            /// </summary>
            public int FadeOutDuration { get; set; }

            /// <summary>
            /// Allow custom positioning of the button within the showcase view.
            /// </summary>
            public LayoutParams ButtonLayoutParams  { get; set; }

            /// <summary>
            /// Whether the text should be centered or stretched in the available space.
            /// </summary>
            public bool CenterText { get; set; }

            public ConfigOptions ()
            {
                Block = true;
                FadeInDuration = SharpShowcaseView.Animation.AnimationUtils.DefaultDuration;
                FadeOutDuration = SharpShowcaseView.Animation.AnimationUtils.DefaultDuration;
            }
        }
    }
}