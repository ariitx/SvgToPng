using System;
using System.IO;
using SkiaSharp;
using Svg.Skia;

string svgPath = "";   // Path to your SVG file
string pngPath = "";  // Output PNG file
int width = 500;                // Desired width
int height = 500;               // Desired height

if (ParseCommandLineArgs(MyArgs.SvgPath))
{
    var val = ParseCommandLineArgsValue(MyArgs.SvgPath);
    if (!string.IsNullOrEmpty(val)) svgPath = val;
}

if (ParseCommandLineArgs(MyArgs.PngPath))
{
    var val = ParseCommandLineArgsValue(MyArgs.PngPath);
    if (!string.IsNullOrEmpty(val)) pngPath = val;
}

if (ParseCommandLineArgs(MyArgs.Width))
{
    var val = ParseCommandLineArgsValue(MyArgs.Width);
    if (!string.IsNullOrEmpty(val)) int.TryParse(val, out width);
}

if (ParseCommandLineArgs(MyArgs.Height))
{
    var val = ParseCommandLineArgsValue(MyArgs.Height);
    if (!string.IsNullOrEmpty(val)) int.TryParse(val, out height);
}

if (string.IsNullOrEmpty(svgPath))
{
    Console.WriteLine("svgPath is missing.");
}
else if (string.IsNullOrEmpty(pngPath))
{
    Console.WriteLine("pngPath is missing.");
}
else if (width <= 0)
{
    Console.WriteLine("width is invalid.");
}
else if (height <= 0)
{
    Console.WriteLine("height is invalid.");
}
else
{
    ConvertSvgToPng(svgPath, pngPath, width, height);
    Console.WriteLine("Conversion completed!");
}



static void ConvertSvgToPng(string svgPath, string pngPath, int width, int height)
{
    if (!File.Exists(svgPath))
    {
        Console.WriteLine("SVG file not found!");
        return;
    }

    // Load SVG
    var svg = new SKSvg();
    svg.Load(svgPath);

    // Create a new bitmap
    using var bitmap = new SKBitmap(width, height);
    using var canvas = new SKCanvas(bitmap);

    // Scale the SVG to fit within the bitmap
    float scaleX = width / svg.Picture.CullRect.Width;
    float scaleY = height / svg.Picture.CullRect.Height;
    float scale = Math.Min(scaleX, scaleY);

    canvas.Clear(SKColors.Transparent); // Set background transparent
    canvas.Scale(scale);
    canvas.DrawPicture(svg.Picture);
        
    // Save as PNG
    using var img = SKImage.FromBitmap(bitmap);
    using var data = img.Encode(SKEncodedImageFormat.Png, 100);
    File.WriteAllBytes(pngPath, data.ToArray());
}


static List<string> GetCommandLineArgs()
{
    var args = Environment.GetCommandLineArgs().Skip(1).ToList();
    // args.Add("-" + MyArgs.SvgPath.ToString());
    // args.Add(@"ss/appiconfg.svg");
    // args.Add("-" + MyArgs.PngPath.ToString());
    // args.Add("ss/appiconfg.png");

    return args;
}
static bool ParseCommandLineArgs(MyArgs arg)
{
    try
    {
        var CommandLineArgs = GetCommandLineArgs();
        bool found = false;
        foreach (string s in CommandLineArgs)
        {
            if (s.StartsWith("-"))
            {
                found = s.ToString().Replace("-", "").Replace(" ", "").ToString().ToLower().Trim() == arg.ToString().ToLower();
                if (found)
                    return true;
            }
        }
    }
    catch (Exception)
    {
    }
    return false;
}

static string ParseCommandLineArgsValue(MyArgs arg)
{
    System.Text.StringBuilder result = new System.Text.StringBuilder();
    try
    {
        var CommandLineArgs = GetCommandLineArgs();
        for (int i = 0; i < CommandLineArgs.Count; i++)
        {
            var s = CommandLineArgs[i];

            if (s.StartsWith("-"))
            {
                var found = s.ToString().Replace("-", "").Replace(" ", "").ToString().ToLower().Trim() == arg.ToString().ToLower();
                if (found && i + 1 <= CommandLineArgs.Count)
                {
                    //argument value must not starts with "-", "/" or "\" (reserverd for main argument only)
                    var val1 = CommandLineArgs[i + 1];
                    if (!val1.StartsWith("-"))
                    {
                        result.Append(val1);
                        break;
                    }
                }
            }
        }
    }
    catch (Exception)
    {
    }

    return result.ToString();
}

enum MyArgs
{
    SvgPath,
    PngPath,
    Width,
    Height,
}