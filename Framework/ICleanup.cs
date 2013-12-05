
namespace ResumeTerminatedAppSample.Framework
{
    /// <summary>
    /// 解放可能なインスタンスのインタフェース
    /// </summary>
    public interface ICleanup
    {
        /// <summary>
        /// インスタンスを解放します
        /// </summary>
        void Cleanup();
    }
}
