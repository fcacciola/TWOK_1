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
  public class AmplitudeGateParams
  {
    public float[] Thresholds { get; set; } 
  }
  
  public class AmplitudeGateModule : AudioInputModule
  {
    public AmplitudeGateModule( AmplitudeGateParams aParams ) : base() { mParams = aParams ; }

    protected override Signal ProcessAudioSignal ( WaveSignal aInput )
    {
      float lMax = aInput.ComputeMax();
      
      float[] lSrc = aInput.Rep.Samples ;
      int lLen = lSrc.Length ;

      float[] rOutput = new float[lLen];

      for ( int i = 0 ; i < lLen ; i++ )  
      {
        float lOut = lSrc[i] / lMax ;
        lOut = lOut > mParams.Thresholds[0] ? 1.0f : 0 ;
        lOut = lOut * lMax ;
        rOutput[i] = lOut ;  
      }

      Signal rSignal = aInput.CopyWith(new DiscreteSignal(aInput.Rep.SamplingRate, rOutput));

      rSignal.Idx              = 2 ;
      rSignal.Name             = "AmplitudeGate";
      rSignal.RenderFillColor  = Color.Empty ;
      rSignal.RenderLineColor  = Color.Black ;
      rSignal.RenderTopLine    = true ;  
      rSignal.RenderBottomLine = false ;  
      rSignal.Render();

      //Context.Log(Context.LogThisSegment(aInput),$"Amplitude Gate:{Environment.NewLine}{rSignal}");

      return rSignal ;
    }

    AmplitudeGateParams mParams ;

  }
}
