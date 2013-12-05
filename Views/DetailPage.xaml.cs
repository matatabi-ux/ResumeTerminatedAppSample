using ResumeTerminatedAppSample.Presenters;

namespace ResumeTerminatedAppSample.Views
{
    /// <summary>
    /// アイテム詳細画面
    /// </summary>
    public sealed partial class DetailPage : PageBase, IPageBase<DetailPagePresenter>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DetailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// この画面の Presenter
        /// </summary>
        public DetailPagePresenter Presenter
        {
            get { return this.PresenterBase as DetailPagePresenter; }
        }
    }
}
