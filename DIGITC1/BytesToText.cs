using System;
using System.Collections;
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
  using BytesSignal = GenericLexicalSignal<ByteSymbol>;
  using TextSignal  = GenericLexicalSignal<TextSymbol>;

  public class BytesToText : LexicalModule
  {
    public BytesToText( string aCharSet ) : base() { mCharSet = aCharSet ; }

     protected override Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput )
     {
       BytesSignal lBytesInput = aInput as BytesSignal ;
       if ( lBytesInput == null )
         throw new ArgumentException("Input Signal must be a Binary Signal");

       Encoding lEncoding = Encoding.GetEncoding(mCharSet);

       List<TextSymbol> lTextSymbols = new List<TextSymbol> ();

       byte[] lBuffer = new byte[1];

       foreach( var lByteSymbol in lBytesInput.Symbols )
       {
         lBuffer[0] = lByteSymbol.Byte; 
         string lText = lEncoding.GetString(lBuffer);
         lTextSymbols.Add( new TextSymbol(lTextSymbols.Count, lText ) );
       }
  
       TextSignal rSignal = new TextSignal(lTextSymbols);

       rSignal.Idx  = aStep + 1 ;
       rSignal.Name = "Text";

       //WaveSignal lView = rSignal.ConvertToWave();
 
       //lView.Idx              = aStep + 1 ;
       //lView.Name             = "DurationBits";
       //lView.RenderFillColor  = Color.Empty ;
       //lView.RenderLineColor  = Color.Black ;
       //lView.RenderTopLine    = true ;  
       //lView.RenderBottomLine = false ;  
       //lView.Render();
 
       //Context.Log(aSegmentIdx==0,$"Duration-based Bits View:{lView}");

       return rSignal ;
    }

    string mCharSet ;
  }
}
