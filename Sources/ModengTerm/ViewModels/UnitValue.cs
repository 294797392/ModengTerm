using ModengTerm.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 封装一个带有单位的数值类型
    /// </summary>
    public class UnitValue : INotifyPropertyChanged, IComparable
    {
        private ulong bytes;
        private double value;
        private SizeUnitsEnum unit;

        /// <summary>
        /// 字节数
        /// </summary>
        public ulong Bytes
        {
            get { return bytes; }
            set
            {
                if (bytes != value)
                {
                    bytes = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Bytes"));
                }
            }
        }

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        /// <summary>
        /// 该值的单位
        /// </summary>
        public SizeUnitsEnum Unit
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

        public int CompareTo(object? obj)
        {
            UnitValue v2 = obj as UnitValue;

            if (v2.Unit == this.Unit && v2.Value == this.Value)
            {
                return 0;           
            }

            if (v2.Unit == this.Unit)
            {
                if (v2.value > this.Value)
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
            return string.Format("{0}{1}", this.Value, ClientUtils.Unit2Suffix(this.Unit));
        }
    }
}
