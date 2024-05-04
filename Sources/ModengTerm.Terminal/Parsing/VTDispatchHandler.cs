using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 处理分发命令的接口
    /// </summary>
    public interface VTDispatchHandler
    {
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

        // 光标控制
        void HVP_HorizontalVerticalPosition(List<int> parameters);
        void CUP_CursorPosition(List<int> parameters);
        void CUF_CursorForward(int n);
        void CUU_CursorUp(int n);
        void CUD_CursorDown(int n);
        void CHA_CursorHorizontalAbsolute(int col);
        void VPA_VerticalLinePositionAbsolute(int row);

        // 滚动控制
        void SD_ScrollDown(List<int> parameters);
        void SU_ScrollUp(List<int> parameters);

        // Margin
        void DECSTBM_SetScrollingRegion(List<int> parameters);
        void DECSLRM_SetLeftRightMargins(int leftMargin, int rightMargin);


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

        /// <summary>
        /// 对指定的字符集应用指定的字符集映射
        /// </summary>
        /// <param name="gsetIndex">要应用到的字符集</param>
        /// <param name="charset">要使用的字符集映射</param>
        void Designate94Charset(int gsetIndex, int charset);
        void Designate96Charset(int gsetIndex, int charset);

        #endregion
    }
}
