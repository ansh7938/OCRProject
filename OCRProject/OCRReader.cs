using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace OCRProject
{
	public class OCRReader
	{
		public async Task<OcrResult> asdf(Stream Stream)
		{
			if (Stream == null)
			{
				throw new Exception("Stream is null.");
			}

			var Language = new Language("en-US");

			if (!OcrEngine.IsLanguageSupported(Language))
			{
				throw new Exception($"{ Language.LanguageTag } is not supported in this system.");
			}

			var Decoder = await BitmapDecoder.CreateAsync(Stream.AsRandomAccessStream());
			var Bitmap = await Decoder.GetSoftwareBitmapAsync();

			var Engine = OcrEngine.TryCreateFromLanguage(Language);
			return await Engine.RecognizeAsync(Bitmap);
		}
	}
}
