using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XTerminal.Controls
{
    internal class TermLineList : ListBox
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TermLineList");

        #endregion

        #region 实例变量

        private ObservableCollection<object> lineObjects;
        private Dictionary<object, TermLine> termLines;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public TermLineList()
        {
            this.Initialize();
        }

        #endregion

        #region 实例方法

        private void Initialize()
        {
            this.termLines = new Dictionary<object, TermLine>();
            this.lineObjects = new ObservableCollection<object>();
            base.ItemsSource = this.lineObjects;

            this.lineObjects.Add(new object());
            this.lineObjects.Add(new object());
            this.lineObjects.Add(new object());
            this.lineObjects.Add(new object());
            this.lineObjects.Add(new object());
            this.lineObjects.Add(new object());
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion

        #region 事件处理器

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TermLine;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TermLine();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
            }
        }

        /// <summary>
        /// 准备指定元素以显示指定项。
        /// 元素的准备工作可能涉及应用样式、设置绑定等。
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            this.termLines[item] = element as TermLine;
        }

        #endregion
    }

    public interface ITerminalUIComponent
    {

    }
}