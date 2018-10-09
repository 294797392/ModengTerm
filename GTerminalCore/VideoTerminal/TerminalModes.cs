using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    public class TerminalModes
    {
        /// <summary>
        /// BI-DIRECTIONAL SUPPORT MODE 
        /// </summary>
        public const int BDSM_EXPLICIT = 1;
        public const int BDSM_IMPLICIT = 2;

        /// <summary>
        /// CONTROL REPRESENTATION MODE
        /// </summary>
        public const int CRM_CONTROL = 3;
        public const int CRM_GRAPHIC = 4;

        /// <summary>
        /// DEVICE COMPONENT SELECT MODE
        /// CPR, CR, DCH, DL, EA, ECH, ED, EF, EL, ICH, IL, LF, NEL, RI, SLH, SLL, SPH, SPL.
        /// </summary>
        public const int DCSM_PRESENTATION = 5;
        public const int DCSM_DATA = 6;
    }
}
