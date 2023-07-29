// cocona lite app
// take these parameters:
// --inputFile="C:/Users/.../log.txt"
// --outputFile="C:/Users/.../test.bin"

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Cocona;

CoconaLiteApp.Run((string inputFile, string outputFile) =>
{
 var line = File.ReadAllLines(inputFile).First();
 var bytes = Snasen.CastFromString(line);
 
 File.WriteAllBytes(outputFile, bytes.ToArray());
});

static class Snasen
{
 // 12-34-56-78-90-AB-CD-EF to byte array
 public static byte[] CastFromString(string input)
 { 
  var split = input.Split('-');
  var output = new byte[split.Length];
  for (var i = 0; i < split.Length; i++)
  {
   output[i] = byte.Parse(split[i], System.Globalization.NumberStyles.HexNumber);
  }
  return output;
 }   
}