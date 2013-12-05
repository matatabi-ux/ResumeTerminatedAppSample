using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeTerminatedAppSample.Common;
using ResumeTerminatedAppSample.Framework;
using ResumeTerminatedAppSample.Models;
using ResumeTerminatedAppSample.ViewModels;
using ResumeTerminatedAppSample.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;

namespace ResumeTerminatedAppSample.Presenters
{
    /// <summary>
    /// メイン Presenter
    /// </summary>
    public class MainPresenter : IPresenter
    {
        #region Privates

        /// <summary>
        /// MainPresenter
        /// </summary>
        private static MainPresenter instance;

        /// <summary>
        /// アプリケーション
        /// </summary>
        private App app;

        /// <summary>
        /// メイン ViewModel
        /// </summary>
        private MainViewModel viewModel;

        #endregion //Privates

        #region Singleton

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private MainPresenter()
        {
        }

        /// <summary>
        /// MainPresenter
        /// </summary>
        public static MainPresenter Instance
        {
            get
            {
                return instance ?? (instance = new MainPresenter());
            }
        }

        #endregion //Singleton

        /// <summary>
        /// メイン ViewModel
        /// </summary>
        public MainViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            app = App.Current as App;

            // セッション情報のシリアライズのために保存クラスを SuspensionManager に共有する
            SuspensionManager.KnownTypes.Add(typeof(BindableBase));
            SuspensionManager.KnownTypes.Add(typeof(ViewModelBase));
            SuspensionManager.KnownTypes.Add(typeof(PhotoGroupViewModel));
            SuspensionManager.KnownTypes.Add(typeof(PhotoItemViewModel));
            SuspensionManager.KnownTypes.Add(typeof(List<PhotoGroupViewModel>));
            SuspensionManager.KnownTypes.Add(typeof(List<PhotoItemViewModel>));

            app.Suspending += this.OnSuspending;

            ServiceLocator.SetLocatorProvider(() => ServiceContainer.Instance);

            PresenterLocator.Set<MainPresenter>(this);

            // Presenter と View のひもづけ
            PresenterLocator.Register<TopPagePresenter, TopPage>();
            PresenterLocator.Register<GroupPagePresenter, GroupPage>();
            PresenterLocator.Register<DetailPagePresenter, DetailPage>();

            // API キーがない場合サンプルデータから写真データを読み込む
            if (string.IsNullOrEmpty(FlickrConstants.API_KEY))
            {
                ServiceContainer.Instance.Register<IRetrievePhotoDataService, RetrievePhotoDataCacheService>();
            }
            else
            {
                ServiceContainer.Instance.Register<IRetrievePhotoDataService, RetrievePhotoDataWebService>();
            }

            viewModel = ViewModelLocator.Get<MainViewModel>();
            viewModel.Initilize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="e">起動要求とプロセスの詳細を表示します。</param>
        public async Task InitializeAsync(LaunchActivatedEventArgs e)
        {
            this.Initialize();

            Frame rootFrame = Window.Current.Content as Frame;

            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください

            if (rootFrame == null)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
                rootFrame = new Frame();
                //フレームを SuspensionManager キーに関連付けます                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                // 既定の言語を設定します
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // 必要な場合のみ、保存されたセッション状態を復元します
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //状態の復元に何か問題があります。
                        //状態がないものとして続行します
                    }
                }

                // フレームを現在のウィンドウに配置します
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // ナビゲーションの履歴スタックが復元されていない場合、最初のページに移動します。
                // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                // 作成します
                rootFrame.Navigate(typeof(TopPage), e.Arguments);
            }

            this.dispatcher = Window.Current.Dispatcher;

            // 現在のウィンドウがアクティブであることを確認します
            Window.Current.Activate();

            SettingsPane.GetForCurrentView().CommandsRequested += this.OnCommandsRequested;
        }

        /// <summary>
        /// チャーム表示要求時
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="args">イベント引数</param>
        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("about app", "このアプリについて", (e) =>
            {
                new AboutFlyout().Show();
            }));
        }

        /// <summary>
        /// 検索タグ
        /// </summary>
        private static readonly IList<string> SearchTags = new List<string>()
        {
            "Ragdoll", "Norwegian Forest Cat", "Birman", "Maine Coon", "Somali",
            "Japanese Bobtail", "Munchkin Cats","American Shorthair",
            "Abyssinian Cat",  "Scottish Fold", "British Shorthair", 
            "Burmese Cats", "Egyptian Mau", "European Shorthair", "Havana Brown",
            "Ocicat", "Russian Blue", "Singapura", "Snowshoe", "Tonkinese",
            "American Bobtail", "Javanese", "Persian Cats", 
            "American Curl", "American Wirehair", 
        };

        /// <summary>
        /// Dispatcher
        /// </summary>
        private CoreDispatcher dispatcher;

        /// <summary>
        /// 情報取得
        /// </summary>
        /// <returns>Task</returns>
        public void LoadData()
        {
            // セッションデータがあれば復元する
            if (SuspensionManager.SessionState.ContainsKey("PhotoGroups"))
            {
                var mainViewModel = (MainViewModel)this.ViewModel;
                var sessionData = SuspensionManager.SessionState["PhotoGroups"] as IList<PhotoGroupViewModel>;
                foreach (var group in sessionData)
                {
                    mainViewModel.Groups.Add(group);
                }
            }
            else
            {
                SuspensionManager.SessionState.Add("PhotoGroups", new List<PhotoGroupViewModel>());
            }

            // 非同期で情報取得をを開始
            this.LasyLoad().ConfigureAwait(false);
        }

        /// <summary>
        /// 情報取得
        /// </summary>
        /// <returns>Task</returns>
        private async Task LasyLoad()
        {
            var mainViewModel = (MainViewModel)this.ViewModel;

            mainViewModel.IsBusy = true;

            // 検索対象タグごとに写真データを取得してグループ化する
            foreach (var tag in SearchTags)
            {
                if (mainViewModel.Groups.Where(g => tag.Equals(g.UniqueId)).Count() > 0)
                {
                    continue;
                }

                var searchTag = tag.ToLower().Contains("cat") ? tag : string.Format("{0} cat", tag);

                var results = await ServiceLocator.Current.GetInstance<IRetrievePhotoDataService>()
                    .GetAsync(searchTag);

                var group = new PhotoGroupViewModel()
                {
                    UniqueId = tag,
                    Header = tag,
                };

                // タイルサイズをランダムに振り分ける
                int columnSpan = 5;
                int rowSpan = 5;
                int count = 1;
                var random = new Random(DateTime.Now.Millisecond);

                foreach (var model in results)
                {
                    var item = PhotoItemViewModel.Convert(model);
                    if (count < 1)
                    {
                        switch (random.Next(4))
                        {
                            case 0:
                                columnSpan = 4;
                                rowSpan = 4;
                                count = 1;
                                break;

                            case 1:
                                columnSpan = 4;
                                rowSpan = 2;
                                count = 1;
                                break;

                            case 2:
                                columnSpan = 2;
                                rowSpan = 2;
                                count = 4;
                                break;

                            case 3:
                                columnSpan = 1;
                                rowSpan = 1;
                                count = 2;
                                break;
                        }
                    }
                    item.ColumnSpan = columnSpan;
                    item.RowSpan = rowSpan;
                    count--;

                    group.Items.Add(item);
                }

                group.LatestTaked = group.Items.Max(i => i.DateTaken);

                // セッションデータにグループを追加する
                ((IList<PhotoGroupViewModel>)SuspensionManager.SessionState["PhotoGroups"]).Add(group);

                // 画面に追加したグループの描画を要求する
                await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    mainViewModel.Groups.Add(group);
                });
            }

            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                mainViewModel.IsBusy = false;
            });
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Discard()
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= this.OnCommandsRequested;
            app.Suspending -= this.OnSuspending;
            app.Exit();
        }

        /// <summary>
        /// 特定のページへの移動が失敗したときに呼び出されます
        /// </summary>
        /// <param name="sender">移動に失敗したフレーム</param>
        /// <param name="e">ナビゲーション エラーの詳細</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// アプリケーションの実行が中断されたときに呼び出されます。アプリケーションの状態は、
        /// アプリケーションが終了されるのか、メモリの内容がそのままで再開されるのか
        /// わからない状態で保存されます。
        /// </summary>
        /// <param name="sender">中断要求の送信元。</param>
        /// <param name="e">中断要求の詳細。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
