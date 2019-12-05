using _8_Puzzle.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace _8_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private Members
        private MyImage[, ] _arrayImage;
        private int _canvasHeight;
        private int _canvasWidth;
        // Vị trí hình trống: X - dòng, Y - cột
        private Point _blankImagePosition;
        private string _templateImagePath;
        private Random _random = null;
        private double _progressValue;
        private double _progressMaximum;
        #endregion

        #region Properties
        public MyImage[, ] ArrayImage { get => _arrayImage; set { _arrayImage = value; OnPropertyChanged(); } }
        public string TemplateImagePath { get => _templateImagePath; set { _templateImagePath = value; OnPropertyChanged(); } }
        public double ProgressValue { get => _progressValue; set { _progressValue = value; OnPropertyChanged(); } }
        public double ProgressMaximum { get => _progressMaximum; set { _progressMaximum = value; OnPropertyChanged(); } }
        public int GameState { get; set; }
        public int CanvasHeight { get => _canvasHeight; set { _canvasHeight = value; OnPropertyChanged(); } }
        public int CanvasWidth { get => _canvasWidth; set { _canvasWidth = value; OnPropertyChanged(); } }
        #endregion

        #region Command
        public ICommand NewGameCommand { get; set; }
        public ICommand MouseMoveImageCommand { get; set; }
        #endregion

        #region Const
        private const int GAME_READY = 0;               // Game state
        private const int GAME_RUNNING = 1;     
        private const int GAME_OVER = 2;        

        private const int ROWS = 3;
        private const int COLUMNS = 3;
        private const int IMAGE_HEIGHT = 200;
        private const int IMAGE_WIDTH = 200;
        private const double MAX_TIME = 3; // 3 phút
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            string root = AppDomain.CurrentDomain.BaseDirectory;

            // Hình mặc định
            TemplateImagePath = $"{root}Image\\templateImage.jpg";

            // Sự kiện
            InitCommand();

            // Default 
            GameState = GAME_READY;
            ProgressMaximum = TimeSpan.FromMinutes(MAX_TIME).TotalMilliseconds;
            _random = new Random();
        }

        private void InitCommand()
        {
            NewGameCommand = new RelayCommand<Canvas>((p) => { return GameState == GAME_READY; }, (p) => 
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    TemplateImagePath = ofd.FileName;
                    ProgressValue = 0;
                    p.Children.Clear();

                    // Cắt hình mặc định
                    ArrayImage = CropImage(TemplateImagePath);

                    // Trộn
                    Shuffle(ArrayImage, _random);

                    // Hiển thị lên canvas
                    Display();

                    // vị trí hình trống; X - dòng; Y cột
                    _blankImagePosition = GetBlankImageIndex();

                    GameState = GAME_RUNNING;

                    // Bộ đếm giờ, ProgressMaximun = 3 phút
                    Thread progress = new Thread(() =>
                    {
                        for (double num = 0.0; num < ProgressMaximum; num += 100.0)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(100.0));
                            ProgressValue += 100.0;
                        }

                        // Hết giờ => game over
                        GameState = GAME_OVER;
                    });
                    progress.IsBackground = true;
                    progress.Start();
                }   
            });
        } 

        private MyImage[, ] CropImage(string imagePath)
        {
            var imageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            int cropHeight = imageSource.PixelHeight / ROWS;
            int cropWidth = imageSource.PixelWidth / COLUMNS;
           

            MyImage[, ] images = new MyImage[ROWS, COLUMNS];

            for (int currentRow = 0; currentRow < ROWS; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < COLUMNS; currentColumn++)
                {
                    var rect = new Int32Rect(currentColumn * cropWidth, currentRow * cropHeight, cropWidth, cropHeight);
                    var croppedBitmap = new CroppedBitmap(imageSource, rect);
                    var croppedImage = new Image();
                    croppedImage.Stretch = Stretch.Fill;
                    croppedImage.Width = cropWidth;
                    croppedImage.Height = cropHeight;
                    croppedImage.Source = croppedBitmap;
                    images[currentRow, currentColumn] = new MyImage { Index = currentRow * ROWS + currentColumn, Image = croppedImage };                         
                }
            }

            // Set phần tử cuối cùng bằng null để làm cờ điều khiển.
            images[ROWS - 1, COLUMNS - 1].Image.Source = null;

            return images;
        }

        private void Shuffle<T>(T[,] array, Random random)
        {
            int lengthRow = array.GetLength(1);

            for (int i = array.Length - 1; i > 0; i--)
            {
                int i0 = i / lengthRow;
                int i1 = i % lengthRow;

                int j = random.Next(i + 1);
                int j0 = j / lengthRow;
                int j1 = j % lengthRow;

                T temp = array[i0, i1];
                array[i0, i1] = array[j0, j1];
                array[j0, j1] = temp;
            }
        }

        private void Display()
        {
            int lengthRow = ArrayImage.GetLength(0);
            int lengthColumn = ArrayImage.GetLength(1);
            for(int i = 0; i < lengthRow; i++)
            {
                for(int j = 0; j < lengthColumn; j++)
                {
                    // Thêm vào canvas                   
                    ImagesCanvas.Children.Add(ArrayImage[i,j].Image);
                    Canvas.SetLeft(ArrayImage[i, j].Image, j * (IMAGE_WIDTH));
                    Canvas.SetTop(ArrayImage[i, j].Image, i * (IMAGE_HEIGHT));
                }
            }
            
        }

        private Point GetBlankImageIndex()
        {
            Point result = new Point();
            int lengthRow = ArrayImage.GetLength(0);
            int lengthColumn = ArrayImage.GetLength(1);

            for (int i = 0; i < lengthRow; i++)
            {
                for (int j = 0; j < lengthColumn; j++)
                {
                    if (ArrayImage[i, j].Image.Source == null)
                    {
                        result.X = j;
                        result.Y = i;
                        break;
                    }
                }
            }

            return result;
        }


        private bool CheckWin()
        {
            bool result = false;    

            return result;
        }

        public class MyImage
        {
            public int Index { get; set; }  // Dựa vào Index để check win
            public Image Image { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
