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
  public class EnvelopeModule : AudioInputModule
  {
    public EnvelopeModule( EnvelopeParams aParams ) : base() { mParams = aParams ; }

    protected override Signal ProcessAudioSignal ( WaveSignal aInput )
    {
      Context.Log(Context.LogThisSegment(aInput),"Envelope...");
      Context.Log(Context.LogThisSegment(aInput),"Input Audio Signal...");
      Context.Log(Context.LogThisSegment(aInput),$"{aInput}");

      var lIn = aInput.Rep.Copy(); 
      lIn.FullRectify();

      var lES = Operation.Envelope(lIn, mParams.AttackTime, mParams.ReleaseTime);

      Signal rSignal = new WaveSignal(lES);

      rSignal.Idx                 = 1 ;
      rSignal.Name                = "Envelope";
      rSignal.RenderLineColor     = Color.Red ;
      rSignal.RenderTopLine       = true ;  

      Context.Log(Context.LogThisSegment(aInput),"Envelope...");
      Context.Log(Context.LogThisSegment(aInput),$"{rSignal}");

      return rSignal ;
    }

    EnvelopeParams mParams ;

  }
}
