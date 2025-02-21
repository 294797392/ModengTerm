using ModengTerm.Document;

namespace ModengTerm.Terminal.Keyboard
{
    /// <summary>
    /// 默认的串口按键映射
    /// </summary>
    public class SerialPortKeyboard : DefaultKeyboard
    {
        private static readonly Dictionary<VTKeys, byte[]> TranslationMap = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Enter, new byte[] { (byte)'\r', (byte)'\n' } }
        };

        public override byte[] TranslateInput(VTKeyboardInput userInput)
        {
            byte[] bytes = null;
            if (TranslationMap.TryGetValue(userInput.Key, out bytes))
            {
                return bytes;
            }

            return base.TranslateInput(userInput);
        }
    }
}
