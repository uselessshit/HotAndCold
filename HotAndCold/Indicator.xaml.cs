using System.Windows;

namespace HotAndCold
{
	/// <summary>
	/// Interaction logic for Indicator.xaml
	/// </summary>
	public partial class Indicator : Window
	{
		public Indicator()
		{
			InitializeComponent();

			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
			Left = desktopWorkingArea.Right - Width;
			Top = desktopWorkingArea.Bottom - Height;
		}
	}
}
