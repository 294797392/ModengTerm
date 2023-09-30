using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModengTerm.Rendering.Wallpaper
{
    public class EffectRendererStar : EffectRenderer
    {
        private class StarVisual : DrawingVisual
        {
            public double OffsetX { get; set; }

            public double OffsetY { get; set; }

            public Brush Background { get; set; }

            public ScaleTransform ScaleTransform { get; set; }

            public RotateTransform RotateTransform { get; set; }

            public Geometry Geometry { get; set; }

            public double XV { get; set; }
            public double YV { get; set; }
            public double XT { get; set; }
            public double YT { get; set; }

            public void Draw()
            {
                DrawingContext dc = this.RenderOpen();
                dc.DrawGeometry(this.Background, null, this.Geometry);
                dc.Close();
            }
        }

        #region 实例变量

        /// <summary>
        /// 星星运动的最小速度
        /// </summary>
        private int _starVMin = 30;

        /// <summary>
        /// 星星运动的最大速度
        /// </summary>
        private int _starVMax = 40;

        /// <summary>
        /// 星星转动的最小速度
        /// </summary>
        private int _starRVMin = 90;

        /// <summary>
        /// 星星转动的最大速度
        /// </summary>
        private int _starRVMax = 360;

        private Random random;

        #endregion

        protected override void OnInitialize()
        {
            this.random = new Random();
            Rect containerRect = this.container.ContentBounds;

            for (int i = 0; i < 30; i++)
            {
                double scale = (double)this.random.Next(3, 5) / 10;
                double rotateCenter = 32 * scale / 2; // 32是星星大小

                StarVisual starVisual = new StarVisual()
                {
                    Background = this.GetRandomColorBursh(),
                    OffsetX = (double)this.random.Next(0, (int)containerRect.Width),
                    OffsetY = (double)this.random.Next(0, (int)containerRect.Height),
                    XV = (double)this.random.Next(-_starVMin, _starVMax) / 60,
                    XT = this.random.Next(6, 301), //帧
                    YV = (double)this.random.Next(-_starVMin, _starVMax) / 60,
                    YT = this.random.Next(6, 301),
                    ScaleTransform = new ScaleTransform(scale, scale),
                    RotateTransform = new RotateTransform(0, rotateCenter, rotateCenter),
                    Geometry = PathGeometry.Parse("M16.001007,0L20.944,10.533997 32,12.223022 23.998993,20.421997 25.889008,32 16.001007,26.533997 6.1109924,32 8,20.421997 0,12.223022 11.057007,10.533997z"),
                };
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(starVisual.ScaleTransform);
                transformGroup.Children.Add(starVisual.RotateTransform);
                starVisual.Transform = transformGroup;
                starVisual.Offset = new Vector(starVisual.OffsetX, starVisual.OffsetY);
                starVisual.Draw();

                this.container.Children.Add(starVisual);
            }
        }

        protected override void OnRelease()
        {
        }

        public override void DrawFrame()
        {
            IEnumerable<StarVisual> starVisuals = this.container.Children.Cast<StarVisual>();

            foreach (StarVisual startVisual in starVisuals)
            {
                Rect bounds = this.container.ContentBounds;

                if (startVisual.XT > 0)
                {
                    //运动时间大于0,继续运动
                    if (startVisual.OffsetX >= bounds.Width || startVisual.OffsetX <= 0)
                    {
                        //碰到边缘,速度取反向
                        startVisual.XV = -startVisual.XV;
                    }
                    //位移加,时间减
                    startVisual.OffsetX += startVisual.XV;
                }
                else
                {
                    startVisual.XV = (double)this.random.Next(-_starVMin, _starVMax) / 60;
                    startVisual.XT = this.random.Next(100, 1001);
                }

                if (startVisual.YT > 0)
                {
                    //运动时间大于0,继续运动
                    if (startVisual.OffsetY >= bounds.Height || startVisual.OffsetY <= 0)
                    {
                        //碰到边缘,速度取反向
                        startVisual.YV = -startVisual.YV;
                    }
                    //位移加,时间减
                    startVisual.OffsetY += startVisual.YV;
                }
                else
                {
                    //运动时间小于0,重新设置速度和时间
                    startVisual.YV = (double)this.random.Next(-_starVMin, _starVMax) / 60;
                    startVisual.YT = this.random.Next(100, 1001);
                }

                startVisual.Offset = new Vector(startVisual.OffsetX, startVisual.OffsetY);

                startVisual.RotateTransform.Angle += 2;
            }
        }

        /// <summary>
        /// 获取随机颜色画刷(偏亮)
        /// </summary>
        /// <returns>SolidColorBrush</returns>
        private SolidColorBrush GetRandomColorBursh()
        {
            byte r = (byte)random.Next(128, 256);
            byte g = (byte)random.Next(128, 256);
            byte b = (byte)random.Next(128, 256);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
    }
}
