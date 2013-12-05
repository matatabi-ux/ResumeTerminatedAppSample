using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 指定された項目を表示するために、指定された要素を準備します
        /// </summary>
        /// <param name="element">指定された項目を表示するために使用する要素</param>
        /// <param name="item">表示する項目</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var container = element as FrameworkElement;

            if(container != null)
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
