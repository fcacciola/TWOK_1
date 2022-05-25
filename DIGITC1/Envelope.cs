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

  public class Envelope : AudioInputModule
  {
    public Envelope( float aAttackTime, float aReleaseTime ) : base() 
    { 
      mAttackTime  = aAttackTime;  
      mReleaseTime = aReleaseTime;  
    }

    public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mAttackTime, mReleaseTime); }

    protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      var lES = Operation.Envelope(aInput.Rep, mAttackTime, mReleaseTime);

      mResult = aInput.CopyWith(lES);

      mResult.Idx              = aStep + 1 ;
      mResult.Name             = "Envelope";
      mResult.RenderFillColor  = Color.Empty ;
      mResult.RenderLineColor  = Color.Red ;
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

      Context.Log(aSegmentIdx==0,$"Envelope:{Environment.NewLine}{mResult}");
    }

    float mAttackTime ;
    float mReleaseTime;

  }
}
