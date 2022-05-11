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
  public class Pipeline
  {
    public Pipeline() {}  

    public void AddModule( Module aModule )
    { 
      mModules.Add( aModule ); 
    }

    public Signal ProcessSignal ( Signal aInput )
    {
      Signal lSignal = aInput ;

      List<Signal> lSegments = aInput.Segment(Context.Params.WindowSizeInSeconds);

      lSegments[0].Render();

      Context.Log(Context.LogThisSegment(lSegments[0]),$"Sound Signal: {lSegments[0]}");

      foreach( Signal lSegment in lSegments )
      {
        Signal lCurrSignal = lSegment ;

        foreach( var lModule in mModules ) 
        {
          lCurrSignal = lModule.ProcessSignal( lCurrSignal );
        }
      }

      return lSignal;
    }

    List<Module> mModules = new List<Module>();  
  }

}
