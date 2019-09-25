using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OCRProject.ViewModel;
using ReactiveUI;
using Windows.Media.Ocr;

namespace OCRProject
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : ReactiveWindow<MainViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();

			ViewModel = new MainViewModel();

			this.WhenActivated(disposableRegistration =>
			{
				this.OneWayBind(ViewModel,
				ViewModel => ViewModel.SearchResults,
				View => View.SearchListBox.ItemsSource)
				.DisposeWith(disposableRegistration);

				this.Bind(ViewModel,
				ViewModel => ViewModel.SearchTerm,
				View => View.SearchBox.Text)
				.DisposeWith(disposableRegistration);
			});
		}
	}
}
