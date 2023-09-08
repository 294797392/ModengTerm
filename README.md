
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
很遗憾，目前XTerminal只能运行在Windows环境下，XTerminal没有计划兼容Linux和Mac系统。但是XTerminal希望未来可以运行在安卓和IOS手机操作系统上，这样可以让开发者坐在公交车或者地铁上就可以随时随地的进行远程。


# 编译项目
使用VS2019打开Sources/VideoTerminal.sln，直接进行编译。项目基于.net framework 4.8版本。

# 参考资料
* https://github.com/microsoft/terminal.git  
* https://devblogs.microsoft.com/commandline/windows-command-line-introducing-the-windows-pseudo-console-conpty/  
* Control Functions for Coded Character Sets Ecma-048.pdf  
* https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences  
* https://invisible-island.net/xterm/ctlseqs/ctlseqs.html  
* https://invisible-island.net/xterm/  
* https://vt100.net/emu/dec_ansi_parser  
* 虚拟终端/控制台/Shell介绍：https://cloud.tencent.com/developer/news/304629  
* SSH协议：https://www.rfc-editor.org/rfc/rfc4254
* https://blog.csdn.net/ScilogyHunter/article/details/106874395




# 一些可以帮助调试的指令
查看当前终端类型：echo $TERM
查看当前终端信息：stty -a


# QA

1. 终端显示有问题（包括但不限于排版错乱，执行的动作不在预期内）

* 终端名字不正确。有的时候不正确的终端名字会导致出现一些异常行为。使用echo $TERM指令查看当前终端名字。终端名字只能是xterm,xterm-256color这里面的其中一个。（出现过当终端名字写成XTerm的时候，当扩大终端行数的时候，光标位置会显示到所在行的最右边的问题）
* 软件BUG
* 如果打开的是Windows命令行会话，那么有可能是不同版本的操作系统上的终端兼容性导致。作者测试过在Windows7和Windows10下使用相同的程序，在命令行里打开VIM，然后扩大终端行数会显示错乱的问题

















