using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    /*
     * 参考xterm-331 VTPsrTbl.c
     */
    public static class VTPrsTbl
    {
        #region SOSTable

        /// <summary>
        /// xterm-331 VTPrsTbl.c 6578行
        /// 
        /// 进入OSC状态后切换此状态表
        /// 
        /// 在OSC模式下，收集控制命令参数字符串，每一个CASE_IGNORE都是一个参数字符
        /// The CASE_IGNORE entries correspond to the
        /// characters that can be accumulated for the string function(e.g., OSC).
        /// 
        /// Start of String Table??
        /// </summary>
        public static readonly byte[] SOSTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,

        };

        #endregion

        #region CigTable

        /// <summary>
        /// 当收到CASE_CSI_IGNORE指令的时候，解析器使用的状态表
        /// </summary>
        public static readonly byte[] CIgTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_ENQ,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTPsrDef.CASE_BS,
            VTPsrDef.CASE_TAB,
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_CR,
            VTPsrDef.CASE_SO,
            VTPsrDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
        };

        #endregion

        #region CsiSpTable

        /// <summary>
        /// ascii字符和CSI的FinalByte字符的映射表
        /// </summary>
        public static readonly byte[] CsiSpTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_ENQ,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTPsrDef.CASE_BS,
            VTPsrDef.CASE_TAB,
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_CR,
            VTPsrDef.CASE_SO,
            VTPsrDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_SL,
            VTPsrDef.CASE_SR,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECSCUSR,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_DECSWBV,
            VTPsrDef.CASE_DECSMBV,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_SL,
            VTPsrDef.CASE_SR,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECSCUSR,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_DECSWBV,
            VTPsrDef.CASE_DECSMBV,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,

        };

        #endregion

        #region Csi2Table

        public static readonly byte[] Csi2Table =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_ENQ,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTPsrDef.CASE_BS,
            VTPsrDef.CASE_TAB,
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_CR,
            VTPsrDef.CASE_SO,
            VTPsrDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_CSI_SPACE_STATE,
            VTPsrDef.CASE_CSI_EX_STATE,
            VTPsrDef.CASE_CSI_QUOTE_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_CSI_DOLLAR_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_TICK_STATE,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_STAR_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_COLON,
            VTPsrDef.CASE_ESC_SEMI,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_ICH,
            VTPsrDef.CASE_CUU,
            VTPsrDef.CASE_CUD,
            VTPsrDef.CASE_CUF,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_CUB,
            VTPsrDef.CASE_CNL,
            VTPsrDef.CASE_CPL,
            VTPsrDef.CASE_HPA,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_CHT,
            VTPsrDef.CASE_ED,
            VTPsrDef.CASE_EL,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_IL,
            VTPsrDef.CASE_DL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_DCH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SU,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_TRACK_MOUSE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_ECH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_CBT,
            VTPsrDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_HPA,
            VTPsrDef.CASE_HPR,
            VTPsrDef.CASE_REP,
            VTPsrDef.CASE_DA1,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_VPA,
            VTPsrDef.CASE_VPR,
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_TBC,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_SET,
            VTPsrDef.CASE_MC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_RST,
            VTPsrDef.CASE_SGR,
            VTPsrDef.CASE_CPR,
            VTPsrDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECLL,
            VTPsrDef.CASE_DECSTBM,
            VTPsrDef.CASE_ANSI_SC,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_XTERM_WINOPS,
            VTPsrDef.CASE_ANSI_RC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_DECREQTPARM,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_CSI_SPACE_STATE,
            VTPsrDef.CASE_CSI_EX_STATE,
            VTPsrDef.CASE_CSI_QUOTE_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_CSI_DOLLAR_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_TICK_STATE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_STAR_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_COLON,
            VTPsrDef.CASE_ESC_SEMI,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_ICH,
            VTPsrDef.CASE_CUU,
            VTPsrDef.CASE_CUD,
            VTPsrDef.CASE_CUF,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_CUB,
            VTPsrDef.CASE_CNL,
            VTPsrDef.CASE_CPL,
            VTPsrDef.CASE_HPA,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_CHT,
            VTPsrDef.CASE_ED,
            VTPsrDef.CASE_EL,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_IL,
            VTPsrDef.CASE_DL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_DCH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SU,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_TRACK_MOUSE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_ECH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_CBT,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_HPA,
            VTPsrDef.CASE_HPR,
            VTPsrDef.CASE_REP,
            VTPsrDef.CASE_DA1,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_VPA,
            VTPsrDef.CASE_VPR,
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_TBC,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_SET,
            VTPsrDef.CASE_MC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_RST,
            VTPsrDef.CASE_SGR,
            VTPsrDef.CASE_CPR,
            VTPsrDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECLL,
            VTPsrDef.CASE_DECSTBM,
            VTPsrDef.CASE_ANSI_SC,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_XTERM_WINOPS,
            VTPsrDef.CASE_ANSI_RC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_DECREQTPARM,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,

        };

        #endregion

        #region CsiTable

        /// <summary>
        /// ascii码与csi控制指令的映射关系表
        /// </summary>
        public static readonly byte[] CSITable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_ENQ,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTPsrDef.CASE_BS,
            VTPsrDef.CASE_TAB,
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_CR,
            VTPsrDef.CASE_SO,
            VTPsrDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_CSI_SPACE_STATE,
            VTPsrDef.CASE_CSI_EX_STATE,
            VTPsrDef.CASE_CSI_QUOTE_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_CSI_DOLLAR_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_TICK_STATE,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_COLON,
            VTPsrDef.CASE_ESC_SEMI,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_DEC3_STATE,
            VTPsrDef.CASE_DEC2_STATE,
            VTPsrDef.CASE_DEC_STATE,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_ICH,
            VTPsrDef.CASE_CUU,
            VTPsrDef.CASE_CUD,
            VTPsrDef.CASE_CUF,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_CUB,
            VTPsrDef.CASE_CNL,
            VTPsrDef.CASE_CPL,
            VTPsrDef.CASE_HPA,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_CHT,
            VTPsrDef.CASE_ED,
            VTPsrDef.CASE_EL,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_IL,
            VTPsrDef.CASE_DL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_DCH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SU,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_TRACK_MOUSE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_ECH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_CBT,
            VTPsrDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_HPA,
            VTPsrDef.CASE_HPR,
            VTPsrDef.CASE_REP,
            VTPsrDef.CASE_DA1,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_VPA,
            VTPsrDef.CASE_VPR,
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_TBC,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_SET,
            VTPsrDef.CASE_MC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_RST,
            VTPsrDef.CASE_SGR,
            VTPsrDef.CASE_CPR,
            VTPsrDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECLL,
            VTPsrDef.CASE_DECSTBM,
            VTPsrDef.CASE_ANSI_SC,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_XTERM_WINOPS,
            VTPsrDef.CASE_ANSI_RC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_DECREQTPARM,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_CSI_SPACE_STATE,
            VTPsrDef.CASE_CSI_EX_STATE,
            VTPsrDef.CASE_CSI_QUOTE_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_CSI_DOLLAR_STATE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_TICK_STATE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_DIGIT,
            VTPsrDef.CASE_ESC_COLON,
            VTPsrDef.CASE_ESC_SEMI,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_CSI_IGNORE,
            VTPsrDef.CASE_DEC3_STATE,
            VTPsrDef.CASE_DEC2_STATE,
            VTPsrDef.CASE_DEC_STATE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_ICH,
            VTPsrDef.CASE_CUU,
            VTPsrDef.CASE_CUD,
            VTPsrDef.CASE_CUF,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_CUB,
            VTPsrDef.CASE_CNL,
            VTPsrDef.CASE_CPL,
            VTPsrDef.CASE_HPA,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_CHT,
            VTPsrDef.CASE_ED,
            VTPsrDef.CASE_EL,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_IL,
            VTPsrDef.CASE_DL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_DCH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SU,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_TRACK_MOUSE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_ECH,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_CBT,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_HPA,
            VTPsrDef.CASE_HPR,
            VTPsrDef.CASE_REP,
            VTPsrDef.CASE_DA1,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_VPA,
            VTPsrDef.CASE_VPR,
            VTPsrDef.CASE_CUP,
            VTPsrDef.CASE_TBC,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_SET,
            VTPsrDef.CASE_MC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_RST,
            VTPsrDef.CASE_SGR,
            VTPsrDef.CASE_CPR,
            VTPsrDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECLL,
            VTPsrDef.CASE_DECSTBM,
            VTPsrDef.CASE_ANSI_SC,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_XTERM_WINOPS,
            VTPsrDef.CASE_ANSI_RC,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_DECREQTPARM,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
        };

        #endregion

        #region EscTable

        /// <summary>
        /// ascii码与Esc控制指令的映射关系表
        /// </summary>
        public static readonly byte[] EscTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_ENQ,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTPsrDef.CASE_BS,
            VTPsrDef.CASE_TAB,
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_VMOT,
            VTPsrDef.CASE_CR,
            VTPsrDef.CASE_SO,
            VTPsrDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_ESC_SP_STATE,
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_SCR_STATE,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_ESC_PERCENT,
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_ESC_IGNORE,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_SCS0_STATE,
            VTPsrDef.CASE_SCS1_STATE,
            VTPsrDef.CASE_SCS2_STATE,
            VTPsrDef.CASE_SCS3_STATE,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_SCS1A_STATE,
            VTPsrDef.CASE_SCS2A_STATE,
            VTPsrDef.CASE_SCS3A_STATE,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECBI,
            VTPsrDef.CASE_DECSC,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_DECRC,
            VTPsrDef.CASE_DECFI,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECKPAM,
            VTPsrDef.CASE_DECKPNM,
            VTPsrDef.CASE_GROUND_STATE,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_HP_BUGGY_LL,
            VTPsrDef.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_XTERM_TITLE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RIS,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_HP_MEM_LOCK,
            VTPsrDef.CASE_HP_MEM_UNLOCK,
            VTPsrDef.CASE_LS2,
            VTPsrDef.CASE_LS3,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_LS3R,
            VTPsrDef.CASE_LS2R,
            VTPsrDef.CASE_LS1R,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_ESC_SP_STATE,
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_SCR_STATE,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_ESC_PERCENT,
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_ESC_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_SCS0_STATE,
            VTPsrDef.CASE_SCS1_STATE,
            VTPsrDef.CASE_SCS2_STATE,
            VTPsrDef.CASE_SCS3_STATE,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_ESC_IGNORE,
            VTPsrDef.CASE_SCS1A_STATE,
            VTPsrDef.CASE_SCS2A_STATE,
            VTPsrDef.CASE_SCS3A_STATE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECBI,
            VTPsrDef.CASE_DECSC,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_DECRC,
            VTPsrDef.CASE_DECFI,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECKPAM,
            VTPsrDef.CASE_DECKPNM,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_HP_BUGGY_LL,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_XTERM_TITLE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RIS,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_HP_MEM_LOCK,
            VTPsrDef.CASE_HP_MEM_UNLOCK,
            VTPsrDef.CASE_LS2,
            VTPsrDef.CASE_LS3,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_LS3R,
            VTPsrDef.CASE_LS2R,
            VTPsrDef.CASE_LS1R,
            VTPsrDef.CASE_IGNORE
        };

        #endregion

        #region AnsiTable

        /// <summary>
        /// Ansi字符与控制功能之间的映射关系表
        /// 数组的索引代表Ansi字符的值，索引的值代表Ansi字符的控制功能
        /// </summary>
        public static readonly byte[] AnsiTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_ENQ,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_BELL,
            /*	BS		HT		LF		VT	*/
            VTPsrDef.CASE_BS,
            VTPsrDef.CASE_TAB,
            VTPsrDef.CASE_LF,
            VTPsrDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTPsrDef.CASE_FF,
            VTPsrDef.CASE_CR,
            VTPsrDef.CASE_SO,
            VTPsrDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            VTPsrDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	$		%		&		'	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	(		)		*		+	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	,		-		.		/	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	0		1		2		3	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	4		5		6		7	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	8		9		:		;	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	<		=		>		?	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	@		A		B		C	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	D		E		F		G	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	H		I		J		K	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	L		M		N		O	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	P		Q		R		S	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	T		U		V		W	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	X		Y		Z		[	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	\		]		^		_	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	`		a		b		c	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	d		e		f		g	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	h		i		j		k	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	l		m		n		o	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	p		q		r		s	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	t		u		v		w	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	x		y		z		{	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*	|		}		~		DEL	*/
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTPsrDef.CASE_IND,
            VTPsrDef.CASE_NEL,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTPsrDef.CASE_HTS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_RI,
            VTPsrDef.CASE_SS2,
            VTPsrDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTPsrDef.CASE_DCS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_SPA,
            VTPsrDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTPsrDef.CASE_SOS,
            VTPsrDef.CASE_GROUND_STATE,
            VTPsrDef.CASE_DECID,
            VTPsrDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTPsrDef.CASE_ST,
            VTPsrDef.CASE_OSC,
            VTPsrDef.CASE_PM,
            VTPsrDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      currency        yen             brokenbar       section         */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      notsign         hyphen          registered      macron          */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      acute           mu              paragraph       periodcentered  */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      agrave          aacute          acircumflex     atilde          */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      eth             ntilde          ograve          oacute          */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
            VTPsrDef.CASE_PRINT,
        };

        #endregion
    }
}