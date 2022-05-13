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
  public class EnvelopeParams
  {
    public float AttackTime { get; set; } =  0.3f ;
    public float ReleaseTime{ get; set; } =  0.3f ;
    public int   WindowSize { get; set; } =  100 ;
  }

  public class EnvelopeModule : AudioInputModule
  {
    public EnvelopeModule( float aAttackTime, float aReleaseTime, int aWindowSize ) : base() 
    { 
      mAttackTime  = aAttackTime;  
      mReleaseTime = aReleaseTime;  
      mWindowSize  = aWindowSize;  
    }

    protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      float[] lAveraged = Average(aInput.Samples, mWindowSize);

      var lAVS = new DiscreteSignal(aInput.SamplingRate, lAveraged);

      var lES = Operation.Envelope(lAVS, mAttackTime, mReleaseTime);

      Signal rSignal = aInput.CopyWith(lES);

      rSignal.Idx              = aStep + 1 ;
      rSignal.Name             = "Envelope";
      rSignal.RenderFillColor  = Color.Empty ;
      rSignal.RenderLineColor  = Color.Red ;
      rSignal.RenderTopLine    = true ;  
      rSignal.RenderBottomLine = false ;  


      if ( aSegmentIdx == 0 ) 
      {
        Context.Form.AddRenderModule(rSignal.Name);
        rSignal.Render();
      }

      Context.Log(aSegmentIdx==0,$"Envelope:{Environment.NewLine}{rSignal}");

      return rSignal ;
    }

    float[] Average( float[] aInput, int aWindowSize )
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
