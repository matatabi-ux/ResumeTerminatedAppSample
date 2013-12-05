using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ResumeTerminatedAppSample.Framework;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    /// グループ詳細画面 ViewModel
    /// </summary>
    public class GroupPageViewModel : ViewModelBase
    {
        #region Privates
        #endregion //Privates

        /// <summary>
        /// グループヘッダ
        /// </summary>
        public string Header { get; set; }
        
        /// <summary>
        /// すべてのアイテム
        /// </summary>
        public IList<PhotoItemViewModel> AllItems { get; set; }

        /// <summary>
        /// 先頭のアイテム
        /// </summary>
        public PhotoItemViewModel TopItem
        {
            get { return this.AllItems.FirstOrDefault(); }
        }

        /// <summary>
        /// その他のアイテム
        /// </summary>
        public IList<PhotoItemViewModel> Items 
        { 
            get { return this.AllItems.Skip(1).ToList(); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupPageViewModel()
        {
        }
    }
}
