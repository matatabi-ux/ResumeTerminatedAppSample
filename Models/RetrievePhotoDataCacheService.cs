using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace ResumeTerminatedAppSample.Models
{
    /// <summary>
    /// キャッシュファイルから写真データを取得するサービス
    /// </summary>
    /// <remarks>
    /// Flickr の API キーがない場合利用されます 
    /// </remarks>
    public class RetrievePhotoDataCacheService : IRetrievePhotoDataService
    {
        /// <summary>
        /// 写真を取得する
        /// </summary>
        /// <param name="searchTag">検索タグ</param>
        /// <returns>写真情報のリスト</returns>
        public async Task<IList<PhotoItem>> GetAsync(string searchTag)
        {
            // ダウンロード感を出すためのダミーウェイト
            await Task.Delay(500);

            var results = new List<PhotoItem>();

            var file = await StorageFile.GetFileFromApplicationUriAsync(
                new Uri(string.Format("ms-appx:///Assets/Data/{0}.xml",
                    searchTag.Replace(" cat", string.Empty).Replace(" ", "_"))));

            var xml = new XmlDocument();
            using (var stream = await file.OpenStreamForReadAsync())
            using( var reader = new StreamReader(stream))
            {
                xml.LoadXml(reader.ReadToEnd());
            }

            foreach (var photo in xml.GetElementsByTagName("photo"))
            {
                var uri = string.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg",
                    photo.Attributes.GetNamedItem("farm").NodeValue as string,
                    photo.Attributes.GetNamedItem("server").NodeValue as string,
                    photo.Attributes.GetNamedItem("id").NodeValue as string,
                    photo.Attributes.GetNamedItem("secret").NodeValue as string);

                results.Add(new PhotoItem(
                    photo.Attributes.GetNamedItem("id").NodeValue as string,
                    photo.Attributes.GetNamedItem("title").NodeValue as string,
                    uri,
                    photo.Attributes.GetNamedItem("datetaken").NodeValue as string,
                    photo.Attributes.GetNamedItem("ownername").NodeValue as string,
                    searchTag));
            }

            return results;
        }
    }
}
