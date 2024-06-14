using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 存储字符映射信息
    /// </summary>
    public class VTCharsetMap
    {
        /// <summary>
        /// 映射前的字符 -> 映射后的字符
        /// </summary>
        private Dictionary<char, char> translationTable;

        //typedef CharSet<L'\x20', 95> AsciiBasedCharSet;
        //typedef CharSet<L'\xa0', 95> Latin1BasedCharSet94;
        //typedef CharSet<L'\xa0', 96> Latin1BasedCharSet96;


        public static readonly VTCharsetMap Ascii = new VTCharsetMap((char)0x20, 94);

        public static readonly VTCharsetMap Latin1 = new VTCharsetMap((char)0xA0, 94);

        public static readonly VTCharsetMap Latin2 = new VTCharsetMap((char)0xA0, 96, new Dictionary<char, char>()
        {
            {'\xa1',  '\u0104' }, //  atin Capita   etter A With Ogonek
            {'\xa2',  '\u02d8' }, // Breve
            {'\xa3',  '\u0141' }, //  atin Capita   etter   With Stroke
            {'\xa5',  '\u013d' }, //  atin Capita   etter   With Caron
            {'\xa6',  '\u015a' }, //  atin Capita   etter S With Acute
            {'\xa9',  '\u0160' }, //  atin Capita   etter S With Caron
            {'\xaa',  '\u015e' }, //  atin Capita   etter S With Cedi  a
            {'\xab',  '\u0164' }, //  atin Capita   etter T With Caron
            {'\xac',  '\u0179' }, //  atin Capita   etter Z With Acute
            {'\xae',  '\u017d' }, //  atin Capita   etter Z With Caron
            {'\xaf',  '\u017b' }, //  atin Capita   etter Z With Dot Above
            {'\xb1',  '\u0105' }, //  atin Sma    etter A With Ogonek
            {'\xb2',  '\u02db' }, // Ogonek
            {'\xb3',  '\u0142' }, //  atin Sma    etter   With Stroke
            {'\xb5',  '\u013e' }, //  atin Sma    etter   With Caron
            {'\xb6',  '\u015b' }, //  atin Sma    etter S With Acute
            {'\xb7',  '\u02c7' }, // Caron
            {'\xb9',  '\u0161' }, //  atin Sma    etter S With Caron
            {'\xba',  '\u015f' }, //  atin Sma    etter S With Cedi  a
            {'\xbb',  '\u0165' }, //  atin Sma    etter T With Caron
            {'\xbc',  '\u017a' }, //  atin Sma    etter Z With Acute
            {'\xbd',  '\u02dd' }, // Doub e Acute Accent
            {'\xbe',  '\u017e' }, //  atin Sma    etter Z With Caron
            {'\xbf',  '\u017c' }, //  atin Sma    etter Z With Dot Above
            {'\xc0',  '\u0154' }, //  atin Capita   etter R With Acute
            {'\xc3',  '\u0102' }, //  atin Capita   etter A With Breve
            {'\xc5',  '\u0139' }, //  atin Capita   etter   With Acute
            {'\xc6',  '\u0106' }, //  atin Capita   etter C With Acute
            {'\xc8',  '\u010c' }, //  atin Capita   etter C With Caron
            {'\xca',  '\u0118' }, //  atin Capita   etter E With Ogonek
            {'\xcc',  '\u011a' }, //  atin Capita   etter E With Caron
            {'\xcf',  '\u010e' }, //  atin Capita   etter D With Caron
            {'\xd0',  '\u0110' }, //  atin Capita   etter D With Stroke
            {'\xd1',  '\u0143' }, //  atin Capita   etter N With Acute
            {'\xd2',  '\u0147' }, //  atin Capita   etter N With Caron
            {'\xd5',  '\u0150' }, //  atin Capita   etter O With Doub e Acute
            {'\xd8',  '\u0158' }, //  atin Capita   etter R With Caron
            {'\xd9',  '\u016e' }, //  atin Capita   etter U With Ring Above
            {'\xdb',  '\u0170' }, //  atin Capita   etter U With Doub e Acute
            {'\xde',  '\u0162' }, //  atin Capita   etter T With Cedi  a
            {'\xe0',  '\u0155' }, //  atin Sma    etter R With Acute
            {'\xe3',  '\u0103' }, //  atin Sma    etter A With Breve
            {'\xe5',  '\u013a' }, //  atin Sma    etter   With Acute
            {'\xe6',  '\u0107' }, //  atin Sma    etter C With Acute
            {'\xe8',  '\u010d' }, //  atin Sma    etter C With Caron
            {'\xea',  '\u0119' }, //  atin Sma    etter E With Ogonek
            {'\xec',  '\u011b' }, //  atin Sma    etter E With Caron
            {'\xef',  '\u010f' }, //  atin Sma    etter D With Caron
            {'\xf0',  '\u0111' }, //  atin Sma    etter D With Stroke
            {'\xf1',  '\u0144' }, //  atin Sma    etter N With Acute
            {'\xf2',  '\u0148' }, //  atin Sma    etter N With Caron
            {'\xf5',  '\u0151' }, //  atin Sma    etter O With Doub e Acute
            {'\xf8',  '\u0159' }, //  atin Sma    etter R With Caron
            {'\xf9',  '\u016f' }, //  atin Sma    etter U With Ring Above
            {'\xfb',  '\u0171' }, //  atin Sma    etter U With Doub e Acute
            {'\xfe',  '\u0163' }, //  atin Sma    etter T With Cedi  a
            {'\xff',  '\u02d9' }, // Dot Above
        });

        public static readonly VTCharsetMap LatinCyrillic = new VTCharsetMap((char)0xA0, 96, new Dictionary<char, char>()
        {
            {  '\xa1',  '\u0401' }, // Cyri  ic Capita   etter Io
            {  '\xa2',  '\u0402' }, // Cyri  ic Capita   etter Dje
            {  '\xa3',  '\u0403' }, // Cyri  ic Capita   etter Gje
            {  '\xa4',  '\u0404' }, // Cyri  ic Capita   etter Ukrainian Ie
            {  '\xa5',  '\u0405' }, // Cyri  ic Capita   etter Dze
            {  '\xa6',  '\u0406' }, // Cyri  ic Capita   etter Bye orussian-Ukrainian I
            {  '\xa7',  '\u0407' }, // Cyri  ic Capita   etter Yi
            {  '\xa8',  '\u0408' }, // Cyri  ic Capita   etter Je
            {  '\xa9',  '\u0409' }, // Cyri  ic Capita   etter  je
            {  '\xaa',  '\u040a' }, // Cyri  ic Capita   etter Nje
            {  '\xab',  '\u040b' }, // Cyri  ic Capita   etter Tshe
            {  '\xac',  '\u040c' }, // Cyri  ic Capita   etter Kje
            {  '\xae',  '\u040e' }, // Cyri  ic Capita   etter Short U
            {  '\xaf',  '\u040f' }, // Cyri  ic Capita   etter Dzhe
            {  '\xb0',  '\u0410' }, // Cyri  ic Capita   etter A
            {  '\xb1',  '\u0411' }, // Cyri  ic Capita   etter Be
            {  '\xb2',  '\u0412' }, // Cyri  ic Capita   etter Ve
            {  '\xb3',  '\u0413' }, // Cyri  ic Capita   etter Ghe
            {  '\xb4',  '\u0414' }, // Cyri  ic Capita   etter De
            {  '\xb5',  '\u0415' }, // Cyri  ic Capita   etter Ie
            {  '\xb6',  '\u0416' }, // Cyri  ic Capita   etter Zhe
            {  '\xb7',  '\u0417' }, // Cyri  ic Capita   etter Ze
            {  '\xb8',  '\u0418' }, // Cyri  ic Capita   etter I
            {  '\xb9',  '\u0419' }, // Cyri  ic Capita   etter Short I
            {  '\xba',  '\u041a' }, // Cyri  ic Capita   etter Ka
            {  '\xbb',  '\u041b' }, // Cyri  ic Capita   etter E 
            {  '\xbc',  '\u041c' }, // Cyri  ic Capita   etter Em
            {  '\xbd',  '\u041d' }, // Cyri  ic Capita   etter En
            {  '\xbe',  '\u041e' }, // Cyri  ic Capita   etter O
            {  '\xbf',  '\u041f' }, // Cyri  ic Capita   etter Pe
            {  '\xc0',  '\u0420' }, // Cyri  ic Capita   etter Er
            {  '\xc1',  '\u0421' }, // Cyri  ic Capita   etter Es
            {  '\xc2',  '\u0422' }, // Cyri  ic Capita   etter Te
            {  '\xc3',  '\u0423' }, // Cyri  ic Capita   etter U
            {  '\xc4',  '\u0424' }, // Cyri  ic Capita   etter Ef
            {  '\xc5',  '\u0425' }, // Cyri  ic Capita   etter Ha
            {  '\xc6',  '\u0426' }, // Cyri  ic Capita   etter Tse
            {  '\xc7',  '\u0427' }, // Cyri  ic Capita   etter Che
            {  '\xc8',  '\u0428' }, // Cyri  ic Capita   etter Sha
            {  '\xc9',  '\u0429' }, // Cyri  ic Capita   etter Shcha
            {  '\xca',  '\u042a' }, // Cyri  ic Capita   etter Hard Sign
            {  '\xcb',  '\u042b' }, // Cyri  ic Capita   etter Yeru
            {  '\xcc',  '\u042c' }, // Cyri  ic Capita   etter Soft Sign
            {  '\xcd',  '\u042d' }, // Cyri  ic Capita   etter E
            {  '\xce',  '\u042e' }, // Cyri  ic Capita   etter Yu
            {  '\xcf',  '\u042f' }, // Cyri  ic Capita   etter Ya
            {  '\xd0',  '\u0430' }, // Cyri  ic Sma    etter A
            {  '\xd1',  '\u0431' }, // Cyri  ic Sma    etter Be
            {  '\xd2',  '\u0432' }, // Cyri  ic Sma    etter Ve
            {  '\xd3',  '\u0433' }, // Cyri  ic Sma    etter Ghe
            {  '\xd4',  '\u0434' }, // Cyri  ic Sma    etter De
            {  '\xd5',  '\u0435' }, // Cyri  ic Sma    etter Ie
            {  '\xd6',  '\u0436' }, // Cyri  ic Sma    etter Zhe
            {  '\xd7',  '\u0437' }, // Cyri  ic Sma    etter Ze
            {  '\xd8',  '\u0438' }, // Cyri  ic Sma    etter I
            {  '\xd9',  '\u0439' }, // Cyri  ic Sma    etter Short I
            {  '\xda',  '\u043a' }, // Cyri  ic Sma    etter Ka
            {  '\xdb',  '\u043b' }, // Cyri  ic Sma    etter E 
            {  '\xdc',  '\u043c' }, // Cyri  ic Sma    etter Em
            {  '\xdd',  '\u043d' }, // Cyri  ic Sma    etter En
            {  '\xde',  '\u043e' }, // Cyri  ic Sma    etter O
            {  '\xdf',  '\u043f' }, // Cyri  ic Sma    etter Pe
            {  '\xe0',  '\u0440' }, // Cyri  ic Sma    etter Er
            {  '\xe1',  '\u0441' }, // Cyri  ic Sma    etter Es
            {  '\xe2',  '\u0442' }, // Cyri  ic Sma    etter Te
            {  '\xe3',  '\u0443' }, // Cyri  ic Sma    etter U
            {  '\xe4',  '\u0444' }, // Cyri  ic Sma    etter Ef
            {  '\xe5',  '\u0445' }, // Cyri  ic Sma    etter Ha
            {  '\xe6',  '\u0446' }, // Cyri  ic Sma    etter Tse
            {  '\xe7',  '\u0447' }, // Cyri  ic Sma    etter Che
            {  '\xe8',  '\u0448' }, // Cyri  ic Sma    etter Sha
            {  '\xe9',  '\u0449' }, // Cyri  ic Sma    etter Shcha
            {  '\xea',  '\u044a' }, // Cyri  ic Sma    etter Hard Sign
            {  '\xeb',  '\u044b' }, // Cyri  ic Sma    etter Yeru
            {  '\xec',  '\u044c' }, // Cyri  ic Sma    etter Soft Sign
            {  '\xed',  '\u044d' }, // Cyri  ic Sma    etter E
            {  '\xee',  '\u044e' }, // Cyri  ic Sma    etter Yu
            {  '\xef',  '\u044f' }, // Cyri  ic Sma    etter Ya
            {  '\xf0',  '\u2116' }, // Numero Sign
            {  '\xf1',  '\u0451' }, // Cyri  ic Sma    etter Io
            {  '\xf2',  '\u0452' }, // Cyri  ic Sma    etter Dje
            {  '\xf3',  '\u0453' }, // Cyri  ic Sma    etter Gje
            {  '\xf4',  '\u0454' }, // Cyri  ic Sma    etter Ukrainian Ie
            {  '\xf5',  '\u0455' }, // Cyri  ic Sma    etter Dze
            {  '\xf6',  '\u0456' }, // Cyri  ic Sma    etter Bye orussian-Ukrainian I
            {  '\xf7',  '\u0457' }, // Cyri  ic Sma    etter Yi
            {  '\xf8',  '\u0458' }, // Cyri  ic Sma    etter Je
            {  '\xf9',  '\u0459' }, // Cyri  ic Sma    etter  je
            {  '\xfa',  '\u045a' }, // Cyri  ic Sma    etter Nje
            {  '\xfb',  '\u045b' }, // Cyri  ic Sma    etter Tshe
            {  '\xfc',  '\u045c' }, // Cyri  ic Sma    etter Kje
            {  '\xfd',  '\u00a7' }, // Section Sign
            {  '\xfe',  '\u045e' }, // Cyri  ic Sma    etter Short U
            {  '\xff',  '\u045f' }, // Cyri  ic Sma    etter Dzhe
        });

        public static readonly VTCharsetMap LatinGreek = new VTCharsetMap((char)0xA0, 96, new Dictionary<char, char>()
        {
            {  '\xa1',  '\u2018' }, //  eft Sing e Quotation Mark
            {  '\xa2',  '\u2019' }, // Right Sing e Quotation Mark
            {  '\xa4',  '\u2426' }, // Undefined
            {  '\xa5',  '\u2426' }, // Undefined
            {  '\xaa',  '\u2426' }, // Undefined
            {  '\xae',  '\u2426' }, // Undefined
            {  '\xaf',  '\u2015' }, // Horizonta  Bar
            {  '\xb4',  '\u0384' }, // Greek Tonos
            {  '\xb5',  '\u0385' }, // Greek Dia ytika Tonos
            {  '\xb6',  '\u0386' }, // Greek Capita   etter A pha With Tonos
            {  '\xb8',  '\u0388' }, // Greek Capita   etter Epsi on With Tonos
            {  '\xb9',  '\u0389' }, // Greek Capita   etter Eta With Tonos
            {  '\xba',  '\u038a' }, // Greek Capita   etter Iota With Tonos
            {  '\xbc',  '\u038c' }, // Greek Capita   etter Omicron With Tonos
            {  '\xbe',  '\u038e' }, // Greek Capita   etter Upsi on With Tonos
            {  '\xbf',  '\u038f' }, // Greek Capita   etter Omega With Tonos
            {  '\xc0',  '\u0390' }, // Greek Sma    etter Iota With Dia ytika And Tonos
            {  '\xc1',  '\u0391' }, // Greek Capita   etter A pha
            {  '\xc2',  '\u0392' }, // Greek Capita   etter Beta
            {  '\xc3',  '\u0393' }, // Greek Capita   etter Gamma
            {  '\xc4',  '\u0394' }, // Greek Capita   etter De ta
            {  '\xc5',  '\u0395' }, // Greek Capita   etter Epsi on
            {  '\xc6',  '\u0396' }, // Greek Capita   etter Zeta
            {  '\xc7',  '\u0397' }, // Greek Capita   etter Eta
            {  '\xc8',  '\u0398' }, // Greek Capita   etter Theta
            {  '\xc9',  '\u0399' }, // Greek Capita   etter Iota
            {  '\xca',  '\u039a' }, // Greek Capita   etter Kappa
            {  '\xcb',  '\u039b' }, // Greek Capita   etter  amda
            {  '\xcc',  '\u039c' }, // Greek Capita   etter Mu
            {  '\xcd',  '\u039d' }, // Greek Capita   etter Nu
            {  '\xce',  '\u039e' }, // Greek Capita   etter Xi
            {  '\xcf',  '\u039f' }, // Greek Capita   etter Omicron
            {  '\xd0',  '\u03a0' }, // Greek Capita   etter Pi
            {  '\xd1',  '\u03a1' }, // Greek Capita   etter Rho
            {  '\xd2',  '\u2426' }, // Undefined
            {  '\xd3',  '\u03a3' }, // Greek Capita   etter Sigma
            {  '\xd4',  '\u03a4' }, // Greek Capita   etter Tau
            {  '\xd5',  '\u03a5' }, // Greek Capita   etter Upsi on
            {  '\xd6',  '\u03a6' }, // Greek Capita   etter Phi
            {  '\xd7',  '\u03a7' }, // Greek Capita   etter Chi
            {  '\xd8',  '\u03a8' }, // Greek Capita   etter Psi
            {  '\xd9',  '\u03a9' }, // Greek Capita   etter Omega
            {  '\xda',  '\u03aa' }, // Greek Capita   etter Iota With Dia ytika
            {  '\xdb',  '\u03ab' }, // Greek Capita   etter Upsi on With Dia ytika
            {  '\xdc',  '\u03ac' }, // Greek Sma    etter A pha With Tonos
            {  '\xdd',  '\u03ad' }, // Greek Sma    etter Epsi on With Tonos
            {  '\xde',  '\u03ae' }, // Greek Sma    etter Eta With Tonos
            {  '\xdf',  '\u03af' }, // Greek Sma    etter Iota With Tonos
            {  '\xe0',  '\u03b0' }, // Greek Sma    etter Upsi on With Dia ytika And Tonos
            {  '\xe1',  '\u03b1' }, // Greek Sma    etter A pha
            {  '\xe2',  '\u03b2' }, // Greek Sma    etter Beta
            {  '\xe3',  '\u03b3' }, // Greek Sma    etter Gamma
            {  '\xe4',  '\u03b4' }, // Greek Sma    etter De ta
            {  '\xe5',  '\u03b5' }, // Greek Sma    etter Epsi on
            {  '\xe6',  '\u03b6' }, // Greek Sma    etter Zeta
            {  '\xe7',  '\u03b7' }, // Greek Sma    etter Eta
            {  '\xe8',  '\u03b8' }, // Greek Sma    etter Theta
            {  '\xe9',  '\u03b9' }, // Greek Sma    etter Iota
            {  '\xea',  '\u03ba' }, // Greek Sma    etter Kappa
            {  '\xeb',  '\u03bb' }, // Greek Sma    etter  amda
            {  '\xec',  '\u03bc' }, // Greek Sma    etter Mu
            {  '\xed',  '\u03bd' }, // Greek Sma    etter Nu
            {  '\xee',  '\u03be' }, // Greek Sma    etter Xi
            {  '\xef',  '\u03bf' }, // Greek Sma    etter Omicron
            {  '\xf0',  '\u03c0' }, // Greek Sma    etter Pi
            {  '\xf1',  '\u03c1' }, // Greek Sma    etter Rho
            {  '\xf2',  '\u03c2' }, // Greek Sma    etter Fina  Sigma
            {  '\xf3',  '\u03c3' }, // Greek Sma    etter Sigma
            {  '\xf4',  '\u03c4' }, // Greek Sma    etter Tau
            {  '\xf5',  '\u03c5' }, // Greek Sma    etter Upsi on
            {  '\xf6',  '\u03c6' }, // Greek Sma    etter Phi
            {  '\xf7',  '\u03c7' }, // Greek Sma    etter Chi
            {  '\xf8',  '\u03c8' }, // Greek Sma    etter Psi
            {  '\xf9',  '\u03c9' }, // Greek Sma    etter Omega
            {  '\xfa',  '\u03ca' }, // Greek Sma    etter Iota With Dia ytika
            {  '\xfb',  '\u03cb' }, // Greek Sma    etter Upsi on With Dia ytika
            {  '\xfc',  '\u03cc' }, // Greek Sma    etter Omicron With Tonos
            {  '\xfd',  '\u03cd' }, // Greek Sma    etter Upsi on With Tonos
            {  '\xfe',  '\u03ce' }, // Greek Sma    etter Omega With Tonos
            {  '\xff',  '\u2426' }, // Undefined
        });

        public static readonly VTCharsetMap LatinHebrew = new VTCharsetMap((char)0xA0, 96, new Dictionary<char, char>()
        {
            {  '\xa1',  '\u2426' }, // Undefined
            {  '\xaa',  '\u00d7' }, // Mu tip ication Sign
            {  '\xba',  '\u00f7' }, // Division Sign
            {  '\xbf',  '\u2426' }, // Undefined
            {  '\xc0',  '\u2426' }, // Undefined
            {  '\xc1',  '\u2426' }, // Undefined
            {  '\xc2',  '\u2426' }, // Undefined
            {  '\xc3',  '\u2426' }, // Undefined
            {  '\xc4',  '\u2426' }, // Undefined
            {  '\xc5',  '\u2426' }, // Undefined
            {  '\xc6',  '\u2426' }, // Undefined
            {  '\xc7',  '\u2426' }, // Undefined
            {  '\xc8',  '\u2426' }, // Undefined
            {  '\xc9',  '\u2426' }, // Undefined
            {  '\xca',  '\u2426' }, // Undefined
            {  '\xcb',  '\u2426' }, // Undefined
            {  '\xcc',  '\u2426' }, // Undefined
            {  '\xcd',  '\u2426' }, // Undefined
            {  '\xce',  '\u2426' }, // Undefined
            {  '\xcf',  '\u2426' }, // Undefined
            {  '\xd0',  '\u2426' }, // Undefined
            {  '\xd1',  '\u2426' }, // Undefined
            {  '\xd2',  '\u2426' }, // Undefined
            {  '\xd3',  '\u2426' }, // Undefined
            {  '\xd4',  '\u2426' }, // Undefined
            {  '\xd5',  '\u2426' }, // Undefined
            {  '\xd6',  '\u2426' }, // Undefined
            {  '\xd7',  '\u2426' }, // Undefined
            {  '\xd8',  '\u2426' }, // Undefined
            {  '\xd9',  '\u2426' }, // Undefined
            {  '\xda',  '\u2426' }, // Undefined
            {  '\xdb',  '\u2426' }, // Undefined
            {  '\xdc',  '\u2426' }, // Undefined
            {  '\xdd',  '\u2426' }, // Undefined
            {  '\xde',  '\u2426' }, // Undefined
            {  '\xdf',  '\u2017' }, // Doub e  ow  ine
            {  '\xe0',  '\u05d0' }, // Hebrew  etter A ef
            {  '\xe1',  '\u05d1' }, // Hebrew  etter Bet
            {  '\xe2',  '\u05d2' }, // Hebrew  etter Gime 
            {  '\xe3',  '\u05d3' }, // Hebrew  etter Da et
            {  '\xe4',  '\u05d4' }, // Hebrew  etter He
            {  '\xe5',  '\u05d5' }, // Hebrew  etter Vav
            {  '\xe6',  '\u05d6' }, // Hebrew  etter Zayin
            {  '\xe7',  '\u05d7' }, // Hebrew  etter Het
            {  '\xe8',  '\u05d8' }, // Hebrew  etter Tet
            {  '\xe9',  '\u05d9' }, // Hebrew  etter Yod
            {  '\xea',  '\u05da' }, // Hebrew  etter Fina  Kaf
            {  '\xeb',  '\u05db' }, // Hebrew  etter Kaf
            {  '\xec',  '\u05dc' }, // Hebrew  etter  amed
            {  '\xed',  '\u05dd' }, // Hebrew  etter Fina  Mem
            {  '\xee',  '\u05de' }, // Hebrew  etter Mem
            {  '\xef',  '\u05df' }, // Hebrew  etter Fina  Nun
            {  '\xf0',  '\u05e0' }, // Hebrew  etter Nun
            {  '\xf1',  '\u05e1' }, // Hebrew  etter Samekh
            {  '\xf2',  '\u05e2' }, // Hebrew  etter Ayin
            {  '\xf3',  '\u05e3' }, // Hebrew  etter Fina  Pe
            {  '\xf4',  '\u05e4' }, // Hebrew  etter Pe
            {  '\xf5',  '\u05e5' }, // Hebrew  etter Fina  Tsadi
            {  '\xf6',  '\u05e6' }, // Hebrew  etter Tsadi
            {  '\xf7',  '\u05e7' }, // Hebrew  etter Qof
            {  '\xf8',  '\u05e8' }, // Hebrew  etter Resh
            {  '\xf9',  '\u05e9' }, // Hebrew  etter Shin
            {  '\xfa',  '\u05ea' }, // Hebrew  etter Tav
            {  '\xfb',  '\u2426' }, // Undefined
            {  '\xfc',  '\u2426' }, // Undefined
            {  '\xfd',  '\u200e' }, //  eft-To-Right Mark
            {  '\xfe',  '\u200f' }, // Right-To- eft Mark
            {  '\xff',  '\u2426' }, // Undefined
        });

        public static readonly VTCharsetMap Latin5 = new VTCharsetMap((char)0xA0, 96, new Dictionary<char, char>()
        {
            {  '\xd0',  '\u011e' }, //  atin Capita   etter G With Breve
            {  '\xdd',  '\u0130' }, //  atin Capita   etter I With Dot Above
            {  '\xde',  '\u015e' }, //  atin Capita   etter S With Cedi  a
            {  '\xf0',  '\u011f' }, //  atin Sma    etter G With Breve
            {  '\xfd',  '\u0131' }, //  atin Sma    etter Dot ess I
            {  '\xfe',  '\u015f' }, //  atin Sma    etter S With Cedi  a
        });

        public static readonly VTCharsetMap DecSupplemental = new VTCharsetMap((char)0xA0, 94, new Dictionary<char, char>()
        {
            {  '\xa4',  '\u2426' }, // Undefined
            {  '\xa6',  '\u2426' }, // Undefined
            {  '\xa8',  '\u00a4' }, // Currency Sign
            {  '\xac',  '\u2426' }, // Undefined
            {  '\xad',  '\u2426' }, // Undefined
            {  '\xae',  '\u2426' }, // Undefined
            {  '\xaf',  '\u2426' }, // Undefined
            {  '\xb4',  '\u2426' }, // Undefined
            {  '\xb8',  '\u2426' }, // Undefined
            {  '\xbe',  '\u2426' }, // Undefined
            {  '\xd0',  '\u2426' }, // Undefined
            {  '\xd7',  '\u0152' }, //  atin Capita   igature Oe
            {  '\xdd',  '\u0178' }, //  atin Capita   etter Y With Diaeresis
            {  '\xde',  '\u2426' }, // Undefined
            {  '\xf0',  '\u2426' }, // Undefined
            {  '\xf7',  '\u0153' }, //  atin Sma    igature Oe
            {  '\xfd',  '\u00ff' }, //  atin Sma    etter Y With Diaeresis
            {  '\xfe',  '\u2426' }, // Undefined
        });

        public static readonly VTCharsetMap DecSpecialGraphics = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>()
        {
            {  '\x5f',  '\u0020' }, // B ank
            {  '\x60',  '\u2666' }, // Diamond (more common y U+25C6, but U+2666 renders better for us)
            {  '\x61',  '\u2592' }, // Checkerboard
            {  '\x62',  '\u2409' }, // HT, SYMBO  FOR HORIZONTA  TABU ATION
            {  '\x63',  '\u240c' }, // FF, SYMBO  FOR FORM FEED
            {  '\x64',  '\u240d' }, // CR, SYMBO  FOR CARRIAGE RETURN
            {  '\x65',  '\u240a' }, //  F, SYMBO  FOR  INE FEED
            {  '\x66',  '\u00b0' }, // Degree symbo 
            {  '\x67',  '\u00b1' }, // P us/minus
            {  '\x68',  '\u2424' }, // N , SYMBO  FOR NEW INE
            {  '\x69',  '\u240b' }, // VT, SYMBO  FOR VERTICA  TABU ATION
            {  '\x6a',  '\u2518' }, //  ower-right corner
            {  '\x6b',  '\u2510' }, // Upper-right corner
            {  '\x6c',  '\u250c' }, // Upper- eft corner
            {  '\x6d',  '\u2514' }, //  ower- eft corner
            {  '\x6e',  '\u253c' }, // Crossing  ines
            {  '\x6f',  '\u23ba' }, // Horizonta   ine - Scan 1
            {  '\x70',  '\u23bb' }, // Horizonta   ine - Scan 3
            {  '\x71',  '\u2500' }, // Horizonta   ine - Scan 5
            {  '\x72',  '\u23bc' }, // Horizonta   ine - Scan 7
            {  '\x73',  '\u23bd' }, // Horizonta   ine - Scan 9
            {  '\x74',  '\u251c' }, //  eft "T"
            {  '\x75',  '\u2524' }, // Right "T"
            {  '\x76',  '\u2534' }, // Bottom "T"
            {  '\x77',  '\u252c' }, // Top "T"
            {  '\x78',  '\u2502' }, // | Vertica  bar
            {  '\x79',  '\u2264' }, //  ess than or equa  to
            {  '\x7a',  '\u2265' }, // Greater than or equa  to
            {  '\x7b',  '\u03c0' }, // Pi
            {  '\x7c',  '\u2260' }, // Not equa  to
            {  '\x7d',  '\u00a3' }, // UK pound sign
            {  '\x7e',  '\u00b7' }, // Centered dot
        });

        public static readonly VTCharsetMap DecCyrillic = new VTCharsetMap( (char)0xA0, 94, new Dictionary<char, char>()
        {
            {  '\xa1',  '\u2426' }, // Undefined
            {  '\xa2',  '\u2426' }, // Undefined
            {  '\xa3',  '\u2426' }, // Undefined
            {  '\xa4',  '\u2426' }, // Undefined
            {  '\xa5',  '\u2426' }, // Undefined
            {  '\xa6',  '\u2426' }, // Undefined
            {  '\xa7',  '\u2426' }, // Undefined
            {  '\xa8',  '\u2426' }, // Undefined
            {  '\xa9',  '\u2426' }, // Undefined
            {  '\xaa',  '\u2426' }, // Undefined
            {  '\xab',  '\u2426' }, // Undefined
            {  '\xac',  '\u2426' }, // Undefined
            {  '\xad',  '\u2426' }, // Undefined
            {  '\xae',  '\u2426' }, // Undefined
            {  '\xaf',  '\u2426' }, // Undefined
            {  '\xb0',  '\u2426' }, // Undefined
            {  '\xb1',  '\u2426' }, // Undefined
            {  '\xb2',  '\u2426' }, // Undefined
            {  '\xb3',  '\u2426' }, // Undefined
            {  '\xb4',  '\u2426' }, // Undefined
            {  '\xb5',  '\u2426' }, // Undefined
            {  '\xb6',  '\u2426' }, // Undefined
            {  '\xb7',  '\u2426' }, // Undefined
            {  '\xb8',  '\u2426' }, // Undefined
            {  '\xb9',  '\u2426' }, // Undefined
            {  '\xba',  '\u2426' }, // Undefined
            {  '\xbb',  '\u2426' }, // Undefined
            {  '\xbc',  '\u2426' }, // Undefined
            {  '\xbd',  '\u2426' }, // Undefined
            {  '\xbe',  '\u2426' }, // Undefined
            {  '\xbf',  '\u2426' }, // Undefined
            {  '\xc0',  '\u044e' }, // Cyri  ic Sma    etter Yu
            {  '\xc1',  '\u0430' }, // Cyri  ic Sma    etter A
            {  '\xc2',  '\u0431' }, // Cyri  ic Sma    etter Be
            {  '\xc3',  '\u0446' }, // Cyri  ic Sma    etter Tse
            {  '\xc4',  '\u0434' }, // Cyri  ic Sma    etter De
            {  '\xc5',  '\u0435' }, // Cyri  ic Sma    etter Ie
            {  '\xc6',  '\u0444' }, // Cyri  ic Sma    etter Ef
            {  '\xc7',  '\u0433' }, // Cyri  ic Sma    etter Ghe
            {  '\xc8',  '\u0445' }, // Cyri  ic Sma    etter Ha
            {  '\xc9',  '\u0438' }, // Cyri  ic Sma    etter I
            {  '\xca',  '\u0439' }, // Cyri  ic Sma    etter Short I
            {  '\xcb',  '\u043a' }, // Cyri  ic Sma    etter Ka
            {  '\xcc',  '\u043b' }, // Cyri  ic Sma    etter E 
            {  '\xcd',  '\u043c' }, // Cyri  ic Sma    etter Em
            {  '\xce',  '\u043d' }, // Cyri  ic Sma    etter En
            {  '\xcf',  '\u043e' }, // Cyri  ic Sma    etter O
            {  '\xd0',  '\u043f' }, // Cyri  ic Sma    etter Pe
            {  '\xd1',  '\u044f' }, // Cyri  ic Sma    etter Ya
            {  '\xd2',  '\u0440' }, // Cyri  ic Sma    etter Er
            {  '\xd3',  '\u0441' }, // Cyri  ic Sma    etter Es
            {  '\xd4',  '\u0442' }, // Cyri  ic Sma    etter Te
            {  '\xd5',  '\u0443' }, // Cyri  ic Sma    etter U
            {  '\xd6',  '\u0436' }, // Cyri  ic Sma    etter Zhe
            {  '\xd7',  '\u0432' }, // Cyri  ic Sma    etter Ve
            {  '\xd8',  '\u044c' }, // Cyri  ic Sma    etter Soft Sign
            {  '\xd9',  '\u044b' }, // Cyri  ic Sma    etter Yeru
            {  '\xda',  '\u0437' }, // Cyri  ic Sma    etter Ze
            {  '\xdb',  '\u0448' }, // Cyri  ic Sma    etter Sha
            {  '\xdc',  '\u044d' }, // Cyri  ic Sma    etter E
            {  '\xdd',  '\u0449' }, // Cyri  ic Sma    etter Shcha
            {  '\xde',  '\u0447' }, // Cyri  ic Sma    etter Che
            {  '\xdf',  '\u044a' }, // Cyri  ic Sma    etter Hard Sign
            {  '\xe0',  '\u042e' }, // Cyri  ic Capita   etter Yu
            {  '\xe1',  '\u0410' }, // Cyri  ic Capita   etter A
            {  '\xe2',  '\u0411' }, // Cyri  ic Capita   etter Be
            {  '\xe3',  '\u0426' }, // Cyri  ic Capita   etter Tse
            {  '\xe4',  '\u0414' }, // Cyri  ic Capita   etter De
            {  '\xe5',  '\u0415' }, // Cyri  ic Capita   etter Ie
            {  '\xe6',  '\u0424' }, // Cyri  ic Capita   etter Ef
            {  '\xe7',  '\u0413' }, // Cyri  ic Capita   etter Ghe
            {  '\xe8',  '\u0425' }, // Cyri  ic Capita   etter Ha
            {  '\xe9',  '\u0418' }, // Cyri  ic Capita   etter I
            {  '\xea',  '\u0419' }, // Cyri  ic Capita   etter Short I
            {  '\xeb',  '\u041a' }, // Cyri  ic Capita   etter Ka
            {  '\xec',  '\u041b' }, // Cyri  ic Capita   etter E 
            {  '\xed',  '\u041c' }, // Cyri  ic Capita   etter Em
            {  '\xee',  '\u041d' }, // Cyri  ic Capita   etter En
            {  '\xef',  '\u041e' }, // Cyri  ic Capita   etter O
            {  '\xf0',  '\u041f' }, // Cyri  ic Capita   etter Pe
            {  '\xf1',  '\u042f' }, // Cyri  ic Capita   etter Ya
            {  '\xf2',  '\u0420' }, // Cyri  ic Capita   etter Er
            {  '\xf3',  '\u0421' }, // Cyri  ic Capita   etter Es
            {  '\xf4',  '\u0422' }, // Cyri  ic Capita   etter Te
            {  '\xf5',  '\u0423' }, // Cyri  ic Capita   etter U
            {  '\xf6',  '\u0416' }, // Cyri  ic Capita   etter Zhe
            {  '\xf7',  '\u0412' }, // Cyri  ic Capita   etter Ve
            {  '\xf8',  '\u042c' }, // Cyri  ic Capita   etter Soft Sign
            {  '\xf9',  '\u042b' }, // Cyri  ic Capita   etter Yeru
            {  '\xfa',  '\u0417' }, // Cyri  ic Capita   etter Ze
            {  '\xfb',  '\u0428' }, // Cyri  ic Capita   etter Sha
            {  '\xfc',  '\u042d' }, // Cyri  ic Capita   etter E
            {  '\xfd',  '\u0429' }, // Cyri  ic Capita   etter Shcha
            {  '\xfe',  '\u0427' }, // Cyri  ic Capita   etter Che
        });

        public static readonly VTCharsetMap DecGreek = new VTCharsetMap((char)0xA0, 94, new Dictionary<char, char>()
        {
            {  '\xa4',  '\u2426' }, // Undefined
            {  '\xa6',  '\u2426' }, // Undefined
            {  '\xa8',  '\u00a4' }, // Currency Sign
            {  '\xac',  '\u2426' }, // Undefined
            {  '\xad',  '\u2426' }, // Undefined
            {  '\xae',  '\u2426' }, // Undefined
            {  '\xaf',  '\u2426' }, // Undefined
            {  '\xb4',  '\u2426' }, // Undefined
            {  '\xb8',  '\u2426' }, // Undefined
            {  '\xbe',  '\u2426' }, // Undefined
            {  '\xc0',  '\u03ca' }, // Greek Sma    etter Iota With Dia ytika
            {  '\xc1',  '\u0391' }, // Greek Capita   etter A pha
            {  '\xc2',  '\u0392' }, // Greek Capita   etter Beta
            {  '\xc3',  '\u0393' }, // Greek Capita   etter Gamma
            {  '\xc4',  '\u0394' }, // Greek Capita   etter De ta
            {  '\xc5',  '\u0395' }, // Greek Capita   etter Epsi on
            {  '\xc6',  '\u0396' }, // Greek Capita   etter Zeta
            {  '\xc7',  '\u0397' }, // Greek Capita   etter Eta
            {  '\xc8',  '\u0398' }, // Greek Capita   etter Theta
            {  '\xc9',  '\u0399' }, // Greek Capita   etter Iota
            {  '\xca',  '\u039a' }, // Greek Capita   etter Kappa
            {  '\xcb',  '\u039b' }, // Greek Capita   etter  amda
            {  '\xcc',  '\u039c' }, // Greek Capita   etter Mu
            {  '\xcd',  '\u039d' }, // Greek Capita   etter Nu
            {  '\xce',  '\u039e' }, // Greek Capita   etter Xi
            {  '\xcf',  '\u039f' }, // Greek Capita   etter Omicron
            {  '\xd0',  '\u2426' }, // Undefined
            {  '\xd1',  '\u03a0' }, // Greek Capita   etter Pi
            {  '\xd2',  '\u03a1' }, // Greek Capita   etter Rho
            {  '\xd3',  '\u03a3' }, // Greek Capita   etter Sigma
            {  '\xd4',  '\u03a4' }, // Greek Capita   etter Tau
            {  '\xd5',  '\u03a5' }, // Greek Capita   etter Upsi on
            {  '\xd6',  '\u03a6' }, // Greek Capita   etter Phi
            {  '\xd7',  '\u03a7' }, // Greek Capita   etter Chi
            {  '\xd8',  '\u03a8' }, // Greek Capita   etter Psi
            {  '\xd9',  '\u03a9' }, // Greek Capita   etter Omega
            {  '\xda',  '\u03ac' }, // Greek Sma    etter A pha With Tonos
            {  '\xdb',  '\u03ad' }, // Greek Sma    etter Epsi on With Tonos
            {  '\xdc',  '\u03ae' }, // Greek Sma    etter Eta With Tonos
            {  '\xdd',  '\u03af' }, // Greek Sma    etter Iota With Tonos
            {  '\xde',  '\u2426' }, // Undefined
            {  '\xdf',  '\u03cc' }, // Greek Sma    etter Omicron With Tonos
            {  '\xe0',  '\u03cb' }, // Greek Sma    etter Upsi on With Dia ytika
            {  '\xe1',  '\u03b1' }, // Greek Sma    etter A pha
            {  '\xe2',  '\u03b2' }, // Greek Sma    etter Beta
            {  '\xe3',  '\u03b3' }, // Greek Sma    etter Gamma
            {  '\xe4',  '\u03b4' }, // Greek Sma    etter De ta
            {  '\xe5',  '\u03b5' }, // Greek Sma    etter Epsi on
            {  '\xe6',  '\u03b6' }, // Greek Sma    etter Zeta
            {  '\xe7',  '\u03b7' }, // Greek Sma    etter Eta
            {  '\xe8',  '\u03b8' }, // Greek Sma    etter Theta
            {  '\xe9',  '\u03b9' }, // Greek Sma    etter Iota
            {  '\xea',  '\u03ba' }, // Greek Sma    etter Kappa
            {  '\xeb',  '\u03bb' }, // Greek Sma    etter  amda
            {  '\xec',  '\u03bc' }, // Greek Sma    etter Mu
            {  '\xed',  '\u03bd' }, // Greek Sma    etter Nu
            {  '\xee',  '\u03be' }, // Greek Sma    etter Xi
            {  '\xef',  '\u03bf' }, // Greek Sma    etter Omicron
            {  '\xf0',  '\u2426' }, // Undefined
            {  '\xf1',  '\u03c0' }, // Greek Sma    etter Pi
            {  '\xf2',  '\u03c1' }, // Greek Sma    etter Rho
            {  '\xf3',  '\u03c3' }, // Greek Sma    etter Sigma
            {  '\xf4',  '\u03c4' }, // Greek Sma    etter Tau
            {  '\xf5',  '\u03c5' }, // Greek Sma    etter Upsi on
            {  '\xf6',  '\u03c6' }, // Greek Sma    etter Phi
            {  '\xf7',  '\u03c7' }, // Greek Sma    etter Chi
            {  '\xf8',  '\u03c8' }, // Greek Sma    etter Psi
            {  '\xf9',  '\u03c9' }, // Greek Sma    etter Omega
            {  '\xfa',  '\u03c2' }, // Greek Sma    etter Fina  Sigma
            {  '\xfb',  '\u03cd' }, // Greek Sma    etter Upsi on With Tonos
            {  '\xfc',  '\u03ce' }, // Greek Sma    etter Omega With Tonos
            {  '\xfd',  '\u0384' }, // Greek Tonos
            {  '\xfe',  '\u2426' }, // Undefined
        });

        public static readonly VTCharsetMap DecHebrew = new VTCharsetMap((char)0xA0, 94, new Dictionary<char, char>()
        {
            {  '\xa4',  '\u2426' }, // Undefined
            {  '\xa6',  '\u2426' }, // Undefined
            {  '\xa8',  '\u00a4' }, // Currency Sign
            {  '\xac',  '\u2426' }, // Undefined
            {  '\xad',  '\u2426' }, // Undefined
            {  '\xae',  '\u2426' }, // Undefined
            {  '\xaf',  '\u2426' }, // Undefined
            {  '\xb4',  '\u2426' }, // Undefined
            {  '\xb8',  '\u2426' }, // Undefined
            {  '\xbe',  '\u2426' }, // Undefined
            {  '\xc0',  '\u2426' }, // Undefined
            {  '\xc1',  '\u2426' }, // Undefined
            {  '\xc2',  '\u2426' }, // Undefined
            {  '\xc3',  '\u2426' }, // Undefined
            {  '\xc4',  '\u2426' }, // Undefined
            {  '\xc5',  '\u2426' }, // Undefined
            {  '\xc6',  '\u2426' }, // Undefined
            {  '\xc7',  '\u2426' }, // Undefined
            {  '\xc8',  '\u2426' }, // Undefined
            {  '\xc9',  '\u2426' }, // Undefined
            {  '\xca',  '\u2426' }, // Undefined
            {  '\xcb',  '\u2426' }, // Undefined
            {  '\xcc',  '\u2426' }, // Undefined
            {  '\xcd',  '\u2426' }, // Undefined
            {  '\xce',  '\u2426' }, // Undefined
            {  '\xcf',  '\u2426' }, // Undefined
            {  '\xd0',  '\u2426' }, // Undefined
            {  '\xd1',  '\u2426' }, // Undefined
            {  '\xd2',  '\u2426' }, // Undefined
            {  '\xd3',  '\u2426' }, // Undefined
            {  '\xd4',  '\u2426' }, // Undefined
            {  '\xd5',  '\u2426' }, // Undefined
            {  '\xd6',  '\u2426' }, // Undefined
            {  '\xd7',  '\u2426' }, // Undefined
            {  '\xd8',  '\u2426' }, // Undefined
            {  '\xd9',  '\u2426' }, // Undefined
            {  '\xda',  '\u2426' }, // Undefined
            {  '\xdb',  '\u2426' }, // Undefined
            {  '\xdc',  '\u2426' }, // Undefined
            {  '\xdd',  '\u2426' }, // Undefined
            {  '\xde',  '\u2426' }, // Undefined
            {  '\xdf',  '\u2426' }, // Undefined
            {  '\xe0',  '\u05d0' }, // Hebrew  etter A ef
            {  '\xe1',  '\u05d1' }, // Hebrew  etter Bet
            {  '\xe2',  '\u05d2' }, // Hebrew  etter Gime 
            {  '\xe3',  '\u05d3' }, // Hebrew  etter Da et
            {  '\xe4',  '\u05d4' }, // Hebrew  etter He
            {  '\xe5',  '\u05d5' }, // Hebrew  etter Vav
            {  '\xe6',  '\u05d6' }, // Hebrew  etter Zayin
            {  '\xe7',  '\u05d7' }, // Hebrew  etter Het
            {  '\xe8',  '\u05d8' }, // Hebrew  etter Tet
            {  '\xe9',  '\u05d9' }, // Hebrew  etter Yod
            {  '\xea',  '\u05da' }, // Hebrew  etter Fina  Kaf
            {  '\xeb',  '\u05db' }, // Hebrew  etter Kaf
            {  '\xec',  '\u05dc' }, // Hebrew  etter  amed
            {  '\xed',  '\u05dd' }, // Hebrew  etter Fina  Mem
            {  '\xee',  '\u05de' }, // Hebrew  etter Mem
            {  '\xef',  '\u05df' }, // Hebrew  etter Fina  Nun
            {  '\xf0',  '\u05e0' }, // Hebrew  etter Nun
            {  '\xf1',  '\u05e1' }, // Hebrew  etter Samekh
            {  '\xf2',  '\u05e2' }, // Hebrew  etter Ayin
            {  '\xf3',  '\u05e3' }, // Hebrew  etter Fina  Pe
            {  '\xf4',  '\u05e4' }, // Hebrew  etter Pe
            {  '\xf5',  '\u05e5' }, // Hebrew  etter Fina  Tsadi
            {  '\xf6',  '\u05e6' }, // Hebrew  etter Tsadi
            {  '\xf7',  '\u05e7' }, // Hebrew  etter Qof
            {  '\xf8',  '\u05e8' }, // Hebrew  etter Resh
            {  '\xf9',  '\u05e9' }, // Hebrew  etter Shin
            {  '\xfa',  '\u05ea' }, // Hebrew  etter Tav
            {  '\xfb',  '\u2426' }, // Undefined
            {  '\xfc',  '\u2426' }, // Undefined
            {  '\xfd',  '\u2426' }, // Undefined
            {  '\xfe',  '\u2426' }, // Undefined
        });

        public static readonly VTCharsetMap DecTurkish = new VTCharsetMap((char)0xA0, 94, new Dictionary<char, char>()
        {
            {  '\xa4',  '\u2426' }, // Undefined
            {  '\xa6',  '\u2426' }, // Undefined
            {  '\xa8',  '\u00a4' }, // Currency Sign
            {  '\xac',  '\u2426' }, // Undefined
            {  '\xad',  '\u2426' }, // Undefined
            {  '\xae',  '\u0130' }, //  atin Capita   etter I With Dot Above
            {  '\xaf',  '\u2426' }, // Undefined
            {  '\xb4',  '\u2426' }, // Undefined
            {  '\xb8',  '\u2426' }, // Undefined
            {  '\xbe',  '\u0131' }, //  atin Sma    etter Dot ess I
            {  '\xd0',  '\u011e' }, //  atin Capita   etter G With Breve
            {  '\xd7',  '\u0152' }, //  atin Capita   igature Oe
            {  '\xdd',  '\u0178' }, //  atin Capita   etter Y With Diaeresis
            {  '\xde',  '\u015e' }, //  atin Capita   etter S With Cedi  a
            {  '\xf0',  '\u011f' }, //  atin Sma    etter G With Breve
            {  '\xf7',  '\u0153' }, //  atin Sma    igature Oe
            {  '\xfd',  '\u00ff' }, //  atin Sma    etter Y With Diaeresis
            {  '\xfe',  '\u015f' }, //  atin Sma    etter S With Cedi  a
        });

        public static readonly VTCharsetMap BritishNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x23', '\u00a3' }, // Pound Sign
        });

        public static readonly VTCharsetMap DutchNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            {  '\x23',  '\u00a3' }, // Pound Sign
            {  '\x40',  '\u00be' }, // Vu gar Fraction Three Quarters
            {  '\x5b',  '\u0133' }, //  atin Sma    igature Ij (sometimes approximated as y with diaeresis)
            {  '\x5c',  '\u00bd' }, // Vu gar Fraction One Ha f
            {  '\x5d',  '\u007c' }, // Vertica   ine
            {  '\x7b',  '\u00a8' }, // Diaeresis
            {  '\x7c',  '\u0192' }, //  atin Sma    etter F With Hook (sometimes approximated as f)
            {  '\x7d',  '\u00bc' }, // Vu gar Fraction One Quarter
            {  '\x7e',  '\u00b4' }, // Acute Accent
        });

        public static readonly VTCharsetMap FinnishNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>()
        {
            { '\x5b', '\u00c4' }, // atin Capita etter A With Diaeresis
            { '\x5c', '\u00d6' }, // atin Capita etter O With Diaeresis
            { '\x5d', '\u00c5' }, // atin Capita etter A With Ring Above
            { '\x5e', '\u00dc' }, // atin Capita etter U With Diaeresis
            { '\x60', '\u00e9' }, // atin Sma etter E With Acute
            { '\x7b', '\u00e4' }, // atin Sma etter A With Diaeresis
            { '\x7c', '\u00f6' }, // atin Sma etter O With Diaeresis
            { '\x7d', '\u00e5' }, // atin Sma etter A With Ring Above
            { '\x7e', '\u00fc' }, // atin Sma etter U With Diaeresis
        });

        public static readonly VTCharsetMap FrenchNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x23', '\u00a3' }, // Pound Sign
            { '\x40', '\u00e0' }, // atin Sma etter A With Grave
            { '\x5b', '\u00b0' }, // Degree Sign
            { '\x5c', '\u00e7' }, // atin Sma etter C With Cedia
            { '\x5d', '\u00a7' }, // Section Sign
            { '\x7b', '\u00e9' }, // atin Sma etter E With Acute
            { '\x7c', '\u00f9' }, // atin Sma etter U With Grave
            { '\x7d', '\u00e8' }, // atin Sma etter E With Grave
            { '\x7e', '\u00a8' }, // Diaeresis
        });

        public static readonly VTCharsetMap FrenchNrcsIso = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x23', '\u00a3' }, // Pound Sign
            { '\x40', '\u00e0' }, // atin Sma etter A With Grave
            { '\x5b', '\u00b0' }, // Degree Sign
            { '\x5c', '\u00e7' }, // atin Sma etter C With Cedia
            { '\x5d', '\u00a7' }, // Section Sign
            { '\x60', '\u00b5' }, // Micro Sign
            { '\x7b', '\u00e9' }, // atin Sma etter E With Acute
            { '\x7c', '\u00f9' }, // atin Sma etter U With Grave
            { '\x7d', '\u00e8' }, // atin Sma etter E With Grave
            { '\x7e', '\u00a8' }, // Diaeresis
        });

        public static readonly VTCharsetMap FrenchCanadianNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x40', '\u00e0' }, // atin Sma etter A With Grave
            { '\x5b', '\u00e2' }, // atin Sma etter A With Circumfex
            { '\x5c', '\u00e7' }, // atin Sma etter C With Cedia
            { '\x5d', '\u00ea' }, // atin Sma etter E With Circumfex
            { '\x5e', '\u00ee' }, // atin Sma etter I With Circumfex
            { '\x60', '\u00f4' }, // atin Sma etter O With Circumfex
            { '\x7b', '\u00e9' }, // atin Sma etter E With Acute
            { '\x7c', '\u00f9' }, // atin Sma etter U With Grave
            { '\x7d', '\u00e8' }, // atin Sma etter E With Grave
            { '\x7e', '\u00fb' }, // atin Sma etter U With Circumfex
        });

        public static readonly VTCharsetMap GermanNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x40', '\u00a7' }, // Section Sign
            { '\x5b', '\u00c4' }, // atin Capita etter A With Diaeresis
            { '\x5c', '\u00d6' }, // atin Capita etter O With Diaeresis
            { '\x5d', '\u00dc' }, // atin Capita etter U With Diaeresis
            { '\x7b', '\u00e4' }, // atin Sma etter A With Diaeresis
            { '\x7c', '\u00f6' }, // atin Sma etter O With Diaeresis
            { '\x7d', '\u00fc' }, // atin Sma etter U With Diaeresis (VT320 manua incorrecty has this as U+00A8)
            { '\x7e', '\u00df' }, // atin Sma etter Sharp S
        });

        public static readonly VTCharsetMap GreekNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x40', '\u03ca' }, // Greek Sma etter Iota With Diaytika
            { '\x41', '\u0391' }, // Greek Capita etter Apha
            { '\x42', '\u0392' }, // Greek Capita etter Beta
            { '\x43', '\u0393' }, // Greek Capita etter Gamma
            { '\x44', '\u0394' }, // Greek Capita etter Deta
            { '\x45', '\u0395' }, // Greek Capita etter Epsion
            { '\x46', '\u0396' }, // Greek Capita etter Zeta
            { '\x47', '\u0397' }, // Greek Capita etter Eta
            { '\x48', '\u0398' }, // Greek Capita etter Theta
            { '\x49', '\u0399' }, // Greek Capita etter Iota
            { '\x4a', '\u039a' }, // Greek Capita etter Kappa
            { '\x4b', '\u039b' }, // Greek Capita etter amda
            { '\x4c', '\u039c' }, // Greek Capita etter Mu
            { '\x4d', '\u039d' }, // Greek Capita etter Nu
            { '\x4e', '\u039e' }, // Greek Capita etter Xi
            { '\x4f', '\u039f' }, // Greek Capita etter Omicron
            { '\x50', '\u2426' }, // Undefined
            { '\x51', '\u03a0' }, // Greek Capita etter Pi
            { '\x52', '\u03a1' }, // Greek Capita etter Rho
            { '\x53', '\u03a3' }, // Greek Capita etter Sigma
            { '\x54', '\u03a4' }, // Greek Capita etter Tau
            { '\x55', '\u03a5' }, // Greek Capita etter Upsion
            { '\x56', '\u03a6' }, // Greek Capita etter Phi
            { '\x57', '\u03a7' }, // Greek Capita etter Chi
            { '\x58', '\u03a8' }, // Greek Capita etter Psi
            { '\x59', '\u03a9' }, // Greek Capita etter Omega
            { '\x5a', '\u03ac' }, // Greek Sma etter Apha With Tonos
            { '\x5b', '\u03ad' }, // Greek Sma etter Epsion With Tonos
            { '\x5c', '\u03ae' }, // Greek Sma etter Eta With Tonos
            { '\x5d', '\u03af' }, // Greek Sma etter Iota With Tonos
            { '\x5e', '\u2426' }, // Undefined
            { '\x5f', '\u03cc' }, // Greek Sma etter Omicron With Tonos
            { '\x60', '\u03cb' }, // Greek Sma etter Upsion With Diaytika
            { '\x61', '\u03b1' }, // Greek Sma etter Apha
            { '\x62', '\u03b2' }, // Greek Sma etter Beta
            { '\x63', '\u03b3' }, // Greek Sma etter Gamma
            { '\x64', '\u03b4' }, // Greek Sma etter Deta
            { '\x65', '\u03b5' }, // Greek Sma etter Epsion
            { '\x66', '\u03b6' }, // Greek Sma etter Zeta
            { '\x67', '\u03b7' }, // Greek Sma etter Eta
            { '\x68', '\u03b8' }, // Greek Sma etter Theta
            { '\x69', '\u03b9' }, // Greek Sma etter Iota
            { '\x6a', '\u03ba' }, // Greek Sma etter Kappa
            { '\x6b', '\u03bb' }, // Greek Sma etter amda
            { '\x6c', '\u03bc' }, // Greek Sma etter Mu
            { '\x6d', '\u03bd' }, // Greek Sma etter Nu
            { '\x6e', '\u03be' }, // Greek Sma etter Xi
            { '\x6f', '\u03bf' }, // Greek Sma etter Omicron
            { '\x70', '\u2426' }, // Undefined
            { '\x71', '\u03c0' }, // Greek Sma etter Pi
            { '\x72', '\u03c1' }, // Greek Sma etter Rho
            { '\x73', '\u03c3' }, // Greek Sma etter Sigma
            { '\x74', '\u03c4' }, // Greek Sma etter Tau
            { '\x75', '\u03c5' }, // Greek Sma etter Upsion
            { '\x76', '\u03c6' }, // Greek Sma etter Phi
            { '\x77', '\u03c7' }, // Greek Sma etter Chi
            { '\x78', '\u03c8' }, // Greek Sma etter Psi
            { '\x79', '\u03c9' }, // Greek Sma etter Omega
            { '\x7a', '\u03c2' }, // Greek Sma etter Fina Sigma
            { '\x7b', '\u03cd' }, // Greek Sma etter Upsion With Tonos
            { '\x7c', '\u03ce' }, // Greek Sma etter Omega With Tonos
            { '\x7d', '\u0384' }, // Greek Tonos
            { '\x7e', '\u2426' }, // Undefined
        });

        public static readonly VTCharsetMap HebrewNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x60', '\u05d0' }, // Hebrew Letter Alef
            { '\x61', '\u05d1' }, // Hebrew Letter Bet
            { '\x62', '\u05d2' }, // Hebrew Letter Gimel
            { '\x63', '\u05d3' }, // Hebrew Letter Dalet
            { '\x64', '\u05d4' }, // Hebrew Letter He
            { '\x65', '\u05d5' }, // Hebrew Letter Vav
            { '\x66', '\u05d6' }, // Hebrew Letter Zayin
            { '\x67', '\u05d7' }, // Hebrew Letter Het
            { '\x68', '\u05d8' }, // Hebrew Letter Tet
            { '\x69', '\u05d9' }, // Hebrew Letter Yod
            { '\x6a', '\u05da' }, // Hebrew Letter Final Kaf
            { '\x6b', '\u05db' }, // Hebrew Letter Kaf
            { '\x6c', '\u05dc' }, // Hebrew Letter Lamed
            { '\x6d', '\u05dd' }, // Hebrew Letter Final Mem
            { '\x6e', '\u05de' }, // Hebrew Letter Mem
            { '\x6f', '\u05df' }, // Hebrew Letter Final Nun
            { '\x70', '\u05e0' }, // Hebrew Letter Nun
            { '\x71', '\u05e1' }, // Hebrew Letter Samekh
            { '\x72', '\u05e2' }, // Hebrew Letter Ayin
            { '\x73', '\u05e3' }, // Hebrew Letter Final Pe
            { '\x74', '\u05e4' }, // Hebrew Letter Pe
            { '\x75', '\u05e5' }, // Hebrew Letter Final Tsadi
            { '\x76', '\u05e6' }, // Hebrew Letter Tsadi
            { '\x77', '\u05e7' }, // Hebrew Letter Qof
            { '\x78', '\u05e8' }, // Hebrew Letter Resh
            { '\x79', '\u05e9' }, // Hebrew Letter Shin
            { '\x7a', '\u05ea' }, // Hebrew Letter Tav
        });

        public static readonly VTCharsetMap ItalianNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x23', '\u00a3' }, // Pound Sign
            { '\x40', '\u00a7' }, // Section Sign
            { '\x5b', '\u00b0' }, // Degree Sign
            { '\x5c', '\u00e7' }, // Latin Small Letter C With Cedilla
            { '\x5d', '\u00e9' }, // Latin Small Letter E With Acute
            { '\x60', '\u00f9' }, // Latin Small Letter U With Grave
            { '\x7b', '\u00e0' }, // Latin Small Letter A With Grave
            { '\x7c', '\u00f2' }, // Latin Small Letter O With Grave
            { '\x7d', '\u00e8' }, // Latin Small Letter E With Grave
            { '\x7e', '\u00ec' }, // Latin Small Letter I With Grave
        });

        public static readonly VTCharsetMap NorwegianDanishNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x40', '\u00c4' }, // Latin Capital Letter A With Diaeresis
            { '\x5b', '\u00c6' }, // Latin Capital Letter Ae
            { '\x5c', '\u00d8' }, // Latin Capital Letter O With Stroke
            { '\x5d', '\u00c5' }, // Latin Capital Letter A With Ring Above
            { '\x5e', '\u00dc' }, // Latin Capital Letter U With Diaeresis
            { '\x60', '\u00e4' }, // Latin Small Letter A With Diaeresis
            { '\x7b', '\u00e6' }, // Latin Small Letter Ae
            { '\x7c', '\u00f8' }, // Latin Small Letter O With Stroke
            { '\x7d', '\u00e5' }, // Latin Small Letter A With Ring Above
            { '\x7e', '\u00fc' }, // Latin Small Letter U With Diaeresis
        });

        public static readonly VTCharsetMap NorwegianDanishNrcsIso = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x5b', '\u00c6' }, // Latin Capital Letter Ae
            { '\x5c', '\u00d8' }, // Latin Capital Letter O With Stroke
            { '\x5d', '\u00c5' }, // Latin Capital Letter A With Ring Above
            { '\x7b', '\u00e6' }, // Latin Small Letter Ae
            { '\x7c', '\u00f8' }, // Latin Small Letter O With Stroke
            { '\x7d', '\u00e5' }, // Latin Small Letter A With Ring Above
        });

        public static readonly VTCharsetMap PortugueseNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x5b', '\u00c3' }, // Latin Capital Letter A With Tilde
            { '\x5c', '\u00c7' }, // Latin Capital Letter C With Cedilla
            { '\x5d', '\u00d5' }, // Latin Capital Letter O With Tilde
            { '\x7b', '\u00e3' }, // Latin Small Letter A With Tilde
            { '\x7c', '\u00e7' }, // Latin Small Letter C With Cedilla
            { '\x7d', '\u00f5' }, // Latin Small Letter O With Tilde
        });

        public static readonly VTCharsetMap RussianNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>() 
        {
            { '\x60', '\u042e' }, // Cyrillic Capital Letter Yu
            { '\x61', '\u0410' }, // Cyrillic Capital Letter A
            { '\x62', '\u0411' }, // Cyrillic Capital Letter Be
            { '\x63', '\u0426' }, // Cyrillic Capital Letter Tse
            { '\x64', '\u0414' }, // Cyrillic Capital Letter De
            { '\x65', '\u0415' }, // Cyrillic Capital Letter Ie
            { '\x66', '\u0424' }, // Cyrillic Capital Letter Ef
            { '\x67', '\u0413' }, // Cyrillic Capital Letter Ghe
            { '\x68', '\u0425' }, // Cyrillic Capital Letter Ha
            { '\x69', '\u0418' }, // Cyrillic Capital Letter I
            { '\x6a', '\u0419' }, // Cyrillic Capital Letter Short I
            { '\x6b', '\u041a' }, // Cyrillic Capital Letter Ka
            { '\x6c', '\u041b' }, // Cyrillic Capital Letter El
            { '\x6d', '\u041c' }, // Cyrillic Capital Letter Em
            { '\x6e', '\u041d' }, // Cyrillic Capital Letter En
            { '\x6f', '\u041e' }, // Cyrillic Capital Letter O
            { '\x70', '\u041f' }, // Cyrillic Capital Letter Pe
            { '\x71', '\u042f' }, // Cyrillic Capital Letter Ya
            { '\x72', '\u0420' }, // Cyrillic Capital Letter Er
            { '\x73', '\u0421' }, // Cyrillic Capital Letter Es
            { '\x74', '\u0422' }, // Cyrillic Capital Letter Te
            { '\x75', '\u0423' }, // Cyrillic Capital Letter U
            { '\x76', '\u0416' }, // Cyrillic Capital Letter Zhe
            { '\x77', '\u0412' }, // Cyrillic Capital Letter Ve
            { '\x78', '\u042c' }, // Cyrillic Capital Letter Soft Sign
            { '\x79', '\u042b' }, // Cyrillic Capital Letter Yeru
            { '\x7a', '\u0417' }, // Cyrillic Capital Letter Ze
            { '\x7b', '\u0428' }, // Cyrillic Capital Letter Sha
            { '\x7c', '\u042d' }, // Cyrillic Capital Letter E
            { '\x7d', '\u0429' }, // Cyrillic Capital Letter Shcha
            { '\x7e', '\u0427' }, // Cyrillic Capital Letter Che
        });

        public static readonly VTCharsetMap SpanishNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>()
        {
            { '\x23', '\u00a3' }, // Pound Sign
            { '\x40', '\u00a7' }, // Section Sign
            { '\x5b', '\u00a1' }, // Inverted Exclamation Mark
            { '\x5c', '\u00d1' }, // Latin Capital Letter N With Tilde
            { '\x5d', '\u00bf' }, // Inverted Question Mark
            { '\x7b', '\u00b0' }, // Degree Sign (VT320 manual has these last 3 off by 1)
            { '\x7c', '\u00f1' }, // Latin Small Letter N With Tilde
            { '\x7d', '\u00e7' }, // Latin Small Letter C With Cedilla
        });

        public static readonly VTCharsetMap SwedishNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>()
        {
            { '\x40', '\u00c9' }, // Latin Capital Letter E With Acute
            { '\x5b', '\u00c4' }, // Latin Capital Letter A With Diaeresis
            { '\x5c', '\u00d6' }, // Latin Capital Letter O With Diaeresis
            { '\x5d', '\u00c5' }, // Latin Capital Letter A With Ring Above
            { '\x5e', '\u00dc' }, // Latin Capital Letter U With Diaeresis
            { '\x60', '\u00e9' }, // Latin Small Letter E With Acute
            { '\x7b', '\u00e4' }, // Latin Small Letter A With Diaeresis
            { '\x7c', '\u00f6' }, // Latin Small Letter O With Diaeresis
            { '\x7d', '\u00e5' }, // Latin Small Letter A With Ring Above
            { '\x7e', '\u00fc' }, // Latin Small Letter U With Diaeresis
        });

        public static readonly VTCharsetMap SwissNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>()
        {
            { '\x23', '\u00f9' }, // Latin Small Letter U With Grave
            { '\x40', '\u00e0' }, // Latin Small Letter A With Grave
            { '\x5b', '\u00e9' }, // Latin Small Letter E With Acute
            { '\x5c', '\u00e7' }, // Latin Small Letter C With Cedilla
            { '\x5d', '\u00ea' }, // Latin Small Letter E With Circumflex
            { '\x5e', '\u00ee' }, // Latin Small Letter I With Circumflex
            { '\x5f', '\u00e8' }, // Latin Small Letter E With Grave
            { '\x60', '\u00f4' }, // Latin Small Letter O With Circumflex
            { '\x7b', '\u00e4' }, // Latin Small Letter A With Diaeresis
            { '\x7c', '\u00f6' }, // Latin Small Letter O With Diaeresis
            { '\x7d', '\u00fc' }, // Latin Small Letter U With Diaeresis
            { '\x7e', '\u00fb' }, // Latin Small Letter U With Circumflex
        });

        public static readonly VTCharsetMap TurkishNrcs = new VTCharsetMap((char)0x20, 94, new Dictionary<char, char>()
        {
            { '\x21', '\u0131' }, // Latin Small Letter Dotless I
            { '\x26', '\u011f' }, // Latin Small Letter G With Breve
            { '\x40', '\u0130' }, // Latin Capital Letter I With Dot Above
            { '\x5b', '\u015e' }, // Latin Capital Letter S With Cedilla
            { '\x5c', '\u00d6' }, // Latin Capital Letter O With Diaeresis
            { '\x5d', '\u00c7' }, // Latin Capital Letter C With Cedilla
            { '\x5e', '\u00dc' }, // Latin Capital Letter U With Diaeresis
            { '\x60', '\u011e' }, // Latin Capital Letter G With Breve
            { '\x7b', '\u015f' }, // Latin Small Letter S With Cedilla
            { '\x7c', '\u00f6' }, // Latin Small Letter O With Diaeresis
            { '\x7d', '\u00e7' }, // Latin Small Letter C With Cedilla
            { '\x7e', '\u00fc' }, // Latin Small Letter U With Diaeresis
        });

        public static readonly VTCharsetMap Drcs94 = new VTCharsetMap('\uEF20', 94, new Dictionary<char, char>()
        {
            { '\uEF20', '\x20' }
        });

        public static readonly VTCharsetMap Drcs96 = new VTCharsetMap('\uEF20', 96);



        /// <summary>
        /// 尝试转换一个字符
        /// 如果找不到则返回原始字符ch
        /// </summary>
        /// <param name="ch">要转换的原始字符</param>
        /// <returns>转换后的字符</returns>
        public char TranslateCharacter(char ch)
        {
            char charFound;
            if (!this.translationTable.TryGetValue(ch, out charFound))
            {
                return ch;
            }

            return charFound;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseChar">字符集里的第一个字符</param>
        /// <param name="size">字符集里的字符数量</param>
        /// <param name="replacements">要替换到字符集里的字符列表</param>
        public VTCharsetMap(char baseChar, int size, Dictionary<char, char> replacements = null)
        {
            this.translationTable = new Dictionary<char, char>();

            for (int i = 0; i < size; i++)
            {
                char ch = (char)(baseChar + i);

                this.translationTable[ch] = ch;
            }

            if (replacements != null)
            {
                foreach (KeyValuePair<char, char> kv in replacements)
                {
                    this.translationTable[kv.Key] = kv.Value;
                }
            }
        }
    }
}
