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
        void DCH_DeleteCharacter(List<int> parameters);
        void ICH_InsertCharacter(List<int> parameters);
        void ECH_EraseCharacters(List<int> parameters);

        // 行操作
        void IL_InsertLine(List<int> parameters);
        void DL_DeleteLine(List<int> parameters);

        // 设备状态
        void DSR_DeviceStatusReport(List<int> parameters);
        void DA_DeviceAttributes(List<int> parameters);

        // 光标控制
        void HVP_HorizontalVerticalPosition(List<int> parameters);
        void CUP_CursorPosition(List<int> parameters);
        void CUF_CursorForward(int n);
        void CUU_CursorUp(int n);
        void CUD_CursorDown(List<int> parameters);
        void CHA_CursorHorizontalAbsolute(List<int> parameters);
        void VPA_VerticalLinePositionAbsolute(List<int> parameters);

        // 滚动控制
        void SD_ScrollDown(List<int> parameters);
        void SU_ScrollUp(List<int> parameters);

        // Margin
        void DECSTBM_SetScrollingRegion(List<int> parameters);
        void DECSLRM_SetLeftRightMargins(List<int> parameters);


        void DECSC_CursorSave();
        void DECRC_CursorRestore();
        void DECKPAM_KeypadApplicationMode();
        void DECKPNM_KeypadNumericMode();


        // SGR
        void PerformSGR(GraphicsOptions options, List<int> parameters);

        void DECCKM_CursorKeysMode(bool isApplicationMode);
        void DECANM_AnsiMode(bool isAnsiMode);
        void DECAWM_AutoWrapMode(bool isAutoWrapMode);
        void ASB_AlternateScreenBuffer(bool enable);
        void XTERM_BracketedPasteMode(bool enable);
        void ATT610_StartCursorBlink(bool enable);
        void DECTCEM_TextCursorEnableMode(bool enable);
    }
}
