using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace HotAndCold
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Fields

		private DirectoryInfo _secretPath;
		private Indicator _indicator;
		private int _currIndex = 0;
		private int _currLine = 0;
		private Random _randomizer;
		private string _currentPath;

		#region Static

		private static int _explorerId;
		private static Process _explorer;

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		#endregion Static

		#endregion Fields

		#region Constructors

		public MainWindow()
		{
			InitializeComponent();

			_randomizer = new Random();

			bool hasAccess = false;
			while (!hasAccess)
			{
				_secretPath = HideFile();

				if (_secretPath == null)
				{
					CustomMessageBox.Show(Properties.Resources.ErrorHideFile, PredefinedDialogLayout.Error);
					DeleteFileAndExit();
				}
				else
				{
					hasAccess = CreateFile();
				}
			}

			Timer typingTimer = new Timer(100);
			typingTimer.Elapsed += new ElapsedEventHandler(OnTypingTimerElapsedEvent);
			typingTimer.Start();
		}

		#endregion Constructors

		#region Methods

		private void ChangeColor(string recordedPath)
		{
			try
			{
				if (_secretPath.FullName.StartsWith(recordedPath))
				{
					if (_secretPath.FullName == recordedPath)
					{
						OpenFile();
					}
					_indicator.circleIndicator.Fill = new SolidColorBrush(Color.FromRgb(214, 28, 28));
				}
				else
				{
					_indicator.circleIndicator.Fill = new SolidColorBrush(Color.FromRgb(46, 80, 150));
				}
			}
			catch
			{
				CustomMessageBox.Show(Properties.Resources.ErrorChangeColor, PredefinedDialogLayout.Error);
				DeleteFileAndExit();
			}
		}

		private bool CreateFile()
		{
			try
			{
				string path = Path.Combine(_secretPath.FullName, Properties.Resources.FileName);
				if (!File.Exists(path))
				{
					using (StreamWriter sw = File.CreateText(path))
					{
						sw.WriteLine(Texts.Texts.FILE_CONTENT);
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void DeleteFileAndExit()
		{
			try
			{
				string path = Path.Combine(_secretPath.FullName, Properties.Resources.FileName);
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				Environment.Exit(0);
			}
			catch
			{
				Environment.Exit(0);
			}
		}

		private static string GetActiveExplorerPath()
		{
			try
			{
				IntPtr handle = GetForegroundWindow();
				ShellWindows shellWindows = new SHDocVw.ShellWindows();

				foreach (InternetExplorer window in shellWindows)
				{
					if (window.HWND == (int)handle)
					{
						var shellWindow = window.Document as Shell32.IShellFolderViewDual2;

						if (shellWindow != null)
						{
							var currentFolder = shellWindow.Folder.Items().Item();

							if (currentFolder == null || currentFolder.Path.StartsWith("::"))
							{
								const int nChars = 256;
								StringBuilder Buff = new StringBuilder(nChars);
								if (GetWindowText(handle, Buff, nChars) > 0)
								{
									return Buff.ToString();
								}
							}
							else
							{
								return currentFolder.Path;
							}
						}

						break;
					}
				}

				return null;
			}
			catch
			{
				return null;
			}
		}

		public DirectoryInfo HideFile(DirectoryInfo info = null)
		{
			try
			{
				DirectoryInfo[] folders;
				if (info == null)
				{
					var drives = new List<DriveInfo>();
					foreach (DriveInfo drive in DriveInfo.GetDrives())
					{
						if (drive.IsReady)
						{
							drives.Add(drive);
						}
					}
					int index = _randomizer.Next(0, drives.Count - 1);
					DirectoryInfo selectedDrive = drives[index].RootDirectory;
					folders = selectedDrive.GetDirectories();
				}
				else
				{
					folders = info.GetDirectories();
				}

				if (folders == null || folders.Length == 0)
				{
					return info;
				}

				int folderToSelect = _randomizer.Next(0, folders.Length - 1);
				DirectoryInfo newPath = folders[folderToSelect];
				return HideFile(newPath);
			}
			catch
			{
				return null;
			}
		}

		public void OpenFile()
		{
			try
			{
				string path = Path.Combine(_secretPath.FullName, Properties.Resources.FileName);
				Process.Start("notepad.exe", path);
				Environment.Exit(0);
			}
			catch
			{
				CustomMessageBox.Show(Properties.Resources.ErrorOpenFile, PredefinedDialogLayout.Error);
				DeleteFileAndExit();
			}
		}

		public void OpenMyComputer()
		{
			try
			{
				_explorer = System.Diagnostics.Process.Start("explorer");
				_explorerId = _explorer.Id;

				Timer recordTimer = new Timer(300);
				recordTimer.Elapsed += new ElapsedEventHandler(OnRecordTimerElapsedEvent);
				recordTimer.Start();
			}
			catch
			{
				CustomMessageBox.Show(Properties.Resources.ErrorExplorerStart, PredefinedDialogLayout.Error);
				DeleteFileAndExit();
			}
		}

		#endregion Methods

		#region Events

		private void OnTypingTimerElapsedEvent(object source, ElapsedEventArgs e)
		{
			this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
			{
				try
				{
					txtTyping.Focus();

					if (_currLine == 14 && _currIndex == 4)
					{
						_indicator = new Indicator();
						_indicator.Show();
					}
					if (_currLine == 20 && _currIndex == 6)
					{
						_indicator.circleIndicator.Fill = new SolidColorBrush(Color.FromRgb(46, 80, 150));
					}

					txtTyping.Text += Texts.Texts.LINES[_currLine][_currIndex].ToString();
					txtTyping.CaretIndex = txtTyping.Text.Length;
					_currIndex++;

					if (_currIndex > Texts.Texts.LINES[_currLine].Length - 1)
					{
						_currIndex = 0;
						_currLine++;
					}

					if (_currLine > Texts.Texts.LINES.Count - 1)
					{
						_currIndex = 0;
						_currLine = 0;
						(source as Timer).Stop();
						OpenMyComputer();
					}
				}
				catch
				{
					CustomMessageBox.Show(Properties.Resources.ErrorGeneric, PredefinedDialogLayout.Error);
				}
			}));
		}

		private void OnRecordTimerElapsedEvent(object source, ElapsedEventArgs e)
		{
			try
			{
				string recordedPath = GetActiveExplorerPath();
				if (_currentPath != recordedPath && !string.IsNullOrEmpty(recordedPath))
				{
					_currentPath = recordedPath;
					this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
					{
						ChangeColor(recordedPath);
					}));
				}
			}
			catch
			{
				this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
				{
					CustomMessageBox.Show(Properties.Resources.ErrorGeneric, PredefinedDialogLayout.Error);
					DeleteFileAndExit();
				}));
			}
		}

		private void wndMainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				if (CustomMessageBox.Show(Properties.Resources.QuestionExit, PredefinedDialogLayout.Question) ==
					MessageBoxResult.Yes)
				{
					DeleteFileAndExit();
				}
			}
		}

		#endregion Events
	}
}
