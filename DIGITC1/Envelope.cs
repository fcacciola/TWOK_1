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

    protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      var lES = Operation.Envelope(aInput.Rep, mAttackTime, mReleaseTime);

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

    float mAttackTime ;
    float mReleaseTime;

  }
}
