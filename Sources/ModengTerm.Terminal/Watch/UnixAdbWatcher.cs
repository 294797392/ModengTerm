using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ModengTerm.Terminal.Watch
{
    public class UnixAdbWatcher : UnixWatcher
    {
        public class FileNames
        {
            public string TempFileName { get; set; }
            public string TempFilePath { get; set; }

            public string LocalFilePath { get; set; }
        }

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("AdbUnixWatcher");

        #endregion

        #region 实例变量

        private string shellPrompt;
        private string adbPath;
        private string password;
        private string passwordPrompt;
        private string userName;
        private string userNamePrompt;
        private int watchTimeout;
        private AdbLoginTypeEnum loginType;
        private Process readProcess; // 用来读取文件的进程
        private Stream readStream;
        private Stream writeStream;
        private Encoding writeEncoding;
        private Encoding readEncoding;
        private string tempDir;
        private Dictionary<string, FileNames> filePath2TempName;

        #endregion

        #region 构造方法

        public UnixAdbWatcher(XTermSession session) :
            base(session)
        {
            this.filePath2TempName = new Dictionary<string, FileNames>();

            this.shellPrompt = session.GetOption<string>(OptionKeyEnum.ADBSH_SH_PROMPT, string.Empty);
            this.adbPath = session.GetOption<string>(OptionKeyEnum.ADBSH_ADB_PATH, MTermConsts.DefaultAdbPath);
            this.password = session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD, string.Empty);
            this.passwordPrompt = session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD_PROMPT, string.Empty);
            this.userName = session.GetOption<string>(OptionKeyEnum.ADBSH_USERNAME, string.Empty);
            this.userNamePrompt = session.GetOption<string>(OptionKeyEnum.ADBSH_USERNAME_PROMPT, string.Empty);
            this.loginType = session.GetOption<AdbLoginTypeEnum>(OptionKeyEnum.ADBSH_LOGIN_TYPE, AdbLoginTypeEnum.None);
            this.writeEncoding = Encoding.GetEncoding(session.GetOption<string>(OptionKeyEnum.SSH_WRITE_ENCODING, MTermConsts.DefaultWriteEncoding));
            this.readEncoding = Encoding.GetEncoding(session.GetOption<string>(OptionKeyEnum.TERM_READ_ENCODING, MTermConsts.DefaultReadEncoding));
            this.watchTimeout = session.GetOption<int>(OptionKeyEnum.ADBSH_ADVANCE_WATCH_TIMEOUT, MTermConsts.DefaultAdbWatchTimeout);
            this.tempDir = session.GetOption<string>(OptionKeyEnum.ADBSH_ADVANCE_TEMP_DIR, MTermConsts.DefaultAdbTempDir);
            this.tempDir = this.tempDir.TrimEnd('/');
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
                            if (!this.HandlePrompt(userNamePrompt, userName + "\r\n", readStream, writeStream, timeout))
                            {
                                return false;
                            }
                        }

                        // 输入密码
                        string password = this.session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD);
                        string passwordPrompt = this.session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD_PROMPT);
                        if (!this.HandlePrompt(passwordPrompt, password + "\r\n", readStream, writeStream, timeout))
                        {
                            return false;
                        }

                        // 下次读取到shPrompt的时候就说明登录成功
                        string shPrompt = this.session.GetOption<string>(OptionKeyEnum.ADBSH_SH_PROMPT);
                        if (!ReadUntil(readStream, shPrompt, timeout))
                        {
                            return false;
                        }

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

        private bool OpenProcess()
        {
            if (this.readProcess != null && !this.readProcess.HasExited)
            {
                return true;
            }

            this.CloseProcess();

            if (!VTermUtils.StartAdbServer(this.session))
            {
                return false;
            }

            string exePath = this.session.GetOption<string>(OptionKeyEnum.ADBSH_ADB_PATH);

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
                this.readProcess = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                logger.Error("启动readProcess异常", ex);
                return false;
            }

            this.readStream = this.readProcess.StandardOutput.BaseStream;
            this.writeStream = this.readProcess.StandardInput.BaseStream;

            if (!this.UserLogin())
            {
                this.CloseProcess();
                return false;
            }

            return true;
        }

        private void CloseProcess()
        {
            if (this.readProcess == null)
            {
                return;
            }

            try
            {
                if (!this.readProcess.HasExited)
                {
                    this.readProcess.Kill();
                }
                this.readProcess.WaitForExit();
                this.readProcess.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error("关闭readProcess异常", ex);
            }
            finally
            { }

            this.readProcess = null;
        }

        /// <summary>
        /// 读取指定目录下的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>如果读取失败则返回string.Empty</returns>
        private string ReadFile(string filePath)
        {
            if (!this.OpenProcess())
            {
                return string.Empty;
            }

            // 根据要读取的文件生成一个保存在临时目录里的临时文件名
            FileNames fileNames;
            if (!this.filePath2TempName.TryGetValue(filePath, out fileNames))
            {
                string tempFileName = string.Format("modengterm_watch_{0}", filePath.Replace("/", "_").Replace(" ", "_"));

                fileNames = new FileNames()
                {
                    TempFileName = tempFileName,
                    TempFilePath = string.Format("{0}\\{1}", this.tempDir, tempFileName),
                    LocalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString())
                };
                this.filePath2TempName[filePath] = fileNames;
            }

            #region 执行读取指令;

            string write = string.Format("{0} > {1}\r\n", filePath, fileNames.TempFilePath);
            byte[] writeBytes = this.writeEncoding.GetBytes(write);

            try
            {
                this.writeStream.Write(writeBytes);
                this.writeStream.Flush();
            }
            catch (Exception ex)
            {
                logger.Error("readFile异常", ex);
                return string.Empty;
            }

            // 下次读到提示符就表示指令执行结束
            if (!this.ReadUntil(this.readStream, this.shellPrompt, this.watchTimeout))
            {
                return string.Empty;
            }

            #endregion

            #region 执行AdbPull指令拷贝到本地

            string content;
            string message;
            AdbReadResult readResult = VTermUtils.AdbReadFile(this.adbPath, fileNames.TempFilePath, fileNames.LocalFilePath, out content, out message);
            if (readResult != AdbReadResult.Susccess) 
            {
                logger.ErrorFormat("{0}, {1}", readResult, message);
                return string.Empty;
            }

            #endregion

            return content;
        }

        #endregion

        #region UnixWatcher

        public override string proc_stat()
        {
            return this.ReadFile("/proc/stat");
        }

        public override string proc_meminfo()
        {
            return this.ReadFile("/proc/meminfo");
        }

        public override string df_h()
        {
            return this.ReadFile("df -h");
        }

        #endregion
    }
}
