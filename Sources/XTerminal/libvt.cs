using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoTerminal
{
    public static class libvt
    {
        private const string DllName = "libvt.dll";

        #region vtssh

        public delegate void vtssh_data_received_callback(IntPtr ssh, IntPtr bytes, int bytesize);
        public delegate void vtssh_status_changed_callback(IntPtr ssh, vtssh_status_enum status);

        public enum vtssh_status_enum
        {
            VTSSH_STATUS_CONNECTING,
            VTSSH_STATUS_CONNECTED,
            VTSSH_STATUS_DISCONNECTED
        }

        public enum vtssh_auth_enum
        {
            VTSSH_AUTH_NONE = 0,
            VTSSH_AUTH_PASSWORD,
            VTSSH_AUTH_PUBLICKEY
        }

        public static class VTSSH_ERR
        {
            public const int VTSSH_ERR_OK = 0;
            public const int VTSSH_ERR_NO_MEM = 1;
            public const int VTSSH_ERR_SYSERR = 2;
            public const int VTSSH_ERR_AUTH_FAILED = 3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct vtssh_options
        {
            public vtssh_data_received_callback on_data_received;
            public vtssh_status_changed_callback on_status_changed;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] serverip;
            public int serverport;

            // 身份验证方式
            public vtssh_auth_enum auth;

            // 登录用户名
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] username;

            // 如果auth方式为PASSWORD，那么存储登录密码
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] password;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] keyfile1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] keyfile2;

            // 要请求的终端类型，vt100,xterm,xterm-256color...etc
            // 在linux里使用echo $TERM可以查看当前终端类型
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] term;

            public int term_columns;       // 终端的列数
            public int term_rows;			// 终端的行数
        }

        [DllImport(DllName)]
        public static extern int vtssh_create(out IntPtr _ssh, IntPtr ssh_options);

        [DllImport(DllName)]
        public static extern int vtssh_connect(IntPtr ssh);

        [DllImport(DllName)]
        public static extern int vtssh_send(IntPtr ssh, IntPtr bytes, int bytesize);

        [DllImport(DllName)]
        public static extern void vtssh_disconnect(IntPtr ssh);

        [DllImport(DllName)]
        public static extern void vtssh_delete(IntPtr ssh);

        #endregion
    }
}
