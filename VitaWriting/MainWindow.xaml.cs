using System.Windows;
using VitaWriting.Utils;

namespace VitaWriting
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FileUtil.SaveResource("Settings.yml", "Test.yml");
        }
    }
}