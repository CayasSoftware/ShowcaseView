using Android.OS;
using Android.Views;
using Android.Animation;
using System;

namespace SharpShowcaseView.Animation
{
    public static class AnimationUtils
    {
        public static int DefaultDuration = 300;

        static readonly string ALPHA = "alpha";
        static readonly string COORD_X = "x";
        static readonly string COORD_Y = "y";

        const float INVISIBLE = 0f;
        const float VISIBLE = 1f;
        const int INSTANT = 0;

        public static float GetX(View view)
        {
            return view.GetX();
        }

        public static float GetY(View view)
        {
            return view.GetY();
        }

        public static void Hide(View view)
        {
            view.Alpha = INVISIBLE;
        }

        public static ObjectAnimator CreateFadeInAnimation(Java.Lang.Object target, EventHandler startAnimationHandler)
        {
            return CreateFadeInAnimation(target, DefaultDuration, startAnimationHandler);
        }

        public static ObjectAnimator CreateFadeInAnimation(Java.Lang.Object target, int duration, EventHandler startAnimationHandler)
        {
            ObjectAnimator oa = ObjectAnimator.OfFloat(target, ALPHA, INVISIBLE, VISIBLE);
            oa.SetDuration(duration).AnimationStart += startAnimationHandler;

            return oa;
        }

        public static ObjectAnimator CreateFadeOutAnimation(Java.Lang.Object target, EventHandler endAnimationHandler)
        {
            return CreateFadeOutAnimation(target, DefaultDuration, endAnimationHandler);
        }

        public static ObjectAnimator CreateFadeOutAnimation(Java.Lang.Object target, int duration, EventHandler endAnimationHandler)
        {
            ObjectAnimator oa = ObjectAnimator.OfFloat(target, ALPHA, INVISIBLE);
            oa.SetDuration(duration).AnimationEnd += endAnimationHandler;

            return oa;
        }

        public static AnimatorSet CreateMovementAnimation(View view, float canvasX, float canvasY, float offsetStartX, float offsetStartY, float offsetEndX, float offsetEndY, EventHandler animationEndHandler)
        {
            view.Alpha = INVISIBLE;

            var alphaIn = ObjectAnimator.OfFloat(view, ALPHA, INVISIBLE, VISIBLE).SetDuration(500);

            var setUpX = ObjectAnimator.OfFloat(view, COORD_X, canvasX + offsetStartX).SetDuration(INSTANT);
            var setUpY = ObjectAnimator.OfFloat(view, COORD_Y, canvasY + offsetStartY).SetDuration(INSTANT);

            var moveX = ObjectAnimator.OfFloat(view, COORD_X, canvasX + offsetEndX).SetDuration(1000);
            var moveY = ObjectAnimator.OfFloat(view, COORD_Y, canvasY + offsetEndY).SetDuration(1000);
            moveX.StartDelay = 1000;
            moveY.StartDelay = 1000;

            var alphaOut = ObjectAnimator.OfFloat(view, ALPHA, INVISIBLE).SetDuration(500);
            alphaOut.StartDelay = 2500;

            var aset = new AnimatorSet();
            aset.Play(setUpX).With(setUpY).Before(alphaIn).Before(moveX).With(moveY).Before(alphaOut);

            var handler = new Handler();
            handler.PostDelayed(() =>
            {
                animationEndHandler(view, EventArgs.Empty);
            }, 3000);

            return aset;
        }

        public static AnimatorSet CreateMovementAnimation(View view, float x, float y)
        {
            var alphaIn = ObjectAnimator.OfFloat(view, ALPHA, INVISIBLE, VISIBLE).SetDuration(500);

            var setUpX = ObjectAnimator.OfFloat(view, COORD_X, x).SetDuration(INSTANT);
            var setUpY = ObjectAnimator.OfFloat(view, COORD_Y, y).SetDuration(INSTANT);

            var aset = new AnimatorSet();
            aset.Play(setUpX).With(setUpY).Before(alphaIn);
            return aset;
        }
    }
}