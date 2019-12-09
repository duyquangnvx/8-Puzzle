using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace _8_Puzzle.Dialog
{
    /// <summary>
    /// Interaction logic for GameOverDialog.xaml
    /// </summary>
    public partial class GameOverDialog : UserControl
    {
        public string Title { get; set; }

        public GameOverDialog(string title) : this()
        {
            Title = title;
        }

        public GameOverDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
