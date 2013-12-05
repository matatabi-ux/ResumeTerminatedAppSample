using System.Linq;
using ResumeTerminatedAppSample.Common;
using ResumeTerminatedAppSample.Framework;
using ResumeTerminatedAppSample.ViewModels;
using ResumeTerminatedAppSample.Views;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// アイテム詳細画面 Presenter
    /// </summary>
    public class DetailPagePresenter : PagePresenterBase<DetailPage, DetailPageViewModel>, IPagePresenter
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
            var viewModel = (DetailPageViewModel)this.ViewModel;

            // データがなければ読み込む
            if (mainViewModel.Groups.Count == 0)
            {
                PresenterLocator.Get<MainPresenter>().LoadData();
            }

            mainViewModel.CurrentItem = mainViewModel.GetItem((string)e.NavigationParameter);

            // セッションデータがあれば復元する
            if (e.PageState != null && e.PageState.ContainsKey("selectedItem"))
            {
                viewModel.SelectedItem = mainViewModel.GetItem(e.PageState["selectedItem"] as string);
            }
            else
            {
                viewModel.SelectedItem = mainViewModel.CurrentItem;
            }

            viewModel.CurrentGroup = mainViewModel.GetGroup(viewModel.SelectedItem);

            this.View.DataContext = viewModel;
        }

        /// <summary>
        /// セッション保存時の処理
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        protected override void OnSaveState(object sender, SaveStateEventArgs e)
        {
            var viewModel = (DetailPageViewModel)this.ViewModel;

            // セッションデータに現在の状態を保存する
            e.PageState["selectedItem"] = viewModel.SelectedItem.UniqueId;

            base.OnSaveState(sender, e);
        }
    }
}
