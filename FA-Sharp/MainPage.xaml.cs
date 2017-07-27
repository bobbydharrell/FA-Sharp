using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace FA_Sharp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region HTML Parser
        public async void Parser()
        {
            progressRing.IsActive = true;
            progressRing.Visibility = Visibility.Visible;

            string html = await myWebview.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });

            //Clear Previous Results
            codeView.Text = string.Empty;

            //Load HTML into Document
            HtmlDocument mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(html);

            if (myWebview.Source == new Uri("http://fontawesome.io/cheatsheet/")) // CheetSheet
            {
                try
                {
                    // Header for Public Class

                    codeView.Text = codeView.Text + "public class FontAwesome" + Environment.NewLine + "{" + Environment.NewLine;
                    codeView.Text = codeView.Text + @"    public FontFamily fontFamily = new FontFamily(" + @"""" + @"/Assets/Fonts/fontawesome.ttf#FontAwesome" + @"""" + ");" + Environment.NewLine; // Font Family for this Font Class

                    //Body of Class - All the Icons
                    foreach (HtmlNode i in mainDoc.DocumentNode.Descendants("i"))
                    {
                        if (i.ParentNode.Name == "div")
                        {
                            string iconName = "fa_" + i.Attributes["title"].Value.Replace("Copy to use ", "").Replace("-", "_");
                            string iconCode = string.Empty;

                            if (i.ParentNode.Element("span").InnerText.Replace("[&amp;#", "").Replace(";]", "") == "(alias)")
                            {
                                iconCode = @"\" + i.NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Replace("[&amp;#", "").Replace(";]", "");
                            }
                            else
                            {
                                iconCode = @"\" + i.ParentNode.Element("span").InnerText.Replace("[&amp;#", "").Replace(";]", "");
                            }

                            codeView.Text = codeView.Text + @"    public const string " + iconName + " = " + @"""" + iconCode + @"""" + @";" + Environment.NewLine;
                        }
                    }

                    //Footer for Public Class
                    codeView.Text = codeView.Text + "}" + Environment.NewLine + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    //Dont Care
                }
            }
            else // Loacal FA5 Document
            {
                try
                {
                    //Root Node
                    HtmlNode renderings = mainDoc.GetElementbyId("renderings");

                    foreach (var section in renderings.Descendants("section"))
                    {
                        string id = section.GetAttributeValue("id", "Unknown");
                        IEnumerable<HtmlNode> sectionItems = section.ChildNodes.Elements();

                        string classText = await ParseFA5Node(sectionItems, id);
                        codeView.Text = codeView.Text + classText;
                    }
                }
                catch (Exception ex)
                {
                    //Dont Care
                }
            }

            progressRing.IsActive = false;
            progressRing.Visibility = Visibility.Collapsed;
        }

        public async Task<string> ParseFA5Node(IEnumerable<HtmlNode> sectionItems, string id)
        {
            string result = string.Empty;

            try
            {
                string className = (sectionItems.ElementAt(0) as HtmlNode).InnerText.Replace(" ", "").Replace("5", "").Replace("&amp; Logos", "").Trim();
                string iconCount = (sectionItems.ElementAt(1) as HtmlNode).InnerText.Trim();

                //Header for Public Class
                result = result + "public class " + className + @" //" + iconCount + Environment.NewLine + "{" + Environment.NewLine;
                result = result + @"    public FontFamily fontFamily = new FontFamily(" + @"""" + @"/Assets/Fonts/fontawesome-" + (sectionItems.ElementAt(0) as HtmlNode).InnerText.Replace("Font Awesome 5", "").Replace("&amp; Logos", "").ToLower().Trim() + ".ttf#" + (sectionItems.ElementAt(0) as HtmlNode).InnerText.Replace("&amp; Logos", "").Trim() + @"""" + ");" + Environment.NewLine; // Font Family for this Font Class

                //Body of Class - All the Icons
                foreach (HtmlNode item in sectionItems)
                {
                    //Format of Class Items - public const string name = "\x####";
                    if (item.Name == "li") // List Item Containing Icon Information
                    {
                        var icon = from nodes in item.ChildNodes.Elements()
                                   where nodes.Name == "span" &&
                                        nodes.Attributes["class"] != null &&
                                        nodes.InnerText.Trim().Length > 0
                                   select nodes;

                        string iconName = "fa_" + (icon.ElementAt(0) as HtmlNode).InnerText.Replace("-", "_").Trim();
                        string iconText = (icon.ElementAt(1) as HtmlNode).InnerText.Trim();
                        string iconCode = @"\x" + iconText.Substring(iconText.IndexOf("f"),4).Trim();

                        if (!string.IsNullOrEmpty(iconName) && !string.IsNullOrEmpty(iconCode))
                        {
                            result = result + @"    public const string " + iconName + " = " + @"""" + iconCode + @"""" + @";" + Environment.NewLine;
                        }
                    }
                }

                //Footer for Public Class
                result = result + "}" + Environment.NewLine + Environment.NewLine;
            }
            catch(Exception ex)
            {
                //Dont Care
            }

            return result;
        }
        #endregion

        #region Events
        private async void MyWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Parser();
        }

        private async void LoadWebFontDoc_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".html");
            StorageFile myFile = await picker.PickSingleFileAsync();

            myWebview.NavigateToString(await FileIO.ReadTextAsync(myFile));
        }

        private void cheatSheetParser_Click(object sender, RoutedEventArgs e)
        {
            myWebview.Navigate(new Uri("http://fontawesome.io/cheatsheet/"));
        }
        #endregion
    }
}
