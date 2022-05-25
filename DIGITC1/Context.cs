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

      if ( ! Directory.Exists(@".\Output") )
        Directory.CreateDirectory(@".\Output");

      mLogFilePath = $".\\Output\\Session_log_{DateTime.Now.ToFileTimeUtc()}.txt";

      File.AppendAllText(mLogFilePath,"DIGITC v1.0" + Environment.NewLine, Encoding.UTF8);
    }

    public static void Create( Form1 aForm )
    {
      mInstance = new Context( aForm );
    }

    static Context mInstance = null ;

    public static Context Instance => mInstance ;

    static public string       InputSample       { get { return Instance.mInputSample    ; } set { Instance.mInputSample    = value ; } }
    static public string       ScriptFile        { get { return Instance.mScriptFile     ; } set { Instance.mScriptFile     = value ; } }
    static public string       BaseScriptContents{ get { return Instance.mScriptContents ; } set { Instance.mScriptContents = value ; } }
    static public WaveSignal   InputSignal       { get { return Instance.mInputSignal    ; } set { Instance.mInputSignal    = value ; } }

    static public ScriptDriver ScriptDriver      { get { return Instance.mScriptDriver ;  } }

    static public Pipeline Pipeline { get { return Instance.mPipeline ; } } 

    static public Form1 Form { get { return Instance.mForm ; } }

    public static bool ShouldRender   ( string aName )                   => Instance._ShouldRender(aName);
    public static void Error          ( string aText )                   => Instance._Error( aText ) ;
    public static void Output         ( string aText )                   => Instance._Output( aText ) ;
    public static void Log            ( string aText )                   => Instance._Log( aText ) ;
    public static void Log            ( bool aDoIt, string aText )       => Instance._Log(aDoIt, aText ) ;
    public static void AddResults     ( string aText )                   => Instance._AddResults( aText ) ;
    public static void LogSessionStart( string aName, string aComments ) => Instance._LogSessionStart(aName, aComments);

    public static void RenderWaveForm   ( DiscreteSignal aSignal, int aIdx, string aName, Color aFillColor, Color aLineColor, int aLineThickness, bool aTopLine, bool aBottomLine ) 
      => Instance._RenderWaveForm( aSignal, aIdx, aName, aFillColor, aLineColor, aLineThickness, aTopLine, aBottomLine ) ;  

    public static Params Params {  get { return Instance.mParams ;} set { Instance.mParams = value ; } }

    void _LogSessionStart( string aName, string aComments )
    {
      using ( StreamWriter lWriter = File.AppendText(mLogFilePath) )  
      {
        lWriter.WriteLine( $"Session {aName } started on Date {DateTime.Now.ToLongDateString()} at Time {DateTime.Now.ToLongTimeString()}");

        if ( aComments != "" )
          lWriter.WriteLine( aComments );
      }
    }

    bool _ShouldRender( string aName ) => mForm.IsRenderModuleChecked(aName);

    void _Log( string aText )
    {
      string lText = aText + Environment.NewLine ;

      mForm.outputBox.SelectionColor = Color.Black ;
      mForm.outputBox.AppendText(lText );

      File.AppendAllText(mLogFilePath, lText ) ;  
    }

    void _Output( string aText )
    {
      string lText = $"{Environment.NewLine}OUTPUT:{Environment.NewLine}[{aText}]{Environment.NewLine}{Environment.NewLine}" ;

      mForm.outputBox.SelectionColor = Color.Blue ;
      //mForm.outputBox.SelectionFont.
      mForm.outputBox.AppendText( lText );

      File.AppendAllText(mLogFilePath, lText ) ;  
    }

    void _Error( string aText )
    {
      string lText = $"{Environment.NewLine}ERROR:{Environment.NewLine}{aText}{Environment.NewLine}" ;
      mForm.outputBox.SelectionColor = Color.Red ;
      mForm.outputBox.AppendText( lText ) ;

      File.AppendAllText(mLogFilePath, lText ) ;  
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

    string       mLogFilePath ;
    string       mInputSample    ;
    string       mScriptFile     ;
    string       mScriptContents ;
    ScriptDriver mScriptDriver   = new ScriptDriver();

    Pipeline mPipeline = new Pipeline();

    public WaveSignal     mInputSignal ;
    public Params         mParams = new Params();
    public readonly Form1 mForm ;  

  }

}
