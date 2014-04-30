using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using SharpShowcaseView;
using SharpShowcaseView.Targets;

namespace Sample
{
    [Activity(Label = "SharpShowcaseView", MainLauncher = true)]
    public class SampleActivity : Activity, IOnShowcaseEventListener
    {
        static float ALPHA_DIM_VALUE = 0.1f;
        ShowcaseView showcaseView;
        Button buttonBlocked;
        ListView listView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            var adapter = new HardcodedListAdapter(this);

            listView = FindViewById<ListView>(Resource.Id.listView);
            listView.Adapter = adapter;
            listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
            {
                switch (e.Position)
                {
                    case 0:
                        StartActivity(typeof(MultipleShowcaseSampleActivity));
                        break;

                    case 1:
                        StartActivity(typeof(ShowcaseFragmentActivity));
                        break;

                    case 2:
                        StartActivity(typeof(AnimationSampleActivity));
                        break;

                // Not currently used
                    case 3:
                        StartActivity(typeof(MemoryManagementTesting));
                        break;
                }
            };

            DimView(listView);

            buttonBlocked = FindViewById<Button>(Resource.Id.buttonBlocked);
            buttonBlocked.Click += delegate
            {
                showcaseView.AnimateGesture(0, 0, 0, 400);
            };

            var co = new ShowcaseView.ConfigOptions();
            co.HideOnClickOutside = true;


            // The following code will reposition the OK button to the left.
//            var layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
//            layoutParams.AddRule(LayoutRules.AlignParentBottom);
//            layoutParams.AddRule(LayoutRules.AlignParentLeft);
//            int margin = (int)Resources.DisplayMetrics.Density * 12;
//            layoutParams.SetMargins(margin, margin, margin, margin);
//            co.ButtonLayoutParams = layoutParams;

            var target = new ViewTarget(Resource.Id.buttonBlocked, this);
            showcaseView = ShowcaseView.InsertShowcaseView(target, this, Resource.String.showcase_main_title, Resource.String.showcase_main_message, co);
            showcaseView.SetOnShowcaseEventListener(this);
        }

        static void DimView(View view)
        {
            if (IsHoneycombOrAbove())
            {
                view.Alpha = ALPHA_DIM_VALUE;
            }
        }

        #region IOnShowcaseEventListener implementation

        public void OnShowcaseViewHide(ShowcaseView showcaseView)
        {
            if (IsHoneycombOrAbove())
            {
                listView.Alpha = 1f;
            }
            buttonBlocked.Enabled = false;
        }

        public void OnShowcaseViewDidHide(ShowcaseView showcaseView)
        {
        }

        public void OnShowcaseViewShow(ShowcaseView showcaseView)
        {
            DimView(listView);
            buttonBlocked.Enabled = true;
        }

        #endregion

        public static bool IsHoneycombOrAbove()
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb;
        }

        class HardcodedListAdapter : ArrayAdapter
        {
            static int[] TITLE_RES_IDS = new int[]
            {
                Resource.String.title_multiple, Resource.String.title_fragments,
                Resource.String.title_animations //, Resource.String.title_memory
            };
            static int[] SUMMARY_RES_IDS = new int[]
            {
                Resource.String.sum_multiple, Resource.String.sum_fragments,
                Resource.String.sum_animations //, Resource.String.sum_memory
            };

            public HardcodedListAdapter(Context context) : base(context, Resource.Layout.item_next_thing)
            {
            }

            public override int Count
            {
                get
                {
                    return TITLE_RES_IDS.Length;
                }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (convertView == null)
                {
                    convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_next_thing, parent, false);
                }

                convertView.FindViewById<TextView>(Resource.Id.item_next_thing_textView).Text = Context.GetString(TITLE_RES_IDS[position]);
                convertView.FindViewById<TextView>(Resource.Id.item_next_thing_textView2).Text = Context.GetString(SUMMARY_RES_IDS[position]);
                return convertView;
            }
        }
    }
}