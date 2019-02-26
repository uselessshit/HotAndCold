using HotAndCold.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HotAndCold
{
	/// <summary>
	/// Interaction logic for CustomMessageBox.xaml
	/// </summary>
	public partial class CustomMessageBox : Window
	{
		#region Constructor

		public CustomMessageBox()
		{
			InitializeComponent();
			Result = MessageBoxResult.None;
			btnYes.Focus();
		}

		#endregion Constructor

		#region Properties

		public MessageBoxResult Result;

		#endregion Properties

		#region Methods

		public static MessageBoxResult Show(string message, PredefinedDialogLayout layout)
		{
			String btnYesCaption = String.Empty;
			String btnNoCaption = String.Empty;
			CustomMessageBoxButtonCombo buttonCombo = CustomMessageBoxButtonCombo.YesNo;
			BitmapImage dialogImage = new BitmapImage();

			switch (layout)
			{
				case PredefinedDialogLayout.Question:
					btnYesCaption = "Yes";
					btnNoCaption = "No";
					buttonCombo = CustomMessageBoxButtonCombo.YesNoCancel;
					dialogImage = ImagesHelper.Question;
					break;

				case PredefinedDialogLayout.Information:
					btnYesCaption = "OK";
					btnNoCaption = String.Empty;
					buttonCombo = CustomMessageBoxButtonCombo.Yes;
					dialogImage = ImagesHelper.Information;
					break;

				case PredefinedDialogLayout.Error:
					btnYesCaption = "OK";
					btnNoCaption = string.Empty;
					buttonCombo = CustomMessageBoxButtonCombo.Yes;
					dialogImage = ImagesHelper.Error;
					break;

				case PredefinedDialogLayout.Warning:
					btnYesCaption = "Yes";
					btnNoCaption = "No";
					buttonCombo = CustomMessageBoxButtonCombo.YesNo;
					dialogImage = ImagesHelper.Warning;
					break;
			}

			return Show(message, buttonCombo, dialogImage, btnYesCaption, btnNoCaption);
		}

		public static MessageBoxResult Show(string message, CustomMessageBoxButtonCombo buttonCombo, BitmapImage dialogImage, string btnYesCaption, string btnNoCaption)
		{
			try
			{
				var cmb = new CustomMessageBox();

				// Dialog owner
				if ((App.Current.Windows[0] as MainWindow).IsVisible)
				{
					cmb.Owner = App.Current.Windows[0];
					cmb.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				}
				else
				{
					cmb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				}

				// Label message
				cmb.lblMessage.Text = message;

				// Visibility for dialog buttons and their positioning
				switch (buttonCombo)
				{
					case CustomMessageBoxButtonCombo.Yes:
						SetButtonIsVisible(cmb.btnYes, false);
						SetButtonIsVisible(cmb.btnNo, false);
						SetButtonIsVisible(cmb.btnOK, true);
						break;

					case CustomMessageBoxButtonCombo.YesNo:
						SetButtonIsVisible(cmb.btnYes, true);
						SetButtonIsVisible(cmb.btnNo, true);
						SetButtonIsVisible(cmb.btnOK, false);
						break;

					case CustomMessageBoxButtonCombo.YesNoCancel:
						SetButtonIsVisible(cmb.btnYes, true);
						SetButtonIsVisible(cmb.btnNo, true);
						SetButtonIsVisible(cmb.btnOK, false);
						break;
				}

				// Dialog image
				if (dialogImage != null)
				{
					cmb.imgDialogImage.Visibility = Visibility.Visible;
					cmb.imgDialogImage.Source = dialogImage;
				}
				else
				{
					cmb.imgDialogImage.Visibility = Visibility.Collapsed;
				}


				// Button captions
				cmb.btnYes.Content = btnYesCaption;
				cmb.btnNo.Content = btnNoCaption;

				cmb.ShowDialog();

				return cmb.Result;
			}
			catch
			{
				return MessageBoxResult.OK;
			}
		}

		private static void SetButtonIsVisible(Button btn, bool isBtnVisible)
		{
			if (isBtnVisible)
			{
				btn.Visibility = Visibility.Visible;
				btn.Margin = new Thickness(5);
			}
			else
			{
				btn.Visibility = Visibility.Collapsed;
			}
		}

		#endregion Methods

		#region Events

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.None;
			Close();
		}

		public void btnNo_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.No;
			Close();
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Yes;
			Close();
		}

		public void btnYes_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Yes;
			Close();
		}

		private void wndMsgBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Result = MessageBoxResult.Yes;
			}
			else if (e.Key == Key.Escape)
			{
				Result = MessageBoxResult.None;
				Close();
			}
		}

		#endregion Events
	}

	#region Enums

	public enum CustomMessageBoxButtonCombo
	{
		Yes,
		YesNo,
		YesNoCancel
	}

	public enum PredefinedDialogLayout
	{
		Question,
		Information,
		Error,
		Warning
	}

	#endregion Enums
}
