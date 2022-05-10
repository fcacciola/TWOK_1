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
  public abstract class AudioSource
  {
    protected AudioSource() {}

    public abstract WaveSignal CreateSignal();  
  }

  public class FileAudioSource : AudioSource
  {
    public FileAudioSource( string aFilename ) : base( )
    {
      mFilename = aFilename;
    }

    public override WaveSignal CreateSignal()
    {
      WaveSignal rSignal = null ;

      using (var stream = new FileStream(mFilename, FileMode.Open))
      {
        var waveContainer = new WaveFile(stream);
        rSignal = new WaveSignal(waveContainer[Channels.Average]);

        rSignal.Idx             = 0 ;
        rSignal.Name            = "AudioSource";
        rSignal.RenderFillColor = Color.FromArgb(128,Color.Blue);
      }

      return rSignal ;
    }
    
    readonly string mFilename ;
  }

}
