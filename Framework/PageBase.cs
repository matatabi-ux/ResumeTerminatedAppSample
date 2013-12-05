using ResumeTerminatedAppSample.Common;
using ResumeTerminatedAppSample.Framework;
using ResumeTerminatedAppSample.Presenters;
using ResumeTerminatedAppSample.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ResumeTerminatedAppSample.Views
{
    /// <summary>
    /// 画面の基底クラス
    /// </summary>
    public class PageBase : Page 
    {
        #region Privates

        /// <summary>
        /// Presenter
        /// </summary>
        private IPagePresenter<PageBase, ViewModelBase> presenter;

        /// <summary>
        /// NavigationHelper
        /// </summary>
        private NavigationHelper navigationHelper;

        #endregion //Privates

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PageBase() 
        {
            this.navigationHelper = new NavigationHelper(this);
        }

        /// <summary>
        /// この画面の Presenter
        /// </summary>
        public virtual IPagePresenter<PageBase, ViewModelBase> PresenterBase
        {
            get { return this.presenter; }
        }

        /// <summary>
        /// NavigationHelper は、ナビゲーションおよびプロセス継続時間管理を
        /// 支援するために、各ページで使用します。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        #region NavigationHelper の登録

        /// <summary>
        /// この画面への遷移時の処理
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.presenter = PresenterLocator.Get(this.GetType()) as IPagePresenter<PageBase, ViewModelBase>;
            this.presenter.View = this;
            this.presenter.Initialize();

            navigationHelper.OnNavigatedTo(e);
        }

        /// <summary>
        /// この画面から離れる時の処理
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);

            this.presenter.Discard();
        }

        #endregion
    }
}
