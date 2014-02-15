using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace ResumeTerminatedAppSample.Models
{
    /// <summary>
    /// 写真データ取得 Web サービス
    /// </summary>
    /// <remarks>
    /// Flickr の API キーがある場合利用されます 
    /// </remarks>
    public class RetrievePhotoDataWebService : IRetrievePhotoDataService
    {
        #region Privates

        /// <summary>
        /// Flickr API エンドポイント
        /// </summary>
        private static readonly Uri EndPoint = new Uri(@"http://api.flickr.com/services", UriKind.Absolute);

        /// <summary>
        /// Flickr API method
        /// </summary>
        private static readonly string Method = @"flickr.photos.search";

        /// <summary>
        /// Flickr sort タイプ
        /// </summary>
        private static readonly string Sort = @"interestingness-desc";

        /// <summary>
        /// Flickr レスポンス形式
        /// </summary>
        private static readonly string Format = @"rest";

        /// <summary>
        /// 取得アイテム数
        /// </summary>
        private static readonly int PageCount = 100;

        /// <summary>
        /// 追加取得項目
        /// </summary>
        private static readonly string Extras = @"date_taken,owner_name";

        #endregion //Privates

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RetrievePhotoDataWebService()
        {
        }

        /// <summary>
        /// 写真を取得する
        /// </summary>
        /// <param name="searchTag">検索タグ</param>
        /// <returns>写真情報のリスト</returns>
        public async Task<IList<PhotoItem>> GetAsync(string searchTag)
        {
            var results = new List<PhotoItem>();
            try
            {
                var client = new HttpClient();

                var requestUri = string.Format(
                    "{0}/{1}?method={2}&per_page={3}&api_key={4}&sort={5}&tags={6}&tag_mode=all&extras={7}&license=1,2,3,4,5,6",
                    EndPoint.AbsoluteUri,
                    Format,
                    Method,
                    PageCount,
                    FlickrConstants.API_KEY,
                    Sort,
                    searchTag.ToLower(),
                    Extras);

                var response = await client.GetAsync(new Uri(requestUri));
                response.EnsureSuccessStatusCode();

                var xml = new XmlDocument();
                var txt = await response.Content.ReadAsStringAsync();
                xml.LoadXml(txt);
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
            }
            catch(WebException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return results;
        }
    }
}
