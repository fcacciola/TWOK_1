using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NWaves.Signals;

namespace NWaves.DemoForms.UserControls
{
    public partial class SignalPlot : UserControl
    {
      public class Layer
      {
        internal int            _idx ;
        internal string         _name ;
        internal DiscreteSignal _signal;
        internal Color          _fillColor ;
        internal Color          _lineColor ;
        internal int            _lineThickness ;
        internal bool           _topLine ;
        internal bool           _bottomLine ;
      }

        /// <summary>
        /// Signal to plot
        /// </summary>
        public List<Layer> _layers = null ;

        public void SetLayer( string aName, int aIdx, DiscreteSignal aSignal, Color aFillColor, Color aLineColor, int aLineThickness, bool aTopLine, bool aBottomLine )
        {
          if ( _layers == null )
            _layers = new List<Layer>() ;

          Layer lLayer = _layers.Find( l => l._name == aName ) ;

          if ( lLayer != null)
          {
            lLayer._name          = aName ;
            lLayer._idx           = aIdx ;
            lLayer._signal        = aSignal ;
            lLayer._fillColor     = aFillColor ;  
            lLayer._lineColor     = aLineColor ;  
            lLayer._lineThickness = aLineThickness ;  
            lLayer._topLine       = aTopLine ;  
            lLayer._bottomLine    = aBottomLine ;  
          }
          else 
          { 
            _layers.Add( new Layer(){ _idx           = aIdx
                                    , _name          = aName
                                    , _signal        = aSignal
                                    , _fillColor     = aFillColor
                                    , _lineColor     = aLineColor
                                    , _lineThickness = aLineThickness 
                                    , _topLine       = aTopLine 
                                    , _bottomLine    = aBottomLine
                                    });


          }

          _layers.Sort( delegate(Layer l1, Layer l2) {  return l1._idx.CompareTo(l2._idx) ; } );

          int lMaxLength = 0  ;
          foreach ( var lL in _layers ) 
            lMaxLength = Math.Max(lMaxLength,lL._signal.Length);

          _stride = lMaxLength / Width ;

          AutoScrollMinSize = new Size(lMaxLength / _stride + 20, 0);

          SetupBitmap();
        }

        void SetupBitmap()
        {
          List<Layer> lLayers = new List<Layer>();

          if ( _layers != null )
          {
            foreach ( var lLayer in _layers ) 
            {
              if ( ( this.ParentForm as DIGITC1.Form1).IsRenderModuleChecked(lLayer._name) )
              {
                lLayers.Add(lLayer );
              }
            }
          }

          MakeBitmap(lLayers);
          Invalidate();
        }

        private int _stride = 64;

        public int Stride
        {
            get { return _stride; }
            set
            {
              _stride = value > 1 ? value : 1;
              SetupBitmap();
            }
        }

        public float Gain { get; set; } = 1;

        public int PaddingX { get; set; } = 24;
        public int PaddingY { get; set; } = 5;

        public void LayersChanged()
        { 
          SetupBitmap();
        }

        public SignalPlot()
        {
            InitializeComponent();
            ForeColor = Color.Blue;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if ( _bmp != null ) 
              e.Graphics.DrawImage(_bmp, 0, 0, new Rectangle(-AutoScrollPosition.X, 0, Width, Height), GraphicsUnit.Pixel);
        }

        private void buttonZoomOut_Click(object sender, System.EventArgs e)
        {
            if (_stride < 4)
            {
                Stride++;
            }
            else
            {
                Stride = (int)(_stride * 1.25);
            }
        }

        private void buttonZoomIn_Click(object sender, System.EventArgs e)
        {
            Stride = (int)(_stride / 1.25);
        }


        private Bitmap _bmp;

        private void MakeBitmap( List<Layer> aLayers )
        {
            var width = Math.Max(AutoScrollMinSize.Width, Width);

            _bmp = new Bitmap(width, Height);

            var g = Graphics.FromImage(_bmp);
            g.Clear(Color.White);

            var offset = Height / 2;

            var gray = new Pen(Color.LightGray) { DashPattern = new[] { 2f, 2f } };

            for (var k = 0; k < offset; k += 10)
            {
                g.DrawLine(gray, 0, offset + k, width, offset + k);
                g.DrawLine(gray, 0, offset - k, width, offset - k);
            }

            gray.Dispose();

            if (aLayers != null && aLayers.Count > 0)
            {
                DrawAxes(g, -(Height - 2 * PaddingY) / (2 * Gain), 
                             (Height - 2 * PaddingY) / (2 * Gain));

                foreach( var lLayer in aLayers )
                {
                  var fillPen = new Pen(lLayer._fillColor);
                  var linePen = new Pen(lLayer._lineColor);

                  var i = 0;
                  var x = PaddingX;

                  float prevty = 0;
                  float prevby = 0;

                  while (i < lLayer._signal.Length - _stride)
                  {
                      var j = 0;
                      var min = 0.0;
                      var max = 0.0;
                      while (j < _stride)
                      {
                          if (lLayer._signal[i + j] > max) max = lLayer._signal[i + j];
                          if (lLayer._signal[i + j] < min) min = lLayer._signal[i + j];
                          j++;
                      }

                      var ty1 = (float) (-max*Gain) + offset ;
                      var by1 = (float) (-min*Gain) + offset ;
                      var ty2 = (float) ty1 + ( lLayer._topLine    ? lLayer._lineThickness : 0 ) ;
                      var by2 = (float) by1 - ( lLayer._bottomLine ? lLayer._lineThickness : 0 ) ;

                      if ( i > 0 && lLayer._topLine)
                        g.DrawLine(linePen, x-1, prevty, x, ty1);

                      if ( ty1 < ty2 )
                        g.DrawLine(linePen, x, ty1, x, ty2);

                      if ( ty2 < by2 )
                        g.DrawLine(fillPen, x, ty2, x, by2);

                      if ( i > 0 && lLayer._bottomLine)
                        g.DrawLine(linePen, x-1, prevby, x, by1);

                      if ( by2 < by1 )  
                        g.DrawLine(linePen, x, by2, x, by1);

                      prevty = ty1;
                      prevby = by1;

                      x++;
                      i += _stride;
                  }

                  fillPen.Dispose();
                  linePen.Dispose();
                }
            }

            g.Dispose();
        }

        private void DrawAxes(Graphics g, float min, float max)
        {
            var black = new Pen(Color.Black);

            //g.DrawLine(black, PaddingX, Height/2, _bmp.Width, Height/2);
            g.DrawLine(black, PaddingX, 10, PaddingX, Height - PaddingY);

            var font = new Font("arial", 5);
            var brush = new SolidBrush(Color.Black);

            const int stride = 20;
            var pos = Height + 2;
            var n = (Height - 2 * PaddingY) / stride;
            for (var i = 0; i <= n; i++)
            {
                g.DrawString(string.Format("{0:F2}", min + i * (max - min) / n), font, brush, 1, pos -= stride);
            }

            font.Dispose();
            brush.Dispose();

            black.Dispose();
        }
    }
}
