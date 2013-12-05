using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    /// メイン ViewModel
    /// </summary>
    [DataContract]
    public class MainViewModel : ViewModelBase
    {
        #region Privates

        /// <summary>
        /// 情報取得中フラグ
        /// </summary>
        private bool isBusy = false;

        #endregion //Privates

        /// <summary>
        /// 情報取得中フラグ
        /// </summary>
        [IgnoreDataMember]
        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.SetProperty<bool>(ref this.isBusy, value); }
        }

        /// <summary>
        /// 写真情報
        /// </summary>
        [DataMember]
        public ObservableCollection<PhotoGroupViewModel> Groups { get; set; }

        /// <summary>
        /// 現在のアイテム
        /// </summary>
        [IgnoreDataMember]
        public PhotoItemViewModel CurrentItem { get; set; }

        /// <summary>
        /// 現在のグループ
        /// </summary>
        [IgnoreDataMember]
        public PhotoGroupViewModel CurrentGroup { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            this.Groups = new ObservableCollection<PhotoGroupViewModel>();
        }

        /// <summary>
        /// 指定した ID のアイテムを取得する
        /// </summary>
        /// <param name="uniqueId">取得対象のアイテムの ID</param>
        /// <returns>アイテム</returns>
        public PhotoItemViewModel GetItem(string uniqueId)
        {
            return this.Groups
                .SelectMany<PhotoGroupViewModel, PhotoItemViewModel>(g => g.Items)
                .Where(i => i.UniqueId.Equals(uniqueId)).FirstOrDefault();
        }

        /// <summary>
        /// 指定した ID のグループを取得する
        /// </summary>
        /// <param name="uniqueId">取得対象のグループの ID</param>
        /// <returns>グループ</returns>
        public PhotoGroupViewModel GetGroup(string uniqueId)
        {
            return this.Groups
                .Where(g => g.UniqueId.Equals(uniqueId)).FirstOrDefault();
        }

        /// <summary>
        /// 指定した アイテムが属するグループを取得する
        /// </summary>
        /// <param name="item">アイテム</param>
        /// <returns>グループ</returns>
        public PhotoGroupViewModel GetGroup(PhotoItemViewModel item)
        {
            return this.Groups
                .Where(g => g.Items.Contains(item)).FirstOrDefault();
        }
    }
}
