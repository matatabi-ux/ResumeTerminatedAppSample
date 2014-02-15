using ResumeTerminatedAppSample.Controls;
using ResumeTerminatedAppSample.Framework;
using ResumeTerminatedAppSample.Presenters;
using ResumeTerminatedAppSample.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ResumeTerminatedAppSample.Views
{
    /// <summary>
    /// トップ画面
    /// </summary>
    public sealed partial class TopPage : PageBase, IPageBase<IGridPagePresenter>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TopPage()
            : base()
        {
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
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
        /// ヘッダクリックイベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnHeaderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            this.Presenter.HeaderClick(button.DataContext as PhotoGroupViewModel);
        }

        /// <summary>
        /// 選択中アイテム変更イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = this.DataContext as TopPageViewModel;
            //this.BottomAppBar.IsOpen = vm.SelectedItems.Count > 0;
        }
    }
}
