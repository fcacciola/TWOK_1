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
  using BitsSignal  = GenericLexicalSignal<BitSymbol>;
  using BytesSignal = GenericLexicalSignal<ByteSymbol>;

  public class BinaryToBytes : LexicalModule
  {
    public BinaryToBytes( int aBitsPerByte, bool aLittleEndian ) : base() { mLittleEndian = aLittleEndian ; mBitsPerByte = aBitsPerByte ; }

     protected override Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput )
     {
       BitsSignal lBinaryInput = aInput as BitsSignal ;
       if ( lBinaryInput == null )
         throw new ArgumentException("Input Signal must be a Binary Signal");

      List<bool> lBitValues  = new List<bool>(); 

      int lLen = lBinaryInput.Symbols.Count ;
      int lByteCount = 0; 
      int i = 0;

      do
      { 
        int lRem = lLen - i ;

        if ( mLittleEndian )
        {
          if (  mBitsPerByte < 8 )
            for( int k = mBitsPerByte ; k < 8 ; k++ ) 
              lBitValues.Add( false ) ;

          if ( lRem < mBitsPerByte )
            for( int k = lRem ; k < mBitsPerByte ; k++ ) 
              lBitValues.Add( false ) ;
        }

        for ( int j = 0 ; i < lLen && j < mBitsPerByte ; j++, i ++ )
          lBitValues.Add( lBinaryInput.Symbols[i].One ) ;  

        //if ( !mLittleEndian && mBitsPerByte < 8 )
        //  for( int k = mBitsPerByte ; k < 8 ; k++ ) 
        //    lBitValues.Add( false ) ;

        lByteCount ++ ;
      }
      while ( i < lLen ) ;

      BitArray lBits = new BitArray(lBitValues.ToArray());
        
      byte[] lBytes = new byte[lByteCount]; 
      lBits.CopyTo( lBytes, 0 ) ;

      List<ByteSymbol> lByteSymbols = new List<ByteSymbol>();

      foreach( byte lByte in lBytes )
        lByteSymbols.Add( new ByteSymbol(lByteSymbols.Count, lByte ) ) ;

      BytesSignal rSignal = new BytesSignal(lByteSymbols);

      rSignal.Idx  = aStep + 1 ;
      rSignal.Name = "Bytes";

      //WaveSignal lView = rSignal.ConvertToWave();
 
      //lView.Idx              = aStep + 1 ;
      //lView.Name             = "DurationBits";
      //lView.RenderFillColor  = Color.Empty ;
      //lView.RenderLineColor  = Color.Black ;
      //lView.RenderTopLine    = true ;  
      //lView.RenderBottomLine = false ;  
      //lView.Render();
 
      //Context.Log(aSegmentIdx==0,$"Duration-based Bits View:{lView}");
      Context.Log($"Bytes Bits:{rSignal}");
 
       return rSignal ;
    }

    bool mLittleEndian ;
    int  mBitsPerByte ;
  }
}
