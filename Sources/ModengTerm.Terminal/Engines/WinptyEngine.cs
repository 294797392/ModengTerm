using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace ModengTerm.Terminal.Engines
{
    /// <summary>
    /// Native API wrapper for WinPty.
    /// </summary>
    internal static class winpty
    {
        /// <summary>
        /// Marshals a LPWStr (const wchar_t *) to a string without destroying the LPWStr.
        /// </summary>
        internal class ConstLPWStrMarshaler : ICustomMarshaler
        {
            private static readonly ICustomMarshaler Instance = new ConstLPWStrMarshaler();

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return Instance;
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                return Marshal.PtrToStringUni(pNativeData);
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
            }

            public int GetNativeDataSize()
            {
                throw new NotSupportedException();
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                throw new NotSupportedException();
            }

            public void CleanUpManagedData(object ManagedObj)
            {
                throw new NotSupportedException();
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int winpty_error_code(IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstLPWStrMarshaler))]
        public static extern string winpty_error_msg(IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void winpty_error_free(IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr winpty_config_new(ulong agentFlags, out IntPtr err);


        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void winpty_config_free(IntPtr cfg);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void winpty_config_set_initial_size(IntPtr cfg, int cols, int rows);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr winpty_open(IntPtr cfg, out IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstLPWStrMarshaler))]
        public static extern string winpty_conin_name(IntPtr wp);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstLPWStrMarshaler))]
        public static extern string winpty_conout_name(IntPtr wp);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstLPWStrMarshaler))]
        public static extern string winpty_conerr_name(IntPtr wp);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr winpty_spawn_config_new(ulong spawnFlags,
                                                            string appname,
                                                            string cmdline,
                                                            string cwd,
                                                            string env,
                                                            out IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void winpty_spawn_config_free(IntPtr cfg);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool winpty_spawn(IntPtr wp,
                                               IntPtr cfg,
                                               out IntPtr process_handle,
                                               out IntPtr thread_handle,
                                               out int create_process_error,
                                               out IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool winpty_set_size(IntPtr wp, int cols, int rows, out IntPtr err);

        [DllImport("winpty.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void winpty_free(IntPtr wp);

#pragma warning restore IDE1006 // Naming Styles

        public const int WINPTY_ERROR_SUCCESS = 0;
        public const int WINPTY_ERROR_OUT_OF_MEMORY = 1;
        public const int WINPTY_ERROR_SPAWN_CREATE_PROCESS_FAILED = 2;
        public const int WINPTY_ERROR_LOST_CONNECTION = 3;
        public const int WINPTY_ERROR_AGENT_EXE_MISSING = 4;
        public const int WINPTY_ERROR_UNSPECIFIED = 5;
        public const int WINPTY_ERROR_AGENT_DIED = 6;
        public const int WINPTY_ERROR_AGENT_TIMEOUT = 7;
        public const int WINPTY_ERROR_AGENT_CREATION_FAILED = 8;

        /// <summary>
        /// Create a new screen buffer(connected to the "conerr" terminal pipe) and
        /// pass it to child processes as the STDERR handle.This flag also prevents
        /// the agent from reopening CONOUT$ when it polls -- regardless of whether the
        /// active screen buffer changes, winpty continues to monitor the original
        /// primary screen buffer.
        /// </summary>
        public const int WINPTY_FLAG_CONERR = 0x1;

        /// <summary>
        /// Don't output escape sequences.
        /// </summary>
        public const int WINPTY_FLAG_PLAIN_OUTPUT = 0x2;

        /// <summary>
        /// Do output color escape sequences.  These escapes are output by default, but
        /// are suppressed with WINPTY_FLAG_PLAIN_OUTPUT.  Use this flag to reenable
        /// them.
        /// </summary>
        public const int WINPTY_FLAG_COLOR_ESCAPES = 0x4;

        /// <summary>
        /// On XP and Vista, winpty needs to put the hidden console on a desktop in a
        /// service window station so that its polling does not interfere with other
        /// (visible) console windows.  To create this desktop, it must change the
        /// process' window station (i.e. SetProcessWindowStation) for the duration of
        /// the winpty_open call.  In theory, this change could interfere with the
        /// winpty client (e.g. other threads, spawning children), so winpty by default
        /// spawns a special agent process to create the hidden desktop.  Spawning
        /// processes on Windows is slow, though, so if
        /// WINPTY_FLAG_ALLOW_CURPROC_DESKTOP_CREATION is set, winpty changes this
        /// process' window station instead.
        /// See https://github.com/rprichard/winpty/issues/58.
        /// </summary>
        public const int WINPTY_FLAG_ALLOW_CURPROC_DESKTOP_CREATION = 0x8;

        public const int WINPTY_FLAG_MASK = (0
            | WINPTY_FLAG_CONERR
            | WINPTY_FLAG_PLAIN_OUTPUT
            | WINPTY_FLAG_COLOR_ESCAPES
            | WINPTY_FLAG_ALLOW_CURPROC_DESKTOP_CREATION);

        /// <summary>
        /// QuickEdit mode is initially disabled, and the agent does not send mouse
        /// mode sequences to the terminal.  If it receives mouse input, though, it
        /// still writes MOUSE_EVENT_RECORD values into CONIN.
        /// </summary>
        public const int WINPTY_MOUSE_MODE_NONE = 0;

        /// <summary>
        /// QuickEdit mode is initially enabled.  As CONIN enters or leaves mouse
        /// input mode (i.e. where ENABLE_MOUSE_INPUT is on and ENABLE_QUICK_EDIT_MODE
        /// is off), the agent enables or disables mouse input on the terminal.
        ///
        /// This is the default mode.
        /// </summary>
        public const int WINPTY_MOUSE_MODE_AUTO = 1;

        /// <summary>
        /// QuickEdit mode is initially disabled, and the agent enables the terminal's
        /// mouse input mode.  It does not disable terminal mouse mode (until exit).
        /// </summary>
        public const int WINPTY_MOUSE_MODE_FORCE = 2;

        /// <summary>
        /// If the spawn is marked "auto-shutdown", then the agent shuts down console
        /// output once the process exits.  The agent stops polling for new console
        /// output, and once all pending data has been written to the output pipe, the
        /// agent closes the pipe.  (At that point, the pipe may still have data in it,
        /// which the client may read.  Once all the data has been read, further reads
        /// return EOF.)
        /// </summary>
        public const int WINPTY_SPAWN_FLAG_AUTO_SHUTDOWN = 1;

        /// <summary>
        /// After the agent shuts down output, and after all output has been written
        /// into the pipe(s), exit the agent by closing the console.  If there any
        /// surviving processes still attached to the console, they are killed.
        ///
        /// Note: With this flag, an RPC call (e.g. winpty_set_size) issued after the
        /// agent exits will fail with an I/O or dead-agent error.
        /// </summary>
        public const int WINPTY_SPAWN_FLAG_EXIT_AFTER_SHUTDOWN = 2;

        /// <summary>
        /// All the spawn flags.
        /// </summary>
        public const int WINPTY_SPAWN_FLAG_MASK = (0
            | WINPTY_SPAWN_FLAG_AUTO_SHUTDOWN
            | WINPTY_SPAWN_FLAG_EXIT_AFTER_SHUTDOWN);
    }

    /// <summary>
    /// 使用winpty库实现与Windows命令行通信
    /// </summary>
    public class WinptyEngine : AbstractEngin
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("WinptySession");

        #endregion

        #region 实例变量

        private IntPtr winpty_config;
        private IntPtr winpty_handle;
        private IntPtr spawn_config;
        private NamedPipeClientStream inputStream;
        private NamedPipeClientStream outputStream;

        #endregion

        #region 构造方法

        public WinptyEngine(XTermSession options) :
            base(options)
        { }

        #endregion

        #region SessionDriver

        public override int Open()
        {
            IntPtr winpty_error;

            // 看了winpty的源码，winpty_error这个参数没使用
            this.winpty_config = winpty.winpty_config_new(winpty.WINPTY_FLAG_COLOR_ESCAPES, out winpty_error);
            if (winpty_error != IntPtr.Zero)
            {
                this.HandleWinptyError("winpty_config_new", winpty_error);
                return ResponseCode.FAILED;
            }

            // 设置终端初始大小
            int cols = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            int rows = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            winpty.winpty_config_set_initial_size(this.winpty_config, cols, rows);

            // winpty_error也没用
            this.winpty_handle = winpty.winpty_open(this.winpty_config, out winpty_error);
            if (winpty_error != IntPtr.Zero)
            {
                // 打开失败
                this.HandleWinptyError("winpty_open", winpty_error);
                this.Release();
                return ResponseCode.FAILED;
            }

            string exePath = Path.Combine(Environment.SystemDirectory, "cmd.exe");
            string envPath = Environment.GetEnvironmentVariable("PATH");
            string env = string.Empty;
            if (!string.IsNullOrEmpty(envPath))
            {
                env = string.Format("PATH={0}", envPath);
            }
            this.spawn_config = winpty.winpty_spawn_config_new(winpty.WINPTY_SPAWN_FLAG_AUTO_SHUTDOWN, exePath, string.Empty, string.Empty, env, out winpty_error);
            if (winpty_error != IntPtr.Zero)
            {
                this.HandleWinptyError("winpty_spawn_config_new", winpty_error);
                this.Release();
                return ResponseCode.FAILED;
            }

            // 与winpty建立NamedPipe信道
            this.inputStream = this.CreatePipe(winpty.winpty_conin_name(winpty_handle), PipeDirection.Out);
            this.outputStream = this.CreatePipe(winpty.winpty_conout_name(winpty_handle), PipeDirection.In);

            if (!winpty.winpty_spawn(winpty_handle, this.spawn_config, out IntPtr process, out IntPtr thread, out int procError, out winpty_error))
            {
                this.HandleWinptyError("winpty_spawn", winpty_error);
                this.Release();
                return ResponseCode.FAILED;
            }

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.Release();

            if (this.inputStream != null)
            {
                try
                {
                    this.inputStream.Dispose();
                    this.inputStream = null;
                }
                catch
                { }
            }

            if (this.outputStream != null)
            {
                try
                {
                    this.outputStream.Dispose();
                    this.outputStream = null;
                }
                catch
                { }
            }
        }

        public override void Write(byte[] bytes)
        {
            this.inputStream.Write(bytes, 0, bytes.Length);
        }

        internal override int Read(byte[] buffer)
        {
            return this.outputStream.Read(buffer, 0, buffer.Length);
        }

        public override void Resize(int row, int col)
        {
            IntPtr err;
            if (!winpty.winpty_set_size(this.winpty_handle, col, row, out err))
            {
                this.HandleWinptyError("winpty_set_size", err);
            }
        }

        #endregion

        #region 实例方法

        private void HandleWinptyError(string api, IntPtr winpty_error)
        {
            string msg = winpty.winpty_error_msg(winpty_error);
            logger.ErrorFormat("{0}失败, msg = {1}", api, msg);
            winpty.winpty_error_free(winpty_error);
        }

        private void Release()
        {
            if (this.winpty_handle != IntPtr.Zero)
            {
                // Kill the agent connection.  This will kill the agent, closing the CONIN
                // and CONOUT pipes on the agent pipe, prompting our I/O handler to shut
                // down.
                winpty.winpty_free(this.winpty_handle);
                this.winpty_handle = IntPtr.Zero;
            }

            if (this.winpty_config != IntPtr.Zero)
            {
                winpty.winpty_config_free(this.winpty_config);
                this.winpty_config = IntPtr.Zero;
            }

            if (this.spawn_config != IntPtr.Zero)
            {
                winpty.winpty_spawn_config_free(this.spawn_config);
                this.spawn_config = IntPtr.Zero;
            }
        }

        private NamedPipeClientStream CreatePipe(string pipeName, PipeDirection direction)
        {
            string serverName = ".";
            if (pipeName.StartsWith("\\"))
            {
                int slash3 = pipeName.IndexOf('\\', 2);
                if (slash3 != -1)
                {
                    serverName = pipeName.Substring(2, slash3 - 2);
                }
                int slash4 = pipeName.IndexOf('\\', slash3 + 1);
                if (slash4 != -1)
                {
                    pipeName = pipeName.Substring(slash4 + 1);
                }
            }

            try
            {
                NamedPipeClientStream pipe = new NamedPipeClientStream(serverName, pipeName, direction);
                pipe.Connect();
                return pipe;
            }
            catch (Exception ex)
            {
                logger.Error("winpty NamedPipe连接失败", ex);
                return null;
            }
        }

        #endregion

        #region 事件处理器

        #endregion
    }
}
