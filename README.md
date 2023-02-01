
# terminal

一个多功能的终端模拟器，可以连接SSH服务器，串口...etc  
类似于XShell，putty和微软开源的terminal，代码参考了xterm和terminal的源码  

## 支持连接的客户端类型
1. SSH服务器
2. 串口 - 正在实现

## 开发环境
使用VS2019，WPF开发

## 项目结构
- __VideoTerminal__  
客户端界面主项目
- __VideoTerminal.Base__  
所有的项目共同使用的公共类库
- __VideoTerminal.Controls__  
封装终端显示控件，渲染终端字符（TextRendering），光标（Caret），选中状态（Selection）等等...
- __VideoTerminal.Parser__  
解析终端数据流并转换成其他模块可以识别的动作 
- __VideoTerminal.Clients  
封装连接各种远程主机的客户端接口（比如SSH，串口...），并提供接口给其他模块使用
- __VideoTerminal.Interface__  
如果你想写一个自己的终端，那么你可以使用VideoTerminalInterface。它帮你把连接终端，解析数据流等所有的操作都封装好了，你只要简单的实现一些接口就可以了，当然渲染终端字符和光标这些还是需要你自己去写。  
VideoTerminal就是基于VideoTerminal.Interface这个库实现的。  
- __VideoTerminalConsole__  
使用C#原生的控制台写的测试程序
- __libvt__  
C语言版本的终端数据流解析器，目的是封装成一个库，可以给其他人去调用，目前还没实现。
等同于VideoTerminal.Interface  

## 实现原理
参考：  
* https://github.com/microsoft/terminal.git
* https://devblogs.microsoft.com/commandline/windows-command-line-introducing-the-windows-pseudo-console-conpty/
* Control Functions for Coded Character Sets Ecma-048.pdf
* 终端字符解析：
https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences
https://invisible-island.net/xterm/ctlseqs/ctlseqs.html
https://invisible-island.net/xterm/
https://vt100.net/emu/dec_ansi_parser
* 虚拟终端/控制台/Shell介绍：https://cloud.tencent.com/developer/news/304629

后面会把实现原理的讲解补充到这上面

## 编译方法
使用VS2019打开VideoTerminal.sln直接编译解决方案

### 数据流解析流程
stateMachine -> OutputStateMachineEngine -> AdaptDispatch/TerminalDispatch -> outputStream



