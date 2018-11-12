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
using AoCli;
using System.Reflection;

namespace BatchImport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {




            InitializeComponent();

            var clc = new AoCli.CommandLineController()
            {
            };

            System.Reflection.PropertyInfo[] properties = clc.GetType().GetProperties();

            string infos = String.Empty;
            properties.ToList().ForEach((property) =>
            {
                //property.CustomAttributes.
                infos += $"{property.Name} {property.PropertyType.Name}\n";
            });
            //MessageBox.Show(infos);
        }
    }
}
