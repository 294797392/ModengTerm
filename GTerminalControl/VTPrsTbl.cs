using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalControl
{
    /*
     * 这个类里的内容摘抄自xterm-331 VTPsrTbl.c
     */
    public static class VTPrsTbl
    {
        /// <summary>
        /// Ansi字符与控制功能之间的映射关系表
        /// 数组的索引代表Ansi字符的值，索引的值代表Ansi字符的控制功能
        /// </summary>
        public static byte[] AnsiTable =
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