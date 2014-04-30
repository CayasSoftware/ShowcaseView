
namespace SharpShowcaseView
{
    public interface IOnShowcaseEventListener
    {
        void OnShowcaseViewHide(ShowcaseView showcaseView);

        void OnShowcaseViewDidHide(ShowcaseView showcaseView);

        void OnShowcaseViewShow(ShowcaseView showcaseView);
    }

    /// <summary>
    /// Empty implementation of IOnShowcaseViewEventListener such that null checks aren't needed
    /// </summary>
    public class NoneOnShowcaseEventListener : IOnShowcaseEventListener
    {
        #region IOnShowcaseEventListener implementation

        public void OnShowcaseViewHide(ShowcaseView showcaseView)
        {
        }

        public void OnShowcaseViewDidHide(ShowcaseView showcaseView)
        {

        }

        public void OnShowcaseViewShow(ShowcaseView showcaseView)
        {

        }

        #endregion
    }
}