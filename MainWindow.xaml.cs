using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyLibrary;

namespace WulffrithLauncher {
	/// <summary>
	///		Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			// Standard Component Initialization
			InitializeComponent();

			// CONSTS
			const string APP_FOLDER = "apps";
			const string EXAMPLE_FILE = $@"{APP_FOLDER}\## - ExampleApplication.appdata";
			const string IMG_FOLDER = $@"{APP_FOLDER}\images";

			const int LARGE_GRID_CELL_COUNT = 9;
			const int MEDIUM_GRID_CELL_COUNT = 2;
			const int SMALL_GRID_CELL_COUNT = 4;

			// Dynamic Window Scaling and Positioning
			double screenHeight = SystemParameters.PrimaryScreenHeight;
			double screenWidth = SystemParameters.PrimaryScreenWidth;
			Width = Math.Min(screenWidth * 0.4, 980);
			Height = Math.Min(screenHeight * 0.35, 500);
			Left = (screenWidth / 2) - (Width / 2);
			Top = screenHeight - Height - 48;

			// Directory Creation
			Directory.CreateDirectory(IMG_FOLDER);
			string[] files = GetNonExampleFiles(APP_FOLDER, EXAMPLE_FILE);

			// Check Directory For Files
			if (files.Length < 1) {
				// Creates Error Message And Opens File Explorer To Directory On Click
				ErrorMessage(gridContainer, APP_FOLDER, [
					"Please add an .appdata file.",
					"An example file has been created for you.",
					"Use the file as a reference for files you make and do not delete the example file.",
					"Click anywhere in the window to open the related directory."
				]);

				// Returns Early
				return;
			}

			// Load File Datas
			string[][] filesData = LoadFileDatas(files, LARGE_GRID_CELL_COUNT, MEDIUM_GRID_CELL_COUNT, SMALL_GRID_CELL_COUNT, out bool fileSizesValid);

			// Check For File Size Validation
			if (!fileSizesValid) {
				// Creates Error Message And Opens File Explorer To Directory On Click
				ErrorMessage(gridContainer, APP_FOLDER, [
					"One or more files have an invalid size.",
					"Please search through the .appdata files and ensure all app sizes are written correctly.",
					"Additionally, ensure your apps fit on the grid.",
					"You may have to shrink the size of some of your apps or possibly remove some apps.",
					"Click anywhere in the window to open the related directory."
				]);

				// Returns Early
				return;
			}

			// Get Images From Image Directory
			string[] imgFiles = Directory.GetFiles(IMG_FOLDER);

			// Checks Images Have Been Loaded
			if (imgFiles.Length == 0) {
				// Error Message
				ErrorMessage(gridContainer, IMG_FOLDER, [
					"No images have been loaded.",
					"Please add an image to load.",
					"Click anywhere in the window to open the related directory."
				]);

				// Returns early
				return;
			}

			// TODO: Redo Grid System
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
		private static string[][] LoadFileDatas(string[] files, int largeCellCount, int mediumCellCount, int smallCellCount, out bool isValid) {
			// Count Of File Data Sizes
			int count = 0;

			// Current Indexing System Based On File Names
			string[][] filesData = new string[files.Length][];
			for (int i = 0; i < files.Length; i++) {
				// Reads File
				string[] lines = File.ReadAllLines(files[i]);

				// Creates Array To Hold File Datas
				filesData[i] = new string[lines.Length];

				// Loads All File Datas From File
				for (int j = 0; j < lines.Length; j++) {
					filesData[i][j] = lines[j].Split('>')[1].Trim();
				}

				// Converts Word Size To Respective Number
				switch (filesData[i][2]) {
					case "Small":
						filesData[i][2] = "1";
						break;
					case "Medium":
						filesData[i][2] = "4";
						break;
					case "Large":
						filesData[i][2] = "8";
						break;
					default:
						filesData[i][2] = $"{largeCellCount * mediumCellCount * smallCellCount + 1}";
						break;
				}
				count += int.Parse(filesData[i][2]);
			}

			// Checks if count does not surpass max allowed apps
			if (count > largeCellCount * mediumCellCount * smallCellCount) {
				isValid = false;
			} else {
				isValid = true;
			}

			// Returns FileDatas
			return filesData;
		}

		// Creates An Error Message Button In Grid Container
		private void ErrorMessage(Grid container, string directory, string[] lines) {
			// Creates Error Message Button
			Button btn = new();

			// Button Properties
			Grid.SetRow(btn, 0);
			Grid.SetRowSpan(btn, 3);
			Grid.SetColumn(btn, 0);
			Grid.SetColumnSpan(btn, 3);
			btn.Padding = new Thickness(4);
			btn.Content = string.Join(Environment.NewLine, lines);
			btn.VerticalContentAlignment = VerticalAlignment.Top;
			btn.HorizontalContentAlignment = HorizontalAlignment.Left;
			btn.Click += (s, e) => {
				MyLib.File.Start("explorer.exe", Path.GetFullPath(directory));
			};

			// Appends To Container
			container.Children.Add(btn);
		}
	}
}