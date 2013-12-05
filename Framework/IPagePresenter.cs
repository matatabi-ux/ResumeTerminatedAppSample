using ResumeTerminatedAppSample.ViewModels;
using ResumeTerminatedAppSample.Views;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// 画面用 Presenter のインタフェース
    /// </summary>
    /// <typeparam name="TView">View</typeparam>
    /// <typeparam name="TViewModel">ViewModel</typeparam>
    public interface IPagePresenter : IPresenter
    {
    }

    /// <summary>
    /// 画面用 Presenter のインタフェース
    /// </summary>
    /// <typeparam name="TView">View</typeparam>
    /// <typeparam name="TViewModel">ViewModel</typeparam>
    public interface IPagePresenter<out TView, out TViewModel> : IPresenter
        where TView : PageBase
        where TViewModel : ViewModelBase, new()
    {
        /// <summary>
        /// View
        /// </summary>
        PageBase View { get; set; }

        /// <summary>
        /// ViewModel
        /// </summary>
        ViewModelBase ViewModel { get; set; }
    }
}
