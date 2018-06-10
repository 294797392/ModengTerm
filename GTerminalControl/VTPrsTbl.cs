using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalControl
{
    /*
     * 参考xterm-331 VTPsrTbl.c
     */
    public static class VTPrsTbl
    {
        /// <summary>
        /// 当收到CASE_CSI_IGNORE指令的时候，解析器使用的状态表
        /// </summary>
        public static readonly byte[] CIgTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_ENQ,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTParseDef.CASE_BS,
            VTParseDef.CASE_TAB,
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_CR,
            VTParseDef.CASE_SO,
            VTParseDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	$		%		&		'	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	(		)		*		+	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	,		-		.		/	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	0		1		2		3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	4		5		6		7	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	8		9		:		;	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	<		=		>		?	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	@		A		B		C	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTParseDef.CASE_IND,
            VTParseDef.CASE_NEL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTParseDef.CASE_HTS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_RI,
            VTParseDef.CASE_SS2,
            VTParseDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTParseDef.CASE_DCS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SPA,
            VTParseDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTParseDef.CASE_SOS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECID,
            VTParseDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTParseDef.CASE_ST,
            VTParseDef.CASE_OSC,
            VTParseDef.CASE_PM,
            VTParseDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,

        };

        /// <summary>
        /// ascii字符和CSI的FinalByte字符的映射表
        /// </summary>
        public static readonly byte[] CsiSpTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_ENQ,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTParseDef.CASE_BS,
            VTParseDef.CASE_TAB,
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_CR,
            VTParseDef.CASE_SO,
            VTParseDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	(		)		*		+	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	4		5		6		7	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	8		9		:		;	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	<		=		>		?	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	@		A		B		C	*/
            VTParseDef.CASE_SL,
            VTParseDef.CASE_SR,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	D		E		F		G	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	H		I		J		K	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	L		M		N		O	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECSCUSR,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            VTParseDef.CASE_DECSWBV,
            VTParseDef.CASE_DECSMBV,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTParseDef.CASE_IND,
            VTParseDef.CASE_NEL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTParseDef.CASE_HTS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_RI,
            VTParseDef.CASE_SS2,
            VTParseDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTParseDef.CASE_DCS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SPA,
            VTParseDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTParseDef.CASE_SOS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECID,
            VTParseDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTParseDef.CASE_ST,
            VTParseDef.CASE_OSC,
            VTParseDef.CASE_PM,
            VTParseDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTParseDef.CASE_SL,
            VTParseDef.CASE_SR,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECSCUSR,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTParseDef.CASE_DECSWBV,
            VTParseDef.CASE_DECSMBV,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,

        };

        public static readonly byte[] Csi2Table =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_ENQ,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTParseDef.CASE_BS,
            VTParseDef.CASE_TAB,
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_CR,
            VTParseDef.CASE_SO,
            VTParseDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTParseDef.CASE_CSI_SPACE_STATE,
            VTParseDef.CASE_CSI_EX_STATE,
            VTParseDef.CASE_CSI_QUOTE_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            VTParseDef.CASE_CSI_DOLLAR_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_TICK_STATE,
            /*	(		)		*		+	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_STAR_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*	4		5		6		7	*/
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*	8		9		:		;	*/
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_COLON,
            VTParseDef.CASE_ESC_SEMI,
            /*	<		=		>		?	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	@		A		B		C	*/
            VTParseDef.CASE_ICH,
            VTParseDef.CASE_CUU,
            VTParseDef.CASE_CUD,
            VTParseDef.CASE_CUF,
            /*	D		E		F		G	*/
            VTParseDef.CASE_CUB,
            VTParseDef.CASE_CNL,
            VTParseDef.CASE_CPL,
            VTParseDef.CASE_HPA,
            /*	H		I		J		K	*/
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_CHT,
            VTParseDef.CASE_ED,
            VTParseDef.CASE_EL,
            /*	L		M		N		O	*/
            VTParseDef.CASE_IL,
            VTParseDef.CASE_DL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTParseDef.CASE_DCH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SU,
            /*	T		U		V		W	*/
            VTParseDef.CASE_TRACK_MOUSE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTParseDef.CASE_ECH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_CBT,
            VTParseDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTParseDef.CASE_HPA,
            VTParseDef.CASE_HPR,
            VTParseDef.CASE_REP,
            VTParseDef.CASE_DA1,
            /*	d		e		f		g	*/
            VTParseDef.CASE_VPA,
            VTParseDef.CASE_VPR,
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_TBC,
            /*	h		i		j		k	*/
            VTParseDef.CASE_SET,
            VTParseDef.CASE_MC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTParseDef.CASE_RST,
            VTParseDef.CASE_SGR,
            VTParseDef.CASE_CPR,
            VTParseDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECLL,
            VTParseDef.CASE_DECSTBM,
            VTParseDef.CASE_ANSI_SC,
            /*	t		u		v		w	*/
            VTParseDef.CASE_XTERM_WINOPS,
            VTParseDef.CASE_ANSI_RC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTParseDef.CASE_DECREQTPARM,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTParseDef.CASE_IND,
            VTParseDef.CASE_NEL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTParseDef.CASE_HTS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_RI,
            VTParseDef.CASE_SS2,
            VTParseDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTParseDef.CASE_DCS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SPA,
            VTParseDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTParseDef.CASE_SOS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECID,
            VTParseDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTParseDef.CASE_ST,
            VTParseDef.CASE_OSC,
            VTParseDef.CASE_PM,
            VTParseDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTParseDef.CASE_CSI_SPACE_STATE,
            VTParseDef.CASE_CSI_EX_STATE,
            VTParseDef.CASE_CSI_QUOTE_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTParseDef.CASE_CSI_DOLLAR_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_TICK_STATE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_STAR_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*      acute           mu              paragraph       periodcentered  */
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_COLON,
            VTParseDef.CASE_ESC_SEMI,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTParseDef.CASE_ICH,
            VTParseDef.CASE_CUU,
            VTParseDef.CASE_CUD,
            VTParseDef.CASE_CUF,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTParseDef.CASE_CUB,
            VTParseDef.CASE_CNL,
            VTParseDef.CASE_CPL,
            VTParseDef.CASE_HPA,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_CHT,
            VTParseDef.CASE_ED,
            VTParseDef.CASE_EL,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTParseDef.CASE_IL,
            VTParseDef.CASE_DL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTParseDef.CASE_DCH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SU,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTParseDef.CASE_TRACK_MOUSE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTParseDef.CASE_ECH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_CBT,
            VTParseDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTParseDef.CASE_HPA,
            VTParseDef.CASE_HPR,
            VTParseDef.CASE_REP,
            VTParseDef.CASE_DA1,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTParseDef.CASE_VPA,
            VTParseDef.CASE_VPR,
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_TBC,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTParseDef.CASE_SET,
            VTParseDef.CASE_MC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTParseDef.CASE_RST,
            VTParseDef.CASE_SGR,
            VTParseDef.CASE_CPR,
            VTParseDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECLL,
            VTParseDef.CASE_DECSTBM,
            VTParseDef.CASE_ANSI_SC,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTParseDef.CASE_XTERM_WINOPS,
            VTParseDef.CASE_ANSI_RC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTParseDef.CASE_DECREQTPARM,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,

        };

        /// <summary>
        /// ascii码与csi控制指令的映射关系表
        /// </summary>
        public static readonly byte[] CsiTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_ENQ,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTParseDef.CASE_BS,
            VTParseDef.CASE_TAB,
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_CR,
            VTParseDef.CASE_SO,
            VTParseDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTParseDef.CASE_CSI_SPACE_STATE,
            VTParseDef.CASE_CSI_EX_STATE,
            VTParseDef.CASE_CSI_QUOTE_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	$		%		&		'	*/
            VTParseDef.CASE_CSI_DOLLAR_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_TICK_STATE,
            /*	(		)		*		+	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	,		-		.		/	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*	0		1		2		3	*/
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*	4		5		6		7	*/
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*	8		9		:		;	*/
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_COLON,
            VTParseDef.CASE_ESC_SEMI,
            /*	<		=		>		?	*/
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_DEC3_STATE,
            VTParseDef.CASE_DEC2_STATE,
            VTParseDef.CASE_DEC_STATE,
            /*	@		A		B		C	*/
            VTParseDef.CASE_ICH,
            VTParseDef.CASE_CUU,
            VTParseDef.CASE_CUD,
            VTParseDef.CASE_CUF,
            /*	D		E		F		G	*/
            VTParseDef.CASE_CUB,
            VTParseDef.CASE_CNL,
            VTParseDef.CASE_CPL,
            VTParseDef.CASE_HPA,
            /*	H		I		J		K	*/
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_CHT,
            VTParseDef.CASE_ED,
            VTParseDef.CASE_EL,
            /*	L		M		N		O	*/
            VTParseDef.CASE_IL,
            VTParseDef.CASE_DL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTParseDef.CASE_DCH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SU,
            /*	T		U		V		W	*/
            VTParseDef.CASE_TRACK_MOUSE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTParseDef.CASE_ECH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_CBT,
            VTParseDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTParseDef.CASE_HPA,
            VTParseDef.CASE_HPR,
            VTParseDef.CASE_REP,
            VTParseDef.CASE_DA1,
            /*	d		e		f		g	*/
            VTParseDef.CASE_VPA,
            VTParseDef.CASE_VPR,
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_TBC,
            /*	h		i		j		k	*/
            VTParseDef.CASE_SET,
            VTParseDef.CASE_MC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTParseDef.CASE_RST,
            VTParseDef.CASE_SGR,
            VTParseDef.CASE_CPR,
            VTParseDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECLL,
            VTParseDef.CASE_DECSTBM,
            VTParseDef.CASE_ANSI_SC,
            /*	t		u		v		w	*/
            VTParseDef.CASE_XTERM_WINOPS,
            VTParseDef.CASE_ANSI_RC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTParseDef.CASE_DECREQTPARM,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTParseDef.CASE_IND,
            VTParseDef.CASE_NEL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTParseDef.CASE_HTS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_RI,
            VTParseDef.CASE_SS2,
            VTParseDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTParseDef.CASE_DCS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SPA,
            VTParseDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTParseDef.CASE_SOS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECID,
            VTParseDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTParseDef.CASE_ST,
            VTParseDef.CASE_OSC,
            VTParseDef.CASE_PM,
            VTParseDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTParseDef.CASE_CSI_SPACE_STATE,
            VTParseDef.CASE_CSI_EX_STATE,
            VTParseDef.CASE_CSI_QUOTE_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTParseDef.CASE_CSI_DOLLAR_STATE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_TICK_STATE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_CSI_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*      acute           mu              paragraph       periodcentered  */
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_DIGIT,
            VTParseDef.CASE_ESC_COLON,
            VTParseDef.CASE_ESC_SEMI,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTParseDef.CASE_CSI_IGNORE,
            VTParseDef.CASE_DEC3_STATE,
            VTParseDef.CASE_DEC2_STATE,
            VTParseDef.CASE_DEC_STATE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTParseDef.CASE_ICH,
            VTParseDef.CASE_CUU,
            VTParseDef.CASE_CUD,
            VTParseDef.CASE_CUF,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTParseDef.CASE_CUB,
            VTParseDef.CASE_CNL,
            VTParseDef.CASE_CPL,
            VTParseDef.CASE_HPA,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_CHT,
            VTParseDef.CASE_ED,
            VTParseDef.CASE_EL,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTParseDef.CASE_IL,
            VTParseDef.CASE_DL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTParseDef.CASE_DCH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SU,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTParseDef.CASE_TRACK_MOUSE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTParseDef.CASE_ECH,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_CBT,
            VTParseDef.CASE_GROUND_STATE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTParseDef.CASE_HPA,
            VTParseDef.CASE_HPR,
            VTParseDef.CASE_REP,
            VTParseDef.CASE_DA1,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTParseDef.CASE_VPA,
            VTParseDef.CASE_VPR,
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_TBC,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTParseDef.CASE_SET,
            VTParseDef.CASE_MC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTParseDef.CASE_RST,
            VTParseDef.CASE_SGR,
            VTParseDef.CASE_CPR,
            VTParseDef.CASE_GROUND_STATE,
            /*      eth             ntilde          ograve          oacute          */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECLL,
            VTParseDef.CASE_DECSTBM,
            VTParseDef.CASE_ANSI_SC,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTParseDef.CASE_XTERM_WINOPS,
            VTParseDef.CASE_ANSI_RC,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTParseDef.CASE_DECREQTPARM,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
        };

        /// <summary>
        /// ascii码与Esc控制指令的映射关系表
        /// </summary>
        public static readonly byte[] EscTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_ENQ,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTParseDef.CASE_BS,
            VTParseDef.CASE_TAB,
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_CR,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DLE		DC1		DC2		DC3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            /*	$		%		&		'	*/
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            /*	(		)		*		+	*/
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            /*	,		-		.		/	*/
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            VTParseDef.CASE_VT52_IGNORE,
            /*	0		1		2		3	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	4		5		6		7	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	8		9		:		;	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	<		=		>		?	*/
            VTParseDef.CASE_VT52_FINISH,
            VTParseDef.CASE_DECKPAM,
            VTParseDef.CASE_DECKPNM,
            VTParseDef.CASE_GROUND_STATE,
            /*	@		A		B		C	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_CUU,
            VTParseDef.CASE_CUD,
            VTParseDef.CASE_CUF,
            /*	D		E		F		G	*/
            VTParseDef.CASE_CUB,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SO,
            VTParseDef.CASE_SI,
            /*	H		I		J		K	*/
            VTParseDef.CASE_CUP,
            VTParseDef.CASE_RI,
            VTParseDef.CASE_ED,
            VTParseDef.CASE_EL,
            /*	L		M		N		O	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	P		Q		R		S	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	T		U		V		W	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	X		Y		Z		[	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_VT52_CUP,
            VTParseDef.CASE_DECID,
            VTParseDef.CASE_GROUND_STATE,
            /*	\		]		^		_	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	`		a		b		c	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	d		e		f		g	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	h		i		j		k	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	l		m		n		o	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	p		q		r		s	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	t		u		v		w	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	x		y		z		{	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*	|		}		~		DEL	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x84            0x85            0x86            0x87    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x90            0x91            0x92            0x93    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x94            0x95            0x96            0x97    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x98            0x99            0x9a            0x9b    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      currency        yen             brokenbar       section         */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      notsign         hyphen          registered      macron          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      acute           mu              paragraph       periodcentered  */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      agrave          aacute          acircumflex     atilde          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      eth             ntilde          ograve          oacute          */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
        };

        /// <summary>
        /// Ansi字符与控制功能之间的映射关系表
        /// 数组的索引代表Ansi字符的值，索引的值代表Ansi字符的控制功能
        /// </summary>
        public static readonly byte[] AnsiTable =
        {
            /*	NUL		SOH		STX		ETX	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	EOT		ENQ		ACK		BEL	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_ENQ,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_BELL,
            /*	BS		HT		NL		VT	*/
            VTParseDef.CASE_BS,
            VTParseDef.CASE_TAB,
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_VMOT,
            /*	FF		CR		SO		SI	*/
            VTParseDef.CASE_VMOT,
            VTParseDef.CASE_CR,
            VTParseDef.CASE_SO,
            VTParseDef.CASE_SI,
            /*	DLE		DC1		DC2		DC3	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	DC4		NAK		SYN		ETB	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	CAN		EM		SUB		ESC	*/
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_ESC,
            /*	FS		GS		RS		US	*/
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            VTParseDef.CASE_IGNORE,
            /*	SP		!		"		#	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	$		%		&		'	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	(		)		*		+	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	,		-		.		/	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	0		1		2		3	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	4		5		6		7	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	8		9		:		;	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	<		=		>		?	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	@		A		B		C	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	D		E		F		G	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	H		I		J		K	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	L		M		N		O	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	P		Q		R		S	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	T		U		V		W	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	X		Y		Z		[	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	\		]		^		_	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	`		a		b		c	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	d		e		f		g	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	h		i		j		k	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	l		m		n		o	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	p		q		r		s	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	t		u		v		w	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	x		y		z		{	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*	|		}		~		DEL	*/
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_IGNORE,
            /*      0x80            0x81            0x82            0x83    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x84            0x85            0x86            0x87    */
            VTParseDef.CASE_IND,
            VTParseDef.CASE_NEL,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x88            0x89            0x8a            0x8b    */
            VTParseDef.CASE_HTS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x8c            0x8d            0x8e            0x8f    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_RI,
            VTParseDef.CASE_SS2,
            VTParseDef.CASE_SS3,
            /*      0x90            0x91            0x92            0x93    */
            VTParseDef.CASE_DCS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            /*      0x94            0x95            0x96            0x97    */
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_SPA,
            VTParseDef.CASE_EPA,
            /*      0x98            0x99            0x9a            0x9b    */
            VTParseDef.CASE_SOS,
            VTParseDef.CASE_GROUND_STATE,
            VTParseDef.CASE_DECID,
            VTParseDef.CASE_CSI_STATE,
            /*      0x9c            0x9d            0x9e            0x9f    */
            VTParseDef.CASE_ST,
            VTParseDef.CASE_OSC,
            VTParseDef.CASE_PM,
            VTParseDef.CASE_APC,
            /*      nobreakspace    exclamdown      cent            sterling        */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      currency        yen             brokenbar       section         */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      diaeresis       copyright       ordfeminine     guillemotleft   */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      notsign         hyphen          registered      macron          */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      degree          plusminus       twosuperior     threesuperior   */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      acute           mu              paragraph       periodcentered  */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      cedilla         onesuperior     masculine       guillemotright  */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      onequarter      onehalf         threequarters   questiondown    */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Agrave          Aacute          Acircumflex     Atilde          */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Adiaeresis      Aring           AE              Ccedilla        */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Egrave          Eacute          Ecircumflex     Ediaeresis      */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Igrave          Iacute          Icircumflex     Idiaeresis      */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Eth             Ntilde          Ograve          Oacute          */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Ocircumflex     Otilde          Odiaeresis      multiply        */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Ooblique        Ugrave          Uacute          Ucircumflex     */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      Udiaeresis      Yacute          Thorn           ssharp          */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      agrave          aacute          acircumflex     atilde          */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      adiaeresis      aring           ae              ccedilla        */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      egrave          eacute          ecircumflex     ediaeresis      */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      igrave          iacute          icircumflex     idiaeresis      */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      eth             ntilde          ograve          oacute          */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      ocircumflex     otilde          odiaeresis      division        */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      oslash          ugrave          uacute          ucircumflex     */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            /*      udiaeresis      yacute          thorn           ydiaeresis      */
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,
            VTParseDef.CASE_PRINT,

        };
    }
}