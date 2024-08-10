﻿using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyLibrary;

namespace WulffrithLauncher {
	/// <summary>
	///		Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private const string APP_FOLDER = "apps";
		private const string EXAMPLE_FILE = $@"{APP_FOLDER}\## - ExampleApplication.appdata";

		private const string IMG_FOLDER = $@"{APP_FOLDER}\images";

		private string[] _files;

		private string[][] _fileDatas;

		private const int MAX_SUM_FILE_SIZE_PER_SECTION = 8;

		public MainWindow() {
			// Standard Component Initialization
			InitializeComponent();

			double screenHeight = SystemParameters.PrimaryScreenHeight;
			double screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (this.Width / 2);
			Top = screenHeight - this.Height - 48;

			// Directory Creation
			Directory.CreateDirectory(IMG_FOLDER);
			_files = GetNonExampleFiles(APP_FOLDER, EXAMPLE_FILE);

			// Check Directory For Files
			if (_files.Length < 1) {
				txtErrorMessage.Text = string.Join(Environment.NewLine, [
					$"Please add an \".appdata\" file to the {APP_FOLDER} folder.",
					"An example file has been created for you.",
					"Use the file as a reference for files you make and do not delete the example file."
				]);
			}

			// Load File Datas
			_fileDatas = new string[_files.Length][];
			for (int i = 0; i < _files.Length; i++) {
				string[] lines = File.ReadAllLines(_files[i]);

				_fileDatas[i] = new string[lines.Length];

				for (int j = 0; j < lines.Length; j++) {
					_fileDatas[i][j] = lines[j].Split('>')[1].Trim();
				}

				switch (_fileDatas[i][2]) {
					default:
					case "Small":
						_fileDatas[i][2] = "1";
						break;
					case "Medium":
						_fileDatas[i][2] = "4";
						break;
					case "Large":
						_fileDatas[i][2] = "8";
						break;
				}
			}
		}

		private static void CreateExampleFile(string file) {
			MyLib.File.WriteAllLines(file, [
				"Application Name > Example App",
				"Image Name > egg.png",
				"Panel Size (i.e. Small, Medium, Large) > Small",
				"Application Location > Location Goes Here",
				"Command Line Arguments > -example argument"
			]);
		}

		private static string[] GetNonExampleFiles(string folder, string exampleFile) {
			if (File.Exists(exampleFile)) {
				File.Delete(exampleFile);
			}

			string[] files = Directory.GetFiles(folder);

			CreateExampleFile(exampleFile);

			return files;
		}
	}
}