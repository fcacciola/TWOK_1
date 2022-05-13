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
    public abstract Signal ProcessSignal ( int aSegmentIdx, int aStep, Signal aInput );

    protected Module () {}
  }

  public abstract class AudioInputModule : Module
  {
    protected AudioInputModule() : base() {}

    public override Signal ProcessSignal ( int aSegmentIdx, int aStep, Signal aInput )
    {
      WaveSignal lWaveSignal = aInput as WaveSignal; 
      if ( lWaveSignal == null )
       throw new ArgumentException("Input Signal must be an Audio Signal.");

       return ProcessAudioSignal(aSegmentIdx, aStep, lWaveSignal);
    }
    
    protected abstract Signal ProcessAudioSignal ( int aSegmentIdx, int aStep, WaveSignal aInput );  

  }

  public abstract class LexicalModule : Module
  {
    protected LexicalModule() : base() {}

    public override Signal ProcessSignal ( int aSegmentIdx, int aStep, Signal aInput )
    {
      LexicalSignal lLexicalSignal = aInput as LexicalSignal; 
      if ( lLexicalSignal == null )
       throw new ArgumentException("Input Signal must be a Lexical Signal.");

       return ProcessLexicalSignal(aSegmentIdx, aStep, lLexicalSignal);
    }
    
    protected abstract Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput );  

  }
}
