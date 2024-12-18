﻿using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 表示一个UI元素
    /// 存储UI元素的状态并调用绘图对象接口进行绘图操作
    /// 该对象负责判断UI元素里哪些是脏区域，仅对需要绘制的东西进行绘制，最大程度的减少绘制所产生的系统开销
    /// </summary>
    public abstract class VTElement
    {
        #region 实例变量

        private int dirtyFlags;
        private double offsetX;
        private double offsetY;
        private bool visible = true;
        private VTDocument ownerDocument;

        #endregion

        #region 属性

        /// <summary>
        /// 该元素所属的文档
        /// </summary>
        internal VTDocument OwnerDocument { get { return this.ownerDocument; } }

        /// <summary>
        /// 该元素左上角的X坐标
        /// </summary>
        public double OffsetX
        {
            get { return offsetX; }
            set
            {
                if (offsetX != value)
                {
                    offsetX = value;
                    SetDirtyFlags(VTDirtyFlags.PositionDirty, true);
                }
            }
        }

        /// <summary>
        /// 该元素左上角的Y坐标
        /// </summary>
        public double OffsetY
        {
            get { return offsetY; }
            set
            {
                if (offsetY != value)
                {
                    offsetY = value;
                    SetDirtyFlags(VTDirtyFlags.PositionDirty, true);
                }
            }
        }

        /// <summary>
        /// 设置该元素是否可见
        /// </summary>
        public bool IsVisible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    SetDirtyFlags(VTDirtyFlags.VisibleDirty, true);
                }
            }
        }

        /// <summary>
        /// 该UI元素的类型
        /// </summary>
        public abstract GraphicsObjectTypes Type { get; }

        /// <summary>
        /// 对应的绘图对象
        /// </summary>
        protected GraphicsObject DrawingObject { get; private set; }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerDocument">该元素所属的文档对象</param>
        public VTElement(VTDocument ownerDocument)
        {
            this.ownerDocument = ownerDocument;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            GraphicsInterface documentController = this.ownerDocument.GraphicsInterface;

            this.DrawingObject = documentController.CreateDrawingObject();

            this.OnInitialize();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            this.OnRelease();

            GraphicsInterface documentController = this.ownerDocument.GraphicsInterface;

            documentController.DeleteDrawingObject(this.DrawingObject);
        }

        /// <summary>
        /// 请求重绘该对象
        /// 首先判断该绘图元素是否有脏区域，有则重绘
        /// </summary>
        public void RequestInvalidate()
        {
            if (GetDirtyFlags(VTDirtyFlags.VisibleDirty))
            {
                DrawingObject.SetOpacity(IsVisible ? 1 : 0);
                SetDirtyFlags(VTDirtyFlags.VisibleDirty, false);
            }

            if (!this.IsVisible)
            {
                // 如果不显示的话直接隐藏，剩下的事情就不做了
                return;
            }

            if (GetDirtyFlags(VTDirtyFlags.PositionDirty))
            {
                DrawingObject.Arrange(OffsetX, OffsetY);
            }

            if (GetDirtyFlags(VTDirtyFlags.RenderDirty))
            {
                this.OnRender();
            }

            ResetDirtyFlags();
        }

        /// <summary>
        /// 在这个阶段，DrawingObject已经被创建出来了
        /// 子类需要做的事情就是对DrawingObject里的一些属性和字段进行初始化赋值
        /// 子类不需要调用DrawingObject的Initialize方法，OnInitialize完之后会自动调用DrawingObject.Initialize
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// 释放该绘图元素
        /// </summary>
        protected abstract void OnRelease();

        protected abstract void OnRender();

        #endregion

        #region 受保护方法

        protected void SetDirtyFlags(VTDirtyFlags flag, bool dirty)
        {
            switch (flag)
            {
                case VTDirtyFlags.VisibleDirty:
                    {
                        dirtyFlags = dirty ? dirtyFlags |= 2 : dirtyFlags &= ~2;
                        break;
                    }

                case VTDirtyFlags.SizeDirty:
                    {
                        dirtyFlags = dirty ? dirtyFlags |= 4 : dirtyFlags &= ~4;
                        break;
                    }

                case VTDirtyFlags.PositionDirty:
                    {
                        dirtyFlags = dirty ? dirtyFlags |= 8 : dirtyFlags &= ~8;
                        break;
                    }

                case VTDirtyFlags.RenderDirty:
                    {
                        dirtyFlags = dirty ? dirtyFlags |= 16 : dirtyFlags &= ~16;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        protected bool GetDirtyFlags(VTDirtyFlags flag)
        {
            switch (flag)
            {
                case VTDirtyFlags.VisibleDirty:
                    {
                        return (dirtyFlags >> 1 & 1) == 1;
                    }

                case VTDirtyFlags.SizeDirty:
                    {
                        return (dirtyFlags >> 2 & 1) == 1;
                    }

                case VTDirtyFlags.PositionDirty:
                    {
                        return (dirtyFlags >> 3 & 1) == 1;
                    }

                case VTDirtyFlags.RenderDirty:
                    {
                        return (dirtyFlags >> 4 & 1) == 1;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        protected void ResetDirtyFlags()
        {
            dirtyFlags = 0;
        }

        #endregion
    }
}
