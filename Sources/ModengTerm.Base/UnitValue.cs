using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    /// <summary>
    /// 指定内存单位的显示方式
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// 字节
        /// </summary>
        Byte = 0,

        /// <summary>
        /// 以KB显示
        /// </summary>
        KB,

        /// <summary>
        /// 以MB方式显示
        /// </summary>
        MB,

        /// <summary>
        /// 以GB方式显示
        /// </summary>
        GB,

        TB
    }

    /// <summary>
    /// 封装一个带有单位的数值类型
    /// </summary>
    public abstract class UnitValue : INotifyPropertyChanged, IComparable
    {
        private UnitType unit;

        /// <summary>
        /// 该值的单位
        /// </summary>
        public UnitType Unit
        {
            get { return this.unit; }
            set
            {
                if (this.unit != value)
                {
                    this.unit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Unit"));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public abstract int CompareTo(object? obj);

        public void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UnitValue64 : UnitValue
    {
        private ulong value;

        /// <summary>
        /// 值
        /// </summary>
        public ulong Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }

        public override int CompareTo(object? obj)
        {
            UnitValue64 v2 = obj as UnitValue64;

            if (v2.Unit == this.Unit && v2.Value == this.Value)
            {
                return 0;
            }

            if (v2.Unit == this.Unit)
            {
                if (v2.Value > this.Value)
                {
                    return -1;
                }

                return 1;
            }
            else
            {
                if (v2.Unit > this.Unit)
                {
                    return -1;
                }

                return 1;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", this.value, VTBaseUtils.Unit2Suffix(this.Unit));
        }
    }

    public class UnitValueDouble : UnitValue
    {
        private ulong fromValue;

        /// <summary>
        /// 从哪个值转换过来的
        /// </summary>
        public ulong FromValue 
        {
            get { return this.fromValue; }
            set
            {
                if (this.fromValue != value) 
                {
                    this.fromValue = value;
                }
            }
        }

        private double value;

        /// <summary>
        /// 值
        /// </summary>
        public double Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }

        public override int CompareTo(object? obj)
        {
            UnitValueDouble v2 = obj as UnitValueDouble;

            if (v2.Unit == this.Unit && v2.Value == this.Value)
            {
                return 0;
            }

            if (v2.Unit == this.Unit)
            {
                if (v2.Value > this.Value)
                {
                    return -1;
                }

                return 1;
            }
            else
            {
                if (v2.Unit > this.Unit)
                {
                    return -1;
                }

                return 1;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", this.value, VTBaseUtils.Unit2Suffix(this.Unit));
        }
    }
}
