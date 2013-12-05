using ResumeTerminatedAppSample.Presenters;
using ResumeTerminatedAppSample.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ResumeTerminatedAppSample.Views
{
    /// <summary>
    /// グループ詳細画面
    /// </summary>
    public sealed partial class GroupPage : PageBase, IPageBase<IGridPagePresenter>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// この画面の Presenter
        /// </summary>
        public IGridPagePresenter Presenter
        {
            get { return this.PresenterBase as IGridPagePresenter; }
        }

        /// <summary>
        /// GridView サイズ変更イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            this.Presenter.ScrollIntoView(sender as ListViewBase);
        }

        /// <summary>
        /// タイルクリックイベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            this.Presenter.ItemClick(e.ClickedItem as PhotoItemViewModel);
        }

        /// <summary>
        /// トップアイテムクリックイベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnHeaderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var viewModel = button.DataContext as GroupPageViewModel;

            this.Presenter.ItemClick(viewModel.TopItem);
        }
    }
}
