using System.IO;
using System.Windows;
using MyLibrary;

namespace WulffrithLauncher {
	/// <summary>
	///		Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private const string APP_FOLDER = "apps";
		private const string EXAMPLE_FILE = $@"{APP_FOLDER}\## - ExampleApplication.appdata";
		private const string IMG_FOLDER = $@"{APP_FOLDER}\images";
		private const int MAX_FILE_SIZE_COUNT = 72;

		private string[] _files;
		private string[][] _fileDatas;
		private string[] _imgFiles;

		public MainWindow() {
			// Standard Component Initialization
			InitializeComponent();

			// Dynamic Window Scaling and Positioning
			double screenHeight = SystemParameters.PrimaryScreenHeight;
			double screenWidth = SystemParameters.PrimaryScreenWidth;
			Width = Math.Min(screenWidth * 0.4, 980);
			Height = Math.Min(screenHeight * 0.35, 500);
			Left = (screenWidth / 2) - (Width / 2);
			Top = screenHeight - Height - 48;

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
			_fileDatas = LoadFileDatas(_files);

			// Get Images From Image Directory
			_imgFiles = Directory.GetFiles(IMG_FOLDER);

			int[,,] grid = new int[9, 2, 4];
			PrepGrid(grid);
		}

		// Closes application when unfocused
		private void OnUnfocus(object sender, EventArgs e) {
			Environment.Exit(0);
		}

		// Creates example file with format reference
		private static void CreateExampleFile(string file) {
			MyLib.File.WriteAllLines(file, [
				"Application Name > Example App",
				"Image Name > egg.png",
				"Panel Size (i.e. Small, Medium, Large) > Small",
				"Application Location > Location Goes Here",
				"Command Line Arguments > -example argument"
			]);
		}

		// Gets all file locations excluding the example file
		private static string[] GetNonExampleFiles(string folder, string exampleFile) {
			if (File.Exists(exampleFile)) {
				File.Delete(exampleFile);
			}

			string[] files = Directory.GetFiles(folder);

			CreateExampleFile(exampleFile);

			return files;
		}

		// Loads File Datas
		private static string[][] LoadFileDatas(string[] files) {
			// Current Indexing System Based On File Names
			string[][] fileDatas = new string[files.Length][];
			for (int i = 0; i < files.Length; i++) {
				// Reads File
				string[] lines = File.ReadAllLines(files[i]);

				// Creates Array To Hold File Datas
				fileDatas[i] = new string[lines.Length];

				// Loads All File Datas From File
				for (int j = 0; j < lines.Length; j++) {
					fileDatas[i][j] = lines[j].Split('>')[1].Trim();
				}

				// Converts Word Size To Respective Number
				// Still in string[] so can't directly convert to int yet
				switch (fileDatas[i][2]) {
					case "Small":
						fileDatas[i][2] = "1";
						break;
					case "Medium":
						fileDatas[i][2] = "4";
						break;
					case "Large":
						fileDatas[i][2] = "8";
						break;
					default:
						fileDatas[i][2] = "" + MAX_FILE_SIZE_COUNT + 1;
						break;
				}
			}

			// Returns FileDatas
			return fileDatas;
		}

		// Fills Grid With An Unindexable Values
		private static void PrepGrid(int[,,] grid) {
			for (int i = 0; i < grid.GetLength(0); i++) {
				for (int j = 0; j < grid.GetLength(1); j++) {
					for (int k = 0; k < grid.GetLength(2); k++) {
						grid[i, j, k] = -1;
					}
				}
			}
		}
	}
}