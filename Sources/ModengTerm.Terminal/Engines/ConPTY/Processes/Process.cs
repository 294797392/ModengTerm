using System;
using System.Runtime.InteropServices;
using static ModengTerm.Terminal.Engines.ConPTY.Native.ProcessApi;

namespace ModengTerm.Terminal.Engines.ConPTY.Processes
{
    /// <summary>
    /// Represents an instance of a process.
    /// </summary>
    internal sealed class Process : IDisposable
    {
        private IntPtr lpAttributeList;
        private IntPtr hProcess;
        private IntPtr hThread;

        public Process(STARTUPINFOEX startupInfo, PROCESS_INFORMATION processInfo)
        {
            StartupInfo = startupInfo;
            ProcessInfo = processInfo;

            this.lpAttributeList = startupInfo.lpAttributeList;
            this.hProcess = processInfo.hProcess;
            this.hThread = processInfo.hThread;
        }

        public STARTUPINFOEX StartupInfo { get; }
        public PROCESS_INFORMATION ProcessInfo { get; }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                // dispose unmanaged state

                // Free the attribute list
                if (this.lpAttributeList != IntPtr.Zero)
                {
                    DeleteProcThreadAttributeList(this.lpAttributeList);
                    Marshal.FreeHGlobal(this.lpAttributeList);
                    this.lpAttributeList = IntPtr.Zero;
                }

                // Close process and thread handles
                if (this.hProcess != IntPtr.Zero)
                {
                    CloseHandle(this.hProcess);
                    this.hProcess = IntPtr.Zero;
                }
                if (this.hThread != IntPtr.Zero)
                {
                    CloseHandle(this.hThread);
                    this.hThread = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        ~Process()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
