実はストアアプリの実行状態は正確にはもっと細かく分類されています

![５つの実行状態](https://heypyw.dm1.livefilestore.com/y2pz_WqRGqx75uiW1Go6E9vhx2B6qFeglr8tnau27Cy9rach_oxUHiP_3LZhY05JSLxXpf2K1SXjAmM2uT_bpAsvOo1Ym839quKSUPH36dx2uY/lifecycle2.PNG?psid=1)

ClosedByUser はユーザーにより終了されたかどうか以外、状態の遷移としては NotRunning と
それほど変わらないです。

しかし、Terminated はメモリ容量が少なくなった場合に OS により終了され、再度アプリが
起動する場合には元の状態に戻す復帰処理をともなうという特殊な遷移をします。
ユーザーが知らないところで OS が終了させたわけですから、再度アプリをアクティブ化して
再開させた場合に元の状態に戻ることは当然ユーザーから期待される動作ですね。
今回は具体的な実装を交えて対処法の一例をご紹介したいと思います。

## サンプルコード 

下記の GitHub リポジトリにて公開しています。

[GitHub ResumeTerminatedAppSample](https://github.com/tatsuji-kuroyanagi/ResumeTerminatedAppSample)

※情報取得に Flickr API を利用していますが、API キーを指定するまではダミーのキャッシュデータを読み込むようになっています。
 実際に Flickr のデータを取得するにはアプリを Flickr に登録し、コード内の FlickrConstants.cs に API キーを指定してください。

## 実装のポイント

* 画面の OnSaveState イベントハンドラ内で各画面に必要なデータを pageState に保存します
* 画面の OnLoadState イベントハンドラ内で各画面に必要なデータを pageState から読み込みます
* 画面共通のデータを SuspentionManager.SessionState に保存し、Terminated からの復帰時に読み込みます
* 保存するデータで利用するクラスを SuspentionManager.KnownTypes に追加します
* シリアライズできるように保存するデータのクラスに属性定義を付加します

## 具体的なやり方

グリッドアプリケーションのプロジェクトテンプレートにはセッションデータの読み書きに便利な
SuspentionManager というクラスが自動生成されます。アプリ側で独自に必要なデータを読み書き
することもできますが、今回はこの SuspentionManager をフル活用する方法で実装してみました。

下記は詳細画面に関して Terminated からのデータ復帰対応を行ったコード部分になります。
※ Presenter パターンを適用しているため ～Presenter というクラスに実装してますが、
画面用 XAML のコードビハインドしてもほぼ同じ実装になると思います。

```DetailPagePresenter.cs
/// <summary>
/// 画面読み込み時の処理
/// </summary>
/// <param name="sender">イベント発行者</param>
/// <param name="e">イベント引数</param>
protected override void OnLoadState(object sender, LoadStateEventArgs e)
{
    base.OnLoadState(sender, e);

    var mainViewModel = ViewModelLocator.Get<MainViewModel>();
    var viewModel = (DetailPageViewModel)this.ViewModel;

    // データがなければ読み込む
    if (mainViewModel.Groups.Count == 0)
    {
        PresenterLocator.Get<MainPresenter>().LoadData();
    }

    // 画面遷移時に選択されたアイテムを取得する
    mainViewModel.CurrentItem
         = mainViewModel.GetItem((string)e.NavigationParameter);

    // セッションデータがあれば復元する
    if (e.PageState != null && e.PageState.ContainsKey("selectedItem"))
    {
        viewModel.SelectedItem
            = mainViewModel.GetItem(e.PageState["selectedItem"] as string);
    }
    else
    {
        viewModel.SelectedItem = mainViewModel.CurrentItem;
    }

    viewModel.CurrentGroup = mainViewModel.GetGroup(viewModel.SelectedItem);

    this.View.DataContext = viewModel;
}

/// <summary>
/// セッション保存時の処理
/// </summary>
/// <param name="sender">イベント発行者</param>
/// <param name="e">イベント引数</param>
protected override void OnSaveState(object sender, SaveStateEventArgs e)
{
    var viewModel = (DetailPageViewModel)this.ViewModel;

    // セッションデータに現在の状態を保存する
    e.PageState["selectedItem"] = viewModel.SelectedItem.UniqueId;

    base.OnSaveState(sender, e);
}
```

OnSaveState イベントハンドラ内で現在の画面に必要なデータを pageState に保存する処理を
追加します。ここでは詳細画面で見ていた写真の ID を保存しています。

```DetailPagePresenter.cs
protected override void OnSaveState(object sender, SaveStateEventArgs e)
{
    var viewModel = (DetailPageViewModel)this.ViewModel;

    // セッションデータに現在の状態を保存する
    e.PageState["selectedItem"] = viewModel.SelectedItem.UniqueId;

    base.OnSaveState(sender, e);
}
```

ここで pageState に設定したデータは OnLoadState イベントハンドラ内で参照できるので
データが入っていた場合は利用するようにします。

```DetailPagePresenter.cs

    // セッションデータがあれば復元する
    if (e.PageState != null && e.PageState.ContainsKey("selectedItem"))
    {
        viewModel.SelectedItem
            = mainViewModel.GetItem(e.PageState["selectedItem"] as string);
    }
    else
    {
        viewModel.SelectedItem = mainViewModel.CurrentItem;
    }
```

この処理をしておくと Alt + → で進む画面遷移をした際など、以前見ていた写真がきちんと
表示されるようになります。

また、OnLoadState イベントハンドラの冒頭では、下記のように画面共通で参照する
グループやアイテムの情報があるか確認し、ない場合は復元する処理を呼び出しています。

```DetailPagePresenter.cs

    var mainViewModel = ViewModelLocator.Get<MainViewModel>();
    var viewModel = (DetailPageViewModel)this.ViewModel;

    // データがなければ読み込む
    if (mainViewModel.Groups.Count == 0)
    {
        PresenterLocator.Get<MainPresenter>().LoadData();
    }
```

呼び出し先の LoadData メソッドはこんな感じのコードになっています。

```MainPresenter.cs
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
        var sessionData = SuspensionManager.SessionState["PhotoGroups"]
                          as IList<PhotoGroupViewModel>;
        foreach (var group in sessionData)
        {
            mainViewModel.Groups.Add(group);
        }
    }
    else
    {
        SuspensionManager.SessionState.Add("PhotoGroups", 
            new List<PhotoGroupViewModel>());
    }

    // 非同期で情報取得をを開始
    this.LasyLoad().ConfigureAwait(false);
}
```

ここでようやく SuspentionManager が登場しました。SessionState に保存されている
「PhotoGroups」のデータがあれば読み込んで、各画面が参照できるようにしています。

SuspentionManager.SessionState に保存されたデータは、中断（Suspended）時に
シリアライズされてディスクに書き込まれます。そして、Terminated からの復帰時には
以下のようなコードでディスクからデシリアライズされて復元されます。

```MainPresenter.cs
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
```

このコードはプロジェクトテンプレート作成直後には App.xaml.cs に書かれているはずです。
このような仕組みでアプリ終了後にセッションデータが復元でいるのですが、内部的には
[DataContractorSerializer](http://msdn.microsoft.com/ja-jp/library/system.runtime.serialization.datacontractserializer.aspx) でシリアライズを行っているので事前に準備が必要になります。

まずは pageState や SessionState に保存するクラス（基本型以外）を、保存処理が動く前に
SuspentionManager.KnownTypes に追加しておきます。

```MainPresenter.cs
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
```

さらにそれぞれのクラスの先頭に DataContract、保存対象のプロパティに
 DataMember の属性定義を付加します。

```PhotoItemViewModel.cs
    /// <summary>
    /// 写真アイテムの ViewModel
    /// </summary>
    [DataContract]
    public class PhotoItemViewModel : ViewModelBase
    {
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
```

これで準備完了。ターミネーターに終了されても復帰時にセッションデータが
復元できるというわけです。もう写真消失事件も起きません！

![shutdown.png](https://qiita-image-store.s3.amazonaws.com/0/32633/c9452856-6d79-c5a9-ab67-7dcfc9adbf30.png)

ちなみに Terminated は VisualStudio の「デバックの場所」ツールバーに表示される
プルダウンの「中断とシャットダウン」を選ぶことで発生させられます。そのあとに
F5 で普通に実行すると Terminated からの復帰と同じ動作になるので動作確認も簡単に行えます！
