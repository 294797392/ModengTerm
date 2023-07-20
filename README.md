
# XTerminal

XTerminal是一个基于C#/WPF开发的多功能的终端模拟器，和市面上的其他远程连接工具差不多，用来连接SSH服务器，串口，telnet等等。

和其他的终端工具不同的是，XTerminal希望可以开发出一些在特定的调试场景中可以方便开发者进行调试的小功能，目前这些小功能还在持续开发中，敬请期待。

XTerminal的目标是为开发者提供一个方便的，舒适的，用着爽的远程开发环境，为软件开发者提供极致的使用体验。  

目前版本的XTerminal界面模仿了XShell的界面，并且里面的终端字节流解析器模块的代码参考了微软开源的terminal项目。  

设计文档请参考 __Documents__ 目录下的 __设计文档.pptx__

# 功能列表
* 支持与SSH服务器连接
* 支持串口连接
* 支持与Windows命令行进行交互

# 跨平台支持
很遗憾，目前XTerminal只能运行在Windwos环境下，XTerminal没有计划兼容Linux和Mac系统。但是XTerminal希望未来可以运行在安卓和IOS手机操作系统上，这样可以让开发者坐在公交车或者地铁上就可以随时随地的进行远程。


# 编译项目
使用VS2019打开Sources/VideoTerminal.sln，直接进行编译。项目基于.net framework 4.8版本。

# 参考资料
* https://github.com/microsoft/terminal.git
* https://devblogs.microsoft.com/commandline/windows-command-line-introducing-the-windows-pseudo-console-conpty/
* Control Functions for Coded Character Sets Ecma-048.pdf
* 终端字符解析：
https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences
https://invisible-island.net/xterm/ctlseqs/ctlseqs.html
https://invisible-island.net/xterm/
https://vt100.net/emu/dec_ansi_parser
* 虚拟终端/控制台/Shell介绍：https://cloud.tencent.com/developer/news/304629






















