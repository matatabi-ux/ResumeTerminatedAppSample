using System;
using System.Linq;
using ResumeTerminatedAppSample.Common;
using ResumeTerminatedAppSample.Framework;
using ResumeTerminatedAppSample.ViewModels;
using ResumeTerminatedAppSample.Views;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// グループ詳細画面 Presenter 
    /// </summary>
    public class GroupPagePresenter : PagePresenterBase<GroupPage, GroupPageViewModel>, IGridPagePresenter
    {
        /// <summary>
        /// 画面読み込み時の処理
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        protected override void OnLoadState(object sender, LoadStateEventArgs e)
        {
            base.OnLoadState(sender, e);

            var mainViewModel = ViewModelLocator.Get<MainViewModel>();

            // データがなければ読み込む
            if (mainViewModel.Groups.Count == 0)
            {
                PresenterLocator.Get<MainPresenter>().LoadData();
            }

            // セッションデータがあれば復元する
            if (e.PageState != null && e.PageState.ContainsKey("currentItem"))
            {
                mainViewModel.CurrentItem = mainViewModel.GetItem(e.PageState["currentItem"] as string);
            }
            if (e.PageState != null && e.PageState.ContainsKey("currentGroup"))
            {
                mainViewModel.CurrentGroup = mainViewModel.GetGroup(e.PageState["currentGroup"] as string);
            }
            var viewModel = (GroupPageViewModel)this.ViewModel;

            var group = mainViewModel.Groups.Where(g => g.UniqueId.Equals(e.NavigationParameter as string)).FirstOrDefault();

            viewModel.Header = group.Header;
            viewModel.AllItems = group.Items;

            foreach(var item in group.Items)
            {
                Debug.WriteLine(string.Format("<photo title=\"{0}\" uri=\"{1}\"/>", item.Title, item.Uri));
            }

            this.View.DataContext = this.ViewModel;
        }

        /// <summary>
        /// セッション保存時の処理
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        protected override void OnSaveState(object sender, SaveStateEventArgs e)
        {
            var mainViewModel = ViewModelLocator.Get<MainViewModel>();

            // セッションデータに現在の状態を保存する
            e.PageState["currentItem"] = mainViewModel.CurrentItem != null ? mainViewModel.CurrentItem.UniqueId : null;
            e.PageState["currentGroup"] = mainViewModel.CurrentGroup != null ? mainViewModel.CurrentGroup.UniqueId : null;

            base.OnSaveState(sender, e);
        }

        #region IGridPagePresenter

        /// <summary>
        /// スクロール位置を復元する
        /// </summary>
        /// <param name="listViewBase">スクロールさせるコントロール</param>
        public void ScrollIntoView(ListViewBase listViewBase)
        {
            var mainViewModel = ViewModelLocator.Get<MainViewModel>();
            if (mainViewModel.CurrentItem != null)
            {
                listViewBase.ScrollIntoView(mainViewModel.CurrentItem);
                mainViewModel.CurrentItem = null;
            }
        }

        /// <summary>
        /// ヘッダクリック
        /// </summary>
        /// <param name="group">クリックしたグループ</param>
        public void HeaderClick(PhotoGroupViewModel group)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// アイテムクリック
        /// </summary>
        /// <param name="item">クリックしたアイテム</param>
        public void ItemClick(PhotoItemViewModel item)
        {
            ViewModelLocator.Get<MainViewModel>().CurrentItem = item;
            this.View.Frame.Navigate(typeof(DetailPage), item.UniqueId);
        }

        #endregion //IGridPagePresenter
    }
}
