using ResumeTerminatedAppSample.ViewModels;
using Windows.UI.Xaml.Controls;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// GridView を表示する画面の Presenter インタフェース
    /// </summary>
    public interface IGridPagePresenter : IPagePresenter
    {
        /// <summary>
        /// ヘッダクリック時の処理
        /// </summary>
        /// <param name="group">グループ ViewModel</param>
        void HeaderClick(PhotoGroupViewModel group);

        /// <summary>
        /// アイテムクリック時の処理
        /// </summary>
        /// <param name="item">アイテム ViewModel</param>
        void ItemClick(PhotoItemViewModel item);

        /// <summary>
        /// スクロール位置を復元する処理
        /// </summary>
        /// <param name="listViewBase">GridView</param>
        void ScrollIntoView(ListViewBase listViewBase);
    }
}
