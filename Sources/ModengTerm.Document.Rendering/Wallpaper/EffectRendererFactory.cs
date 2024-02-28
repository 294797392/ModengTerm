using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Rendering.Wallpaper
{
    public static class EffectRendererFactory
    {
        public static EffectRenderer Create(EffectTypeEnum effectType)
        {
            switch (effectType)
            {
                case EffectTypeEnum.None: return new EffectRendererNone();
                case EffectTypeEnum.Snow: return new EffectRendererSnow();
                case EffectTypeEnum.Star: return new EffectRendererStar();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
