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
/// �������ܣ�
/// ���value��437��ch��2����ô���صĽ������4372
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
/// ��Cancel����
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
	// ����https://vt100.net/emu/dec_ansi_parser#STDCSIGN�����ҳ�ϵı�񣬵�һ�ν���Escape״̬��ʱ����Ҫ��clear����
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
	// ����https://vt100.net/emu/dec_ansi_parser#STDCSIGN�����ҳ�ϵı�񣬵�һ�ν���csi entry״̬��ʱ����Ҫ��clear����
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
/// �ռ�OSC�����Ķ���
/// </summary>
/// <param name="ch"></param>
static void ActionOSCParam(char ch)
{
	this.oscParam = this.AccumulateTo(ch, this.oscParam);
}

/// <summary>
/// �ַ�OSC�¼�
/// </summary>
static void ActionOSCDispatch(char ch)
{
	//Console.WriteLine("�ַ���OSC�ַ��� = {0}", this.oscString);
}

/// <summary>
/// �ռ�CSI����EscapeIntermediate״̬�µ�Intermediate�ַ�
/// </summary>
/// <param name="ch"></param>
static void ActionCollect(char ch)
{
	this.intermediate.Add(ch);
}

/// <summary>
/// �ռ�CSI״̬�µ�Parameter�ַ�
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
/// �����豸�����ַ�����ʶ������һ���ַ�ʱ����״̬������ͨ���ʵ����ƹ��ܵĴ�������ͨ����Ȼ�����к����ַ����ݸ��˱��ô������ֱ�������ַ�����ֹ
/// </summary>
/// <param name="ch">final char</param>
static void ActionDCSDispatch(char ch)
{
	// �������һ���ַ��ж�Ҫʹ�õ�DCS���¼�������
	this.dcsStringHandler = this.ActionDCSDispatch(ch, this.parameters);

	if (this.dcsStringHandler != null)
	{
		// ��ȡ���˸�DCS�¼�����������ôҪ�����ռ�DCS�����ַ���״̬
		this.EnterDCSPassThrough();
	}
	else
	{
		// û��ȡ����DCS�¼���������ͨ�������ûʵ�ָô���������֧�ִ�����¼�������ôֱ�ӽ������DCS�¼�״̬
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
/// ��״̬�ı�ΪGround��ʱ�򴥷�
/// </summary>
/// <param name="ch"></param>
static void EventGround(char ch)
{
	if (ASCIITable.IsC0Code(ch) || ASCIITable.IsDelete(ch))
	{
		// �����C0�����ַ���Delete�ַ���˵��Ҫִ�ж���
		this.ActionExecute(ch);
	}
	else if (ASCIITable.IsPrintable(ch))
	{
		// �����ַ�ֱ�Ӵ�ӡ
		this.ActionPrint(ch);
	}
	else
	{
		// ���ǿɼ��ַ��������ֽ��ַ�������UTF8����
		// UTF8�ο���https://www.cnblogs.com/fnlingnzb-learner/p/6163205.html
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
/// ��״̬���Escape��ʱ�򴥷�
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
			// 0x5B�����뵽��csi entry״̬
			this.EnterCSIEntry();
		}
		else if (ASCIITable.IsOSCIndicator(ch))
		{
			// 0x5D�����뵽��osc״̬
			this.EnterOSCParam();
		}
		else if (ASCIITable.IsDCSIndicator(ch))
		{
			// 0x50�����뵽��dcs״̬
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
		// �ж��Ƿ���VT52ģʽ�µ��ƶ����ָ��, ��������VT52ģʽ�²Żᴥ��
		// ��VT52ģʽ��ֻ���ƶ�����ָ���в���������������ƶ�����ָ���������
		this.EnterVt52Param();
	}
	else
	{
		// �����������Ĳ���������VT52�����ַ�
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
/// ���뵽��OSC״̬����ʼ����OSC����
/// </summary>
/// <param name="ch"></param>
static void EventOSCParam(char ch)
{
	if (ASCIITable.IsOSCTerminator(ch))
	{
		// OSC״̬�³�����BEL������
		// �ο�terminal������������Ground״̬
		this.EnterGround();
	}
	else if (ASCIITable.IsNumericParamValue(ch))
	{
		// OSC״̬�µ����֣��ռ�����
		this.ActionOSCParam(ch);
	}
	else if (ASCIITable.IsOSCDelimiter(ch))
	{
		// OSC״̬�³����˷ָ�����˵��Ҫ��ʼ�ռ��ַ�����
		this.EnterOSCString();
	}
	else
	{
		// �������е��ַ�������
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
		// ������OSC����������ô����Ground״̬
		this.ActionOSCDispatch(ch);
		this.EnterGround();
	}
	else if (ASCIITable.IsEscape(ch))
	{
		// OSC״̬�³�����ESC�ַ�����ô�������������֣�
		// 1. ESC������ST�ַ���˵����OSC״̬������
		// 2. ESC����û��ST�ַ���˵����ESC״̬
		// �������ﶨ��һ��OSCTermination״̬������������״̬
		this.EnterOSCTermination();
	}
	else if (ASCIITable.IsOSCIndicator(ch))
	{
		// OSC�Ƿ��ַ�������
		this.ActionIgnore(ch);
	}
	else
	{
		// ʣ�µľ���OSC����Ч�ַ����ռ�
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
		// OSC״̬�³�����ESC�󣬺��������ST�ַ���˵����OSC״̬������
		this.ActionOSCDispatch(ch);
		this.EnterGround();
	}
	else
	{
		// OSC״̬�³�����ESC�󣬺���û��ST�ַ���˵��ҪCancel OSC״̬��ֱ�ӽ���ESCģʽ
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

			// ��������˵��Yָ��Ĳ����ռ����ˣ�����ִ���ˣ���ΪYָ�����ƶ����ָ�����ֻ����������
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
