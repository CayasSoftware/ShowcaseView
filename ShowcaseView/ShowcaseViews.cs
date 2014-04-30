using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.OS;
using Android.Graphics;

namespace SharpShowcaseView
{
    public class ShowcaseViews
    {
        List<ShowcaseView> views = new List<ShowcaseView>();
        List<float[]> animations = new List<float[]>();
        Activity activity;
        IOnShowcaseAcknowledged showcaseAcknowledgedListener;
        int ABSOLUTE_COORDINATES = 0;
        int RELATIVE_COORDINATES = 1;

        public interface IOnShowcaseAcknowledged
        {
            void OnShowCaseAcknowledged(ShowcaseView showcaseView);
        }

        class OnShowcaseAcknowledged : IOnShowcaseAcknowledged
        {
            #region IOnShowcaseAcknowledged implementation

            public void OnShowCaseAcknowledged(ShowcaseView showcaseView)
            {
                //DEFAULT LISTENER - DOESN'T DO ANYTHING!
            }

            #endregion
        }

        public ShowcaseViews(Activity activity)
        {
            this.activity = activity;
        }

        public ShowcaseViews(Activity activity, IOnShowcaseAcknowledged acknowledgedListener) : this(activity)
        {
            showcaseAcknowledgedListener = acknowledgedListener;
        }

        public ShowcaseViews AddView(ItemViewProperties properties)
        {
            ShowcaseViewBuilder builder = new ShowcaseViewBuilder(activity)
                .SetText(properties.TitleResId, properties.MessageResId)
                .SetShowcaseIndicatorScale(properties.Scale)
                .SetConfigOptions(properties.ConfigurationOptions);

            if (ShowcaseActionBar(properties))
            {
                builder.SetShowcaseItem((int)properties.ItemType, properties.Id, activity);
            }
            else if (properties.Id == (int)ItemViewProperties.ItemViewType.NoShowcase)
            {
                builder.SetShowcaseNoView();
            }
            else
            {
                builder.SetShowcaseView(activity.FindViewById(properties.Id));
            }

            ShowcaseView showcaseView = builder.Build();
            showcaseView.OverrideButtonClick((s,e) =>
            {
                showcaseView.OnClick(showcaseView); //Needed for TYPE_ONE_SHOT

                int fadeOutTime = showcaseView.ConfigurationOptions.FadeOutDuration;

                if (fadeOutTime > 0)
                {
                    var handler = new Handler();
                    handler.PostDelayed(() =>
                    {
                        ShowNextView(showcaseView);
                    }, fadeOutTime);
                }
                else
                {
                    ShowNextView(showcaseView);
                }
            });

            views.Add(showcaseView);

            animations.Add(null);

            return this;
        }

        /// <summary>
        /// Add an animated gesture to the view at position viewIndex
        /// </summary>
        /// <param name="viewIndex">The position of the view the gesture should be added to.</param>
        /// <param name="offsetStartX">X-coordinate or x-offset of the start position</param>
        /// <param name="offsetStartY">Y-coordinate or y-offset of the start position</param>
        /// <param name="offsetEndX">X-coordinate or x-offset of the end position.</param>
        /// <param name="offsetEndY">Y-coordinate or y-offset of the end position.</param>
        public void AddAnimatedGestureToView(int viewIndex, float offsetStartX, float offsetStartY, float offsetEndX, float offsetEndY)
        {
            AddAnimatedGestureToView(viewIndex, offsetStartX, offsetStartY, offsetEndX, offsetEndY, false);
        }

        /// <summary>
        /// Add an animated gesture to the view at position viewIndex
        /// </summary>
        /// <param name="viewIndex">The position of the view the gesture should be added to.</param>
        /// <param name="startX">X-coordinate or x-offset of the start position</param>
        /// <param name="startY">Y-coordinate or y-offset of the start position</param>
        /// <param name="endX">X-coordinate or x-offset of the end position.</param>
        /// <param name="endY">Y-coordinate or y-offset of the end position.</param>
        /// <param name="absoluteCoordinates">If set to <c>true</c>, this will use absolute coordinates instead of coordinates relative to the center of the showcased view.</param>
        public void AddAnimatedGestureToView(int viewIndex, float startX, float startY, float endX, float endY, bool absoluteCoordinates)
        {
            animations.RemoveAt(viewIndex);
            animations.Insert(viewIndex, new float[]{ absoluteCoordinates ? ABSOLUTE_COORDINATES : RELATIVE_COORDINATES, startX, startY, endX, endY });
        }

        static bool ShowcaseActionBar(ItemViewProperties properties)
        {
            return properties.ItemType != ItemViewProperties.ItemViewType.NotInActionbar;
        }

        void ShowNextView(ShowcaseView showcaseView)
        {
            if (views.Count == 0)
            {
                showcaseAcknowledgedListener.OnShowCaseAcknowledged(showcaseView);
            }
            else
            {
                Show();
            }
        }

        public void Show()
        {
            if (views.Count == 0)
            {
                return;
            }

            ShowcaseView view = views[0];

            bool hasShot = activity.GetSharedPreferences(ShowcaseView.PREFSSHOWCASEINTERNAL, FileCreationMode.Private).GetBoolean("hasShot" + view.ConfigurationOptions.ShowcaseId, false);

            if (hasShot && view.ConfigurationOptions.IsOneShot)
            {
                // The showcase has already been shot once, so we don't need to do show it again.
                view.Visibility = ViewStates.Gone;
                views.RemoveAt(0);
                animations.RemoveAt(0);

                view.ConfigurationOptions.FadeOutDuration = 0;
                view.PerformButtonClick();
                return;
            }

            view.Visibility = ViewStates.Invisible;
            ((ViewGroup)activity.Window.DecorView).AddView(view);
            view.Show();

            float[] animation = animations[0];
            if (animation != null)
            {
                view.AnimateGesture(animation[1], animation[2], animation[3], animation[4], animation[0] == ABSOLUTE_COORDINATES);
            }

            views.RemoveAt(0);
            animations.RemoveAt(0);
        }

        public bool HasViews()
        {
            return views.Count > 0;
        }

        public class ItemViewProperties
        {
            public enum ItemViewType
            {
                NoShowcase = -2202,
                NotInActionbar = -1,
                Spinner = 0,
                Title = 1,
                Overflow = 2
            }

            public static float DefaultScale = 1f;

            public int TitleResId;
            public int MessageResId;
            public int Id;
            public ItemViewType ItemType;

            public float Scale { get; set;}
            public ShowcaseView.ConfigOptions ConfigurationOptions { get; set;}

            public ItemViewProperties(int titleResId, int messageResId) : this((int)ItemViewType.NoShowcase, titleResId, messageResId, ItemViewType.NotInActionbar, DefaultScale, null)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId) : this(id, titleResId, messageResId, ItemViewType.NotInActionbar, DefaultScale, null)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, float scale) : this(id, titleResId, messageResId, ItemViewType.NotInActionbar, scale, null)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, ItemViewType itemType) : this(id, titleResId, messageResId, itemType, DefaultScale, null)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, ItemViewType itemType, float scale) : this(id, titleResId, messageResId, itemType, scale, null)
            { }

            public ItemViewProperties(int titleResId, int messageResId, ShowcaseView.ConfigOptions configOptions) : this((int)ItemViewType.NoShowcase, titleResId, messageResId, ItemViewType.NotInActionbar, DefaultScale, configOptions)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, ShowcaseView.ConfigOptions configOptions) : this(id, titleResId, messageResId, ItemViewType.NotInActionbar, DefaultScale, configOptions)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, float scale, ShowcaseView.ConfigOptions configOptions) : this(id, titleResId, messageResId, ItemViewType.NotInActionbar, scale, configOptions)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, ItemViewType itemType, ShowcaseView.ConfigOptions configOptions) : this(id, titleResId, messageResId, itemType, DefaultScale, configOptions)
            { }

            public ItemViewProperties(int id, int titleResId, int messageResId, ItemViewType itemType, float scale, ShowcaseView.ConfigOptions configOptions)
            {
                this.Id = id;
                this.TitleResId = titleResId;
                this.MessageResId = messageResId;
                this.ItemType = itemType;
                this.Scale = scale;
                this.ConfigurationOptions = configOptions;
            }
        }
    }
}