using ResumeTerminatedAppSample.Presenters;

namespace ResumeTerminatedAppSample.Views
{
    /// <summary>
    /// Presenter を持つ基底ページクラスのインタフェース
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    public interface IPageBase<TPresenter> where TPresenter : IPagePresenter
    {
        /// <summary>
        /// Presenter の Get アクセサ
        /// </summary>
        TPresenter Presenter { get; }
    }
}
