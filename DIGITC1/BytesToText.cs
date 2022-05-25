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

     public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mCharSet); }

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
  
       mResult = new TextSignal(lTextSymbols);

       mResult.Idx  = aStep + 1 ;
       mResult.Name = "Text";

       return mResult ;
    }

    string mCharSet ;
  }
}
