using ResumeTerminatedAppSample.Common;
using ResumeTerminatedAppSample.Framework;
using ResumeTerminatedAppSample.ViewModels;
using ResumeTerminatedAppSample.Views;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// 画面用 Presenter
    /// </summary>
    /// <typeparam name="TView">View</typeparam>
    /// <typeparam name="TViewModel">ViewModel</typeparam>
    public class PagePresenterBase<TView, TViewModel> : IPagePresenter<TView, TViewModel>, ICleanup
        where TView : PageBase
        where TViewModel : ViewModelBase, new()
    {
        #region Privates

        /// <summary>
        /// View
        /// </summary>
        private PageBase view;

        /// <summary>
        /// ViewModel
        /// </summary>
        private ViewModelBase viewModel;

        #endregion //Privates

        /// <summary>
        /// View
        /// </summary>
        public PageBase View
        {
            get
            {
                return this.view;
            }

            set
            {
                this.view = value;
            }
        }

        /// <summary>
        /// ViewModel
        /// </summary>
        public ViewModelBase ViewModel
        {
            get
            {
                return this.viewModel;
            }

            set
            {
                this.viewModel = value;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Initialize()
        {
            this.View.NavigationHelper.LoadState += this.OnLoadState;
            this.View.NavigationHelper.SaveState += this.OnSaveState;

            this.ViewModel = ViewModelLocator.Get<TViewModel>();
            this.ViewModel.Initilize();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public virtual void Discard()
        {
            this.Cleanup();
        }

        /// <summary>
        /// 状態読み込み処理
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnLoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// 状態保存処理
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnSaveState(object sender, SaveStateEventArgs e)
        {
        }

        /// <summary>
        /// インスタンスを解放します
        /// </summary>
        public virtual void Cleanup()
        {
            this.ViewModel.Discard();

            this.View.NavigationHelper.LoadState -= this.OnLoadState;
            this.View.NavigationHelper.SaveState -= this.OnSaveState;
        }

    }
}
