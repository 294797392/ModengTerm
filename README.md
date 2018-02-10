# xterminal

XTerminal.Base
每个类库公用的类和工具方法

XTerminal.Terminal
封装与每一种终端通讯的接口，包括ssh(SSHTerminal)，串口(SerialTerminal)
接口：
	ITerminal
	Initialize();
	Release();
	Connect();
	Disconnect();
	Send();
	DataReceived;

类：
	SSHTerminal:ITerminal
	SerialTermial:ITerminal

XTerminal.Client
客户端主程序

XTerminal.Client.TerminalConsole
实现终端控制台控件，作用是用来渲染终端样式，接收用户的输入和显示终端的输出
类：
	TerminalConsole:
	用来处理用户输入输出，渲染字符串，管理Caret，Selection两个显示层
	TerminalTextLayer：
	渲染输入输出的控件
	SelectionLayer:
	用来显示用户选中的字符串上的背景颜色效果
	CaretLayer:
	管理光标的显示
	TerminalLine:
	表示屏幕上的一行
	TerminalText:
	表示一行里的多个字符串