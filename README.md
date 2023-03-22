
# terminal

一个多功能的终端模拟器，可以连接SSH服务器，串口...etc  
类似于XShell，putty和微软开源的terminal，代码参考了xterm和terminal的源码  

## 支持连接的客户端类型
1. SSH服务器
2. 串口 - 正在实现

## 开发环境
使用VS2019，WPF开发

## 项目结构

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


# 虚拟终端支持启用或禁用以下模式
| 模式 | 描述 | 启用/禁用方法 |
| :--- | ---------------|---|
| DECAWM | 是否自动换行 |tput smam/tput rmam|


# 渲染方式对比
| 功能 | FlowDocument |
| :--- | ---------------|
| 显示光标        | &#10004; |
| 修改光标样式    | &#10008; |
| 文本选中    | &#10004; |
| 自定义右键菜单    | &#10004; |

# 不同程序终端的输出对比
| 程序 | 动作 | 输出 |
| :--- | ----|------|
| vim  | 打开|      |
| vim  | PageUp/PageDown ||
| man | 打开| |
| man | PageUp/PageDown ||





