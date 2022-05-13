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

  public class BinarizeByDuration : LexicalModule
  {
    public BinarizeByDuration( double aThreshold ) : base() { mThreshold = aThreshold ; }

     protected override Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput )
     {
       GatedLexicalSignal lGatedInput = aInput as GatedLexicalSignal ;
       if ( lGatedInput == null )
         throw new ArgumentException("Input Signal must be a Gated Lexical Signal");

       List<BitSymbol> lBits = new List<BitSymbol> ();
             
       double lAccDuration = 0 ;
       lGatedInput.Symbols.ForEach( s => lAccDuration += s.Duration ) ;
       double lAvgDuration = lAccDuration / (double)lGatedInput.Symbols.Count ;

       lGatedInput.Symbols.ForEach( s => lBits.Add( new BitSymbol( lBits.Count, ( s.Duration / lAvgDuration ) > mThreshold )) ) ;
   
       BinarySignal rSignal = new BinarySignal(lBits);

       rSignal.Idx  = aStep + 1 ;
       rSignal.Name = "DurationBits";

       //WaveSignal lView = rSignal.ConvertToWave();
 
       //lView.Idx              = aStep + 1 ;
       //lView.Name             = "DurationBits";
       //lView.RenderFillColor  = Color.Empty ;
       //lView.RenderLineColor  = Color.Black ;
       //lView.RenderTopLine    = true ;  
       //lView.RenderBottomLine = false ;  
       //lView.Render();
 
       //Context.Log(aSegmentIdx==0,$"Duration-based Bits View:{lView}");
       Context.Log($"Duration-based Bits:{rSignal}");
 
       return rSignal ;
    }

    double mThreshold ;
  }
}
