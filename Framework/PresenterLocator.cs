using System;
using System.Collections.Generic;
using ResumeTerminatedAppSample.Presenters;
using Windows.UI.Xaml;

namespace ResumeTerminatedAppSample.Framework
{
    /// <summary>
    /// Presenter 管理クラス
    /// </summary>
    public class PresenterLocator
    {
        #region Privates

        /// <summary>
        /// Presenter の登録情報
        /// </summary>
        private static readonly Dictionary<Type, Type> Registoriy = new Dictionary<Type, Type>();

        /// <summary>
        /// Presenter コンテナ
        /// </summary>
        private static readonly Dictionary<Type, IPresenter> Container = new Dictionary<Type, IPresenter>();

        #endregion //Privates

        /// <summary>
        /// 指定された Presenter を管理対象に登録する
        /// </summary>
        /// <typeparam name="TPresenter">登録対象の Presenter</typeparam>
        public static void Register<TPresenter>() where TPresenter : IPresenter
        {
            Registoriy.Add(typeof(TPresenter), typeof(TPresenter));
        }

        /// <summary>
        /// 指定された Presenter を View にひもづけて管理対象に登録する
        /// </summary>
        /// <typeparam name="TPresenter">登録対象の Presenter</typeparam>
        /// <typeparam name="TView">ひもづける View</typeparam>
        public static void Register<TPresenter, TView>()
            where TPresenter : IPresenter
            where TView : FrameworkElement
        {
            Registoriy.Add(typeof(TView), typeof(TPresenter));
        }

        /// <summary>
        /// 指定された View の Presenter を設定する
        /// </summary>
        /// <param name="viewType">ひもづける View</param>
        /// <param name="presenter">設定する Presenter</param>
        public static void Set(Type viewType, IPresenter presenter)
        {
            if (!Registoriy.ContainsKey(viewType))
            {
                Registoriy.Add(viewType, presenter.GetType());
            }
            Container[viewType] = presenter;
        }

        /// <summary>
        /// 指定された Presenter を設定する
        /// </summary> 
        /// <typeparam name="TPresenter">キーとなる Presenter</typeparam>
        /// <param name="presenter">設定する Presenter</param>
        public static void Set<TPresenter>(TPresenter presenter) where TPresenter : IPresenter
        {
            if (!Registoriy.ContainsKey(typeof(TPresenter)))
            {
                Registoriy.Add(typeof(TPresenter), presenter.GetType());
            }
            Container[typeof(TPresenter)] = presenter;
        }

        /// <summary>
        /// 指定された View の Presenter を取得する
        /// </summary>
        /// <param name="viewType">View</param>
        /// <returns>Presenter</returns>
        public static IPresenter Get(Type viewType)
        {
            if(!Container.ContainsKey(viewType))
            {
                var presenterType = Registoriy[viewType];

                Container.Add(viewType, Activator.CreateInstance(presenterType) as IPresenter);
            }

            return Container[viewType];
        }

        /// <summary>
        /// 指定された Presenter を取得する
        /// </summary> 
        /// <typeparam name="TPresenter">取得する Presenter</typeparam>
        /// <returns>Presenter</returns>
        public static TPresenter Get<TPresenter>() where TPresenter : IPresenter
        {
            if(!Container.ContainsKey(typeof(TPresenter)))
            {
                Container.Add(typeof(TPresenter), Activator.CreateInstance(typeof(TPresenter)) as IPresenter);
            }

            return (TPresenter)Container[typeof(TPresenter)];
        }
    }
}
