using System;
using System.Runtime.Serialization;
using ResumeTerminatedAppSample.Models;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    /// 写真アイテムの ViewModel
    /// </summary>
    [DataContract]
    public class PhotoItemViewModel : ViewModelBase
    {
        #region Privates

        /// <summary>
        /// ID
        /// </summary>
        private string uniqueId;

        /// <summary>
        /// Column サイズ
        /// </summary>
        private int columnSpan = 2;

        /// <summary>
        /// Row サイズ
        /// </summary>
        private int rowSpan = 2;

        /// <summary>
        /// タイトル
        /// </summary>
        private string title;

        /// <summary>
        /// 写真 Uri
        /// </summary>
        private string uri;

        /// <summary>
        /// 撮影日
        /// </summary>
        private DateTime dataTaken;

        /// <summary>
        /// 概要
        /// </summary>
        private string description;

        /// <summary>
        /// タグ
        /// </summary>
        private string tag;

        #endregion //Privates

        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public string UniqueId 
        {
            get { return this.uniqueId; }
            set { this.SetProperty(ref this.uniqueId, value); }
        }

        /// <summary>
        /// Column サイズ
        /// </summary>
        [DataMember]
        public int ColumnSpan
        {
            get { return this.columnSpan; }
            set { this.SetProperty(ref this.columnSpan, value); }
        }

        /// <summary>
        /// Row サイズ
        /// </summary>
        [DataMember]
        public int RowSpan
        {
            get { return this.rowSpan; }
            set { this.SetProperty(ref this.rowSpan, value); }
        }

        /// <summary>
        /// タイトル
        /// </summary>
        [DataMember]
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        /// <summary>
        /// 写真 Uri
        /// </summary>
        [DataMember]
        public string Uri
        {
            get { return this.uri; }
            set { this.SetProperty(ref this.uri, value); }
        }

        /// <summary>
        /// 撮影日
        /// </summary>
        [DataMember]
        public DateTime DateTaken
        {
            get { return this.dataTaken; }
            set { this.SetProperty(ref this.dataTaken, value); }
        }

        /// <summary>
        /// 概要
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        /// <summary>
        /// タグ
        /// </summary>
        [DataMember]
        public string Tag
        {
            get { return this.tag; }
            set { this.SetProperty(ref this.tag, value); }
        }

        [IgnoreDataMember]
        public bool IsShowTitle
        {
            get
            {
                return this.columnSpan > 1 && this.rowSpan > 1;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PhotoItemViewModel()
        {
        }

        /// <summary>
        /// PhotoItem から PhotoItemViewModel を生成します
        /// </summary>
        /// <param name="source">PhotoItem</param>
        /// <returns>PhotoItemViewModel</returns>
        public static PhotoItemViewModel Convert(PhotoItem source)
        {
            return new PhotoItemViewModel()
            {
                UniqueId = source.UniqueId,
                Uri = source.Uri,
                DateTaken = source.DateTaken,
                Title = source.Title,
                Description = source.Description,
                Tag = source.Tag,
            };
        }
    }
}
