using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq ;

using NWaves.Signals;

namespace NWaves.DemoForms.UserControls
{
    public partial class SignalPlot : UserControl
    {
      public class Layer
      {
        internal int            mIdx ;
        internal string         mName ;
        internal DiscreteSignal mSignal;
        internal Color          mFillColor ;
        internal Color          mLineColor ;
        internal int            mLineThickness ;
        internal bool           mDrawTopLine ;
        internal bool           mDrawBottomLine ;

        internal float mAmplitude ;

        internal void Render( Graphics aG, int aHeight, int aPaddingX, int aPaddingY, int aTimeAxisHeight, int aSamplePerPixel)
        {
          var fillPen = new Pen(mFillColor);
          var linePen = new Pen(mLineColor);

          var x = aPaddingX;

          float prevty = 0;
          float prevby = 0;

          int lEffectiveHalfHeight = ( aHeight / 2 ) - aPaddingY - aPaddingY - aTimeAxisHeight ;

          var lOffset = aHeight / 2;

          for ( int i = 0; i < mSignal.Length; i += aSamplePerPixel )
          {
            float lMinV = 0.0f;
            float lMaxV = 0.0f;

            for ( int j = 0 ; j < aSamplePerPixel && (i + j ) < mSignal.Length ; ++ j )
            {
              float lV = mSignal[i+j];
              if (lV > lMaxV) lMaxV = lV ;
              if (lV < lMinV) lMinV = lV;
            }

            float lMaxPxs = ( lMaxV / mAmplitude ) * lEffectiveHalfHeight ;
            float lMinPxs = ( lMinV / mAmplitude ) * lEffectiveHalfHeight ;


            var ty1 = -lMaxPxs + lOffset ;
            var by1 = -lMinPxs + lOffset ;
            var ty2 = (float) ty1 + ( mDrawTopLine    ? mLineThickness : 0 ) ;
            var by2 = (float) by1 - ( mDrawBottomLine ? mLineThickness : 0 ) ;

            if ( i > 0 && mDrawTopLine)
              aG.DrawLine(linePen, x-1, prevty, x, ty1);

            if ( ty1 < ty2 )
              aG.DrawLine(linePen, x, ty1, x, ty2);

            if ( ty2 < by2 )
              aG.DrawLine(fillPen, x, ty2, x, by2);

            if ( i > 0 && mDrawBottomLine)
              aG.DrawLine(linePen, x-1, prevby, x, by1);

            if ( by2 < by1 )  
              aG.DrawLine(linePen, x, by2, x, by1);

            prevty = ty1;
            prevby = by1;

            x++;
          }

          fillPen.Dispose();
          linePen.Dispose();

        }
      }

      List<Layer> mLayers = null ;
      Bitmap      mBitmap;
      double      mSamplingRate = 44100 ;
      int         mMaxLength    = 0 ;

      public void SetLayer( string aName, int aIdx, DiscreteSignal aSignal, Color aFillColor, Color aLineColor, int aLineThickness, bool aTopLine, bool aBottomLine )
      {
        if ( mLayers == null )
          mLayers = new List<Layer>() ;

        float lAmplitude = Math.Max( aSignal.Samples.Max(), - aSignal.Samples.Min() ) ;

        Layer lLayer = mLayers.Find( l => l.mName == aName ) ;

        if ( lLayer != null)
        {
          lLayer.mName           = aName ;
          lLayer.mIdx            = aIdx ;
          lLayer.mSignal         = aSignal ;
          lLayer.mFillColor      = aFillColor ;  
          lLayer.mLineColor      = aLineColor ;  
          lLayer.mLineThickness  = aLineThickness ;  
          lLayer.mDrawTopLine    = aTopLine ;  
          lLayer.mDrawBottomLine = aBottomLine ;  
          lLayer.mAmplitude      = lAmplitude ;
        }
        else 
        { 
          mLayers.Add( new Layer(){ mIdx            = aIdx
                                  , mName           = aName
                                  , mSignal         = aSignal
                                  , mFillColor      = aFillColor
                                  , mLineColor      = aLineColor
                                  , mLineThickness  = aLineThickness 
                                  , mDrawTopLine    = aTopLine 
                                  , mDrawBottomLine = aBottomLine
                                  , mAmplitude      = lAmplitude
                                  });


        }

        mLayers.Sort( delegate(Layer l1, Layer l2) {  return l1.mIdx.CompareTo(l2.mIdx) ; } );

        mMaxLength = 0  ;
        foreach ( var lL in mLayers ) 
          mMaxLength = Math.Max(mMaxLength,lL.mSignal.Length);

        mSamplesPerPixel = (int)Math.Ceiling((float)mMaxLength / (float)(Width-PaddingX*2)) ;

        SetupAutoScrollMinSize();

        mSamplingRate = aSignal.SamplingRate ;

        SetupBitmap();
      }

      void SetupAutoScrollMinSize()
      {
        AutoScrollMinSize = new Size((mMaxLength / mSamplesPerPixel) + PaddingX * 2 + 20, 0);
      }

      void SetupBitmap()
      {
        List<Layer> lLayers = new List<Layer>();

        if ( mLayers != null )
        {
          foreach ( var lLayer in mLayers ) 
          {
            if ( ( this.ParentForm as DIGITC1.Form1).IsRenderModuleChecked(lLayer.mName) )
            {
              lLayers.Add(lLayer );
            }
          }
        }

        MakeBitmap(lLayers);
        Invalidate();
      }

      int mSamplesPerPixel = 0;
      
      public int SamplesPerPixel
      {
        get { return mSamplesPerPixel; }
        set 
        { 
          mSamplesPerPixel = value; 
          SetupAutoScrollMinSize();
          SetupBitmap();
          Invalidate();
        }
      }

      public int PaddingX { get; set; } = 30;
      public int PaddingY { get; set; } = 5;

      public void LayersChanged()
      { 
        SetupBitmap();
      }

      public SignalPlot()
      {
          InitializeComponent();
      }

      protected override void OnPaint(PaintEventArgs e)
      {
          base.OnPaint(e);
          if ( mBitmap != null ) 
           e.Graphics.DrawImage(mBitmap, 0, 0, new Rectangle(-AutoScrollPosition.X, 0, Width, Height), GraphicsUnit.Pixel);
      }

      private void buttonZoomOut_Click(object sender, System.EventArgs e)
      {
        SamplesPerPixel = (int)(mSamplesPerPixel * 1.25);
      }

      private void buttonZoomIn_Click(object sender, System.EventArgs e)
      {
        SamplesPerPixel = (int)Math.Max(mSamplesPerPixel / 1.25,1);
      }


      static int c = 0 ;

      private void MakeBitmap( List<Layer> aLayers )
      {
        var lWidth = Math.Max(AutoScrollMinSize.Width, Width);

        mBitmap = new Bitmap(lWidth, Height);

        using ( var lG = Graphics.FromImage(mBitmap) )
        {
          lG.Clear(Color.White);

          int lTimeAxisHeight = DrawTimeAxis(lG, lWidth);

          aLayers.ForEach( lLayer => lLayer.Render(lG, Height, PaddingX, PaddingY, lTimeAxisHeight, mSamplesPerPixel) ) ;
        }
        mBitmap.Save($"C:\\Work\\DIGITC_{c}.png"); ++ c ;
      }

      int DrawTimeAxis( Graphics aG, int aWidth )
      {
        var lFont = new Font("arial", 10);
        var black = new Pen(Color.Black);
        var brush = new SolidBrush(Color.Black);
        
        int lEffectiveWidth = aWidth - PaddingX * 2 ;

        SizeF lTimeLabelSize = aG.MeasureString(TimeMarkToLabel(119), lFont);

        int lMarks = (int)Math.Ceiling(lEffectiveWidth / ( lTimeLabelSize.Width + 4 )) ;

        if ( lMarks % 2 == 0)
          ++ lMarks;

        float lMarkWidth = (float)lEffectiveWidth / (float)( lMarks - 1 ) ;

        float lMarkH = lTimeLabelSize.Height * 0.4f ;
        float lAxisY = lTimeLabelSize.Height * 2f ;

        float lLX = (float)PaddingX;
        float lHX = (float)(aWidth - PaddingX);

        aG.DrawLine(black, lLX, lAxisY, lHX, lAxisY);

        float lMarkX  = lLX ;
        float lMarkTY = lAxisY - lMarkH ;
        float lMarkBY = lAxisY ;
        for ( int i = 0 ; i < lMarks ; ++ i, lMarkX += lMarkWidth )
        {
          aG.DrawLine(black, lMarkX, lMarkTY, lMarkX, lMarkBY);
        }
        aG.DrawLine(black, lHX, lMarkTY, lHX, lMarkBY);

        int lLabels = lMarks / 2 ;
        float lLabelX = lLX ;
        float lLabelY = 4 ;

        float lTotalTime = (float)lEffectiveWidth * (float)mSamplesPerPixel / (float)mSamplingRate ;

        float lTimeMark = 0f ;
        float lTimeStep = lTotalTime / lLabels ;
        for ( int i = 0 ; i <= lLabels ; ++ i, lLabelX += lMarkWidth*2, lTimeMark += lTimeStep )
        {
          string lLabel = TimeMarkToLabel(lTimeMark);
          lTimeLabelSize = aG.MeasureString(lLabel, lFont);
          aG.DrawString(lLabel,lFont, brush, lLabelX - lTimeLabelSize.Width/2, lLabelY);
        }

        return (int)lAxisY ;
      }

      string TimeMarkToLabel ( float aTimeMark )
      {
        if ( aTimeMark <= 120 )
             return $"{aTimeMark:F2}s";
        else return $"{(aTimeMark/120):F2}m";
      }
    }
  
}
