using System;
using System.Windows.Media.Imaging;

namespace HotAndCold.Classes
{
	public static class ImagesHelper
	{
		/// <summary>
		/// Returns Question.png from Images
		/// </summary>
		public static BitmapImage Question
		{
			get { return new BitmapImage(new Uri(@"../Assets/Question.png", UriKind.Relative)); }
		}

		/// <summary>
		/// Returns Information.png from Images
		/// </summary>
		public static BitmapImage Information
		{
			get { return new BitmapImage(new Uri(@"../Assets/Information.png", UriKind.Relative)); }
		}

		/// <summary>
		/// Returns Error.png from Images
		/// </summary>
		public static BitmapImage Error
		{
			get { return new BitmapImage(new Uri(@"../Assets/Error.png", UriKind.Relative)); }
		}

		/// <summary>
		/// Returns Error.png from Images
		/// </summary>
		public static BitmapImage Warning
		{
			get { return new BitmapImage(new Uri(@"../Assets/Warning.png", UriKind.Relative)); }
		}
	}
}
