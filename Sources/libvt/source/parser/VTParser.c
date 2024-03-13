#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTParser.h"
#include "VTParserStatements.h"

// The DEC STD 070 reference recommends supporting up to at least 16384 for
// parameter values, so 32767 should be more than enough. At most we might
// want to increase this to 65535, since that is what XTerm and VTE support,
// but for now 32767 is the safest limit for our existing code base.
#define MAX_PARAMETER_VALUE							32767

struct VTParser
{
	VTStates state;
	int isAnsiMode;
	int isApplicationMode;

	int oscParam;
	char oscString[128];
	int oscStringOffset;

	char intermediate[128];
	int parameters[128];
};

static int isEscape(char c)
{
	return c == 0x1b;
}

/// <summary>
/// 函数功能：
/// 如果value是437，ch是2，那么返回的结果就是4372
/// </summary>
/// <param name="ch"></param>
/// <param name="value"></param>
/// <returns></returns>
static int AccumulateTo(char ch, int value)
{
	int digit = ch - '0';

	value = value * 10 + digit;

	if (value > MAX_PARAMETER_VALUE)
	{
		value = MAX_PARAMETER_VALUE;
	}

	return value;
}

/// <summary>
/// 做Cancel操作
/// </summary>
static void ActionClear()
{
	this.oscParam = 0;
	this.oscString.Clear();

	this.intermediate.Clear();
	this.parameters.Clear();

	this.unicodeText.Clear();

	this.dcsStringHandler = null;
}

/// <summary>
/// This state is entered whenever the C0 control ESC is received. This will immediately cancel any escape sequence, control sequence or control string in progress
/// </summary>
static void EnterEscape()
{
	this.state = VTSTAT_Escape;
	// 根据https://vt100.net/emu/dec_ansi_parser#STDCSIGN这个网页上的表格，第一次进入Escape状态的时候需要做clear动作
	this.ActionClear();
}

static void EnterEscapeIntermediate()
{
	this.state = VTSTAT_EscapeIntermediate;
}

static void EnterGround()
{
	this.state = VTSTAT_Ground;
}

static void EnterCSIEntry()
{
	this.state = VTSTAT_CSIEntry;
	// 根据https://vt100.net/emu/dec_ansi_parser#STDCSIGN这个网页上的表格，第一次进入csi entry状态的时候需要做clear动作
	this.ActionClear();
}

static void EnterCSIIntermediate()
{
	this.state = VTSTAT_CSIIntermediate;
}

static void EnterCSIIgnore()
{
	this.state = VTSTAT_CSIIgnore;
}

static void EnterCSIParam()
{
	this.state = VTSTAT_CSIParam;
}

static void EnterOSCParam()
{
	this.state = VTSTAT_OSCParam;
}

static void EnterOSCString()
{
	this.state = VTSTAT_OSCString;
}

static void EnterOSCTermination()
{
	this.state = VTSTAT_OSCTermination;
}

static void EnterDCSEntry()
{
	this.state = VTSTAT_DCSEntry;
	this.ActionClear();
}

static void EnterDCSIgnore()
{
	this.state = VTSTAT_DCSIgnore;
}

static void EnterDCSParam()
{
	this.state = VTSTAT_DCSParam;
}

static void EnterDCSIntermediate()
{
	this.state = VTSTAT_DCSIntermediate;
}

static void EnterDCSPassThrough()
{
	this.state = VTSTAT_DCSPassthrough;
}

static void EnterVt52Param()
{
	this.state = VTSTAT_Vt52Param;
}

static void ActionIgnore(char ch)
{

}

static void ActionCSIDispatch(char ch)
{
	this.ActionCSIDispatch(ch, this.parameters);
}

/// <summary>
/// 收集OSC参数的动作
/// </summary>
/// <param name="ch"></param>
static void ActionOSCParam(char ch)
{
	this.oscParam = this.AccumulateTo(ch, this.oscParam);
}

/// <summary>
/// 分发OSC事件
/// </summary>
static void ActionOSCDispatch(char ch)
{
	//Console.WriteLine("分发的OSC字符串 = {0}", this.oscString);
}

/// <summary>
/// 收集CSI或者EscapeIntermediate状态下的Intermediate字符
/// </summary>
/// <param name="ch"></param>
static void ActionCollect(char ch)
{
	this.intermediate.Add(ch);
}

/// <summary>
/// 收集CSI状态下的Parameter字符
///  - Triggers the Param action to indicate that the state machine should store this character as a part of a parameter
///   to a control sequence.
/// </summary>
/// <param name="ch"></param>
static void ActionParam(char ch)
{
	if (this.parameters.Count == 0)
	{
		this.parameters.Add(0);
	}

	if (ASCIITable.IsParameterDelimiter(ch))
	{
		this.parameters.Add(0);
	}
	else
	{
		int last = this.parameters.Last();
		this.parameters[this.parameters.Count - 1] = this.AccumulateTo(ch, last);
	}
}

/// <summary>
/// When a final character has been recognised in a device control string, this state will establish a channel to a handler for the appropriate control function, and then pass all subsequent characters through to this alternate handler, until the data string is terminated
/// 当在设备控制字符串中识别出最后一个字符时，此状态将建立通向适当控制功能的处理程序的通道，然后将所有后续字符传递给此备用处理程序，直到数据字符串终止
/// </summary>
/// <param name="ch">final char</param>
static void ActionDCSDispatch(char ch)
{
	// 根据最后一个字符判断要使用的DCS的事件处理器
	this.dcsStringHandler = this.ActionDCSDispatch(ch, this.parameters);

	if (this.dcsStringHandler != null)
	{
		// 获取到了该DCS事件处理器，那么要进入收集DCS后续字符的状态
		this.EnterDCSPassThrough();
	}
	else
	{
		// 没获取到该DCS事件处理器（通常情况是没实现该处理器，不支持处理该事件），那么直接进入忽略DCS事件状态
		this.EnterDCSIgnore();
	}
}

/// <summary>
/// - Triggers the Vt52EscDispatch action to indicate that the listener should handle
///      a VT52 escape sequence. These sequences start with ESC and a single letter,
///      sometimes followed by parameters.
/// </summary>
/// <param name="ch"></param>
static void ActionVt52EscDispatch(char ch)
{
	this.ActionVt52EscDispatch(ch, this.parameters);
}

/// <summary>
/// 当状态改变为Ground的时候触发
/// </summary>
/// <param name="ch"></param>
static void EventGround(char ch)
{
	if (ASCIITable.IsC0Code(ch) || ASCIITable.IsDelete(ch))
	{
		// 如果是C0控制字符和Delete字符，说明要执行动作
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsPrintable(ch))
	{
		// 其他字符直接打印
		this.ActionPrint(ch);
	}
	else
	{
		// 不是可见字符，当多字节字符处理，用UTF8编码
		// UTF8参考：https://www.cnblogs.com/fnlingnzb-learner/p/6163205.html
		if (this.unicodeText.Count == 0)
		{
			bool bit6 = DotNEToolkit.Utility.charUtils.GetBit(ch, 5);
			this.unicodeText.Capacity = bit6 ? 3 : 2;
		}

		this.unicodeText.Add(ch);
		if (this.unicodeText.Count == this.unicodeText.Capacity)
		{
			string text = Encoding.UTF8.GetString(this.unicodeText.ToArray());
			this.ActionPrint(text[0]);
			this.unicodeText.Clear();
		}
	}
}

/// <summary>
/// 当状态变成Escape的时候触发
/// </summary>
/// <param name="ch"></param>
static void EventEscape(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
		this.EnterEscapeIntermediate();
	}
	else if (this.isAnsiMode)
	{
		if (ASCIITable.IsCSIIndicator(ch))
		{
			// 0x5B，进入到了csi entry状态
			this.EnterCSIEntry();
		}
		else if (ASCIITable.IsOSCIndicator(ch))
		{
			// 0x5D，进入到了osc状态
			this.EnterOSCParam();
		}
		else if (ASCIITable.IsDCSIndicator(ch))
		{
			// 0x50，进入到了dcs状态
			this.EnterDCSEntry();
		}
		else
		{
			this.ActionEscDispatch(ch);
			this.EnterGround();
		}
	}
	else if (ASCIITable.IsVt52CursorAddress(ch))
	{
		// 判断是否是VT52模式下的移动光标指令, 当进入了VT52模式下才会触发
		// 在VT52模式下只有移动光标的指令有参数，所以这里把移动光标的指令单独做处理
		this.EnterVt52Param();
	}
	else
	{
		// 这里是其他的不带参数的VT52控制字符
		this.ActionVt52EscDispatch(ch);
		this.EnterGround();
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the EscapeIntermediate state.
///   Events in this state will:
///   1. Execute C0 control characters
///   2. Ignore Delete characters
///   3. Collect Intermediate characters
///   4. Dispatch an Escape action.
/// </summary>
/// <param name="ch"></param>
static void EventEscapeIntermediate(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (this.isAnsiMode)
	{
		this.ActionEscDispatch(ch);
		this.EnterGround();
	}
	else if (ASCIITable.IsVt52CursorAddress(ch))
	{
		this.EnterVt52Param();
	}
	else
	{
		this.ActionVt52EscDispatch(ch);
		this.EnterGround();
	}
}

/// <summary>
/// 进入到了OSC状态，开始解析OSC命令
/// </summary>
/// <param name="ch"></param>
static void EventOSCParam(char ch)
{
	if (ASCIITable.IsOSCTerminator(ch))
	{
		// OSC状态下出现了BEL结束符
		// 参考terminal的做法，进入Ground状态
		this.EnterGround();
	}
	else if (ASCIITable.IsNumericParamValue(ch))
	{
		// OSC状态下的数字，收集起来
		this.ActionOSCParam(ch);
	}
	else if (ASCIITable.IsOSCDelimiter(ch))
	{
		// OSC状态下出现了分隔符，说明要开始收集字符串了
		this.EnterOSCString();
	}
	else
	{
		// 其他所有的字符都忽略
		this.ActionIgnore(ch);
	}
}

/// <summary>
/// - Processes a character event into a Action that occurs while in the OscParam state.
///   Events in this state will:
///   1. Trigger the OSC action associated with the param on an OscTerminator
///   2. If we see a ESC, enter the OscTermination state. We'll wait for one
///      more character before we dispatch the string.
///   3. Ignore OscInvalid characters.
///   4. Collect everything else into the OscString
/// </summary>
/// <param name="ch"></param>
static void EventOSCString(char ch)
{
	if (ASCIITable.IsOSCTerminator(ch))
	{
		// 出现了OSC结束符，那么进入Ground状态
		this.ActionOSCDispatch(ch);
		this.EnterGround();
	}
	else if (ASCIITable.IsEscape(ch))
	{
		// OSC状态下出现了ESC字符，那么有两种情况会出现：
		// 1. ESC后面有ST字符，说明是OSC状态结束了
		// 2. ESC后面没有ST字符，说明是ESC状态
		// 所以这里定义一个OSCTermination状态来处理这两种状态
		this.EnterOSCTermination();
	}
	else if (ASCIITable.IsOSCIndicator(ch))
	{
		// OSC非法字符，忽略
		this.ActionIgnore(ch);
	}
	else
	{
		// 剩下的就是OSC的有效字符，收集
		this.oscString.Append((char)ch);
	}
}

/// <summary>
/// - Handle the two-character termination of a OSC sequence.
///   Events in this state will:
///   1. Trigger the OSC action associated with the param on an OscTerminator
///   2. Otherwise treat this as a normal escape character event.
/// </summary>
/// <param name="ch"></param>
static void EventOSCTermination(char ch)
{
	if (ASCIITable.IsStringTermination(ch))
	{
		// OSC状态下出现了ESC后，后面紧跟着ST字符，说明是OSC状态结束了
		this.ActionOSCDispatch(ch);
		this.EnterGround();
	}
	else
	{
		// OSC状态下出现了ESC后，后面没有ST字符，说明要Cancel OSC状态并直接进入ESC模式
		this.EnterEscape();
		this.EventEscape(ch);
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the CsiEntry state.
///   Events in this state will:
///   1. Execute C0 control characters
///   2. Ignore Delete characters
///   3. Collect Intermediate characters
///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
///   5. Store parameter data
///   6. Collect Control Sequence static markers
///   7. Dispatch a control sequence with parameters for action
/// </summary>
/// <param name="ch"></param>
static void EventCSIEntry(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
		this.EnterCSIIntermediate();
	}
	else if (ASCIITable.IsCSIInvalid(ch))
	{
		this.EnterCSIIgnore();
	}
	else if (ASCIITable.IsNumericParamValue(ch) || ASCIITable.IsParameterDelimiter(ch))
	{
		this.ActionParam(ch);
		this.EnterCSIParam();
	}
	else if (ASCIITable.IsCSIstaticMarker(ch))
	{
		this.ActionCollect(ch);
		this.EnterCSIParam();
	}
	else
	{
		this.ActionCSIDispatch(ch);
		this.EnterGround();
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the CsiIntermediate state.
///   Events in this state will:
///   1. Execute C0 control characters
///   2. Ignore Delete characters
///   3. Collect Intermediate characters
///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
///   5. Dispatch a control sequence with parameters for action
/// </summary>
/// <param name="ch"></param>
static void EventCSIIntermediate(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsIntermediateInvalid(ch))
	{
		this.EnterCSIIgnore();
	}
	else
	{
		this.ActionCSIDispatch(ch);
		this.EnterGround();
	}
}

/// <summary>
///  Processes a character event into an Action that occurs while in the CsiIgnore state.
///   Events in this state will:
///   1. Execute C0 control characters
///   2. Ignore Delete characters
///   3. Collect Intermediate characters
///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
///   5. Return to Ground
/// </summary>
/// <param name="ch"></param>
static void EventCSIIgnore(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsIntermediateInvalid(ch))
	{
		this.ActionIgnore(ch);
	}
	else
	{
		this.EnterGround();
	}
}

/// <summary>
///  - Processes a character event into an Action that occurs while in the CsiParam state.
///   Events in this state will:
///   1. Execute C0 control characters
///   2. Ignore Delete characters
///   3. Collect Intermediate characters
///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
///   5. Store parameter data
///   6. Dispatch a control sequence with parameters for action
/// </summary>
/// <param name="ch"></param>
static void EventCSIParam(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsNumericParamValue(ch) || ASCIITable.IsParameterDelimiter(ch))
	{
		this.ActionParam(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
		this.EnterCSIIntermediate();
	}
	else if (ASCIITable.IsParameterInvalid(ch))
	{
		this.EnterCSIIgnore();
	}
	else
	{
		this.ActionCSIDispatch(ch);
		this.EnterGround();
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the DcsEntry state.
///   Events in this state will:
///   1. Ignore C0 control characters
///   2. Ignore Delete characters
///   3. Begin to ignore all remaining characters when an invalid character is detected (DcsIgnore)
///   4. Store parameter data
///   5. Collect Intermediate characters
///   6. Dispatch the Final character in preparation for parsing the data string
///  DCS sequences are structurally almost the same as CSI sequences, just with an
///      extra data string. It's safe to reuse CSI functions for
///      determining if a character is a parameter, delimiter, or invalid.
/// </summary>
/// <param name="ch"></param>
static void EventDCSEntry(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsCSIInvalid(ch))
	{
		this.EnterDCSIgnore();
	}
	else if (ASCIITable.IsNumericParamValue(ch) || ASCIITable.IsParameterDelimiter(ch))
	{
		this.ActionParam(ch);
		this.EnterDCSParam();
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
		this.EnterDCSIntermediate();
	}
	else
	{
		this.ActionDCSDispatch(ch);
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the DcsIgnore state.
///   In this state the entire DCS string is considered invalid and we will ignore everything.
///   The termination state is handled outside when an ESC is seen.
/// </summary>
/// <param name="ch"></param>
static void EventDCSIgnore(char ch)
{
	this.ActionIgnore(ch);
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the DcsIntermediate state.
///   Events in this state will:
///   1. Ignore C0 control characters
///   2. Ignore Delete characters
///   3. Collect intermediate data.
///   4. Begin to ignore all remaining intermediates when an invalid character is detected (DcsIgnore)
///   5. Dispatch the Final character in preparation for parsing the data string
/// </summary>
/// <param name="ch"></param>
static void EventDCSIntermediate(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
	}
	else if (ASCIITable.IsIntermediateInvalid(ch))
	{
		this.EnterDCSIgnore();
	}
	else
	{
		this.ActionDCSDispatch(ch);
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the DcsParam state.
///   Events in this state will:
///   1. Ignore C0 control characters
///   2. Ignore Delete characters
///   3. Collect DCS parameter data
///   4. Enter DcsIntermediate if we see an intermediate
///   5. Begin to ignore all remaining parameters when an invalid character is detected (DcsIgnore)
///   6. Dispatch the Final character in preparation for parsing the data string
/// </summary>
/// <param name="ch"></param>
static void EventDCSParam(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else if (ASCIITable.IsNumericParamValue(ch) || ASCIITable.IsParameterDelimiter(ch))
	{
		this.ActionParam(ch);
	}
	else if (ASCIITable.IsIntermediate(ch))
	{
		this.ActionCollect(ch);
		this.EnterDCSIntermediate();
	}
	else if (ASCIITable.IsParameterInvalid(ch))
	{
		this.EnterDCSIgnore();
	}
	else
	{
		this.ActionDCSDispatch(ch);
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the DcsPassThrough state.
///   Events in this state will:
///   1. Pass through if character is valid.
///   2. Ignore everything else.
///   The termination state is handled outside when an ESC is seen.
/// </summary>
/// <param name="ch"></param>
static void EventDCSPassThrough(char ch)
{
	if (ASCIITable.IsC0Code(ch) || ASCIITable.IsDCSPassThroughValid(ch))
	{
		if (!this.dcsStringHandler(ch))
		{
			this.EnterDCSIgnore();
		}
	}
	else
	{
		this.ActionIgnore(ch);
	}
}

/// <summary>
/// - Processes a character event into an Action that occurs while in the Vt52Param state.
///   Events in this state will:
///   1. Execute C0 control characters
///   2. Ignore Delete characters
///   3. Store exactly two parameter characters
///   4. Dispatch a control sequence with parameters for action (always Direct Cursor Address)
/// </summary>
/// <param name="ch"></param>
static void EventVt52Param(char ch)
{
	if (ASCIITable.IsC0Code(ch))
	{
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsDelete(ch))
	{
		this.ActionIgnore(ch);
	}
	else
	{
		this.parameters.Add(ch);
		if (this.parameters.Count == 2)
		{
			// The command character is processed before the parameter values,
			// but it will always be 'Y', the Direct Cursor Address command.

			// 到了这里说明Y指令的参数收集完了，可以执行了，因为Y指令是移动光标指令，有且只有两个参数
			this.ActionVt52EscDispatch((char)'Y');
			this.EnterGround();
		}
	}
}




VTParser *VTParser_new()
{
	VTParser *parser = (VTParser *)calloc(1, sizeof(VTParser));

	parser->isAnsiMode = 0;
	parser->isApplicationMode = 0;
	parser->state = VTSTAT_Ground;

	return parser;
}

void VTParser_free(VTParser *parser)
{
	free(parser);
}

void VTParser_porcess(VTParser *parser, const char *chars, size_t size)
{
	for (size_t i = 0; i < size; i++)
	{
		char *ch = chars[i];

		if (isEscape(ch) && parser->state != VTSTAT_OSCString)
		{
		}
		else
		{
			switch (parser->state)
			{
				case VTSTAT_Ground:
				{
					EventGround(ch);
					break;
				}

				case VTSTAT_Escape:
				{
					EventEscape(ch);
					break;
				}

				case VTSTAT_EscapeIntermediate:
				{
					EventEscapeIntermediate(ch);
					break;
				}

				case VTSTAT_OSCParam:
				{
					EventOSCParam(ch);
					break;
				}

				case VTSTAT_OSCString:
				{
					EventOSCString(ch);
					break;
				}

				case VTSTAT_OSCTermination:
				{
					EventOSCTermination(ch);
					break;
				}

				case VTSTAT_CSIEntry:
				{
					EventCSIEntry(ch);
					break;
				}

				case VTSTAT_CSIIntermediate:
				{
					EventCSIIntermediate(ch);
					break;
				}

				case VTSTAT_CSIIgnore:
				{
					EventCSIIgnore(ch);
					break;
				}

				case VTSTAT_CSIParam:
				{
					EventCSIParam(ch);
					break;
				}

				case VTSTAT_DCSEntry:
				{
					EventDCSEntry(ch);
					break;
				}

				case VTSTAT_DCSIgnore:
				{
					EventDCSIgnore(ch);
					break;
				}

				case VTSTAT_DCSIntermediate:
				{
					EventDCSIntermediate(ch);
					break;
				}

				case VTSTAT_DCSParam:
				{
					EventDCSParam(ch);
					break;
				}

				case VTSTAT_DCSPassthrough:
				{
					EventDCSPassThrough(ch);
					break;
				}

				case VTSTAT_Vt52Param:
				{
					EventVt52Param(ch);
					break;
				}

				default:
				{
					break;
				}
			}
		}
	}
}
