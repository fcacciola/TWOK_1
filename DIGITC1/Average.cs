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
    public Average( int aWindowSize, int aIterations ) : base() 
    { 
      mWindowSize  = aWindowSize;  
      mIterations  = aIterations;  
    }

    public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mWindowSize, mIterations ); }

    protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      float[] lAveraged = AverageSamples(aInput.Samples);

      var lAVS = new DiscreteSignal(aInput.SamplingRate, lAveraged);

      mResult = aInput.CopyWith(lAVS);

      mResult.Idx              = aStep + 1 ;
      mResult.Name             = "Average";
      mResult.RenderFillColor  = Color.FromArgb(128,Color.DarkBlue);
      mResult.RenderLineColor  = Color.Empty  ;
      mResult.RenderTopLine    = false ;  
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

      Context.Log(aSegmentIdx==0,$"Average:{Environment.NewLine}{mResult}");
    }

    float[] AverageSamples( float[] aSamples)
    {
      int lLen = aSamples.Length ;

      float[] lInput  = new float[lLen];
      aSamples.CopyTo(lInput, 0);

      float[] rOutput = new float[lLen];

      for ( int c = 0 ; c < mIterations ; c++ )
      {
        for (int i = 0; i < lLen; i++) 
        {
          float lMax = 0f ;

          for (int j = 0; j < mWindowSize; j++) 
          {
            int k = i < j ? lLen - j : i - j ;
            lMax = Math.Max(Math.Abs(lInput[k]), lMax);
          }

          rOutput[i] = lMax ;
        }
    
        if ( c - 1 < mIterations)
          rOutput.CopyTo(lInput, 0);
      }

      return rOutput ;

    }

    int mWindowSize ;
    int mIterations ;


  }
}
