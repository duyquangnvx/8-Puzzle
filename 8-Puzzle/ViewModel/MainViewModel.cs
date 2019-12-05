using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _8_Puzzle.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Members
        private ObservableCollection<MyImage> _images;
        private int _blankImage;
        #endregion

        #region Properties
        public ObservableCollection<MyImage> Images { get => _images; set { _images = value; OnPropertyChanged(); } }
        public int BlankImage { get => _blankImage; set { _blankImage = value; OnPropertyChanged(); } }
        public string TemplateImagePath { get; set; }
        public int MaxColums { get; set; }
        public int MaxRows { get; set; }
        #endregion

        #region Command
        public ICommand MouseMoveImageCommand { get; set; }
        #endregion

        public MainViewModel()
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;
            TemplateImagePath = $"{root}Image\\templateImage.jpg";
            MaxColums = MaxRows = 3;

            MouseMoveImageCommand = new RelayCommand<Image>((p) => { return true; }, (p) => { MessageBox.Show("a"); });
        }


        public class MyImage
        {
            public int Index { get; set; }
            public Image Image { get; set; }
        }
    }
}
