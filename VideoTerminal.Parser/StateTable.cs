using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 针对每个控制字符定义的状态转换表
    /// 
    /// 数组的索引是收到的字符，索引对应的元素是状态机的ID
    /// 
    /// 参考xterm-331 VTPsrTbl.c 
    /// </summary>
    public class StateTable
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateTable");

        /// <summary>
        /// 解析器当前所处于的解析状态
        /// </summary>
        public VTStates State { get; private set; }

        /// <summary>
        /// 当前的状态所对应的状态转换图
        /// </summary>
        public List<VTStates> StateTransitions { get; private set; }

        //#region CigTable

        ///// <summary>
        ///// 当收到CASE_CSI_IGNORE指令的时候，解析器使用的状态表
        ///// </summary>
        //public static readonly StateMachineID[] CIgTable =
        //{
        //    /*	NUL		SOH		STX		ETX	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	EOT		ENQ		ACK		BEL	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_ENQ,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_BELL,
        //    /*	BS		HT		NL		VT	*/
        //    StateMachineID.CASE_BS,
        //    StateMachineID.CASE_TAB,
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_VMOT,
        //    /*	FF		CR		SO		SI	*/
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_CR,
        //    StateMachineID.CASE_SO,
        //    StateMachineID.CASE_SI,
        //    /*	DLE		DC1		DC2		DC3	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	DC4		NAK		SYN		ETB	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	CAN		EM		SUB		ESC	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_ESC,
        //    /*	FS		GS		RS		US	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	SP		!		"		#	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	$		%		&		'	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	(		)		*		+	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	,		-		.		/	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	0		1		2		3	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	4		5		6		7	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	8		9		:		;	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	<		=		>		?	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	@		A		B		C	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	D		E		F		G	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	H		I		J		K	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	L		M		N		O	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	P		Q		R		S	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	T		U		V		W	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	X		Y		Z		[	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	\		]		^		_	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	`		a		b		c	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	d		e		f		g	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	h		i		j		k	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	l		m		n		o	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	p		q		r		s	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	t		u		v		w	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	x		y		z		{	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	|		}		~		DEL	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    /*      0x80            0x81            0x82            0x83    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x84            0x85            0x86            0x87    */
        //    StateMachineID.CASE_IND,
        //    StateMachineID.CASE_NEL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x88            0x89            0x8a            0x8b    */
        //    StateMachineID.CASE_HTS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x8c            0x8d            0x8e            0x8f    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_RI,
        //    StateMachineID.CASE_SS2,
        //    StateMachineID.CASE_SS3,
        //    /*      0x90            0x91            0x92            0x93    */
        //    StateMachineID.CASE_DCS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x94            0x95            0x96            0x97    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SPA,
        //    StateMachineID.CASE_EPA,
        //    /*      0x98            0x99            0x9a            0x9b    */
        //    StateMachineID.CASE_SOS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECID,
        //    StateMachineID.CASE_CSI_STATE,
        //    /*      0x9c            0x9d            0x9e            0x9f    */
        //    StateMachineID.CASE_ST,
        //    StateMachineID.SMID_OSC,
        //    StateMachineID.CASE_PM,
        //    StateMachineID.CASE_APC,
        //    /*      nobreakspace    exclamdown      cent            sterling        */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      currency        yen             brokenbar       section         */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      diaeresis       copyright       ordfeminine     guillemotleft   */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      notsign         hyphen          registered      macron          */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      degree          plusminus       twosuperior     threesuperior   */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      acute           mu              paragraph       periodcentered  */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      cedilla         onesuperior     masculine       guillemotright  */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      onequarter      onehalf         threequarters   questiondown    */
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*      Agrave          Aacute          Acircumflex     Atilde          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Adiaeresis      Aring           AE              Ccedilla        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Eth             Ntilde          Ograve          Oacute          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Udiaeresis      Yacute          Thorn           ssharp          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      agrave          aacute          acircumflex     atilde          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      adiaeresis      aring           ae              ccedilla        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      egrave          eacute          ecircumflex     ediaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      igrave          iacute          icircumflex     idiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      eth             ntilde          ograve          oacute          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      ocircumflex     otilde          odiaeresis      division        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      oslash          ugrave          uacute          ucircumflex     */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      udiaeresis      yacute          thorn           ydiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //};

        //#endregion

        //#region CsiSpTable

        ///// <summary>
        ///// ascii字符和CSI的FinalByte字符的映射表
        ///// </summary>
        //public static readonly StateMachineID[] CsiSpTable =
        //{
        //    /*	NUL		SOH		STX		ETX	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	EOT		ENQ		ACK		BEL	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_ENQ,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_BELL,
        //    /*	BS		HT		NL		VT	*/
        //    StateMachineID.CASE_BS,
        //    StateMachineID.CASE_TAB,
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_VMOT,
        //    /*	FF		CR		SO		SI	*/
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_CR,
        //    StateMachineID.CASE_SO,
        //    StateMachineID.CASE_SI,
        //    /*	DLE		DC1		DC2		DC3	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	DC4		NAK		SYN		ETB	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	CAN		EM		SUB		ESC	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_ESC,
        //    /*	FS		GS		RS		US	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	SP		!		"		#	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	$		%		&		'	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	(		)		*		+	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	,		-		.		/	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	0		1		2		3	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	4		5		6		7	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	8		9		:		;	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	<		=		>		?	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	@		A		B		C	*/
        //    StateMachineID.CASE_SL,
        //    StateMachineID.CASE_SR,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	D		E		F		G	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	H		I		J		K	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	L		M		N		O	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	P		Q		R		S	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	T		U		V		W	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	X		Y		Z		[	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	\		]		^		_	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	`		a		b		c	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	d		e		f		g	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	h		i		j		k	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	l		m		n		o	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	p		q		r		s	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECSCUSR,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	t		u		v		w	*/
        //    StateMachineID.CASE_DECSWBV,
        //    StateMachineID.CASE_DECSMBV,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	x		y		z		{	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	|		}		~		DEL	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    /*      0x80            0x81            0x82            0x83    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x84            0x85            0x86            0x87    */
        //    StateMachineID.CASE_IND,
        //    StateMachineID.CASE_NEL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x88            0x89            0x8a            0x8b    */
        //    StateMachineID.CASE_HTS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x8c            0x8d            0x8e            0x8f    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_RI,
        //    StateMachineID.CASE_SS2,
        //    StateMachineID.CASE_SS3,
        //    /*      0x90            0x91            0x92            0x93    */
        //    StateMachineID.CASE_DCS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x94            0x95            0x96            0x97    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SPA,
        //    StateMachineID.CASE_EPA,
        //    /*      0x98            0x99            0x9a            0x9b    */
        //    StateMachineID.CASE_SOS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECID,
        //    StateMachineID.CASE_CSI_STATE,
        //    /*      0x9c            0x9d            0x9e            0x9f    */
        //    StateMachineID.CASE_ST,
        //    StateMachineID.SMID_OSC,
        //    StateMachineID.CASE_PM,
        //    StateMachineID.CASE_APC,
        //    /*      nobreakspace    exclamdown      cent            sterling        */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      currency        yen             brokenbar       section         */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      diaeresis       copyright       ordfeminine     guillemotleft   */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      notsign         hyphen          registered      macron          */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      degree          plusminus       twosuperior     threesuperior   */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      acute           mu              paragraph       periodcentered  */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      cedilla         onesuperior     masculine       guillemotright  */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      onequarter      onehalf         threequarters   questiondown    */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      Agrave          Aacute          Acircumflex     Atilde          */
        //    StateMachineID.CASE_SL,
        //    StateMachineID.CASE_SR,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Adiaeresis      Aring           AE              Ccedilla        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Eth             Ntilde          Ograve          Oacute          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Udiaeresis      Yacute          Thorn           ssharp          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      agrave          aacute          acircumflex     atilde          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      adiaeresis      aring           ae              ccedilla        */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      egrave          eacute          ecircumflex     ediaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      igrave          iacute          icircumflex     idiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      eth             ntilde          ograve          oacute          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECSCUSR,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      ocircumflex     otilde          odiaeresis      division        */
        //    StateMachineID.CASE_DECSWBV,
        //    StateMachineID.CASE_DECSMBV,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      oslash          ugrave          uacute          ucircumflex     */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      udiaeresis      yacute          thorn           ydiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,

        //};

        //#endregion

        //#region Csi2Table

        //public static readonly StateMachineID[] Csi2Table =
        //{
        //    /*	NUL		SOH		STX		ETX	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	EOT		ENQ		ACK		BEL	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_ENQ,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_BELL,
        //    /*	BS		HT		NL		VT	*/
        //    StateMachineID.CASE_BS,
        //    StateMachineID.CASE_TAB,
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_VMOT,
        //    /*	FF		CR		SO		SI	*/
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_CR,
        //    StateMachineID.CASE_SO,
        //    StateMachineID.CASE_SI,
        //    /*	DLE		DC1		DC2		DC3	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	DC4		NAK		SYN		ETB	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	CAN		EM		SUB		ESC	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_ESC,
        //    /*	FS		GS		RS		US	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	SP		!		"		#	*/
        //    StateMachineID.CASE_CSI_SPACE_STATE,
        //    StateMachineID.CASE_CSI_EX_STATE,
        //    StateMachineID.CASE_CSI_QUOTE_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	$		%		&		'	*/
        //    StateMachineID.CASE_CSI_DOLLAR_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_TICK_STATE,
        //    /*	(		)		*		+	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_STAR_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	,		-		.		/	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	0		1		2		3	*/
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*	4		5		6		7	*/
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*	8		9		:		;	*/
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_COLON,
        //    StateMachineID.CASE_ESC_SEMI,
        //    /*	<		=		>		?	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	@		A		B		C	*/
        //    StateMachineID.CASE_ICH,
        //    StateMachineID.CASE_CUU,
        //    StateMachineID.CASE_CUD,
        //    StateMachineID.CASE_CUF,
        //    /*	D		E		F		G	*/
        //    StateMachineID.CASE_CUB,
        //    StateMachineID.CASE_CNL,
        //    StateMachineID.CASE_CPL,
        //    StateMachineID.CASE_HPA,
        //    /*	H		I		J		K	*/
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_CHT,
        //    StateMachineID.CASE_ED,
        //    StateMachineID.CASE_EL,
        //    /*	L		M		N		O	*/
        //    StateMachineID.CASE_IL,
        //    StateMachineID.CASE_DL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	P		Q		R		S	*/
        //    StateMachineID.CASE_DCH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SU,
        //    /*	T		U		V		W	*/
        //    StateMachineID.CASE_TRACK_MOUSE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	X		Y		Z		[	*/
        //    StateMachineID.CASE_ECH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_CBT,
        //    StateMachineID.SMID_GROUND,
        //    /*	\		]		^		_	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	`		a		b		c	*/
        //    StateMachineID.CASE_HPA,
        //    StateMachineID.CASE_HPR,
        //    StateMachineID.CASE_REP,
        //    StateMachineID.CASE_DA1,
        //    /*	d		e		f		g	*/
        //    StateMachineID.CASE_VPA,
        //    StateMachineID.CASE_VPR,
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_TBC,
        //    /*	h		i		j		k	*/
        //    StateMachineID.CASE_SET,
        //    StateMachineID.CASE_MC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	l		m		n		o	*/
        //    StateMachineID.CASE_RST,
        //    StateMachineID.CASE_SGR,
        //    StateMachineID.CASE_CPR,
        //    StateMachineID.SMID_GROUND,
        //    /*	p		q		r		s	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECLL,
        //    StateMachineID.CASE_DECSTBM,
        //    StateMachineID.CASE_ANSI_SC,
        //    /*	t		u		v		w	*/
        //    StateMachineID.CASE_XTERM_WINOPS,
        //    StateMachineID.CASE_ANSI_RC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	x		y		z		{	*/
        //    StateMachineID.CASE_DECREQTPARM,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	|		}		~		DEL	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    /*      0x80            0x81            0x82            0x83    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x84            0x85            0x86            0x87    */
        //    StateMachineID.CASE_IND,
        //    StateMachineID.CASE_NEL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x88            0x89            0x8a            0x8b    */
        //    StateMachineID.CASE_HTS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x8c            0x8d            0x8e            0x8f    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_RI,
        //    StateMachineID.CASE_SS2,
        //    StateMachineID.CASE_SS3,
        //    /*      0x90            0x91            0x92            0x93    */
        //    StateMachineID.CASE_DCS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x94            0x95            0x96            0x97    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SPA,
        //    StateMachineID.CASE_EPA,
        //    /*      0x98            0x99            0x9a            0x9b    */
        //    StateMachineID.CASE_SOS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECID,
        //    StateMachineID.CASE_CSI_STATE,
        //    /*      0x9c            0x9d            0x9e            0x9f    */
        //    StateMachineID.CASE_ST,
        //    StateMachineID.SMID_OSC,
        //    StateMachineID.CASE_PM,
        //    StateMachineID.CASE_APC,
        //    /*      nobreakspace    exclamdown      cent            sterling        */
        //    StateMachineID.CASE_CSI_SPACE_STATE,
        //    StateMachineID.CASE_CSI_EX_STATE,
        //    StateMachineID.CASE_CSI_QUOTE_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      currency        yen             brokenbar       section         */
        //    StateMachineID.CASE_CSI_DOLLAR_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_TICK_STATE,
        //    /*      diaeresis       copyright       ordfeminine     guillemotleft   */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_STAR_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      notsign         hyphen          registered      macron          */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      degree          plusminus       twosuperior     threesuperior   */
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*      acute           mu              paragraph       periodcentered  */
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*      cedilla         onesuperior     masculine       guillemotright  */
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_COLON,
        //    StateMachineID.CASE_ESC_SEMI,
        //    /*      onequarter      onehalf         threequarters   questiondown    */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      Agrave          Aacute          Acircumflex     Atilde          */
        //    StateMachineID.CASE_ICH,
        //    StateMachineID.CASE_CUU,
        //    StateMachineID.CASE_CUD,
        //    StateMachineID.CASE_CUF,
        //    /*      Adiaeresis      Aring           AE              Ccedilla        */
        //    StateMachineID.CASE_CUB,
        //    StateMachineID.CASE_CNL,
        //    StateMachineID.CASE_CPL,
        //    StateMachineID.CASE_HPA,
        //    /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_CHT,
        //    StateMachineID.CASE_ED,
        //    StateMachineID.CASE_EL,
        //    /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
        //    StateMachineID.CASE_IL,
        //    StateMachineID.CASE_DL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Eth             Ntilde          Ograve          Oacute          */
        //    StateMachineID.CASE_DCH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SU,
        //    /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
        //    StateMachineID.CASE_TRACK_MOUSE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
        //    StateMachineID.CASE_ECH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_CBT,
        //    StateMachineID.SMID_GROUND,
        //    /*      Udiaeresis      Yacute          Thorn           ssharp          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      agrave          aacute          acircumflex     atilde          */
        //    StateMachineID.CASE_HPA,
        //    StateMachineID.CASE_HPR,
        //    StateMachineID.CASE_REP,
        //    StateMachineID.CASE_DA1,
        //    /*      adiaeresis      aring           ae              ccedilla        */
        //    StateMachineID.CASE_VPA,
        //    StateMachineID.CASE_VPR,
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_TBC,
        //    /*      egrave          eacute          ecircumflex     ediaeresis      */
        //    StateMachineID.CASE_SET,
        //    StateMachineID.CASE_MC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      igrave          iacute          icircumflex     idiaeresis      */
        //    StateMachineID.CASE_RST,
        //    StateMachineID.CASE_SGR,
        //    StateMachineID.CASE_CPR,
        //    StateMachineID.SMID_GROUND,
        //    /*      eth             ntilde          ograve          oacute          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECLL,
        //    StateMachineID.CASE_DECSTBM,
        //    StateMachineID.CASE_ANSI_SC,
        //    /*      ocircumflex     otilde          odiaeresis      division        */
        //    StateMachineID.CASE_XTERM_WINOPS,
        //    StateMachineID.CASE_ANSI_RC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      oslash          ugrave          uacute          ucircumflex     */
        //    StateMachineID.CASE_DECREQTPARM,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      udiaeresis      yacute          thorn           ydiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,

        //};

        //#endregion

        //#region CsiTable

        ///// <summary>
        ///// ascii码与csi控制指令的映射关系表
        ///// </summary>
        //public static readonly StateMachineID[] CSITable =
        //{
        //    /*	NUL		SOH		STX		ETX	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	EOT		ENQ		ACK		BEL	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_ENQ,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_BELL,
        //    /*	BS		HT		NL		VT	*/
        //    StateMachineID.CASE_BS,
        //    StateMachineID.CASE_TAB,
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_VMOT,
        //    /*	FF		CR		SO		SI	*/
        //    StateMachineID.CASE_VMOT,
        //    StateMachineID.CASE_CR,
        //    StateMachineID.CASE_SO,
        //    StateMachineID.CASE_SI,
        //    /*	DLE		DC1		DC2		DC3	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	DC4		NAK		SYN		ETB	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	CAN		EM		SUB		ESC	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_ESC,
        //    /*	FS		GS		RS		US	*/
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    StateMachineID.CASE_IGNORE,
        //    /*	SP		!		"		#	*/
        //    StateMachineID.CASE_CSI_SPACE_STATE,
        //    StateMachineID.CASE_CSI_EX_STATE,
        //    StateMachineID.CASE_CSI_QUOTE_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	$		%		&		'	*/
        //    StateMachineID.CASE_CSI_DOLLAR_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_TICK_STATE,
        //    /*	(		)		*		+	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	,		-		.		/	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*	0		1		2		3	*/
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*	4		5		6		7	*/
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*	8		9		:		;	*/
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_COLON,
        //    StateMachineID.CASE_ESC_SEMI,
        //    /*	<		=		>		?	*/
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_DEC3_STATE,
        //    StateMachineID.CASE_DEC2_STATE,
        //    StateMachineID.CASE_DEC_STATE,
        //    /*	@		A		B		C	*/
        //    StateMachineID.CASE_ICH,
        //    StateMachineID.CASE_CUU,
        //    StateMachineID.CASE_CUD,
        //    StateMachineID.CASE_CUF,
        //    /*	D		E		F		G	*/
        //    StateMachineID.CASE_CUB,
        //    StateMachineID.CASE_CNL,
        //    StateMachineID.CASE_CPL,
        //    StateMachineID.CASE_HPA,
        //    /*	H		I		J		K	*/
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_CHT,
        //    StateMachineID.CASE_ED,
        //    StateMachineID.CASE_EL,
        //    /*	L		M		N		O	*/
        //    StateMachineID.CASE_IL,
        //    StateMachineID.CASE_DL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	P		Q		R		S	*/
        //    StateMachineID.CASE_DCH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SU,
        //    /*	T		U		V		W	*/
        //    StateMachineID.CASE_TRACK_MOUSE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	X		Y		Z		[	*/
        //    StateMachineID.CASE_ECH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_CBT,
        //    StateMachineID.SMID_GROUND,
        //    /*	\		]		^		_	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	`		a		b		c	*/
        //    StateMachineID.CASE_HPA,
        //    StateMachineID.CASE_HPR,
        //    StateMachineID.CASE_REP,
        //    StateMachineID.CASE_DA1,
        //    /*	d		e		f		g	*/
        //    StateMachineID.CASE_VPA,
        //    StateMachineID.CASE_VPR,
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_TBC,
        //    /*	h		i		j		k	*/
        //    StateMachineID.CASE_SET,
        //    StateMachineID.CASE_MC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	l		m		n		o	*/
        //    StateMachineID.CASE_RST,
        //    StateMachineID.CASE_SGR,
        //    StateMachineID.CASE_CPR,
        //    StateMachineID.SMID_GROUND,
        //    /*	p		q		r		s	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECLL,
        //    StateMachineID.CASE_DECSTBM,
        //    StateMachineID.CASE_ANSI_SC,
        //    /*	t		u		v		w	*/
        //    StateMachineID.CASE_XTERM_WINOPS,
        //    StateMachineID.CASE_ANSI_RC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	x		y		z		{	*/
        //    StateMachineID.CASE_DECREQTPARM,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*	|		}		~		DEL	*/
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //    /*      0x80            0x81            0x82            0x83    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x84            0x85            0x86            0x87    */
        //    StateMachineID.CASE_IND,
        //    StateMachineID.CASE_NEL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x88            0x89            0x8a            0x8b    */
        //    StateMachineID.CASE_HTS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x8c            0x8d            0x8e            0x8f    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_RI,
        //    StateMachineID.CASE_SS2,
        //    StateMachineID.CASE_SS3,
        //    /*      0x90            0x91            0x92            0x93    */
        //    StateMachineID.CASE_DCS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      0x94            0x95            0x96            0x97    */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SPA,
        //    StateMachineID.CASE_EPA,
        //    /*      0x98            0x99            0x9a            0x9b    */
        //    StateMachineID.CASE_SOS,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECID,
        //    StateMachineID.CASE_CSI_STATE,
        //    /*      0x9c            0x9d            0x9e            0x9f    */
        //    StateMachineID.CASE_ST,
        //    StateMachineID.SMID_OSC,
        //    StateMachineID.CASE_PM,
        //    StateMachineID.CASE_APC,
        //    /*      nobreakspace    exclamdown      cent            sterling        */
        //    StateMachineID.CASE_CSI_SPACE_STATE,
        //    StateMachineID.CASE_CSI_EX_STATE,
        //    StateMachineID.CASE_CSI_QUOTE_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      currency        yen             brokenbar       section         */
        //    StateMachineID.CASE_CSI_DOLLAR_STATE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_TICK_STATE,
        //    /*      diaeresis       copyright       ordfeminine     guillemotleft   */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      notsign         hyphen          registered      macron          */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_CSI_IGNORE,
        //    /*      degree          plusminus       twosuperior     threesuperior   */
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*      acute           mu              paragraph       periodcentered  */
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    /*      cedilla         onesuperior     masculine       guillemotright  */
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_DIGIT,
        //    StateMachineID.CASE_ESC_COLON,
        //    StateMachineID.CASE_ESC_SEMI,
        //    /*      onequarter      onehalf         threequarters   questiondown    */
        //    StateMachineID.CASE_CSI_IGNORE,
        //    StateMachineID.CASE_DEC3_STATE,
        //    StateMachineID.CASE_DEC2_STATE,
        //    StateMachineID.CASE_DEC_STATE,
        //    /*      Agrave          Aacute          Acircumflex     Atilde          */
        //    StateMachineID.CASE_ICH,
        //    StateMachineID.CASE_CUU,
        //    StateMachineID.CASE_CUD,
        //    StateMachineID.CASE_CUF,
        //    /*      Adiaeresis      Aring           AE              Ccedilla        */
        //    StateMachineID.CASE_CUB,
        //    StateMachineID.CASE_CNL,
        //    StateMachineID.CASE_CPL,
        //    StateMachineID.CASE_HPA,
        //    /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_CHT,
        //    StateMachineID.CASE_ED,
        //    StateMachineID.CASE_EL,
        //    /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
        //    StateMachineID.CASE_IL,
        //    StateMachineID.CASE_DL,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Eth             Ntilde          Ograve          Oacute          */
        //    StateMachineID.CASE_DCH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_SU,
        //    /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
        //    StateMachineID.CASE_TRACK_MOUSE,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
        //    StateMachineID.CASE_ECH,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_CBT,
        //    StateMachineID.SMID_GROUND,
        //    /*      Udiaeresis      Yacute          Thorn           ssharp          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      agrave          aacute          acircumflex     atilde          */
        //    StateMachineID.CASE_HPA,
        //    StateMachineID.CASE_HPR,
        //    StateMachineID.CASE_REP,
        //    StateMachineID.CASE_DA1,
        //    /*      adiaeresis      aring           ae              ccedilla        */
        //    StateMachineID.CASE_VPA,
        //    StateMachineID.CASE_VPR,
        //    StateMachineID.CASE_CUP,
        //    StateMachineID.CASE_TBC,
        //    /*      egrave          eacute          ecircumflex     ediaeresis      */
        //    StateMachineID.CASE_SET,
        //    StateMachineID.CASE_MC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      igrave          iacute          icircumflex     idiaeresis      */
        //    StateMachineID.CASE_RST,
        //    StateMachineID.CASE_SGR,
        //    StateMachineID.CASE_CPR,
        //    StateMachineID.SMID_GROUND,
        //    /*      eth             ntilde          ograve          oacute          */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_DECLL,
        //    StateMachineID.CASE_DECSTBM,
        //    StateMachineID.CASE_ANSI_SC,
        //    /*      ocircumflex     otilde          odiaeresis      division        */
        //    StateMachineID.CASE_XTERM_WINOPS,
        //    StateMachineID.CASE_ANSI_RC,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      oslash          ugrave          uacute          ucircumflex     */
        //    StateMachineID.CASE_DECREQTPARM,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    /*      udiaeresis      yacute          thorn           ydiaeresis      */
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.SMID_GROUND,
        //    StateMachineID.CASE_IGNORE,
        //};

        //#endregion

        #region OSCTable

        /// <summary>
        /// 参考：
        /// xterm-331 - VTPrsTbl.c 6578行
        /// terminal - stateMachine.cpp _EventOscParam函数
        /// 
        /// 进入OSC状态后切换此状态表
        /// 
        /// 在OSC模式下，收集控制命令参数字符串，每一个CASE_IGNORE都是一个参数字符
        /// The CASE_IGNORE entries correspond to the
        /// characters that can be accumulated for the string function(e.g., OSC).
        /// 
        /// Start of String Table??
        /// </summary>
        public static readonly StateTable OSCTable = new StateTable()
        {
            State = VTStates.OSC,
            StateTransitions = new List<VTStates>()
            {
                /*	NUL		SOH		STX		ETX	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	EOT		ENQ		ACK		BEL	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	BS		HT		NL		VT	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	FF		CR		SO		SI	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	DLE		DC1		DC2		DC3	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	DC4		NAK		SYN		ETB	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	CAN		EM		SUB		ESC	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	FS		GS		RS		US	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	SP		!		"		#	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	$		%		&		'	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	(		)		*		+	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	,		-		.		/	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	0		1		2		3	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	4		5		6		7	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	8		9		:		;	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	<		=		>		?	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	@		A		B		C	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	D		E		F		G	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	H		I		J		K	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	L		M		N		O	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	P		Q		R		S	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	T		U		V		W	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	X		Y		Z		[	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	\		]		^		_	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	`		a		b		c	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	d		e		f		g	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	h		i		j		k	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	l		m		n		o	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	p		q		r		s	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	t		u		v		w	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	x		y		z		{	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*	|		}		~		DEL	*/
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x80            0x81            0x82            0x83    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x84            0x85            0x86            0x87    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x88            0x89            0x8a            0x8b    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x8c            0x8d            0x8e            0x8f    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x90            0x91            0x92            0x93    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x94            0x95            0x96            0x97    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x98            0x99            0x9a            0x9b    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      0x9c            0x9d            0x9e            0x9f    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      nobreakspace    exclamdown      cent            sterling        */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      currency        yen             brokenbar       section         */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      diaeresis       copyright       ordfeminine     guillemotleft   */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      notsign         hyphen          registered      macron          */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      degree          plusminus       twosuperior     threesuperior   */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      acute           mu              paragraph       periodcentered  */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      cedilla         onesuperior     masculine       guillemotright  */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      onequarter      onehalf         threequarters   questiondown    */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Agrave          Aacute          Acircumflex     Atilde          */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Adiaeresis      Aring           AE              Ccedilla        */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Eth             Ntilde          Ograve          Oacute          */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      Udiaeresis      Yacute          Thorn           ssharp          */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      agrave          aacute          acircumflex     atilde          */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      adiaeresis      aring           ae              ccedilla        */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      egrave          eacute          ecircumflex     ediaeresis      */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      igrave          iacute          icircumflex     idiaeresis      */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      eth             ntilde          ograve          oacute          */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      ocircumflex     otilde          odiaeresis      division        */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      oslash          ugrave          uacute          ucircumflex     */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
                /*      udiaeresis      yacute          thorn           ydiaeresis      */
                VTStates.OSC, VTStates.OSC, VTStates.OSC, VTStates.OSC,
            }
        };

        #endregion

        #region EscapeTable

        /// <summary>
        /// 当状态机进入Escape状态的时候使用的动作转换表
        /// </summary>
        public static readonly StateTable EscapeTable = new StateTable()
        {
            State = VTStates.Escape,
            StateTransitions = new List<VTStates>()
            {
                /*	NUL		SOH		STX		ETX	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,
                /*	EOT		ENQ		ACK		BEL	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,
                /*	BS		HT		NL		VT	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,
                /*	FF		CR		SO		SI	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,
                /*	DLE		DC1		DC2		DC3	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,
                /*	DC4		NAK		SYN		ETB	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	CAN		EM		SUB		ESC	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	FS		GS		RS		US	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	SP		!		"		#	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	$		%		&		'	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	(		)		*		+	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	,		-		.		/	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	0		1		2		3	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	4		5		6		7	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	8		9		:		;	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	<		=		>		?	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,               
                /*	@		A		B		C	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,                
                /*	D		E		F		G	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,           
                /*	H		I		J		K	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*	L		M		N		O	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*	P		Q		R		S	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*	T		U		V		W	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,           
                /*	X		Y		Z		[	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,           
                /*	\		]		^		_	*/
                VTStates.Escape, VTStates.OSC, VTStates.Escape, VTStates.Escape,             
                /*	`		a		b		c	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*	d		e		f		g	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*	h		i		j		k	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*	l		m		n		o	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*	p		q		r		s	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*	t		u		v		w	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,        
                /*	x		y		z		{	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,        
                /*	|		}		~		DEL	*/
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,        
                /*      0x80            0x81            0x82            0x83    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,       
                /*      0x84            0x85            0x86            0x87    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,       
                /*      0x88            0x89            0x8a            0x8b    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,        
                /*      0x8c            0x8d            0x8e            0x8f    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*      0x90            0x91            0x92            0x93    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*      0x94            0x95            0x96            0x97    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*      0x98            0x99            0x9a            0x9b    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,        
                /*      0x9c            0x9d            0x9e            0x9f    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,             
                /*      nobreakspace    exclamdown      cent            sterling        */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,           
                /*      currency        yen             brokenbar       section         */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,            
                /*      diaeresis       copyright       ordfeminine     guillemotleft   */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,            
                /*      notsign         hyphen          registered      macron          */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*      degree          plusminus       twosuperior     threesuperior   */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*      acute           mu              paragraph       periodcentered  */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*      cedilla         onesuperior     masculine       guillemotright  */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*      onequarter      onehalf         threequarters   questiondown    */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,           
                /*      Agrave          Aacute          Acircumflex     Atilde          */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*      Adiaeresis      Aring           AE              Ccedilla        */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,          
                /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,         
                /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,       
                /*      Eth             Ntilde          Ograve          Oacute          */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      Udiaeresis      Yacute          Thorn           ssharp          */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      agrave          aacute          acircumflex     atilde          */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      adiaeresis      aring           ae              ccedilla        */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      egrave          eacute          ecircumflex     ediaeresis      */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      igrave          iacute          icircumflex     idiaeresis      */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      eth             ntilde          ograve          oacute          */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      ocircumflex     otilde          odiaeresis      division        */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,        
                /*      oslash          ugrave          uacute          ucircumflex     */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape,      
                /*      udiaeresis      yacute          thorn           ydiaeresis      */
                VTStates.Escape, VTStates.Escape, VTStates.Escape, VTStates.Escape
            }
        };

        #endregion

        #region GroundTable

        /// <summary>
        /// 当状态机的状态是基态的时候所使用的状态转换表
        /// </summary>
        public static readonly StateTable GroundTable = new StateTable()
        {
            State = VTStates.Ground,
            StateTransitions = new List<VTStates>()
            {
                /*	NUL		SOH		STX		ETX	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	EOT		ENQ		ACK		BEL	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	BS		HT		LF		VT	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	FF		CR		SO		SI	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	DLE		DC1		DC2		DC3	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	DC4		NAK		SYN		ETB	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	CAN		EM		SUB		ESC	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Escape,
                /*	FS		GS		RS		US	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	SP		!		"		#	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	$		%		&		'	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	(		)		*		+	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	,		-		.		/	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	0		1		2		3	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	4		5		6		7	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	8		9		:		;	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	<		=		>		?	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	@		A		B		C	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	D		E		F		G	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	H		I		J		K	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	L		M		N		O	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	P		Q		R		S	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	T		U		V		W	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	X		Y		Z		[	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	\		]		^		_	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	`		a		b		c	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	d		e		f		g	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	h		i		j		k	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	l		m		n		o	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	p		q		r		s	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	t		u		v		w	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	x		y		z		{	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*	|		}		~		DEL	*/
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x80            0x81            0x82            0x83    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x84            0x85            0x86            0x87    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x88            0x89            0x8a            0x8b    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x8c            0x8d            0x8e            0x8f    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x90            0x91            0x92            0x93    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x94            0x95            0x96            0x97    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x98            0x99            0x9a            0x9b    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      0x9c            0x9d            0x9e            0x9f    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      nobreakspace    exclamdown      cent            sterling        */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,
                /*      currency        yen             brokenbar       section         */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      diaeresis       copyright       ordfeminine     guillemotleft   */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      notsign         hyphen          registered      macron          */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      degree          plusminus       twosuperior     threesuperior   */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      acute           mu              paragraph       periodcentered  */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      cedilla         onesuperior     masculine       guillemotright  */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      onequarter      onehalf         threequarters   questiondown    */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Agrave          Aacute          Acircumflex     Atilde          */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Adiaeresis      Aring           AE              Ccedilla        */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Eth             Ntilde          Ograve          Oacute          */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      Udiaeresis      Yacute          Thorn           ssharp          */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      agrave          aacute          acircumflex     atilde          */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      adiaeresis      aring           ae              ccedilla        */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      egrave          eacute          ecircumflex     ediaeresis      */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      igrave          iacute          icircumflex     idiaeresis      */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      eth             ntilde          ograve          oacute          */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      ocircumflex     otilde          odiaeresis      division        */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      oslash          ugrave          uacute          ucircumflex     */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,                
                /*      udiaeresis      yacute          thorn           ydiaeresis      */
                VTStates.Ground, VTStates.Ground, VTStates.Ground, VTStates.Ground,            
            }
        };

        #endregion

        /// <summary>
        /// 获取对应状态的状态机状态转换表
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static StateTable GetTable(VTStates state)
        {
            switch (state)
            {
                case VTStates.Ground: return GroundTable;
                case VTStates.Escape: return EscapeTable;
                case VTStates.OSC: return OSCTable;

                default:
                    logger.ErrorFormat("没实现{0}的状态转换表", state);
                    throw new NotImplementedException(string.Format("没实现{0}的状态转换表", state));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}Table", this.State);
        }
    }
}