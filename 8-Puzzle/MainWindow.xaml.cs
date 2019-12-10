using _8_Puzzle.Dialog;
using _8_Puzzle.ViewModel;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Media.Animation;
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
        private MyImage[,] _arrayImage;
        private int _canvasHeight;
        private int _canvasWidth;
        private Storyboard _storyboard;
        // Vị trí hình trống: X - dòng, Y - cột
        private Point _blankImagePosition;
        private string _templateImagePath;
        private Random _random = null;
        private double _progressValue;
        private double _progressMaximum;
        Thread _progress = null;

        public Point _currentPos;
        #endregion

        #region Properties
        public MyImage[,] ArrayImage { get => _arrayImage; set { _arrayImage = value; OnPropertyChanged(); } }
        public string TemplateImagePath { get => _templateImagePath; set { _templateImagePath = value; OnPropertyChanged(); } }
        public double ProgressValue { get => _progressValue; set { _progressValue = value; OnPropertyChanged(); } }
        public double ProgressMaximum { get => _progressMaximum; set { _progressMaximum = value; OnPropertyChanged(); } }
        public int GameState { get; set; }
        public int CanvasHeight { get => _canvasHeight; set { _canvasHeight = value; OnPropertyChanged(); } }
        public int CanvasWidth { get => _canvasWidth; set { _canvasWidth = value; OnPropertyChanged(); } }

        public bool isDragging = false;
        public Point START_POINT = new Point(10.4, 40.8);
        #endregion

        #region Command
        public ICommand NewGameCommand { get; set; }
        public ICommand SaveGameCommand { get; set; }
        public ICommand LoadGameCommand { get; set; }
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

            _storyboard = new Storyboard();

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
                    _progress = new Thread(async () =>
                    {
                        for (ProgressValue = 0.0; ProgressValue < ProgressMaximum; ProgressValue += 100.0)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(100.0));
                        }

                        // Hết giờ => game over                      
                        GameState = GAME_OVER;

                        Application.Current.Dispatcher.Invoke((Action)async delegate
                        {
                            var result = await DialogHost.Show(new GameOverDialog("Thua rồi haha :)))"), "MainDialog");
                            if ((bool)result)
                            {
                                GameState = GAME_READY;
                            }

                        });                       
             
                    });
<<<<<<< HEAD
                    _progress.IsBackground = true;
                    _progress.Start();
=======
                    progress.IsBackground = true;
                    progress.Start();

                }
            });
            SaveGameCommand = new RelayCommand<Canvas>((p) => { return GameState == GAME_RUNNING; }, (p) =>
            {
                const string filename = "save.txt";
                var writer = new StreamWriter(filename);
                writer.WriteLine(TemplateImagePath);
                writer.WriteLine(ProgressValue);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        writer.Write($"{ArrayImage[i,j].Index}");
                        if (j != 2)
                        {
                            writer.Write(" ");
                        }

                    }
                    writer.WriteLine("");
                }
                writer.Close();

                MessageBox.Show("Game is saved");
                
            });
            LoadGameCommand = new RelayCommand<Canvas>((p) => { return GameState == GAME_READY; }, (p) =>
            {
            var screen = new OpenFileDialog();
                if (screen.ShowDialog() == true)
                {
                    var filename = screen.FileName;

                    var reader = new StreamReader(filename);
                    TemplateImagePath = reader.ReadLine();
                    ProgressValue = Convert.ToInt32(reader.ReadLine());
                    int[,] temporaryArray = new int[ROWS, COLUMNS];
                    int q;
                    for (int currentRow = 0; currentRow < ROWS; currentRow++)
                    {
                        for (int currentColumn = 0; currentColumn < COLUMNS; currentColumn++)
                        {
                          
                            temporaryArray[currentRow,currentColumn] = Convert.ToInt32(reader.Read()) - 48;
                            q = reader.Read();
                        }
                        q = reader.Read();
                    }
                            ArrayImage = CropImage_LoadGame(TemplateImagePath,reader,temporaryArray);
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

>>>>>>> origin/SaveLoad
                }

            });
        }

        private MyImage[,] CropImage(string imagePath)
        {
            var imageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            int cropHeight = imageSource.PixelHeight / ROWS;
            int cropWidth = imageSource.PixelWidth / COLUMNS;


            MyImage[,] images = new MyImage[ROWS, COLUMNS];

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
                    images[currentRow, currentColumn].Image.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                    images[currentRow, currentColumn].Image.MouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;

                    Canvas.SetZIndex(images[currentRow, currentColumn].Image, 0);

                }
            }

            // Set phần tử cuối cùng bằng null để làm cờ điều khiển.
            images[ROWS - 1, COLUMNS - 1].Image.Source = null;

            return images;
        }
        private MyImage[,] CropImage_LoadGame(string imagePath,StreamReader reader, int[,] array)
        {
            var imageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            int cropHeight = imageSource.PixelHeight / ROWS;
            int cropWidth = imageSource.PixelWidth / COLUMNS;


            MyImage[,] images = new MyImage[ROWS, COLUMNS];

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
                    for(int i=0;i<ROWS;i++)
                    {
                        for(int j=0;j<COLUMNS;j++)
                        {
                            if(currentRow * ROWS + currentColumn == array[i,j])
                            {
                                images[i, j] = new MyImage { Index = array[i, j], Image = croppedImage };
                                images[i, j].Image.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                                images[i, j].Image.MouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;

                                Canvas.SetZIndex(images[i, j].Image, 0);
                            }
                            if(currentRow * ROWS + currentColumn == array[i, j] && array[i,j]==8)
                            {
                    
                                images[i,j].Image.Source = null;
                            }
                        }
                    }
                }
            }

            

            return images;
        }
        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GameState != GAME_RUNNING)
            {
                return;
            }

            var position = e.GetPosition(this);

            _currentPos.X = ((int)(position.X - START_POINT.X) / (IMAGE_WIDTH));
            _currentPos.Y = ((int)(position.Y - START_POINT.Y) / (IMAGE_HEIGHT));

            Point blankPos = new Point();
            blankPos = GetBlankImageIndex();

            int distanceX, distanceY;
            distanceX = Convert.ToInt32(_currentPos.X) - Convert.ToInt32(blankPos.X);
            distanceY = Convert.ToInt32(_currentPos.Y) - Convert.ToInt32(blankPos.Y);

            if (distanceX >= -1 && distanceX <= 1 && distanceY == 0 || distanceY >= -1 && distanceY <= 1 && distanceX == 0)
            {
                isDragging = true;
                Canvas.SetZIndex(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, 1);
            }
                     
        }

        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (isDragging == true)
            {

                Canvas.SetZIndex(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, 0);
                isDragging = false;
                Mouse.OverrideCursor = Cursors.Arrow;


                var position = e.GetPosition(this);



                if (checkSnap(position))
                {
                    Point posB = new Point();
                    posB = GetBlankImageIndex();
                    changePos(ArrayImage, _currentPos, posB);
                }


                int lengthRow = ArrayImage.GetLength(0);
                int lengthColumn = ArrayImage.GetLength(1);
                for (int i = 0; i < lengthRow; i++)
                {
                    for (int j = 0; j < lengthColumn; j++)
                    {
                        Canvas.SetLeft(ArrayImage[i, j].Image, j * (IMAGE_WIDTH));
                        Canvas.SetTop(ArrayImage[i, j].Image, i * (IMAGE_HEIGHT));
                    }
                }

                
            }


        }

        private async void Window_MouseMove(object sender, MouseEventArgs e)
        {

            if (isDragging)
            {
                Mouse.OverrideCursor = Cursors.Hand;
                var position = e.GetPosition(this);

                var dx = position.X;
                var dy = position.Y;

                if (checkSnap(position))
                {
                    Canvas.SetLeft(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, (GetBlankImageIndex().X + 1) * IMAGE_WIDTH - IMAGE_WIDTH);
                    Canvas.SetTop(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, (GetBlankImageIndex().Y + 1) * IMAGE_WIDTH - IMAGE_WIDTH);
                }
                else if (!checkInsideCanvas(position))
                {
                    var newX = dx - START_POINT.X - IMAGE_WIDTH / 2;
                    var newY = dy - START_POINT.Y - IMAGE_HEIGHT / 2;

                    if (dx - START_POINT.X - IMAGE_WIDTH / 2 < 0)
                    {
                        newX = 0;
                    }
                    if (dx + IMAGE_WIDTH / 2 >= START_POINT.X + 3 * IMAGE_WIDTH + 15)
                    {
                        newX = START_POINT.X + 2 * IMAGE_WIDTH + 15;
                    }
                    if (dy - START_POINT.Y - IMAGE_HEIGHT / 2 < 0)
                    {
                        newY = 0;
                    }
                    if (dy + IMAGE_HEIGHT / 2 >= START_POINT.Y + 3 * IMAGE_HEIGHT + 15)
                    {
                        newY = START_POINT.Y + 2 * IMAGE_HEIGHT - 30;
                    }

                    Canvas.SetLeft(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, newX);
                    Canvas.SetTop(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, newY);
                }
                else
                {
                    Canvas.SetLeft(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, dx - START_POINT.X - IMAGE_WIDTH / 2);
                    Canvas.SetTop(ArrayImage[Convert.ToInt32(_currentPos.Y), Convert.ToInt32(_currentPos.X)].Image, dy - START_POINT.Y - IMAGE_HEIGHT / 2);
                    // Checkwin
                    if (checkWin())
                    {
                        GameState = GAME_OVER;
                        _progress.Abort();
                        var result = await DialogHost.Show(new GameOverDialog("Chúc mừng, bạn đã phá đảo thành công 8 Puzzle !"), "MainDialog");

                        if ((bool)result)
                        {
                            GameState = GAME_READY;
                        }
                    }
                }
            }
        }

        private bool checkInsideCanvas(Point pos)
        {
            var dx = pos.X;
            var dy = pos.Y;

            if (dx - START_POINT.X - IMAGE_WIDTH / 2 >= 0)
            {
                if (dx + IMAGE_WIDTH / 2 <= START_POINT.X + COLUMNS * IMAGE_WIDTH + 15)
                {
                    if (dy - START_POINT.Y - IMAGE_HEIGHT / 2 >= 0)
                    {
                        if (dy + IMAGE_HEIGHT / 2 <= START_POINT.Y + ROWS * IMAGE_HEIGHT + 15)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool checkWin()
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLUMNS; j++)
                {
                    if (ArrayImage[i, j].Index != i * ROWS + j)
                        return false;
                }
            return true;
        }

        private bool checkSnap(Point pos)
        {
            var dx = pos.X;
            var dy = pos.Y;

            if (dx - START_POINT.X - IMAGE_WIDTH / 2 >= ((GetBlankImageIndex().X + 1) * IMAGE_WIDTH - IMAGE_WIDTH) - IMAGE_WIDTH / 4)
            {
                if (dx - START_POINT.X - IMAGE_WIDTH / 2 <= ((GetBlankImageIndex().X + 1) * IMAGE_WIDTH - IMAGE_WIDTH) + IMAGE_WIDTH / 4)
                {
                    if (dy - START_POINT.Y - IMAGE_HEIGHT / 2 >= ((GetBlankImageIndex().Y + 1) * IMAGE_WIDTH - IMAGE_HEIGHT) - IMAGE_WIDTH / 4)
                    {
                        if (dy - START_POINT.Y - IMAGE_HEIGHT / 2 <= ((GetBlankImageIndex().Y + 1) * IMAGE_WIDTH - IMAGE_HEIGHT) + IMAGE_WIDTH / 4)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }



        private void changePos<T>(T[,] array, Point a, Point b)
        {
            T temp = array[Convert.ToInt32(a.Y), Convert.ToInt32(a.X)];
            array[Convert.ToInt32(a.Y), Convert.ToInt32(a.X)] = array[Convert.ToInt32(b.Y), Convert.ToInt32(b.X)];
            array[Convert.ToInt32(b.Y), Convert.ToInt32(b.X)] = temp;


        }

        private void Shuffle<T>(T[,] array, Random random)
        {
            //int lengthRow = array.GetLength(1);

            //for (int i = array.Length - 1; i > 0; i--)
            //{
            //    int i0 = i / lengthRow;
            //    int i1 = i % lengthRow;

            //    int j = random.Next(i + 1);
            //    int j0 = j / lengthRow;
            //    int j1 = j % lengthRow;

            //    T temp = array[i0, i1];
            //    array[i0, i1] = array[j0, j1];
            //    array[j0, j1] = temp;
            //}


            for (int i = 0; i<100;i++)
            {
                int j = random.Next(4);

                Point blankPos = new Point();
                blankPos = GetBlankImageIndex();
                if (j == 0)
                {
                    if (blankPos.Y < ROWS-1)
                    {


                        Point currentKeyPos = new Point();
                        currentKeyPos.X = blankPos.X;
                        currentKeyPos.Y = blankPos.Y + 1;
                        changePos(ArrayImage, blankPos, currentKeyPos);

                    }
                }
                else if (j == 1)
                {
                    if (blankPos.Y > 0)
                    {
                        Point currentKeyPos = new Point();
                        currentKeyPos.X = blankPos.X;
                        currentKeyPos.Y = blankPos.Y - 1;
                        changePos(ArrayImage, blankPos, currentKeyPos);

                    }
                }
                else if (j == 2)
                {
                    if (blankPos.X > 0)
                    {
                        Point currentKeyPos = new Point();
                        currentKeyPos.X = blankPos.X - 1;
                        currentKeyPos.Y = blankPos.Y;
                        changePos(ArrayImage, blankPos, currentKeyPos);

                    }
                }
                else if (j == 3)
                {
                    if (blankPos.X < COLUMNS-1)
                    {
                        Point currentKeyPos = new Point();
                        currentKeyPos.X = blankPos.X + 1;
                        currentKeyPos.Y = blankPos.Y;
                        changePos(ArrayImage, blankPos, currentKeyPos);

                    }
                }
            }

           

            
        }
      
        private void Display()
        {
            int lengthRow = ArrayImage.GetLength(0);
            int lengthColumn = ArrayImage.GetLength(1);
            for (int i = 0; i < lengthRow; i++)
            {
                for (int j = 0; j < lengthColumn; j++)
                {
                    // Thêm vào canvas                   
                    ImagesCanvas.Children.Add(ArrayImage[i, j].Image);
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

        

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (GameState != GAME_RUNNING)
                return;

            Point blankPos = new Point();
            blankPos = GetBlankImageIndex();
            Point currentKeyPos = new Point();

            bool check = false;

            if (e.Key == Key.Up || e.Key == Key.W)
            {
                if (blankPos.Y < ROWS-1)
                {
                    currentKeyPos.X = blankPos.X;
                    currentKeyPos.Y = blankPos.Y + 1;
                    check = true;
                }
            }
            else if (e.Key == Key.Down || e.Key == Key.S)
            {
                if (blankPos.Y > 0)
                {
                    currentKeyPos.X = blankPos.X;
                    currentKeyPos.Y = blankPos.Y - 1;
                    check = true;
                }
            }
            else if (e.Key == Key.Right || e.Key == Key.D)
            {
                if (blankPos.X > 0)
                {
                    currentKeyPos.X = blankPos.X-1;
                    currentKeyPos.Y = blankPos.Y;
                    check = true;

                }
            }
            else if (e.Key == Key.Left || e.Key == Key.A)
            {
                if (blankPos.X < COLUMNS-1)
                {
                    currentKeyPos.X = blankPos.X + 1;
                    currentKeyPos.Y = blankPos.Y;
                    check = true;
                }
            }

            if (check)
            {
                int oldX = (int)currentKeyPos.X;
                int oldY = (int)currentKeyPos.Y;
                int newX = (int)blankPos.X;
                int newY = (int)blankPos.Y;

                var selectedImage = ArrayImage[oldY, oldX].Image;
                var animation_left = new DoubleAnimation(oldX * IMAGE_HEIGHT, newX * IMAGE_HEIGHT, TimeSpan.FromSeconds(0.3f));
                var animation_top = new DoubleAnimation(oldY * IMAGE_WIDTH, newY * IMAGE_WIDTH, TimeSpan.FromSeconds(0.3f));
                animation_left.FillBehavior = FillBehavior.Stop;
                animation_top.FillBehavior = FillBehavior.Stop;

                var story = new Storyboard();
                story.Children.Add(animation_left);
                story.Children.Add(animation_top);

                Storyboard.SetTarget(animation_left, selectedImage);
                Storyboard.SetTarget(animation_top, selectedImage);
                Storyboard.SetTargetProperty(animation_left, new PropertyPath(Canvas.LeftProperty));
                Storyboard.SetTargetProperty(animation_top, new PropertyPath(Canvas.TopProperty));
                story.Begin(this);

                ImagesCanvas.Children.Remove(selectedImage);

       
                ImagesCanvas.Children.Add(selectedImage);
                Canvas.SetLeft(selectedImage, newX * IMAGE_WIDTH);
                Canvas.SetTop(selectedImage, newY * IMAGE_HEIGHT);
                Canvas.SetTop(selectedImage, newY * IMAGE_HEIGHT);
                // clear animation
                //selectedImage.RenderTransform = new TranslateTransform();

                changePos(ArrayImage, blankPos, currentKeyPos);


                // Checkwin
                if (checkWin())
                {
                    GameState = GAME_OVER;
                    _progress.Abort();
                    var result = await DialogHost.Show(new GameOverDialog("Chúc mừng, bạn đã phá đảo thành công 8 Puzzle !"), "MainDialog");                 

                    if ((bool)result)
                    {
                        GameState = GAME_READY;
                        
                    }
                }
            }

        }
    }
}
