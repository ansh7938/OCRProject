using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ReactiveUI;
using Windows.Media.Ocr;

namespace OCRProject.ViewModel
{
	public class MainViewModel : ReactiveObject
	{
		private string _SearchTerm;
		public string SearchTerm
		{
			get => _SearchTerm;
			set => this.RaiseAndSetIfChanged(ref _SearchTerm, value);
		}

		private readonly ObservableAsPropertyHelper<IEnumerable<string>> _SearchResults;
		public IEnumerable<string> SearchResults => _SearchResults.Value;

		public MainViewModel()
		{
			_SearchResults = this.WhenAnyValue(X => X.SearchTerm)
				.Select(Term => Term?.Trim())
				.DistinctUntilChanged()
				.Where(Term => !string.IsNullOrWhiteSpace(Term))
				.Where(Term => Term == SearchTerm)
				.SelectMany(SearchNugetPackages)
				.ObserveOn(RxApp.MainThreadScheduler)
				.ToProperty(this, X => X.SearchResults);

			if (!DesignerProperties.GetIsInDesignMode(App.Current.MainWindow))
			{
				asdf();
			}
		}

		private async void asdf()
		{
			OCRReader Reader = new OCRReader();
			var Stream = File.OpenRead(@"C:\Users\Admin\Desktop\Workspace\1.png");

			var OCRTask = Reader.asdf(Stream);
			var OldBitmap = new System.Drawing.Bitmap(Stream);
			var b = new Image<Bgr, byte>(OldBitmap);

			var OCRResult = await OCRTask;
			var RectDictionary = new Dictionary<OcrLine, Rectangle>();

			for (int i = 0; i < OCRResult.Lines.Count; i++)
			{
				int MinX = int.MaxValue;
				int MinY = int.MaxValue;
				int MaxX = int.MinValue;
				int MaxY = int.MinValue;

				for (int j = 0; j < OCRResult.Lines[i].Words.Count; j++)
				{
					var Rect = OCRResult.Lines[i].Words[j].BoundingRect;

					if (MinX > Rect.Left)
					{
						MinX = (int) Rect.Left;
					}

					if (MinY > Rect.Top)
					{
						MinY = (int) Rect.Top;
					}

					if (MaxX < Rect.Right)
					{
						MaxX = (int) Rect.Right;
					}

					if (MaxY < Rect.Bottom)
					{
						MaxY = (int) Rect.Bottom;
					}
				}

				RectDictionary.Add(OCRResult.Lines[i], new Rectangle(MinX, MinY, MaxX - MinX, MaxY - MinY));

				var NewBitmap = OldBitmap.Clone(RectDictionary[OCRResult.Lines[i]], System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				var c = new Image<Bgr, byte>(NewBitmap);

				var d = new Image<Hsv, byte>(c.Width, c.Height);
				CvInvoke.CvtColor(c, d, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

				var Channels = d.Split();
				var Hue = Channels[0];
				var Sat = Channels[1];
				var Value = Channels[2];

				var Histogram1 = new DenseHistogram(255, new RangeF(0, 255));
				Histogram1.Calculate(new Image<Gray, byte>[] { Hue }, true, null);
				Histogram1.MinMax(out double[] MinV1, out double[] MaxV1, out Point[] MinL1, out Point[] MaxL1);

				var Histogram2 = new DenseHistogram(255, new RangeF(0, 255));
				Histogram2.Calculate(new Image<Gray, byte>[] { Sat }, true, null);
				Histogram2.MinMax(out double[] MinV2, out double[] MaxV2, out Point[] MinL2, out Point[] MaxL2);

				var Histogram3 = new DenseHistogram(255, new RangeF(0, 255));
				Histogram3.Calculate(new Image<Gray, byte>[] { Value }, true, null);
				Histogram3.MinMax(out double[] MinV3, out double[] MaxV3, out Point[] MinL3, out Point[] MaxL3);

				var HSV1 = new Hsv(MaxL1[0].Y - 1, MaxL2[0].Y - 50, MaxL3[0].Y - 50);
				var HSV2 = new Hsv(MaxL1[0].Y + 1, MaxL2[0].Y + 50, MaxL3[0].Y + 50);

				var f = d.InRange(HSV1, HSV2);

				var VectorMat = new VectorOfMat(Hue.Mat & f.Mat, Channels[1].Mat & f.Mat, Channels[2].Mat & f.Mat);
				var e = new Image<Hsv, byte>(d.Width, d.Height);

				CvInvoke.Merge(VectorMat, e);

				NewBitmap.Save(@"C:\Users\Admin\Desktop\Workspace\NewBitmap.png");
				d.Bitmap.Save(@"C:\Users\Admin\Desktop\Workspace\d.png");
				e.Bitmap.Save(@"C:\Users\Admin\Desktop\Workspace\e.png");
				f.Bitmap.Save(@"C:\Users\Admin\Desktop\Workspace\f.png");
			}
		}

		private async Task<IEnumerable<string>> SearchNugetPackages(string Term, CancellationToken Token)
		{
			return await Task.Run(() =>
			{
				return new List<string>() { "a", "b", "c", "d", "e" };
			});
		}
	}
}
