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
			const string EXAMPLE_FILE = $@"{APP_FOLDER}\## - ExampleApplication.appdata";
			const string IMG_FOLDER = $@"{APP_FOLDER}\images";

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
			string[][] filesData = LoadFileDatas(files, GRID_WIDTH_EFFECTIVE, GRID_HEIGHT_EFFECTIVE, out bool fileSizesValid);

			// TODO: Check if Files Exist

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

			// TODO: Check if IMGs Exist

			// Adds Icons To Grid
			FillGrid(gridIcons, filesData, imgFiles, GRID_WIDTH_ACTUAL, GRID_HEIGHT_ACTUAL, gridContainer, APP_FOLDER, IMG_FOLDER);
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
					case "Large":
						count += (size.Large().Width / 2 + 1) * (size.Large().Height / 2 + 1);
						break;
					default:
						count += width * height + 1;
						break;
				}
			}

			// Checks if count does not surpass max allowed apps
			if (count > width * height) {
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

		// Returns New ImageBrush With Image For Background Images
		private ImageBrush SetImage(string path) {
			return new ImageBrush(new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)));
		}

		// Fills Grid
		private void FillGrid(Grid grid, string[][] filesData, string[] imgFiles, int gridWidth, int gridHeight, Grid errGrid, string appFolder, string imgFolder) {
			// For Each App
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
					case "Large":
						size = new IconSize().Large();
						break;
				}

				// Creates Button
				Button btn = CreateAppIcon(fileData, size, grid, imgFolder);

				// Loops Through Grid
				IconSize step = new IconSize().Small();
				for (int row = 0; row < gridHeight; row += step.Height + 1) {
					bool done = false;
					for (int col = 0; col < gridWidth && !done; col += step.Width + 1) {
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
			}
		}

		// Creates Button And Adds It To Grid For It To Be Shifted To Correct Row & Col Later
		private Button CreateAppIcon(string[] fileData, IconSize size, Grid grid, string imgFolder) {
			// Button And Click Event
			Button btn = new();
			btn.Click += (s, e) => {
				MyLib.File.Start(fileData[3], fileData[4]);
			};

			// Background
			btn.Background = SetImage($@"{imgFolder}\{fileData[1]}");

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
				bool outsideHeight = row < Grid.GetRow(child) || row > Grid.GetRow(child) + Grid.GetRowSpan(child);
				bool outsideWidth = col < Grid.GetColumn(child) || col > Grid.GetColumn(child) + Grid.GetColumnSpan(child);
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

			public IconSize Large() {
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