using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.CodeDom.Compiler ;
using Microsoft.CSharp ;

using NWaves.Audio;
using NWaves.Operations;
using NWaves.Signals;

namespace DIGITC1
{
  public abstract class ScriptBase
  {
    public void DoRun()
    {
      try
      { 
        mPipeline = new Pipeline();

        UserCode();

        mPipeline.ProcessSignal(Context.InputSignal);  
      }
      catch (Exception ex)
      {
        Context.Error(ex.ToString());
      }

    }

    public abstract void UserCode();

    protected void print( string aS )
    {
      Context.Log( aS );
    }

    protected void envelope( double AttackTime, double ReleaseTime, int WindowSize )
    {
      mPipeline.AddModule( new EnvelopeModule( (float)AttackTime, (float)ReleaseTime, WindowSize )) ; 
    }

    protected void amplitudeGate( double Threshold )
    {
      mPipeline.AddModule( new AmplitudeGateModule( new float[1]{(float)Threshold} )); 
    }

    protected void extractGatedSymbols( double MinDuration, double MergeGap )
    {
      mPipeline.AddModule( new ExtractGatedlSymbols((float)MinDuration, (float)MergeGap) ); 
    }

    protected void binarizeByDuration( double Threshold )
    {
      mPipeline.AddModule( new BinarizeByDuration(Threshold) ); 
    }

    protected void binaryToBytes( int BitsPerByte, bool LittleEndian )
    {
      mPipeline.AddModule( new BinaryToBytes(BitsPerByte, LittleEndian) ); 
    }

    protected void bytesToText( string CharSet )
    {
      mPipeline.AddModule( new BytesToText(CharSet) ); 

    }

    Pipeline mPipeline = null;

  }


  public class ScriptDriver
  {
    public ScriptDriver() {}

    public void Run( string aUserCode )
    {
      CSharpCodeProvider lProvider   = new CSharpCodeProvider();
      CompilerParameters lParameters = new CompilerParameters();

      lParameters.GenerateExecutable = false ;
      lParameters.GenerateInMemory   = true  ;
      lParameters.ReferencedAssemblies.Add("DIGITC1.exe");
      lParameters.ReferencedAssemblies.Add("nwaves.dll");
      lParameters.IncludeDebugInformation = true ;

      string lBaseScript = Context.BaseScriptContents ;
      string lScript     = lBaseScript.Replace("//<_USER_CODE_HERE>",aUserCode);

      var lResults = lProvider.CompileAssemblyFromSource(lParameters,lScript);

      if ( lResults.Errors.Count > 0 )
      {
        foreach( var lError in lResults.Errors )
          Context.Error(lError.ToString());
      }
      else
      {
        var lCVSType = lResults.CompiledAssembly.GetType("DIGITC1.Script",true);

        var lCVSRunMethod = lCVSType.GetMethod( "Run" ) ;

        lCVSRunMethod.Invoke( null, new object[]{} ) ;
      }
    }
  }

}
