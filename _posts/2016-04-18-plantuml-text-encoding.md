---
layout: post
title:  "PlantUml Text Encoding"
date:   2016-04-18
tags:   dotnet linqpad plantuml
---

A lot of UML I find cumbersome and not terribly helpful. However, when dealing with a large data model, a class diagram can be really helpful. A key problem though is the maintenance overhead, so I went hunting for a class diagram format that I could automate in our build and would look good in a source control history. I stumbled upon [PlantUML](http://www.plantuml.com), text-based definitions for UML diagrams. Perfect.

PlantUML also provide a [Web Service](http://plantuml.com/server.html) that can be called to generate SVG/PNG from the text-based definition. As with so many things, there's a catch. Custom encoding. Fortunately it's a fairly simple port over to the C# code I needed and here it is. Might be useful to someone...

Available on [GitHub](https://gist.github.com/incongruousm/509ef8820532883f9899b6e980ef6503)

```csharp
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PlantUml.Utils
{
   public class PlantUmlTextEncoder
   {
      public string Encode(TextReader reader)
      {
         using (var output = new MemoryStream())
         {
            using (var writer = new StreamWriter(new DeflateStream(output, CompressionLevel.Optimal), Encoding.UTF8))
               writer.Write(reader.ReadToEnd());
            return Encode(output.ToArray());
         }
      }
      
      private static string Encode(IReadOnlyList<byte> bytes)
      {
         var length = bytes.Count;
         var s = new StringBuilder();
         for (var i = 0; i < length; i += 3)
         {
            var b1 = bytes[i];
            var b2 = i + 1 <= length ? bytes[i + 1] : (byte) 0;
            var b3 = i + 2 <= length ? bytes[i + 2] : (byte) 0;
            s.Append(Append3Bytes(b1, b2, b3));
         }
         return s.ToString();
      }
      
      private static char[] Append3Bytes(byte b1, byte b2, byte b3)
      {
         var c1 = b1 >> 2;
         var c2 = (b1 & 0x3) << 4 | b2 >> 4;
         var c3 = (b2 & 0xF) << 2 | b3 >> 6;
         var c4 = b3 & 0x3F;
         return new[]
         {
            EncodeByte((byte) (c1 & 0x3F)),
            EncodeByte((byte) (c2 & 0x3F)),
            EncodeByte((byte) (c3 & 0x3F)),
            EncodeByte((byte) (c4 & 0x3F))
         };
      }
      
      private static char EncodeByte(byte b)
      {
         var ascii = Encoding.ASCII;
         if (b < 10)
            return ascii.GetChars(new[] {(byte) (48 + b)})[0];
         b -= 10;
         if (b < 26)
            return ascii.GetChars(new[] {(byte) (65 + b)})[0];
         b -= 26;
         if (b < 26)
            return ascii.GetChars(new[] {(byte) (97 + b)})[0];
         b -= 26;
         if (b == 0)
            return '-';
         if (b == 1)
            return '_';
         return '?';
      }
   }
}
```