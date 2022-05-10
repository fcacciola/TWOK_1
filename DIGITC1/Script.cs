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

  public abstract class Experiment
  {
    public Experiment( Form1 aForm, string aAudioFile )
    {
      mForm      = aForm;
      mAudioFile = aAudioFile;  
      mPipeline  = new Pipeline();
    }

    protected void AddModule( Module aModule ) { mPipeline.AddModule(aModule) ; }

    readonly protected Form1   mForm ;
    readonly protected string  mAudioFile ;

    public abstract void Process() ;

    protected Pipeline mPipeline = null;

  }

  public class TWOK1 : Experiment
  {
    public TWOK1( Form1 aForm, string aAudioFile ) : base(aForm, aAudioFile) 
    {
    }

    public override void Process()
    {
      var lASource = new FileAudioSource(mAudioFile);
      var lASignal = lASource.CreateSignal();

      
      AddModule( new EnvelopeModule(Context.Params.EnvelopeParams) ) ;

      mPipeline.ProcessSignal( lASignal );  
    }

  }
}
