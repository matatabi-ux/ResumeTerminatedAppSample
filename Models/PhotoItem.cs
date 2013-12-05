using System;
using System.Globalization;

namespace ResumeTerminatedAppSample.Models
{
    /// <summary>
    /// 写真情報
    /// </summary>
    public class PhotoItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string UniqueId { get; protected set; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// 写真 Uri
        /// </summary>
        public string Uri { get; protected set; }

        /// <summary>
        /// 撮影日
        /// </summary>
        public DateTime DateTaken { get; protected set; }

        /// <summary>
        /// 概要
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// タグ
        /// </summary>
        public string Tag { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="uri">写真 Uri</param>
        /// <param name="description">概要</param>
        /// <param name="tag">タグ</param>
        public PhotoItem(
            string uniqueId,
            string title,
            string uri,
            string dateTaken,
            string description = default(string),
            string tag = default(string))
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Uri = uri;
            if(!string.IsNullOrEmpty(dateTaken))
            {
                this.DateTaken = DateTime.ParseExact(dateTaken, "yyyy-MM-dd HH:mm:ss", new CultureInfo("ja-JP"));
            }
            else
            {
                this.DateTaken = DateTime.MinValue;
            }
            this.Description = description;
            this.Tag = tag;
        }
    }
}
