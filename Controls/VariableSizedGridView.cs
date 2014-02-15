using ResumeTerminatedAppSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ResumeTerminatedAppSample.Controls
{
    /// <summary>
    /// 色々なサイズのタイルを表示する GridView
    /// </summary>
    public class VariableSizedGridView : GridView
    {
        #region SelectedItems 依存関係プロパティ
        /// <summary>
        /// 選択中アイテム 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty SelectedItemViewModelsProperty
            = DependencyProperty.Register("SelectedItemViewModels", typeof(IList<ViewModelBase>), typeof(VariableSizedGridView), new PropertyMetadata(new ObservableCollection<ViewModelBase>()));

        /// <summary>
        /// 選択中アイテム
        /// </summary>
        public IList<ViewModelBase> SelectedItemViewModels
        {
            get { return (IList<ViewModelBase>)this.GetValue(SelectedItemViewModelsProperty); }
            set { this.SetValue(SelectedItemViewModelsProperty, value); }
        }
        #endregion //SelectedItems 依存関係プロパティ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VariableSizedGridView() : base()
        {
            this.SelectionChanged += this.OnSelectionChanged;
        }

        /// <summary>
        /// 子要素の破棄
        /// </summary>
        protected override void OnDisconnectVisualChildren()
        {
            this.SelectionChanged -= this.OnSelectionChanged;
            base.OnDisconnectVisualChildren();
        }

        /// <summary>
        /// 選択中アイテム変更イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行者</param>
        /// <param name="e">イベント引数</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    this.SelectedItemViewModels.Add((ViewModelBase)item);
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems)
                {
                    this.SelectedItemViewModels.Remove((ViewModelBase)item);
                }
            }

            var expression = this.GetBindingExpression(VariableSizedGridView.SelectedItemViewModelsProperty);
            if(expression != null && expression.DataItem != null)
            {
                expression.UpdateSource();
            }
        }

        /// <summary>
        /// 指定された項目を表示するために、指定された要素を準備します
        /// </summary>
        /// <param name="element">指定された項目を表示するために使用する要素</param>
        /// <param name="item">表示する項目</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var container = element as FrameworkElement;

            if (container != null)
            {
                // Container に ViewModel の ColumnSpan と RowSpan をバインドする
                container.SetBinding(VariableSizedWrapGrid.ColumnSpanProperty,
                    new Binding()
                    {
                        Source = item,
                        Path = new PropertyPath("ColumnSpan"),
                        Mode = BindingMode.OneTime,
                        TargetNullValue = 1,
                        FallbackValue = 1,
                    });

                container.SetBinding(VariableSizedWrapGrid.RowSpanProperty,
                    new Binding()
                    {
                        Source = item,
                        Path = new PropertyPath("RowSpan"),
                        Mode = BindingMode.OneTime,
                        TargetNullValue = 1,
                        FallbackValue = 1,
                    });
            }

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
