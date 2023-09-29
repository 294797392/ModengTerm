using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Rendering.Wallpaper
{
    public static class EffectRendererFactory
    {
        public static EffectRenderer Create(BackgroundEffectEnum effectType)
        {
            switch (effectType)
            {
                case BackgroundEffectEnum.None: return new EffectRendererNone();
                case BackgroundEffectEnum.Snow: return new EffectRendererSnow();
                case BackgroundEffectEnum.Star: return new EffectRendererStar();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
