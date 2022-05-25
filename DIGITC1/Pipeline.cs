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

    public void Clear()
    {
      mModulesCache.Clear();
    }

    public void Start()
    {
      mModules.Clear();
    }

    public void AddModule( Module aModule )
    { 
      ModuleSignature lSignature = aModule.GetSignature();
      if ( ! mModulesCache.ContainsKey(lSignature.Value) )
      {
        mModulesCache.Add(lSignature.Value, aModule);
        mModules.Add(aModule);
      }
      else
      {
        mModules.Add(mModulesCache[lSignature.Value]);
      }
    }

    public Signal ProcessSignal ( Signal aInput )
    {
      List<Signal> lResults = new List<Signal>();
      List<Signal> lSegments = aInput.Segment(Context.Params.WindowSizeInSeconds);

      lSegments[0].Render();

      Context.Log($"Sound Signal: {lSegments[0]}");

      int lSegmentIdx = 0 ;

      foreach( Signal lSegment in lSegments )
      {
        Signal lCurrSignal = lSegment ;

        int lStep = 0 ;

        foreach( var lModule in mModules ) 
        {
          lCurrSignal = lModule.ProcessSignal( lSegmentIdx, lStep ++ , lCurrSignal );
        }

        lResults.Add( lCurrSignal );  

        lSegmentIdx ++ ;
      }

      Signal lResult = Signal.Merge(lResults) ; 

      Context.Output($"Output:{Environment.NewLine}{lResult}");

      return lResult;
    }

    List<Module> mModules = new List<Module>();  

    Dictionary<string,Module> mModulesCache = new Dictionary<string, Module>();
  }

}
