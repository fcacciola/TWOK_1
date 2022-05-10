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
  public class Context
  {
    Context( Form1 aForm )
    {
      mForm = aForm;  

      mForm.signalPlot1.Gain   = 100 ;
      mForm.signalPlot1.Stride = 150 ;
    }

    public static void Create( Form1 aForm )
    {
      mInstance = new Context( aForm );
    }

    static Context mInstance = null ;

    public static Context Instance => mInstance ;

    public static void Log              ( string aText )             => Instance._Log( aText ) ;
    public static void Log              ( bool aDoIt, string aText ) => Instance._Log(aDoIt, aText ) ;
    public static void AddResults       ( string aText )             => Instance._AddResults( aText ) ;
    public static bool LogThisSegment   ( Signal aSignal )           => Instance._LogThisSegment( aSignal ) ;  
    public static bool RenderThisSegment( Signal aSignal )           => Instance._RenderThisSegment( aSignal ) ;

    public static void RenderWaveForm   ( DiscreteSignal aSignal, int aIdx, string aName, Color aFillColor, Color aLineColor, int aLineThickness, bool aTopLine, bool aBottomLine ) 
      => Instance._RenderWaveForm( aSignal, aIdx, aName, aFillColor, aLineColor, aLineThickness, aTopLine, aBottomLine ) ;  

    public static Params Params {  get { return Instance.mParams ;} set { Instance.mParams = value ; } }

    void _Log( string aText )
    {
      mForm.outputBox.AppendText( aText + Environment.NewLine);
    }

    void _Log( bool aDoIt, string aText )
    {
      if ( aDoIt)
       Log(aText);
    }

    void _AddResults( string aText )
    {
      mForm.outputBox.AppendText( aText + Environment.NewLine);
    }

    void _RenderWaveForm( DiscreteSignal aSignal, int aIdx, string aName, Color aFillColor, Color aLineColor, int aLineThickness, bool aTopLine, bool aBottomLine  )
    {
      mForm.signalPlot1.SetLayer(aName, aIdx, aSignal, aFillColor, aLineColor, aLineThickness, aTopLine, aBottomLine);
    }

    bool _LogThisSegment( Signal aSignal )
    {
      return ( mParams.SegmentIdxToLog == -1 || mParams.SegmentIdxToLog == aSignal.SegmentIdx );
    }

    bool _RenderThisSegment( Signal aSignal )
    {
      return ( mParams.SegmentIdxToLog == -1 || mParams.SegmentIdxToLog == aSignal.SegmentIdx );
    }

    Params mParams = new Params();

    readonly Form1 mForm ;  

  }

}
