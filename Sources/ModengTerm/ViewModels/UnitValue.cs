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
    public class UnitValue : INotifyPropertyChanged
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

        public override string ToString()
        {
            return string.Format("{0}{1}", this.Value, ClientUtils.Unit2Suffix(this.Unit));
        }
    }
}
