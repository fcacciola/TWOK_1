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
 
  public class AmplitudeGate : AudioInputModule
  {
    public AmplitudeGate( float[] aThresholds ) : base() { mThresholds = aThresholds ; }

    protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      float lMax = aInput.ComputeMax();
      
      float[] lSrc = aInput.Samples ;
      int lLen = lSrc.Length ;

      float[] rOutput = new float[lLen];

      for ( int i = 0 ; i < lLen ; i++ )  
      {
        float lOut = lSrc[i] / lMax ;
        lOut = lOut > mThresholds[0] ? 1.0f : 0 ;
        lOut = lOut * lMax ;
        rOutput[i] = lOut ;  
      }

      Signal rSignal = aInput.CopyWith(new DiscreteSignal(aInput.SamplingRate, rOutput));

      rSignal.Idx              = aStep + 1 ;
      rSignal.Name             = "AmplitudeGate";
      rSignal.RenderFillColor  = Color.Empty ;
      rSignal.RenderLineColor  = Color.FromArgb(128, Color.Green) ;
      rSignal.RenderTopLine    = true ;  
      rSignal.RenderBottomLine = false ;  

      if ( aSegmentIdx == 0 ) 
      {
        Context.Form.AddRenderModule(rSignal.Name);
        rSignal.Render();
      }

      Context.Log(aSegmentIdx==0,$"Amplitude Gate:{Environment.NewLine}{rSignal}");

      return rSignal ;
    }

    float[] mThresholds;

  }
}
