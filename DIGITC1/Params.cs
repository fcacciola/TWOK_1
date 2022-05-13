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
  public class Params
  {
    public string SamplesFolder       { get ; set ; }
    public string ScriptsFolder       { get ; set ; }
    public float  WindowSizeInSeconds { get ; set ; } = 0f ;
    public int    SegmentIdxToRender  { get ; set ; } = 0 ;
    public int    SegmentIdxToLog     { get ; set ; } = 0 ;
  }
}
