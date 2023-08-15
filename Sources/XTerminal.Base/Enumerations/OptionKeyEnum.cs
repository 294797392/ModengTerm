using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Enumerations
{
    public enum OptionKeyEnum
    {
        SSH_TERM_ROW,
        SSH_TERM_COL,
        SSH_TERM_TYPE,
        SSH_SERVER_ADDR,
        SSH_SERVER_PORT,
        SSH_SERVER_USER_NAME,
        SSH_SERVER_PASSWORD,
        SSH_SERVER_PRIVATE_KEY_FILE,
        SSH_SERVER_Passphrase,
        SSH_AUTH_TYPE,

        SERIAL_PORT_NAME,
        SERIAL_PORT_BAUD_RATE,

        THEME_FONT_FAMILY,
        THEME_FONT_SIZE,
        THEME_FONT_COLOR,

        /// <summary>
        /// 输出编码格式
        /// </summary>
        WRITE_ENCODING,
        WRITE_BUFFER_SIZE,
        READ_BUFFER_SIZE,

        CURSOR_STYLE,
        CURSOR_INTERVAL,

        MOUSE_SCROLL_DELTA,


        SFTP_SERVER_ADDRESS,
        SFTP_SERVER_PORT,
        SFTP_USER_NAME,
        SFTP_USER_PASSWORD,
        SFTP_AUTH_TYPE,
        SFTP_SERVER_INITIAL_DIRECTORY,
        SFTP_CLIENT_INITIAL_DIRECTORY,
    }
}
