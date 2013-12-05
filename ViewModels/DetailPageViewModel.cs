
namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    /// 詳細画面 ViewModel
    /// </summary>
    public class DetailPageViewModel : ViewModelBase
    {
        #region Privates

        /// <summary>
        /// 現在のアイテム
        /// </summary>
        private PhotoItemViewModel currentItem;
        
        #endregion //Privates

        /// <summary>
        /// 現在のアイテム
        /// </summary>
        public PhotoItemViewModel SelectedItem 
        { 
            get{ return this.currentItem; }
            set { this.SetProperty(ref this.currentItem, value); }
        }

        /// <summary>
        /// 現在のグループ
        /// </summary>
        public PhotoGroupViewModel CurrentGroup { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DetailPageViewModel()
        {
        }
    }
}
