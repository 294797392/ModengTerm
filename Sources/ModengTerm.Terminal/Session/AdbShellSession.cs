using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ModengTerm.Terminal.Session
{
    /// <summary>
    /// adb 的工作原理：
    /// 当您启动某个adb客户端时，该客户端会先检查是否有adb服务器进程已在运行。如果没有，它会启动服务器进程。服务器在启动后会与本地TCP端口5037绑定，并监听adb客户端发出的命令。
    /// </summary>
    public class AdbShellSession : SessionDriver
    {
        #region 类变量

        private static readonly byte[] LF = new byte[] { (byte)'\n' };

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("AdbShellSession");

        #endregion

        #region 实例变量

        private Process process;
        private Stream readStream;
        private Stream writeStream;

        /// <summary>
        /// 保存在登录成功之前读取到的所有数据
        /// 登录成功之后先返回这些数据
        /// </summary>
        private List<byte> pendingReads;
        private AutoResetEvent loginEvent;

        #endregion

        #region 构造方法

        public AdbShellSession(XTermSession session) : base(session)
        {

        }

        #endregion

        #region SessionDriver

        public override int Open()
        {
            this.pendingReads = new List<byte>();

            int timeout = this.session.GetOption<int>(OptionKeyEnum.ADBSH_LOGIN_TIMEOUT);
            string exePath = this.session.GetOption<string>(OptionKeyEnum.ADBSH_ADB_PATH);
            AdbLoginTypeEnum loginType = this.session.GetOption<AdbLoginTypeEnum>(OptionKeyEnum.ADBSH_LOGIN_TYPE);

            if (!this.EnsureStartServer()) 
            {
                return ResponseCode.FAILED;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                Arguments = "shell"
            };

            try
            {
                this.process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                logger.Error("启动adb进程异常", ex);
                return ResponseCode.FAILED;
            }

            this.readStream = this.process.StandardOutput.BaseStream;
            this.writeStream = this.process.StandardInput.BaseStream;

            if (loginType != AdbLoginTypeEnum.None)
            {
                // 处理登录逻辑
                this.loginEvent = new AutoResetEvent(false);
                Task.Factory.StartNew(this.UserLogin);
                bool success = this.loginEvent.WaitOne(timeout);
                this.loginEvent.Dispose();
                if (!success)
                {
                    logger.ErrorFormat("adb登录超时");
                    this.Close();
                    return ResponseCode.FAILED;
                }
            }

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            try
            {
                this.process.Kill();
                this.process.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        public override void Resize(int row, int col)
        {
        }

        public override void Write(byte[] buffer)
        {
            this.writeStream.Write(buffer);

            // 用户输入回车的时候，经过VTKeyboard翻译成\r
            // adb环境下需要再次输入一个\n才能正确换行
            if (buffer.Length == 1 && buffer[0] == '\r')
            {
                this.writeStream.Write(LF);
            }
            this.writeStream.Flush();
        }

        internal override int Read(byte[] buffer)
        {
            if (this.pendingReads.Count > 0)
            {
                if (buffer.Length >= this.pendingReads.Count)
                {
                    byte[] pendingBytes = this.pendingReads.ToArray();
                    this.pendingReads.Clear();
                    Buffer.BlockCopy(pendingBytes, 0, buffer, 0, pendingBytes.Length);
                    return pendingBytes.Length;
                }
                else
                {
                    byte[] pendingBytes = this.pendingReads.Take(buffer.Length).ToArray();
                    this.pendingReads.RemoveRange(0, buffer.Length);
                    Buffer.BlockCopy(pendingBytes, 0, buffer, 0, buffer.Length);
                    return buffer.Length;
                }
            }
            else
            {
                return this.readStream.Read(buffer, 0, buffer.Length);
            }
        }

        #endregion

        #region 实例方法

        private bool UserLogin()
        {
            AdbLoginTypeEnum loginType = this.session.GetOption<AdbLoginTypeEnum>(OptionKeyEnum.ADBSH_LOGIN_TYPE);

            switch (loginType)
            {
                case AdbLoginTypeEnum.None: return true;
                case AdbLoginTypeEnum.UserNamePassword:
                case AdbLoginTypeEnum.Password:
                    {
                        int timeout = this.session.GetOption<int>(OptionKeyEnum.ADBSH_LOGIN_TIMEOUT);

                        // 如果需要用户名和密码登录，那么先输入用户名
                        if (loginType == AdbLoginTypeEnum.UserNamePassword)
                        {
                            string userName = this.session.GetOption<string>(OptionKeyEnum.ADBSH_USERNAME);
                            string userNamePrompt = this.session.GetOption<string>(OptionKeyEnum.ADBSH_USERNAME_PROMPT);
                            if (!this.HandlePrompt(userNamePrompt, userName + "\r\n", this.readStream, this.writeStream, timeout))
                            {
                                return false;
                            }
                        }

                        // 输入密码
                        string password = this.session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD);
                        string passwordPrompt = this.session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD_PROMPT);
                        if (!this.HandlePrompt(passwordPrompt, password + "\r\n", this.readStream, this.writeStream, timeout))
                        {
                            return false;
                        }

                        // 下次读取到shPrompt的时候就说明登录成功
                        string shPrompt = this.session.GetOption<string>(OptionKeyEnum.ADBSH_SH_PROMPT);
                        if (!ReadUntil(this.readStream, shPrompt, timeout))
                        {
                            return false;
                        }

                        this.loginEvent.Set();

                        return true;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private bool ReadUntil(Stream streamReader, string until, int timeout)
        {
            string read = string.Empty;
            int left = timeout;

            while (true)
            {
                byte[] bytes = new byte[1024];
                int size = streamReader.Read(bytes, 0, bytes.Length);
                if (size < 0)
                {
                    // 读取失败直接退出
                    return false;
                }
                else if (size == 0)
                {
                    // 没读取到数据
                }
                else
                {
                    string read1 = Encoding.ASCII.GetString(bytes, 0, size);
                    read += read1;

                    this.pendingReads.AddRange(bytes.Take(size));

                    if (read.Contains(until))
                    {
                        return true;
                    }
                }

                // 50毫秒之后再继续读
                Thread.Sleep(50);

                left -= 50;
                if (left <= 0)
                {
                    return false;
                }
            }
        }

        private bool HandlePrompt(string prompt, string write, Stream streamReader, Stream streamWriter, int timeout)
        {
            if (!ReadUntil(streamReader, prompt, timeout))
            {
                return false;
            }

            byte[] writeBytes = Encoding.ASCII.GetBytes(write);

            streamWriter.Write(writeBytes, 0, writeBytes.Length);
            streamWriter.Flush();

            return true;
        }

        /// <summary>
        /// 确保Adb守护进程已启动
        /// </summary>
        private bool EnsureStartServer()
        {
            string exePath = this.session.GetOption<string>(OptionKeyEnum.ADBSH_ADB_PATH);
            int timeout = this.session.GetOption<int>(OptionKeyEnum.ADBSH_START_SVR_TIMEOUT, MTermConsts.DefaultAdbStartServerTimeout);

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                Arguments = "start-server"
            };

            try
            {
                Process process = Process.Start(startInfo);
                if (!process.WaitForExit(timeout))
                {
                    logger.ErrorFormat("启动adb server失败, 超时了, 可能端口被占用了");
                    // 进程超时了还没退出
                    // 此时表示启动失败
                    process.Kill();
                    process.Dispose();
                    return false;
                }
                else
                {
                    if (process.ExitCode != 0)
                    {
                        // 说明进程因为启动失败退出
                        // 比如5037端口被占用，adb尝试连接5037端口，然后占用5037端口的进程退出，此时exitCode不等于0
                        logger.ErrorFormat("启动adb server失败, exitCode = {0}", process.ExitCode);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("启动adb守护进程", ex);
                return false;
            }
        }

        #endregion
    }
}
