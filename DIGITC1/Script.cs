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
      mPipeline = new Pipeline();

      UserCode();

      mPipeline.ProcessSignal(Context.InputSignal);  

    }

    public abstract void UserCode();

    protected void print( string s )
    {
      Context.Log( s );
    }

    protected void envelope( float art )
    {
      mPipeline.AddModule( new EnvelopeModule( new EnvelopeParams(){ AttackTime = art, ReleaseTime = art })); 
    }

    Pipeline mPipeline = null;

  }


  public class ScriptDriver
  {
    public ScriptDriver() {}

    public void Run( string aScriptFile )
    {
      CSharpCodeProvider lProvider   = new CSharpCodeProvider();
      CompilerParameters lParameters = new CompilerParameters();

      lParameters.GenerateExecutable = false ;
      lParameters.GenerateInMemory   = true  ;
      lParameters.ReferencedAssemblies.Add("DIGITC1.exe");
      lParameters.ReferencedAssemblies.Add("nwaves.dll");
      lParameters.IncludeDebugInformation = true ;

      string lUserCode   = File.ReadAllText(aScriptFile) ;
      string lBaseScript = Context.BaseScriptContents ;
      string lScript     = lBaseScript.Replace("//<_USER_CODE_HERE>",lUserCode);

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
