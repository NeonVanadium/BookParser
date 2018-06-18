using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BookParser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InterfacePage : Page
    {

        public static InterfacePage cur;
        public Parser p { get; set; }
        private WordTracker w;

        public InterfacePage()
        {
            this.InitializeComponent();
            cur = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clearText();
            p = new Parser("Assets/" + ((MenuFlyoutItem)sender).Text + ".txt");
            ChapterDisplay.ItemsSource = p.chapters;
            TextStatsBlock.Text = p.fullTextWordTracker.getStats();
        }

        private void ChapterDisplay_ItemClick(object sender, ItemClickEventArgs e)
        {
            w = ((WordTracker)e.ClickedItem);
            updateText();
        }

        private void clearText()
        {
            SectionTitleBlock.Text = "";
            SectionInfoBlock.Text = "(No section selected)";
            SearchBox.Text = "";
        }

        private void updateText()
        {
            if(w != null)
            {
                SectionTitleBlock.Text = w.name;
                SectionInfoBlock.Text = w.getStats();
            }
            
            TextStatsBlock.Text = p.fullTextWordTracker.getStats();
        }

        private void TermSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(SearchBox.Text))  p.searchTerm = SearchBox.Text;
            updateText();
        }

        public static string getSearchTerm()
        {
            return cur.p.searchTerm;
        }
    }
}
