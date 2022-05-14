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
  using BitsSignal = GenericLexicalSignal<BitSymbol>;

  public class BinarizeByDuration : LexicalModule
  {
    public BinarizeByDuration( double aThreshold ) : base() { mThreshold = aThreshold ; }

     protected override Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput )
     {
       GatedLexicalSignal lGatedInput = aInput as GatedLexicalSignal ;
       if ( lGatedInput == null )
         throw new ArgumentException("Input Signal must be a Gated Lexical Signal");

       List<BitSymbol>   lBits     = new List<BitSymbol>   ();
       List<GatedSymbol> lBitViews = new List<GatedSymbol> ();
             
       double lAccDuration = 0 ;
       lGatedInput.Symbols.ForEach( s => lAccDuration += s.Duration ) ;
       double lAvgDuration = lAccDuration / (double)lGatedInput.Symbols.Count ;

       foreach ( GatedSymbol lGI in lGatedInput.Symbols ) 
       {
         bool lOne = ( lGI.Duration / lAvgDuration ) > mThreshold ;

         GatedSymbol lViewSym = lGI.Clone() as GatedSymbol ;
         lViewSym.Amplitude = lOne ? - lGI.Amplitude * 0.5f : - lGI.Amplitude * 0.2f;
         lBitViews.Add( lViewSym ) ; 

         lBits.Add( new BitSymbol( lBits.Count, lOne, lViewSym )) ;

       }
   
       BitsSignal rSignal = new BitsSignal(lBits);

       rSignal.Idx  = aStep + 1 ;
       rSignal.Name = "DurationBits";

       GatedLexicalSignal lViewGS = new GatedLexicalSignal(lBitViews) ;
       WaveSignal lView = lViewGS.ConvertToWave();
 
       lView.Idx              = aStep + 1 ;
       lView.Name             = "DurationBits";
       lView.RenderFillColor  = Color.Empty ;
       lView.RenderLineColor  = Color.BlueViolet ;
       lView.RenderTopLine    = false ;  
       lView.RenderBottomLine = true ;  

       if ( aSegmentIdx == 0 ) 
       {
         Context.Form.AddRenderModule(lView.Name);
         lView.Render();
       }
 
       //Context.Log(aSegmentIdx==0,$"Duration-based Bits View:{lView}");
       Context.Log($"Duration-based Bits:{rSignal}");
 
       return rSignal ;
    }

    double mThreshold ;
  }
}
