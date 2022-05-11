using System;
using System.Collections.Generic;
using NWaves.Audio;
using NWaves.Operations;
using NWaves.Signals;

namespace DIGITC1
{

public class Script : ScriptBase
{
  public static void Run()
  {
    Script script = new Script();
    script.DoRun(); 
  }

  public override void UserCode()
  {
    //<_USER_CODE_HERE>
  }

  //void envelope( float art )
  //{
  //  mPipeline.AddModule( new EnvelopeModule( new EnvelopeParams(){ AttackTime = art, ReleaseTime = art })); 
  //}

  //Context  mContext ;
  //Pipeline mPipeline = null;

}

}