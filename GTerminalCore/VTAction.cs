using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalCore
{
    /// <summary>
    /// 定义终端要执行的动作
    /// </summary>
    public enum VTAction
    {
        Print

        ///// <summary>
        ///// The character or control is not processed. No observable difference in the terminal’s state would occur if the character that caused this action was not present in the input stream. (Therefore, this action can only occur within a state.)
        ///// </summary>
        //Ignore,

        ///// <summary>
        ///// This action only occurs in ground state. The current code should be mapped to a glyph according to the character set mappings and shift states in effect, and that glyph should be displayed. 20 (SP) and 7F (DEL) have special behaviour in later VT series, as described in ground.
        ///// </summary>
        //Print,

        ///// <summary>
        ///// The C0 or C1 control function should be executed, which may have any one of a variety of effects, including changing the cursor position, suspending or resuming communications or changing the shift states in effect. There are no parameters to this action.
        ///// </summary>
        //Execute,

        ///// <summary>
        ///// This action causes the current private flag, intermediate characters, final character and parameters to be forgotten. This occurs on entry to the escape, csi entry and dcs entry states, so that erroneous sequences like CSI 3 ; 1 CSI 2 J are handled correctly.
        ///// </summary>
        //Clear,

        ///// <summary>
        ///// The private marker or intermediate character should be stored for later use in selecting a control function to be executed when a final character arrives. X3.64 doesn’t place any limit on the number of intermediate characters allowed before a final character, although it doesn’t define any control sequences with more than one. Digital defined escape sequences with two intermediate characters, and control sequences and device control strings with one. If more than two intermediate characters arrive, the parser can just flag this so that the dispatch can be turned into a null operation.
        ///// </summary>
        //Collect,

        ///// <summary>
        ///// This action collects the characters of a parameter string for a control sequence or device control sequence and builds a list of parameters. The characters processed by this action are the digits 0-9 (codes 30-39) and the semicolon (code 3B). The semicolon separates parameters. There is no limit to the number of characters in a parameter string, although a maximum of 16 parameters need be stored. If more than 16 parameters arrive, all the extra parameters are silently ignored.
        ///// The VT500 Programmer Information is inconsistent regarding the maximum value that a parameter can take. In section 4.3.3.2 of EK-VT520-RM it says that “any parameter greater than 9999 (decimal) is set to 9999 (decimal)”. However, in the description of DECSR (Secure Reset), its parameter is allowed to range from 0 to 16383. Because individual control functions need to make sure that numeric parameters are within specific limits, the supported maximum is not critical, but it must be at least 16383.
        ///// Most control functions support default values for their parameters. The default value for a parameter is given by either leaving the parameter blank, or specifying a value of zero. Judging by previous threads on the newsgroup comp.terminals, this causes some confusion, with the occasional assertion that zero is the default parameter value for control functions. This is not the case: many control functions have a default value of 1, one (GSM) has a default value of 100, and some have no default. However, in all cases the default value is represented by either zero or a blank value.
        ///// In the standard ECMA-48, which can be considered X3.64’s successor², there is a distinction between a parameter with an empty value (representing the default value), and one that has the value zero. There used to be a mode, ZDM (Zero Default Mode), in which the two cases were treated identically, but that is now deprecated in the fifth edition (1991). Although a VT500 parser needs to treat both empty and zero parameters as representing the default, it is worth considering future extensions by distinguishing them internally.
        ///// </summary>
        //Param,

        ///// <summary>
        ///// The final character of an escape sequence has arrived, so determined the control function to be executed from the intermediate character(s) and final character, and execute it. The intermediate characters are available because collect stored them as they arrived.
        ///// </summary>
        //Esc_Dispatch,

        ///// <summary>
        ///// A final character has arrived, so determine the control function to be executed from private marker, intermediate character(s) and final character, and execute it, passing in the parameter list. The private marker and intermediate characters are available because collect stored them as they arrived.
        ///// Digital mostly used private markers to extend the parameters of existing X3.64-defined control functions, while keeping a similar meaning. A few examples are shown in the table below.
        ///// No Private Marker	With Private Marker
        ///// SM, Set ANSI Modes	SM, Set Digital Private Modes
        ///// ED, Erase in Display	DECSED, Selective Erase in Display
        ///// CPR, Cursor Position Report	DECXCPR, Extended Cursor Position Report
        ///// In the cases above, csi_dispatch needn’t know about the private marker at all, as long as it is passed along to the control function when it is executed. However, the VT500 has a single case where the use of a private marker selects an entirely different control function (DECSTBM, Set Top and Bottom Margins and DECPCTERM, Enter/Exit PCTerm or Scancode Mode), so this action needs to use the private marker in its choice. xterm takes the same approach for efficiency, even though it doesn’t support DECPCTERM.
        ///// The selected control function will have access to the list of parameters, which it will use some or all of. If more parameters are supplied than the control function requires, only the earliest parameters will be used and the rest will be ignored. If too few parameters are supplied, default values will be used. If the control function has no default values, defaulted parameters will be ignored; this may result in the control function having no effect. For example, if the SM (Set Mode) control function is invoked with the sequence CSI 2;0;5 h, the second parameter will be ignored because SM has no default value.
        ///// </summary>
        //Csi_Dispatch,

        ///// <summary>
        ///// This action is invoked when a final character arrives in the first part of a device control string. It determines the control function from the private marker, intermediate character(s) and final character, and executes it, passing in the parameter list. It also selects a handler function for the rest of the characters in the control string. This handler function will be called by the put action for every character in the control string as it arrives.
        ///// This way of handling device control strings has been selected because it allows the simple plugging-in of extra parsers as functionality is added. Support for a fairly simple control string like DECDLD (Downline Load) could be added into the main parser if soft characters were required, but the main parser is no place for complicated protocols like ReGIS.
        ///// </summary>
        //Hook,

        ///// <summary>
        ///// This action passes characters from the data string part of a device control string to a handler that has previously been selected by the hook action. C0 controls are also passed to the handler.
        ///// </summary>
        //Put,

        ///// <summary>
        ///// When a device control string is terminated by ST, CAN, SUB or ESC, this action calls the previously selected handler function with an “end of data” parameter. This allows the handler to finish neatly.
        ///// </summary>
        //Unhook,

        ///// <summary>
        ///// When the control function OSC (Operating System Command) is recognised, this action initializes an external parser (the “OSC Handler”) to handle the characters from the control string. OSC control strings are not structured in the same way as device control strings, so there is no choice of parsers.
        ///// </summary>
        //Osc_start,

        ///// <summary>
        ///// This action passes characters from the control string to the OSC Handler as they arrive. There is therefore no need to buffer characters until the end of the control string is recognised.
        ///// </summary>
        //Osc_put,

        ///// <summary>
        ///// This action is called when the OSC string is terminated by ST, CAN, SUB or ESC, to allow the OSC handler to finish neatly.
        ///// </summary>
        //Osc_end
    }
}