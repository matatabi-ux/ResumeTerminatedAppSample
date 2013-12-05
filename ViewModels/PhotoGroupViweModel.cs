using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    /// 写真グループ ViewModel
    /// </summary>
    [DataContract]
    public class PhotoGroupViewModel : ViewModelBase
    {
        #region Privates

        /// <summary>
        /// 写真アイテム
        /// </summary>
        private IList<PhotoItemViewModel> items;

        #endregion //Privates

        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public string UniqueId { get; set; }

        /// <summary>
        /// ヘッダ
        /// </summary>
        [DataMember]
        public string Header { get; set; }

        /// <summary>
        /// 撮影日
        /// </summary>
        [DataMember]
        public DateTime LatestTaked { get; set; }

        /// <summary>
        /// 写真アイテム
        /// </summary>
        [DataMember]
        public IList<PhotoItemViewModel> Items 
        { 
            get { return this.items; }
            
            set 
            { 
                this.SetProperty(ref this.items, value);
                this.OnPropertyChanged("TopItems");
            }
        }

        /// <summary>
        /// 上位 10 写真アイテム
        /// </summary>
        [IgnoreDataMember]
        public IList<PhotoItemViewModel> TopItems
        {
            get
            {
                return this.Items.Take<PhotoItemViewModel>(10).ToList();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PhotoGroupViewModel()
        {
            this.Items = new List<PhotoItemViewModel>();
        }
    }
}
