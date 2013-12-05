using ResumeTerminatedAppSample.ViewModels;

namespace ResumeTerminatedAppSample.Framework
{
    /// <summary>
    /// ViewModel 管理クラス
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// 指定した Viewmodel を取得する
        /// </summary>
        /// <typeparam name="TViewModel">取得する ViewModel の型</typeparam>
        /// <returns>ViewModel</returns>
        public static TViewModel Get<TViewModel>() where TViewModel : ViewModelBase
        {
            if (!ServiceContainer.Instance.IsRegistered<TViewModel>())
            {
                ServiceContainer.Instance.Register<TViewModel>();
            }
            return ServiceContainer.Instance.GetInstance<TViewModel>();
        }
    }
}
