// takes absolute path to input file and output file
// input file needs one line formatted of bytes
// example: 12-34-56-78-90-AB-CD-EF
Cocona.CoconaLiteApp.Run((string inputFile, string outputFile) =>
{
 IEnumerable<byte> CastFromString(string input)
 {
  var split = input.Split('-');
  var output = new byte[split.Length];
  for (var i = 0; i < split.Length; i++)
  {
   output[i] = byte.Parse(split[i], System.Globalization.NumberStyles.HexNumber);
  }

  return output;
 }

 var inputLine = File.ReadAllLines(inputFile).First();
 var bytes = CastFromString(inputLine);
 
 File.WriteAllBytes(outputFile, bytes.ToArray());
});