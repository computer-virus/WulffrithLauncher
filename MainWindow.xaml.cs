﻿using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
			const string EXAMPLE_FILE = $@"{APP_FOLDER}\## - ExampleApplication.appdata", SETTINGS_FILE = $@"{APP_FOLDER}\99 - Launcher Settings (DO NOT EDIT).appdata";
			const string IMG_FOLDER = $@"{APP_FOLDER}\images";
			const string SETTINGS_IMG = $@"{IMG_FOLDER}\MoonSettings.png";
			const string LAUNCHER_BEHAVIOURS_FOLDER = "launcher settings";
			const string AUTO_RUN_FILE = $@"{LAUNCHER_BEHAVIOURS_FOLDER}\autorun.bool";

			const int GRID_WIDTH_EFFECTIVE = 12;
			const int GRID_WIDTH_ACTUAL = GRID_WIDTH_EFFECTIVE * 2;
			const int GRID_HEIGHT_EFFECTIVE = 6;
			const int GRID_HEIGHT_ACTUAL = GRID_HEIGHT_EFFECTIVE * 2;

			// Dynamic Window Scaling and Positioning
			double screenHeight = SystemParameters.PrimaryScreenHeight;
			double screenWidth = SystemParameters.PrimaryScreenWidth;
			Width = Math.Min(screenWidth * 0.4, 980);
			Height = Math.Min(screenHeight * 0.35, 500);
			Left = (screenWidth / 2) - (Width / 2);
			Top = screenHeight - Height - 48;

			// Directory Creation
			Directory.CreateDirectory(IMG_FOLDER);
			Directory.CreateDirectory(LAUNCHER_BEHAVIOURS_FOLDER);

			// Registry Key To Auto-Run On Startup If Auto-Run Is Set To True In Launcher Behaviours Folder
			SetAutoLaunch(AUTO_RUN_FILE);

			// Load Files
			string[] files = GetNonExampleFile(APP_FOLDER, EXAMPLE_FILE, SETTINGS_FILE, SETTINGS_IMG);

			// Check Directory For Files
			if (files.Length < 2) {
				// Creates Error Message And Opens File Explorer To Directory On Click
				ErrorMessage(gridContainer, APP_FOLDER, [
					"Please add an .appdata file.",
					"An example file has been created for you.",
					"Use the file as a reference for apps you integrate and do not delete the example file.",
					"",
					"Note that you do not need to specify the supporting application for most paths and doing so could actually be detrimental.",
					"Attempting, for example, to open a directory by specifying explorer.exe in the path causes multiple instances of explorer.exe (which is not a good thing).",
					"Instead, input the directory directly into the path and the program should safely use the default support application. (If it doesn't, let me know.)",
					"That said, I can't do that for every app.",
					"",
					"Click anywhere in the window to open the related directory."
				]);

				// Returns Early
				return;
			}

			// Load File Datas
			string[][] filesData = LoadFileDatas(files, GRID_WIDTH_EFFECTIVE, GRID_HEIGHT_EFFECTIVE, out bool fileSizesValid);

			// Check For File Size Validation
			if (!fileSizesValid) {
				// Creates Error Message And Opens File Explorer To Directory On Click
				ErrorMessage(gridContainer, APP_FOLDER, [
					"One or more apps have an invalid size OR one or more apps don't fit on the grid.",
					"Please search through the .appdata files and ensure all app sizes are written correctly.",
					"Additionally, you may have to shrink the size of some of your apps or possibly remove some apps.",
					"Click anywhere in the window to open the related directory."
				]);

				// Returns Early
				return;
			}

			// Get Images From Image Directory
			string[] imgFiles = Directory.GetFiles(IMG_FOLDER);

			// Checks Images Have Been Loaded
			if (imgFiles.Length < 2) {
				// Error Message
				ErrorMessage(gridContainer, IMG_FOLDER, [
					"No images have been loaded.",
					"Please add an image to load.",
					"Click anywhere in the window to open the related directory."
				]);

				// Returns early
				return;
			}

			// Adds Icons To Grid
			FillGrid(gridIcons, files, filesData, imgFiles, GRID_WIDTH_ACTUAL, GRID_HEIGHT_ACTUAL, gridContainer, APP_FOLDER, IMG_FOLDER);

			// Auto Minimize Window When Done
			WindowState = WindowState.Minimized;
		}

		// Creates autorun file if not exist and sets app to autorun on startup if file value is true
		private void SetAutoLaunch(string file) {
			// Default is false
			bool autorun = false;
			
			// Set based on file
			if (File.Exists(file)) {
				string[] data = File.ReadAllLines(file);
				try {
					autorun = bool.Parse(data[0]);
				} catch {
					MyLib.File.WriteAllLines(file, ["" + autorun]);
				}
			} else {
				MyLib.File.WriteAllLines(file, ["" + autorun]);
			}

			// If true, create Registry Key and Set Value
			string appName = "WulffrithLauncher";
			string execPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\WulffrithLauncher.exe";
			RegistryKey? reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
			if (reg != null && autorun) {
				reg.SetValue(appName, execPath);
				reg.Close();
			} else if (reg != null) {
				reg.DeleteValue(appName, false);
			}
		}

		// Minimizes application when unfocused
		private void OnUnfocus(object sender, EventArgs e) {
			WindowState = WindowState.Minimized;
		}

		// TO DO: Refresh Button That Clears Icon Grid And Calls All Loading Methods Again

		// Creates example files with format reference
		private static void CreateExampleFiles(string appFolder, string exampleFile, string settingsFile, string imgFile) {
			MyLib.File.WriteAllLines(exampleFile, [
				"Application Name (Doesn't have to be accurate. It's used for a tooltip.) > Example App",
				"Image Name (Most bitmap image formats work. Does not animate yet.) > example.png",
				"Panel Size (i.e. Small, Medium, Wide) > Small",
				@$"Path (Can be any file location, directory, url, etc. This is what gets launched when you click the icon.) > {exampleFile}",
				"Command Line Arguments > Arguments you want to launch the the app with, like loading an image onto mspaint.exe. Leave empty for no arguments."
			]);

			MyLib.File.WriteAllLines(settingsFile, [
				"Application Name > Launcher Settings",
				"Image Name > MoonSettings.png",
				"Panel Size > Small",
				@$"Directory > {appFolder}",
				"Arguements > "
			]);

			File.WriteAllBytes(imgFile, ImageDataManager.GetSettingImgBytes());
		}

		// Gets all file locations excluding the example files
		private static string[] GetNonExampleFile(string appFolder, string exampleFile, string settingsFile, string imgFile) {
			if (File.Exists(exampleFile)) {
				File.Delete(exampleFile);
			}

			string[] files = Directory.GetFiles(appFolder);

			CreateExampleFiles(appFolder, exampleFile, settingsFile, imgFile);

			return files;
		}

		// Loads File Datas
		private static string[][] LoadFileDatas(string[] files, int width, int height, out bool isValid) {
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
				IconSize size = new IconSize();
				switch (filesData[i][2]) {
					case "Small":
						count += (size.Small().Width / 2 + 1) * (size.Small().Height / 2 + 1);
						break;
					case "Medium":
						count += (size.Medium().Width / 2 + 1) * (size.Medium().Height / 2 + 1);
						break;
					case "Wide":
					case "Large":
						count += (size.Wide().Width / 2 + 1) * (size.Wide().Height / 2 + 1);
						break;
					default:
						count += width * height + 1;
						break;
				}
			}

			// Checks if count does not surpass max allowed apps
			if (count < width * height) {
				isValid = true;
			} else {
				isValid = false;
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
			btn.VerticalContentAlignment = VerticalAlignment.Top;
			btn.HorizontalContentAlignment = HorizontalAlignment.Left;
			btn.Click += (s, e) => {
				MyLib.File.Open(Path.GetFullPath(directory));
			};
			TextBlock txt = new TextBlock();
			txt.Text = string.Join(Environment.NewLine, lines);
			txt.TextWrapping = TextWrapping.Wrap;
			btn.Content = txt;

			// Appends To Container
			container.Children.Add(btn);
		}

		// Returns New ImageBrush With Image For Background Images
		private ImageBrush SetImage(string path) {
			return new ImageBrush(new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)));
		}

		// Fills Grid
		private void FillGrid(Grid grid, string[] files, string[][] filesData, string[] imgFiles, int gridWidth, int gridHeight, Grid errGrid, string appFolder, string imgFolder) {
			// For Each App
			int count = 0;
			foreach (string[] fileData in filesData) {
				// Finds Appropriate Size
				IconSize size;
				switch (fileData[2]) {
					default:
					case "Small":
						size = new IconSize().Small();
						break;
					case "Medium":
						size = new IconSize().Medium();
						break;
					case "Wide":
					case "Large":
						size = new IconSize().Wide();
						break;
				}

				// Creates Button
				Button btn = CreateAppIcon(files[count], fileData, size, grid, imgFolder, appFolder, errGrid);

				// Loops Through Grid
				IconSize step = new IconSize().Small();
				for (int row = 0; row + size.Height - 1 < gridHeight; row += step.Height + 1) {
					bool done = false;
					for (int col = 0; col + size.Width - 1 < gridWidth && !done; col += step.Width + 1) {
						// Sets Grid To Current Position
						Grid.SetRow(btn, row);
						Grid.SetColumn(btn, col);

						// Checks For Overlap
						if (!isAppOverlapping(btn, grid, row, col)) {
							// No Overlap, Placement Is Done								
							done = true;
						}
					}

					if (done) {
						break;
					}
				}

				count++;
			}
		}

		// Creates Button And Adds It To Grid For It To Be Shifted To Correct Row & Col Later
		private Button CreateAppIcon(string file, string[] fileData, IconSize size, Grid grid, string imgFolder, string appFolder, Grid errGrid) {
			// Button And Click Events (Prevents Launching Multiple File Explorers)
			Button btn = new();

			// Left Click
			btn.Click += (s, e) => {
				// Reactive File Validation For More Freedom
				try {
					MyLib.File.Open(fileData[3], fileData[4]);
				} catch {
					grid.Children.Clear();
					ErrorMessage(errGrid, appFolder, [
						"Something went wrong launching the selected app.",
						"Please ensure you correctly wrote the path into the .appdata file.",
						"",
						"Click anywhere in the window to open the related directory"
					]);
				}
			};

			// Right Click
			btn.MouseRightButtonUp += (s, e) => {
				MyLib.File.Open(file);
				MyLib.File.Open(imgFolder);
			};

			// Background
			string imageFile = $@"{imgFolder}\{fileData[1]}";
			if (File.Exists(imageFile)) {
				btn.Background = SetImage(imageFile);
			} else {
				btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff0000") ?? new());
				btn.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff") ?? new());
				btn.Content = "Invalid" + Environment.NewLine + " Image";
			}

			// Tooltip
			ToolTip toolTip = new();
			toolTip.Content = fileData[0];
			toolTip.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e3e3e3") ?? new());
			toolTip.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#04060c") ?? new());
			toolTip.FontWeight = FontWeights.Bold;
			btn.ToolTip = toolTip;

			// Size
			Grid.SetColumnSpan(btn, size.Width);
			Grid.SetRowSpan(btn, size.Height);

			// Adds To Grid (Not Positioned Yet)
			grid.Children.Add(btn);

			// Returns Button
			return btn;
		}

		// Checks If App Icon Overlaps Others
		private bool isAppOverlapping(Button btn, Grid grid, int row, int col) {
			// Loop Through All Buttons On Grid
			bool overlap = false;
			foreach (Button child in grid.Children.OfType<Button>()) {
				// If The A Different Button Is Intersecting Current Button
				bool outsideHeight = row + Grid.GetRowSpan(btn) < Grid.GetRow(child) || row > Grid.GetRow(child) + Grid.GetRowSpan(child);
				bool outsideWidth = col + Grid.GetColumnSpan(btn) < Grid.GetColumn(child) || col > Grid.GetColumn(child) + Grid.GetColumnSpan(child);
				if (child != btn && !outsideHeight && !outsideWidth) {
					// There's An Overlap
					overlap = true;
					break;
				}
			}
			// Return Result
			return overlap;
		}

		// Used For App Icon Sizes
		private class IconSize() {
			private int _width;
			private int _height;

			public int Width {
				get {
					return _width;
				}
			}

			public int Height {
				get {
					return _height;
				}
			}

			public IconSize Wide() {
				_width = 7;
				_height = 3;
				return this;
			}

			public IconSize Medium() {
				_width = 3;
				_height = 3;
				return this;
			}

			public IconSize Small() {
				_width = 1;
				_height = 1;
				return this;
			}
		}
	}
}