using System.Runtime.Serialization;
using ResumeTerminatedAppSample.Common;
using ResumeTerminatedAppSample.Framework;

namespace ResumeTerminatedAppSample.ViewModels
{
    /// <summary>
    /// ViewModel 基底クラス
    /// </summary>
    [DataContract]
    public class ViewModelBase : BindableBase, ICleanup
    {
        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Initilize()
        {
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public virtual void Discard()
        {
            this.Cleanup();
        }

        /// <summary>
        /// インスタンスを解放します
        /// </summary>
        public virtual void Cleanup()
        {
        }
    }
}
