using ResumeTerminatedAppSample.ViewModels;
using Windows.UI.Xaml;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// Presenter のインタフェース
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// 初期化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 終了処理
        /// </summary>
        void Discard();
    }

    /// <summary>
    /// Presenter のインタフェース
    /// </summary>
    /// <typeparam name="TView">View</typeparam>
    /// <typeparam name="TViewModel">ViewModel</typeparam>
    public interface IPresenter<TView, TViewModel> : IPresenter
        where TView : FrameworkElement
        where TViewModel : ViewModelBase, new()
    {
        /// <summary>
        /// View
        /// </summary>
        TView View { get; set; }

        /// <summary>
        /// ViewModel
        /// </summary>
        TViewModel ViewModel { get; set; }
    }
}
