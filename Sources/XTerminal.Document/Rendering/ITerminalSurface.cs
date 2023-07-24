using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 表示一个用来渲染终端输出的表面
    /// </summary>
    public interface ITerminalSurface
    {
        /// <summary>
        /// 初始化渲染器
        /// </summary>
        /// <param name="options"></param>
        void Initialize(TerminalSurfaceOptions options);

        /// <summary>
        /// 测量某个文本行的大小
        /// 测量后的结果存储在VTextLine.Metrics属性里
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <returns></returns>
        void MeasureLine(VTextLine textLine);

        /// <summary>
        /// 测量文本块的大小
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <param name="maxCharacters">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        VTSize MeasureBlock(VTextLine textLine, int maxCharacters);

        /// <summary>
        /// 测量一行里某个字符的测量信息
        /// 注意该接口只能测量出来X偏移量，Y偏移量需要外部根据高度自己计算
        /// </summary>
        /// <param name="textLine">要测量的文本行</param>
        /// <param name="characterIndex">要测量的字符</param>
        /// <returns>文本坐标，X=文本左边的X偏移量，Y永远是0，因为边界框是相对于该行的</returns>
        VTRect MeasureCharacter(VTHistoryLine textLine, int characterIndex);

        /// <summary>
        /// 画
        /// 如果是文本元素，将对文本进行重新排版并渲染
        /// 排版是比较耗时的操作
        /// </summary>
        /// <param name="drawable"></param>
        void Draw(VTDocumentElement drawable);

        /// <summary>
        /// 根据元素的OffsetX和OffsetY属性，设置元素在Surface中的位置
        /// 而不用重新画，速度要比Draw快
        /// 画文本的速度还是比较慢的，因为需要对文本进行排版，耗时都花在排版上面了
        /// 所以能不排版就最好不排版
        /// </summary>
        /// <param name="drawable"></param>
        void Arrange(VTDocumentElement drawable);

        /// <summary>
        /// 设置元素的透明度
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="opacity"></param>
        void SetOpacity(VTDocumentElement drawable, double opacity);

        /// <summary>
        /// 获取相对于整个显示器屏幕的Canvas边界框
        /// </summary>
        /// <returns></returns>
        VTRect GetRectRelativeToDesktop();
    }
}
