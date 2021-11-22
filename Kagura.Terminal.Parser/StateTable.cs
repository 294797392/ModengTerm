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
        public List<StateMachineID> StateMachineList { get; private set; }

        #region CigTable

        /// <summary>
        /// 当收到CASE_CSI_IGNORE指令的时候，解析器使用的状态表
        /// </summary>
        public static readonly StateMachineID[] CIgTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_ENQ,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            StateMachineID.CASE_BS,
            StateMachineID.CASE_TAB,
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_CR,
            StateMachineID.CASE_SO,
            StateMachineID.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_ESC,
            /*	FS		GS		RS		US	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	SP		!		"		#	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	$		%		&		'	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	(		)		*		+	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	,		-		.		/	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	0		1		2		3	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	4		5		6		7	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	8		9		:		;	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	<		=		>		?	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	@		A		B		C	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            StateMachineID.CASE_IND,
            StateMachineID.CASE_NEL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            StateMachineID.CASE_HTS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_RI,
            StateMachineID.CASE_SS2,
            StateMachineID.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            StateMachineID.CASE_DCS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SPA,
            StateMachineID.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            StateMachineID.CASE_SOS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECID,
            StateMachineID.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            StateMachineID.CASE_ST,
            StateMachineID.SMID_OSC,
            StateMachineID.CASE_PM,
            StateMachineID.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      currency        yen             brokenbar       section         */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      adiaeresis      aring           ae              ccedilla        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
        };

        #endregion

        #region CsiSpTable

        /// <summary>
        /// ascii字符和CSI的FinalByte字符的映射表
        /// </summary>
        public static readonly StateMachineID[] CsiSpTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_ENQ,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            StateMachineID.CASE_BS,
            StateMachineID.CASE_TAB,
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_CR,
            StateMachineID.CASE_SO,
            StateMachineID.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_ESC,
            /*	FS		GS		RS		US	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	SP		!		"		#	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	(		)		*		+	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	4		5		6		7	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	8		9		:		;	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	<		=		>		?	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	@		A		B		C	*/
            StateMachineID.CASE_SL,
            StateMachineID.CASE_SR,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECSCUSR,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            StateMachineID.CASE_DECSWBV,
            StateMachineID.CASE_DECSMBV,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            StateMachineID.CASE_IND,
            StateMachineID.CASE_NEL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            StateMachineID.CASE_HTS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_RI,
            StateMachineID.CASE_SS2,
            StateMachineID.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            StateMachineID.CASE_DCS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SPA,
            StateMachineID.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            StateMachineID.CASE_SOS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECID,
            StateMachineID.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            StateMachineID.CASE_ST,
            StateMachineID.SMID_OSC,
            StateMachineID.CASE_PM,
            StateMachineID.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            StateMachineID.CASE_SL,
            StateMachineID.CASE_SR,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      adiaeresis      aring           ae              ccedilla        */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECSCUSR,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            StateMachineID.CASE_DECSWBV,
            StateMachineID.CASE_DECSMBV,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,

        };

        #endregion

        #region Csi2Table

        public static readonly StateMachineID[] Csi2Table =
        {
            /*	NUL		SOH		STX		ETX	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_ENQ,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            StateMachineID.CASE_BS,
            StateMachineID.CASE_TAB,
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_CR,
            StateMachineID.CASE_SO,
            StateMachineID.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_ESC,
            /*	FS		GS		RS		US	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	SP		!		"		#	*/
            StateMachineID.CASE_CSI_SPACE_STATE,
            StateMachineID.CASE_CSI_EX_STATE,
            StateMachineID.CASE_CSI_QUOTE_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            StateMachineID.CASE_CSI_DOLLAR_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_TICK_STATE,
            /*	(		)		*		+	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_STAR_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*	4		5		6		7	*/
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*	8		9		:		;	*/
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_COLON,
            StateMachineID.CASE_ESC_SEMI,
            /*	<		=		>		?	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	@		A		B		C	*/
            StateMachineID.CASE_ICH,
            StateMachineID.CASE_CUU,
            StateMachineID.CASE_CUD,
            StateMachineID.CASE_CUF,
            /*	D		E		F		G	*/
            StateMachineID.CASE_CUB,
            StateMachineID.CASE_CNL,
            StateMachineID.CASE_CPL,
            StateMachineID.CASE_HPA,
            /*	H		I		J		K	*/
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_CHT,
            StateMachineID.CASE_ED,
            StateMachineID.CASE_EL,
            /*	L		M		N		O	*/
            StateMachineID.CASE_IL,
            StateMachineID.CASE_DL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            StateMachineID.CASE_DCH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SU,
            /*	T		U		V		W	*/
            StateMachineID.CASE_TRACK_MOUSE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            StateMachineID.CASE_ECH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_CBT,
            StateMachineID.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            StateMachineID.CASE_HPA,
            StateMachineID.CASE_HPR,
            StateMachineID.CASE_REP,
            StateMachineID.CASE_DA1,
            /*	d		e		f		g	*/
            StateMachineID.CASE_VPA,
            StateMachineID.CASE_VPR,
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_TBC,
            /*	h		i		j		k	*/
            StateMachineID.CASE_SET,
            StateMachineID.CASE_MC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            StateMachineID.CASE_RST,
            StateMachineID.CASE_SGR,
            StateMachineID.CASE_CPR,
            StateMachineID.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECLL,
            StateMachineID.CASE_DECSTBM,
            StateMachineID.CASE_ANSI_SC,
            /*	t		u		v		w	*/
            StateMachineID.CASE_XTERM_WINOPS,
            StateMachineID.CASE_ANSI_RC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            StateMachineID.CASE_DECREQTPARM,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            StateMachineID.CASE_IND,
            StateMachineID.CASE_NEL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            StateMachineID.CASE_HTS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_RI,
            StateMachineID.CASE_SS2,
            StateMachineID.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            StateMachineID.CASE_DCS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SPA,
            StateMachineID.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            StateMachineID.CASE_SOS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECID,
            StateMachineID.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            StateMachineID.CASE_ST,
            StateMachineID.SMID_OSC,
            StateMachineID.CASE_PM,
            StateMachineID.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            StateMachineID.CASE_CSI_SPACE_STATE,
            StateMachineID.CASE_CSI_EX_STATE,
            StateMachineID.CASE_CSI_QUOTE_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            StateMachineID.CASE_CSI_DOLLAR_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_TICK_STATE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_STAR_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*      acute           mu              paragraph       periodcentered  */
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_COLON,
            StateMachineID.CASE_ESC_SEMI,
            /*      onequarter      onehalf         threequarters   questiondown    */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            StateMachineID.CASE_ICH,
            StateMachineID.CASE_CUU,
            StateMachineID.CASE_CUD,
            StateMachineID.CASE_CUF,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            StateMachineID.CASE_CUB,
            StateMachineID.CASE_CNL,
            StateMachineID.CASE_CPL,
            StateMachineID.CASE_HPA,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_CHT,
            StateMachineID.CASE_ED,
            StateMachineID.CASE_EL,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            StateMachineID.CASE_IL,
            StateMachineID.CASE_DL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            StateMachineID.CASE_DCH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SU,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            StateMachineID.CASE_TRACK_MOUSE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            StateMachineID.CASE_ECH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_CBT,
            StateMachineID.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            StateMachineID.CASE_HPA,
            StateMachineID.CASE_HPR,
            StateMachineID.CASE_REP,
            StateMachineID.CASE_DA1,
            /*      adiaeresis      aring           ae              ccedilla        */
            StateMachineID.CASE_VPA,
            StateMachineID.CASE_VPR,
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_TBC,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            StateMachineID.CASE_SET,
            StateMachineID.CASE_MC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            StateMachineID.CASE_RST,
            StateMachineID.CASE_SGR,
            StateMachineID.CASE_CPR,
            StateMachineID.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECLL,
            StateMachineID.CASE_DECSTBM,
            StateMachineID.CASE_ANSI_SC,
            /*      ocircumflex     otilde          odiaeresis      division        */
            StateMachineID.CASE_XTERM_WINOPS,
            StateMachineID.CASE_ANSI_RC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            StateMachineID.CASE_DECREQTPARM,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,

        };

        #endregion

        #region CsiTable

        /// <summary>
        /// ascii码与csi控制指令的映射关系表
        /// </summary>
        public static readonly StateMachineID[] CSITable =
        {
            /*	NUL		SOH		STX		ETX	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_ENQ,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            StateMachineID.CASE_BS,
            StateMachineID.CASE_TAB,
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            StateMachineID.CASE_VMOT,
            StateMachineID.CASE_CR,
            StateMachineID.CASE_SO,
            StateMachineID.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_ESC,
            /*	FS		GS		RS		US	*/
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            StateMachineID.CASE_IGNORE,
            /*	SP		!		"		#	*/
            StateMachineID.CASE_CSI_SPACE_STATE,
            StateMachineID.CASE_CSI_EX_STATE,
            StateMachineID.CASE_CSI_QUOTE_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            StateMachineID.CASE_CSI_DOLLAR_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_TICK_STATE,
            /*	(		)		*		+	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*	4		5		6		7	*/
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*	8		9		:		;	*/
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_COLON,
            StateMachineID.CASE_ESC_SEMI,
            /*	<		=		>		?	*/
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_DEC3_STATE,
            StateMachineID.CASE_DEC2_STATE,
            StateMachineID.CASE_DEC_STATE,
            /*	@		A		B		C	*/
            StateMachineID.CASE_ICH,
            StateMachineID.CASE_CUU,
            StateMachineID.CASE_CUD,
            StateMachineID.CASE_CUF,
            /*	D		E		F		G	*/
            StateMachineID.CASE_CUB,
            StateMachineID.CASE_CNL,
            StateMachineID.CASE_CPL,
            StateMachineID.CASE_HPA,
            /*	H		I		J		K	*/
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_CHT,
            StateMachineID.CASE_ED,
            StateMachineID.CASE_EL,
            /*	L		M		N		O	*/
            StateMachineID.CASE_IL,
            StateMachineID.CASE_DL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            StateMachineID.CASE_DCH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SU,
            /*	T		U		V		W	*/
            StateMachineID.CASE_TRACK_MOUSE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            StateMachineID.CASE_ECH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_CBT,
            StateMachineID.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            StateMachineID.CASE_HPA,
            StateMachineID.CASE_HPR,
            StateMachineID.CASE_REP,
            StateMachineID.CASE_DA1,
            /*	d		e		f		g	*/
            StateMachineID.CASE_VPA,
            StateMachineID.CASE_VPR,
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_TBC,
            /*	h		i		j		k	*/
            StateMachineID.CASE_SET,
            StateMachineID.CASE_MC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            StateMachineID.CASE_RST,
            StateMachineID.CASE_SGR,
            StateMachineID.CASE_CPR,
            StateMachineID.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECLL,
            StateMachineID.CASE_DECSTBM,
            StateMachineID.CASE_ANSI_SC,
            /*	t		u		v		w	*/
            StateMachineID.CASE_XTERM_WINOPS,
            StateMachineID.CASE_ANSI_RC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            StateMachineID.CASE_DECREQTPARM,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            StateMachineID.CASE_IND,
            StateMachineID.CASE_NEL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            StateMachineID.CASE_HTS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_RI,
            StateMachineID.CASE_SS2,
            StateMachineID.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            StateMachineID.CASE_DCS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SPA,
            StateMachineID.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            StateMachineID.CASE_SOS,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECID,
            StateMachineID.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            StateMachineID.CASE_ST,
            StateMachineID.SMID_OSC,
            StateMachineID.CASE_PM,
            StateMachineID.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            StateMachineID.CASE_CSI_SPACE_STATE,
            StateMachineID.CASE_CSI_EX_STATE,
            StateMachineID.CASE_CSI_QUOTE_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            StateMachineID.CASE_CSI_DOLLAR_STATE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_TICK_STATE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*      acute           mu              paragraph       periodcentered  */
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_DIGIT,
            StateMachineID.CASE_ESC_COLON,
            StateMachineID.CASE_ESC_SEMI,
            /*      onequarter      onehalf         threequarters   questiondown    */
            StateMachineID.CASE_CSI_IGNORE,
            StateMachineID.CASE_DEC3_STATE,
            StateMachineID.CASE_DEC2_STATE,
            StateMachineID.CASE_DEC_STATE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            StateMachineID.CASE_ICH,
            StateMachineID.CASE_CUU,
            StateMachineID.CASE_CUD,
            StateMachineID.CASE_CUF,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            StateMachineID.CASE_CUB,
            StateMachineID.CASE_CNL,
            StateMachineID.CASE_CPL,
            StateMachineID.CASE_HPA,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_CHT,
            StateMachineID.CASE_ED,
            StateMachineID.CASE_EL,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            StateMachineID.CASE_IL,
            StateMachineID.CASE_DL,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            StateMachineID.CASE_DCH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_SU,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            StateMachineID.CASE_TRACK_MOUSE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            StateMachineID.CASE_ECH,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_CBT,
            StateMachineID.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            StateMachineID.CASE_HPA,
            StateMachineID.CASE_HPR,
            StateMachineID.CASE_REP,
            StateMachineID.CASE_DA1,
            /*      adiaeresis      aring           ae              ccedilla        */
            StateMachineID.CASE_VPA,
            StateMachineID.CASE_VPR,
            StateMachineID.CASE_CUP,
            StateMachineID.CASE_TBC,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            StateMachineID.CASE_SET,
            StateMachineID.CASE_MC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            StateMachineID.CASE_RST,
            StateMachineID.CASE_SGR,
            StateMachineID.CASE_CPR,
            StateMachineID.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_DECLL,
            StateMachineID.CASE_DECSTBM,
            StateMachineID.CASE_ANSI_SC,
            /*      ocircumflex     otilde          odiaeresis      division        */
            StateMachineID.CASE_XTERM_WINOPS,
            StateMachineID.CASE_ANSI_RC,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            StateMachineID.CASE_DECREQTPARM,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_GROUND_STATE,
            StateMachineID.CASE_IGNORE,
        };

        #endregion

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
            StateMachineList = new List<StateMachineID>()
            {
                /*	NUL		SOH		STX		ETX	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	EOT		ENQ		ACK		BEL	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	BS		HT		NL		VT	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	FF		CR		SO		SI	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	DLE		DC1		DC2		DC3	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	DC4		NAK		SYN		ETB	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	CAN		EM		SUB		ESC	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.SMID_OSC,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_ESC,
                /*	FS		GS		RS		US	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	SP		!		"		#	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	$		%		&		'	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	(		)		*		+	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	,		-		.		/	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	0		1		2		3	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	4		5		6		7	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	8		9		:		;	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	<		=		>		?	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	@		A		B		C	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	D		E		F		G	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	H		I		J		K	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	L		M		N		O	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	P		Q		R		S	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	T		U		V		W	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	X		Y		Z		[	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	\		]		^		_	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	`		a		b		c	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	d		e		f		g	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	h		i		j		k	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	l		m		n		o	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	p		q		r		s	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	t		u		v		w	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	x		y		z		{	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*	|		}		~		DEL	*/
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      0x80            0x81            0x82            0x83    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x84            0x85            0x86            0x87    */
                StateMachineID.CASE_IND,
                StateMachineID.CASE_NEL,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x88            0x89            0x8a            0x8b    */
                StateMachineID.CASE_HTS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x8c            0x8d            0x8e            0x8f    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RI,
                StateMachineID.CASE_SS2,
                StateMachineID.CASE_SS3,
                /*      0x90            0x91            0x92            0x93    */
                StateMachineID.CASE_DCS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x94            0x95            0x96            0x97    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_SPA,
                StateMachineID.CASE_EPA,
                /*      0x98            0x99            0x9a            0x9b    */
                StateMachineID.CASE_SOS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECID,
                StateMachineID.CASE_CSI_STATE,
                /*      0x9c            0x9d            0x9e            0x9f    */
                StateMachineID.CASE_ST,
                StateMachineID.SMID_OSC,
                StateMachineID.CASE_PM,
                StateMachineID.CASE_APC,
                /*      nobreakspace    exclamdown      cent            sterling        */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      currency        yen             brokenbar       section         */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      diaeresis       copyright       ordfeminine     guillemotleft   */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      notsign         hyphen          registered      macron          */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      degree          plusminus       twosuperior     threesuperior   */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      acute           mu              paragraph       periodcentered  */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      cedilla         onesuperior     masculine       guillemotright  */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      onequarter      onehalf         threequarters   questiondown    */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Agrave          Aacute          Acircumflex     Atilde          */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Adiaeresis      Aring           AE              Ccedilla        */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Eth             Ntilde          Ograve          Oacute          */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      Udiaeresis      Yacute          Thorn           ssharp          */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      agrave          aacute          acircumflex     atilde          */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      adiaeresis      aring           ae              ccedilla        */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      egrave          eacute          ecircumflex     ediaeresis      */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      igrave          iacute          icircumflex     idiaeresis      */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      eth             ntilde          ograve          oacute          */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      ocircumflex     otilde          odiaeresis      division        */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      oslash          ugrave          uacute          ucircumflex     */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                /*      udiaeresis      yacute          thorn           ydiaeresis      */
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
                StateMachineID.SMID_OSC,
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
            StateMachineList = new List<StateMachineID>()
            {
                /*	NUL		SOH		STX		ETX	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	EOT		ENQ		ACK		BEL	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_ENQ,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_BELL,
                /*	BS		HT		NL		VT	*/
                StateMachineID.CASE_BS,
                StateMachineID.CASE_TAB,
                StateMachineID.CASE_VMOT,
                StateMachineID.CASE_VMOT,
                /*	FF		CR		SO		SI	*/
                StateMachineID.CASE_VMOT,
                StateMachineID.CASE_CR,
                StateMachineID.CASE_SO,
                StateMachineID.CASE_SI,
                /*	DLE		DC1		DC2		DC3	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	DC4		NAK		SYN		ETB	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	CAN		EM		SUB		ESC	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_ESC,
                /*	FS		GS		RS		US	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	SP		!		"		#	*/
                StateMachineID.CASE_ESC_SP_STATE,
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_SCR_STATE,
                /*	$		%		&		'	*/
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_ESC_PERCENT,
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_ESC_IGNORE,
                /*	(		)		*		+	*/
                StateMachineID.CASE_SCS0_STATE,
                StateMachineID.CASE_SCS1_STATE,
                StateMachineID.CASE_SCS2_STATE,
                StateMachineID.CASE_SCS3_STATE,
                /*	,		-		.		/	*/
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_SCS1A_STATE,
                StateMachineID.CASE_SCS2A_STATE,
                StateMachineID.CASE_SCS3A_STATE,
                /*	0		1		2		3	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	4		5		6		7	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECBI,
                StateMachineID.CASE_DECSC,
                /*	8		9		:		;	*/
                StateMachineID.CASE_DECRC,
                StateMachineID.CASE_DECFI,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	<		=		>		?	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECKPAM,
                StateMachineID.CASE_DECKPNM,
                StateMachineID.CASE_GROUND_STATE,
                /*	@		A		B		C	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	D		E		F		G	*/
                StateMachineID.CASE_IND,
                StateMachineID.CASE_NEL,
                StateMachineID.CASE_HP_BUGGY_LL,
                StateMachineID.CASE_GROUND_STATE,
                /*	H		I		J		K	*/
                StateMachineID.CASE_HTS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	L		M		N		O	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RI,
                StateMachineID.CASE_SS2,
                StateMachineID.CASE_SS3,
                /*	P		Q		R		S	*/
                StateMachineID.CASE_DCS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	T		U		V		W	*/
                StateMachineID.CASE_XTERM_TITLE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_SPA,
                StateMachineID.CASE_EPA,
                /*	X		Y		Z		[	*/
                StateMachineID.CASE_SOS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECID,
                StateMachineID.CASE_CSI_STATE,
                /*	\		]		^		_	*/
                StateMachineID.CASE_ST,
                StateMachineID.SMID_OSC,
                StateMachineID.CASE_PM,
                StateMachineID.CASE_APC,
                /*	`		a		b		c	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RIS,
                /*	d		e		f		g	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	h		i		j		k	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	l		m		n		o	*/
                StateMachineID.CASE_HP_MEM_LOCK,
                StateMachineID.CASE_HP_MEM_UNLOCK,
                StateMachineID.CASE_LS2,
                StateMachineID.CASE_LS3,
                /*	p		q		r		s	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	t		u		v		w	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	x		y		z		{	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*	|		}		~		DEL	*/
                StateMachineID.CASE_LS3R,
                StateMachineID.CASE_LS2R,
                StateMachineID.CASE_LS1R,
                StateMachineID.CASE_IGNORE,
                /*      0x80            0x81            0x82            0x83    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x84            0x85            0x86            0x87    */
                StateMachineID.CASE_IND,
                StateMachineID.CASE_NEL,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x88            0x89            0x8a            0x8b    */
                StateMachineID.CASE_HTS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x8c            0x8d            0x8e            0x8f    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RI,
                StateMachineID.CASE_SS2,
                StateMachineID.CASE_SS3,
                /*      0x90            0x91            0x92            0x93    */
                StateMachineID.CASE_DCS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x94            0x95            0x96            0x97    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_SPA,
                StateMachineID.CASE_EPA,
                /*      0x98            0x99            0x9a            0x9b    */
                StateMachineID.CASE_SOS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECID,
                StateMachineID.CASE_CSI_STATE,
                /*      0x9c            0x9d            0x9e            0x9f    */
                StateMachineID.CASE_ST,
                StateMachineID.SMID_OSC,
                StateMachineID.CASE_PM,
                StateMachineID.CASE_APC,
                /*      nobreakspace    exclamdown      cent            sterling        */
                StateMachineID.CASE_ESC_SP_STATE,
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_SCR_STATE,
                /*      currency        yen             brokenbar       section         */
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_ESC_PERCENT,
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_ESC_IGNORE,
                /*      diaeresis       copyright       ordfeminine     guillemotleft   */
                StateMachineID.CASE_SCS0_STATE,
                StateMachineID.CASE_SCS1_STATE,
                StateMachineID.CASE_SCS2_STATE,
                StateMachineID.CASE_SCS3_STATE,
                /*      notsign         hyphen          registered      macron          */
                StateMachineID.CASE_ESC_IGNORE,
                StateMachineID.CASE_SCS1A_STATE,
                StateMachineID.CASE_SCS2A_STATE,
                StateMachineID.CASE_SCS3A_STATE,
                /*      degree          plusminus       twosuperior     threesuperior   */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      acute           mu              paragraph       periodcentered  */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECBI,
                StateMachineID.CASE_DECSC,
                /*      cedilla         onesuperior     masculine       guillemotright  */
                StateMachineID.CASE_DECRC,
                StateMachineID.CASE_DECFI,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      onequarter      onehalf         threequarters   questiondown    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECKPAM,
                StateMachineID.CASE_DECKPNM,
                StateMachineID.CASE_GROUND_STATE,
                /*      Agrave          Aacute          Acircumflex     Atilde          */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      Adiaeresis      Aring           AE              Ccedilla        */
                StateMachineID.CASE_IND,
                StateMachineID.CASE_NEL,
                StateMachineID.CASE_HP_BUGGY_LL,
                StateMachineID.CASE_GROUND_STATE,
                /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
                StateMachineID.CASE_HTS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RI,
                StateMachineID.CASE_SS2,
                StateMachineID.CASE_SS3,
                /*      Eth             Ntilde          Ograve          Oacute          */
                StateMachineID.CASE_DCS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
                StateMachineID.CASE_XTERM_TITLE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_SPA,
                StateMachineID.CASE_EPA,
                /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
                StateMachineID.CASE_SOS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECID,
                StateMachineID.CASE_CSI_STATE,
                /*      Udiaeresis      Yacute          Thorn           ssharp          */
                StateMachineID.CASE_ST,
                StateMachineID.SMID_OSC,
                StateMachineID.CASE_PM,
                StateMachineID.CASE_APC,
                /*      agrave          aacute          acircumflex     atilde          */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RIS,
                /*      adiaeresis      aring           ae              ccedilla        */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      egrave          eacute          ecircumflex     ediaeresis      */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      igrave          iacute          icircumflex     idiaeresis      */
                StateMachineID.CASE_HP_MEM_LOCK,
                StateMachineID.CASE_HP_MEM_UNLOCK,
                StateMachineID.CASE_LS2,
                StateMachineID.CASE_LS3,
                /*      eth             ntilde          ograve          oacute          */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      ocircumflex     otilde          odiaeresis      division        */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      oslash          ugrave          uacute          ucircumflex     */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      udiaeresis      yacute          thorn           ydiaeresis      */
                StateMachineID.CASE_LS3R,
                StateMachineID.CASE_LS2R,
                StateMachineID.CASE_LS1R,
                StateMachineID.CASE_IGNORE
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
            StateMachineList = new List<StateMachineID>()
            {
                /*	NUL		SOH		STX		ETX	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	EOT		ENQ		ACK		BEL	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_ENQ,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_BELL,
                /*	BS		HT		LF		VT	*/
                StateMachineID.CASE_BS,
                StateMachineID.CASE_TAB,
                StateMachineID.CASE_LF,
                StateMachineID.CASE_VMOT,
                /*	FF		CR		SO		SI	*/
                StateMachineID.CASE_FF,
                StateMachineID.CASE_CR,
                StateMachineID.CASE_SO,
                StateMachineID.CASE_SI,
                /*	DLE		DC1		DC2		DC3	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	DC4		NAK		SYN		ETB	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	CAN		EM		SUB		ESC	*/
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_ESC,
                /*	FS		GS		RS		US	*/
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                StateMachineID.CASE_IGNORE,
                /*	SP		!		"		#	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	$		%		&		'	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	(		)		*		+	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	,		-		.		/	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	0		1		2		3	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	4		5		6		7	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	8		9		:		;	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	<		=		>		?	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	@		A		B		C	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	D		E		F		G	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	H		I		J		K	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	L		M		N		O	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	P		Q		R		S	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	T		U		V		W	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	X		Y		Z		[	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	\		]		^		_	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	`		a		b		c	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	d		e		f		g	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	h		i		j		k	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	l		m		n		o	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	p		q		r		s	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	t		u		v		w	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	x		y		z		{	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*	|		}		~		DEL	*/
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_IGNORE,
                /*      0x80            0x81            0x82            0x83    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x84            0x85            0x86            0x87    */
                StateMachineID.CASE_IND,
                StateMachineID.CASE_NEL,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x88            0x89            0x8a            0x8b    */
                StateMachineID.CASE_HTS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x8c            0x8d            0x8e            0x8f    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_RI,
                StateMachineID.CASE_SS2,
                StateMachineID.CASE_SS3,
                /*      0x90            0x91            0x92            0x93    */
                StateMachineID.CASE_DCS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                /*      0x94            0x95            0x96            0x97    */
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_SPA,
                StateMachineID.CASE_EPA,
                /*      0x98            0x99            0x9a            0x9b    */
                StateMachineID.CASE_SOS,
                StateMachineID.CASE_GROUND_STATE,
                StateMachineID.CASE_DECID,
                StateMachineID.CASE_CSI_STATE,
                /*      0x9c            0x9d            0x9e            0x9f    */
                StateMachineID.CASE_ST,
                StateMachineID.SMID_OSC,
                StateMachineID.CASE_PM,
                StateMachineID.CASE_APC,
                /*      nobreakspace    exclamdown      cent            sterling        */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      currency        yen             brokenbar       section         */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      diaeresis       copyright       ordfeminine     guillemotleft   */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      notsign         hyphen          registered      macron          */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      degree          plusminus       twosuperior     threesuperior   */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      acute           mu              paragraph       periodcentered  */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      cedilla         onesuperior     masculine       guillemotright  */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      onequarter      onehalf         threequarters   questiondown    */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Agrave          Aacute          Acircumflex     Atilde          */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Adiaeresis      Aring           AE              Ccedilla        */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Eth             Ntilde          Ograve          Oacute          */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      Udiaeresis      Yacute          Thorn           ssharp          */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      agrave          aacute          acircumflex     atilde          */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      adiaeresis      aring           ae              ccedilla        */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      egrave          eacute          ecircumflex     ediaeresis      */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      igrave          iacute          icircumflex     idiaeresis      */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      eth             ntilde          ograve          oacute          */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      ocircumflex     otilde          odiaeresis      division        */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      oslash          ugrave          uacute          ucircumflex     */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                /*      udiaeresis      yacute          thorn           ydiaeresis      */
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
                StateMachineID.CASE_PRINT,
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
    }
}