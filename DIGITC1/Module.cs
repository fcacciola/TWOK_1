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
  
  public class ModuleSignature
  {
    public ModuleSignature( params object[] aArguments )
    {
      StringBuilder sb = new StringBuilder();  
      foreach( object arg in aArguments )  
       sb.Append( arg.ToString() + " | " );  
      Value = sb.ToString();
    }

    public string Value { get; private set; } 
  }

  public abstract class Module
  {
    public Signal ProcessSignal ( int aSegmentIdx, int aStep, Signal aInput )
    {
      if ( mResult == null )
        DoProcessSignal ( aSegmentIdx, aStep, aInput );

      ShowResult(aSegmentIdx, aStep) ;

      return mResult;
    }

    public abstract Signal DoProcessSignal ( int aSegmentIdx, int aStep, Signal aInput );

    public virtual void ShowResult ( int aSegmentIdx, int aStep ) { }

    public abstract ModuleSignature GetSignature();

    protected Module () {}

    protected Signal mResult ;

  }

  public abstract class AudioInputModule : Module
  {
    protected AudioInputModule() : base() {}

    public override Signal DoProcessSignal ( int aSegmentIdx, int aStep, Signal aInput )
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

    public override Signal DoProcessSignal ( int aSegmentIdx, int aStep, Signal aInput )
    {
      LexicalSignal lLexicalSignal = aInput as LexicalSignal; 
      if ( lLexicalSignal == null )
       throw new ArgumentException("Input Signal must be a Lexical Signal.");

       return ProcessLexicalSignal(aSegmentIdx, aStep, lLexicalSignal);
    }
    
    protected abstract Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput );  

  }
}
