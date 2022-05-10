﻿using System;
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

      foreach( Signal lSegment in lSegments )
      {
        Signal lCurrSignal = lSegment ;

        lCurrSignal.Render();
        foreach( var lModule in mModules ) 
        {
          lCurrSignal = lModule.ProcessSignal( lCurrSignal );
          lCurrSignal.Render();
        }
      }

      return lSignal;
    }

    List<Module> mModules = new List<Module>();  
  }

}