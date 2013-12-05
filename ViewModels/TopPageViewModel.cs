using System.Collections.Generic;
using ResumeTerminatedAppSample.Framework;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    ///  トップ画面 ViewModel
    /// </summary>
    public class TopPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 情報取得中フラグ
        /// </summary>
        public bool IsBusy
        {
            get 
            { 
                return ViewModelLocator.Get<MainViewModel>().IsBusy;
            }
        }

        /// <summary>
        /// 写真情報
        /// </summary>
        public IList<PhotoGroupViewModel> Groups { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TopPageViewModel()
        {
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initilize()
        {
            base.Initilize();

            this.OnPropertyChanged("IsBusy");
            ViewModelLocator.Get<MainViewModel>().PropertyChanged += this.OnMainViewModelPropertyChanged;
        }

        /// <summary>
        /// MainViewModel の状態更新イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnMainViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// インスタンスを解放します
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();

            ViewModelLocator.Get<MainViewModel>().PropertyChanged -= this.OnMainViewModelPropertyChanged;
        }
    }
}
