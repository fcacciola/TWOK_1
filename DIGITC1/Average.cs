using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NWaves.Audio;
using NWaves.Operations;
using NWaves.Signals;

namespace DIGITC1
{
  public class Average : AudioInputModule
  {
    public Average( int aWindowSize ) : base() 
    { 
      mWindowSize  = aWindowSize;  
    }

    protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      float[] lAveraged = AverageSamples(aInput.Samples, mWindowSize);

      var lAVS = new DiscreteSignal(aInput.SamplingRate, lAveraged);

      Signal rSignal = aInput.CopyWith(lAVS);

      rSignal.Idx              = aStep + 1 ;
      rSignal.Name             = "Average";
      rSignal.RenderFillColor  = Color.FromArgb(128,Color.DarkBlue);
      rSignal.RenderLineColor  = Color.Empty  ;
      rSignal.RenderTopLine    = false ;  
      rSignal.RenderBottomLine = false ;  


      if ( aSegmentIdx == 0 ) 
      {
        Context.Form.AddRenderModule(rSignal.Name);
        rSignal.Render();
      }

      Context.Log(aSegmentIdx==0,$"Average:{Environment.NewLine}{rSignal}");

      return rSignal ;
    }

    float[] AverageSamples( float[] aInput, int aWindowSize )
    {
      int lLen = aInput.Length ;

      float[] rOutput = new float[lLen];

      for (int i = 0; i < lLen; i++) 
      {
        float lMax = 0f ;

        for (int j = 0; j < aWindowSize; j++) 
        {
          int k = i < j ? lLen - j : i - j ;
          lMax = Math.Max(Math.Abs(aInput[k]), lMax);
        }

        rOutput[i] = lMax ;
      }
      return rOutput ;

    }

    float mAttackTime ;
    float mReleaseTime;
    int   mWindowSize ;

  }
}
