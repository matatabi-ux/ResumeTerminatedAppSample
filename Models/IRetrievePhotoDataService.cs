using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeTerminatedAppSample.Models
{
    /// <summary>
    /// 写真データ取得サービスインタフェース
    /// </summary>
    public interface IRetrievePhotoDataService
    {
        /// <summary>
        /// 写真を取得する
        /// </summary>
        /// <param name="searchTag">検索タグ</param>
        /// <returns>写真情報のリスト</returns>
        Task<IList<PhotoItem>> GetAsync(string searchTag);
    }
}
