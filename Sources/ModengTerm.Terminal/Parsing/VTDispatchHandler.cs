using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 处理分发命令的接口
    /// </summary>
    public interface VTDispatchHandler
    {
        /// <summary>
        /// 可视区域的行数
        /// </summary>
        int ViewportRow { get; }

        /// <summary>
        /// 可视区域的列数
        /// </summary>
        int ViewportColumn { get; }

        void PlayBell();
        void Backspace();
        void ForwardTab();
        void CarriageReturn();

        // 换行和反向换行
        void LineFeed();
        void RI_ReverseLineFeed();

        void PrintCharacter(char ch);
        void EraseDisplay(VTEraseType eraseType);
        void EL_EraseLine(VTEraseType eraseType);

        // 字符操作
        void DCH_DeleteCharacter(int count);
        void ICH_InsertCharacter(int count);
        void ECH_EraseCharacters(int count);

        // 行操作
        void IL_InsertLine(int count);
        void DL_DeleteLine(int count);

        // 设备状态
        void DSR_DeviceStatusReport(StatusType statusType);
        void DA_DeviceAttributes(List<int> parameters);


        /// <summary>
        /// 设置光标在终端里的位置
        /// 左上角是0，0
        /// </summary>
        /// <param name="row">光标所在行</param>
        /// <param name="col">光标所在列</param>
        void CUP_CursorPosition(int row, int col);
        void CUF_CursorForward(int n);
        void CUU_CursorUp(int n);
        void CUD_CursorDown(int n);
        void CHA_CursorHorizontalAbsolute(int col);
        void VPA_VerticalLinePositionAbsolute(int row);

        // 滚动控制
        void SD_ScrollDown(List<int> parameters);
        void SU_ScrollUp(List<int> parameters);

        #region Margin

        /// <summary>
        /// 不可以操作滚动区域以外的行，只能对滚动区域内的行进行操作
        /// 滚动区域不包含topMargin和bottomMargin
        /// </summary>
        /// <param name="topMargin"></param>
        /// <param name="bottomMargin"></param>
        void DECSTBM_SetScrollingRegion(int topMargin, int bottomMargin);
        void DECSLRM_SetLeftRightMargins(int leftMargin, int rightMargin);

        #endregion


        void DECSC_CursorSave();
        void DECRC_CursorRestore();
        void DECKPAM_KeypadApplicationMode();
        void DECKPNM_KeypadNumericMode();


        // SGR
        void PerformSGR(GraphicsOptions options, VTColor extColor);

        void DECCKM_CursorKeysMode(bool isApplicationMode);
        void DECANM_AnsiMode(bool isAnsiMode);
        void DECAWM_AutoWrapMode(bool isAutoWrapMode);
        void ASB_AlternateScreenBuffer(bool enable);
        void XTERM_BracketedPasteMode(bool enable);
        void ATT610_StartCursorBlink(bool enable);
        void DECTCEM_TextCursorEnableMode(bool enable);

        #region 字符集

        void SS2_SingleShift();
        void SS3_SingleShift();

        void LS2_LockingShift();
        void LS3_LockingShift();
        void LS1R_LockingShift();
        void LS2R_LockingShift();
        void LS3R_LockingShift();

        /// <summary>
        /// 对指定的集合应用指定的字符集映射
        /// </summary>
        /// <param name="gsetIndex">要映射到的字符集合</param>
        /// <param name="charset">要使用的字符集映射</param>
        void Designate94Charset(int gsetIndex, ulong charset);
        void Designate96Charset(int gsetIndex, ulong charset);

        #endregion
    }
}
