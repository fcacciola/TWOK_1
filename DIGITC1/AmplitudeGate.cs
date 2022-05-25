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

    public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mThresholds ); }

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

      mResult = aInput.CopyWith(new DiscreteSignal(aInput.SamplingRate, rOutput));

      mResult.Idx              = aStep + 1 ;
      mResult.Name             = "AmplitudeGate";
      mResult.RenderFillColor  = Color.Empty ;
      mResult.RenderLineColor  = Color.FromArgb(128, Color.Green) ;
      mResult.RenderTopLine    = true ;  
      mResult.RenderBottomLine = false ;  

      return mResult ;
    }

    public override void ShowResult ( int aSegmentIdx,  int aStep )
    {
      if ( aSegmentIdx == 0 ) 
      {
        Context.Form.AddRenderModule(mResult.Name);
        mResult.Render();
      }

      Context.Log(aSegmentIdx==0,$"Amplitude Gate:{Environment.NewLine}{mResult}");

    }

    float[] mThresholds;


  }
}
