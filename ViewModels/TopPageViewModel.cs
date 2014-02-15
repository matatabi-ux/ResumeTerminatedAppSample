using System.Collections.Generic;
using ResumeTerminatedAppSample.Framework;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    ///  トップ画面 ViewModel
    /// </summary>
    [DataContract]
    public class TopPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 情報取得中フラグ
        /// </summary>
        [IgnoreDataMember]
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
        [IgnoreDataMember]
        public IList<PhotoGroupViewModel> Groups { get; set; }

        /// <summary>
        /// 選択中アイテム
        /// </summary>
        private IList<ViewModelBase> selectedItems = new ObservableCollection<ViewModelBase>();

        /// <summary>
        /// 選択中アイテム
        /// </summary>
        public IList<ViewModelBase> SelectedItems 
        {
            get { return this.selectedItems; }
            set 
            {
                this.SetProperty<IList<ViewModelBase>>(ref this.selectedItems, value);
                this.OnPropertyChanged("IsAppBarOpen");
            }
        }

        /// <summary>
        /// アプリバー表示フラグ
        /// </summary>
        public bool IsAppBarOpen 
        { 
            get { return this.selectedItems.Count > 0; }
        }

        private double horizontalOffset = 0;

        [DataMember]
        public double HorizontalOffset
        {
            get { return this.horizontalOffset; }
            set { this.SetProperty<double>(ref this.horizontalOffset, value); }
        }

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
