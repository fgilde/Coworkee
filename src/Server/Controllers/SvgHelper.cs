using System.IO;
using Nextended.Core.Extensions;
using SkiaSharp;
using Svg;
using Svg.Skia;

namespace Coworkee.Server.Controllers;

public class Svg
{
	private readonly SvgDocument _svgDocument;

	public Svg(string svg)
	{
		_svgDocument = SvgDocument.FromSvg<SvgDocument>(svg);
	}

	public byte[] ToByteArray()
	{
		var skSvg = new SKSvg();
		skSvg.FromSvgDocument(_svgDocument);

		var stream = new MemoryStream();
		skSvg.Save(stream, SKColor.Empty);
		stream.Position = 0;
		stream.Flush();
		return stream.ToByteArray();
	}

	public int Height
	{
		get => (int) _svgDocument.Height.Value;
        set => _svgDocument.Height = value;
    }

	public int Width
	{
		get => (int) _svgDocument.Width.Value;
        set => _svgDocument.Width = value;
    }
    public static Svg FromSvg(string svg)
    {
        return new Svg(svg);
    }
}