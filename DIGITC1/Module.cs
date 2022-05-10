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
  public abstract class Module
  {
    public abstract Signal ProcessSignal ( Signal aInput );

    protected Module () {}
  }

  public abstract class AudioInputModule : Module
  {
    protected AudioInputModule() : base() {}

    public override Signal ProcessSignal ( Signal aInput )
    {
      WaveSignal lWaveSignal = aInput as WaveSignal; 
      if ( lWaveSignal == null )
       throw new ArgumentException("Input Signal must be an Audio Signal.");

       return ProcessAudioSignal(lWaveSignal);
    }
    
    protected abstract Signal ProcessAudioSignal ( WaveSignal aInput );  

  }
}
