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

namespace AoWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public StackPanel HeadStackPanel { set; get; } = new StackPanel()
        //{
        //    //Children = new UIElementCollection(null, null);
        //};

        public MainWindow()
        {
            InitializeComponent();

            BaseTabItem.MouseDoubleClick += (a, b) =>
            {
                var head = ((TabItem)a).Header;
                ;
            };

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tabText = AddTabButtonText.Text;
            MyTabItem myTabItem = new MyTabItem(tabText);
            var content = myTabItem.Content;

            var uc1 = new UserControl1();
            uc1.InitializeComponent();
            uc1.FirstButtonInUserControl1.Content = uc1.FirstButtonInUserControl1.Content + tabText;
            content = uc1;

            //content = BaseTabItem.Content;

            //content = new Button()
            //{
            //    Content = tabText
            //};


            MainTabControl.Items.Add(myTabItem);





            myTabItem.ClosedClick += (a, b) =>
            {


                MainTabControl.Items.Remove(myTabItem);
            };

        }

    }

    public class MyTabStackPanel : StackPanel
    {
        public MyTabStackPanel(string headerText)
        {
            HeadTextBlock.Text = headerText;
            Children.Add(HeadTextBlock);
            CloseButton.Content = "×";
            CloseButton.BorderThickness = new Thickness(0);
            CloseButton.Margin = new Thickness(5, 0, 0, 0);
            Children.Add(CloseButton);
            Orientation = Orientation.Horizontal;
        }

        public TextBlock HeadTextBlock { get; set; } = new TextBlock();
        public Button CloseButton = new Button();
        public TabItem TabItem { get; set; }

    }

    public class MyTabItem : TabItem
    {
        public MyTabItem(string headerText)
        {
            Header = new MyTabStackPanel(headerText: headerText);
            ((MyTabStackPanel)Header).CloseButton.Click += (a, b) =>
            {
                ClosedClick?.Invoke(this, b);
            };
            StackPanel stackPanel = new StackPanel();
         
        }
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler ClosedClick;
    }



}
