using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Printing;
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
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Advanced;

namespace KarpatiaHelp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private File readFile;
        private File writeFile;
        private List<Order> ordersList;
        private List<Article> articlesList;

        private List<int> numberColumnList;// kolumny do skopiowania
        private List<string> namesColumnList; // takie same indeksy co numberColumnList

        private int n = 100;

        public MainWindow()
        {
            InitializeComponent();
            readFile = new File("dane.txt");
            writeFile = new File("krojenie.txt");
            ordersList = new List<Order>();
            articlesList = new List<Article>();
            numberColumnList = new List<int>();
            namesColumnList = new List<string>();

            // 
            //this.textBoxForLoadFile.Content = readFile.FileName;


            btnLoadFile.IsEnabled = false;
            moveItemsToPrint.IsEnabled = false;
            moveItemsToPrint.Visibility = Visibility.Hidden;
            btnSortOrders.IsEnabled = false;
        }

        /*
         * Metody zarządzania oknem
         */

        // zmniejszanie na pasek
        private void btnMinimalize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        // powiekszanie na cały ekran -- wyłaczona opcja
        private void btnMaximilize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;

            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        // zamykanie okna
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // przesuniecie okna - narazie przyłaczone do tytułu
        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }



        /*
         * Metody przycisków
         */
        private void BtnFindFile_Click(object sender, RoutedEventArgs e)
        {
            textBoxForLoadFile.Content = readFile.LoadFile();
            btnLoadFile.IsEnabled = true;
            btnLoadFile.Content = "Wczytaj";
        }

        private void BtnLoadFile_Click(object sender, RoutedEventArgs ev)
        {
            // zmiana pliku do wczytania
            readFile.ChangePath(textBoxForLoadFile.Content.ToString());

            Encoding encoding = Encoding.Default;
            
            // wczytanie pliku
            try
            {

                using (StreamReader sr = new StreamReader(readFile.Path, encoding))
                {
                    
                    string line;
                    int option = 1;
                    string option2 = "";
                    string prevOrder = "ZA000001";
                    int numberOfLine = 0;
                    int numberOfOrder = 0;

                    // czyszczenie list
                    ordersList.Clear();
                    numberColumnList.Clear();
                    namesColumnList.Clear();
                    articlesList.Clear();


                    Order oldOrder = new Order();

                    // czytanie po linii
                    while ((line = sr.ReadLine()) != null)
                    {


                        switch (option)
                        {
                            // zapis nazw kolumn do listy
                            case 1:
                                SaveColumn(line);
                                option = 2;
                                break;

                            // zapis zamowienia
                            case 2:

                                Order newOrder = new Order();
                                Article article = new Article();


                                string word = "";
                                int numberCol = 0;


                                for (int i = 0; i < line.Length; i++)
                                {
                                    // dopoki nie ma średnika zapisuj do słowa
                                    if (line[i] != ';')
                                    {
                                        word += line[i];
                                    }
                                    else
                                    {
                                        // sprawdz czy znajduje sie w liscie
                                        for (int j = 0; j < numberColumnList.Count; j++)
                                        {

                                            if (numberCol == numberColumnList[j])
                                            {
                                                option2 = namesColumnList[j];

                                                string charReplace;

                                                // sprawdzenie która kolumna
                                                switch (option2)
                                                {
                                                    // zamówienie
                                                    case "DOKUMENT":
                                                        if (newOrder.orderNumber == "")
                                                        {
                                                            newOrder.orderNumber = word.ToLower();
                                                        }
                                                        break;

                                                    case "DATA_DST":
                                                        if (newOrder.date == "")
                                                            newOrder.date = word.ToLower();
                                                        break;
                                                    case "LOGO":
                                                        if (newOrder.customer == "")
                                                            newOrder.customer = word.ToLower();
                                                        break;
                                                    case "UWAGI_OBD":
                                                        if (newOrder.remarksOdb == "")
                                                            newOrder.remarksOdb = word.ToLower();
                                                        break;
                                                    case "UWAGI":
                                                        if (newOrder.remarks == "")
                                                            newOrder.remarks = word.ToLower();
                                                        break;

                                                    // artykuły
                                                    case "ARTYKUL":
                                                        article.name = word.ToLower();
                                                        break;
                                                    case "WG_JM2":
                                                        article.wg_jm2 = word.ToLower();
                                                        break;
                                                    case "JM":
                                                        article.unit1 = word.ToLower();
                                                        break;
                                                    case "ILOSC":
                                                        charReplace = word.Replace('.', ',');
                                                        article.unitNumber1 = double.Parse(charReplace);
                                                        break;
                                                    case "JM2":
                                                        article.unit2 = word.ToLower();
                                                        break;
                                                    case "ILOSC_JM2":
                                                        charReplace = word.Replace('.', ',');
                                                        article.unitNumber2 = double.Parse(charReplace);
                                                        break;
                                                    case "CENA_NETTO":
                                                        charReplace = word.Replace('.', ',');
                                                        article.price = double.Parse(charReplace);
                                                        break;

                                                    default:
                                                        Console.WriteLine("jakis błąd");
                                                        break;


                                                }


                                            }
                                        }

                                        // reset słowa, przejscie do nowej linii
                                        word = "";
                                        numberCol++;
                                    }



                                }

                                if (newOrder.orderNumber != prevOrder)
                                {
                                    // zapis że poprzednie to nowe 
                                    prevOrder = newOrder.orderNumber;

                                    // swprawdz czy lista istnieje
                                    if (articlesList.Count > 0)
                                    {
                                        ordersList.Add(new Order(oldOrder.customer, oldOrder.orderNumber, articlesList, oldOrder.remarks, oldOrder.remarksOdb, oldOrder.date));
                                    }

                                    // zapisz do nowego zamowienia 
                                    oldOrder.customer = newOrder.customer;
                                    oldOrder.orderNumber = newOrder.orderNumber;
                                    oldOrder.remarks = newOrder.remarks;
                                    oldOrder.remarksOdb = newOrder.remarksOdb;
                                    oldOrder.date = newOrder.date;

                                    // zerowanie listy
                                    articlesList.Clear();

                                    numberOfOrder++;

                                }

                                // zapis artykułow do listy
                                articlesList.Add(new Article(article.name, article.wg_jm2, article.unit1, article.unit2, article.unitNumber1, article.unitNumber2, article.price));

                                break;

                            case 3:
                                break;
                            default:
                                Console.WriteLine("błąd");
                                break;
                        }


                        numberOfLine++;
                    }

                    if (articlesList.Count > 0)
                    {
                        // zapis ostatniego zamowienia 
                        ordersList.Add(new Order(oldOrder.customer, oldOrder.orderNumber, articlesList, oldOrder.remarks, oldOrder.remarksOdb, oldOrder.date));

                    }

                    articlesList.Clear();

                    btnLoadFile.Content = "Wczytano plik";
                    btnLoadFile.IsEnabled = false;
                    btnSortOrders.IsEnabled = true;


                }

            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                MessageBox.Show("Nie mozna otworzyc pliku " + e.Message);
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }


        }

        // zaznaczenie elementów do krojenia
        private void BtnSortOrders_Click(object sender, RoutedEventArgs e)
        {

            panelOrder.Children.Clear();
            listBoxPrint.Text = "";

            for (int i = 0; i < ordersList.Count; i++)
            {
                ListBox l = new ListBox();
                l.SelectionMode = SelectionMode.Multiple;
                
                l.Items.Add("\t" + ordersList[i].customer + " " + ordersList[i].orderNumber);
                l.Margin = new Thickness(20, 5, 20, 5);
                string text = "";
                foreach (var article in ordersList[i].articles)
                {
                    if (article.wg_jm2 == "TAK" || article.wg_jm2 == "tak")
                    {

                        text = article.name + "  " + article.unitNumber2 + " " + article.unit2;

                    }
                    else
                    {
                        text = article.name + "  " + article.unitNumber1 + " " + article.unit1;
                    }

                    l.Items.Add(text);

                    if ((bool)checkBoxMeat.IsChecked && article.isMeatCut)
                    {
                        l.SelectedItems.Add(text);
                    }

                    if ((bool)checkBoxSausage.IsChecked && article.isSausageCut)
                    {
                        l.SelectedItems.Add(text);
                    }

                    if ((bool)checkBoxChicken.IsChecked && article.isChickenCut)
                    {
                        l.SelectedItems.Add(text);
                    }
                    text = "";

                }
                l.Items.Add("Uwagi: " + ordersList[i].remarks);

                panelOrder.Children.Add(l);

            }

            if(panelOrder.Children.Count > 0)
            moveItemsToPrint.IsEnabled = true;
            moveItemsToPrint.Visibility = Visibility.Visible;


            string folder = Directory.GetCurrentDirectory() + "\\";
            listBoxPrint.Text += "\n" + folder;

        }


        private void BtnMoveItemsToPrint_Click(object sender, RoutedEventArgs e)
        {

            listBoxPrint.Text = "";
            foreach (ListBox lb in panelOrder.Children)
            {
                if (lb.SelectedItems.Count > 0)
                {
                    if (lb.SelectedItems.Count == 1 && lb.SelectedItems.Contains(lb.Items[0]))
                    {
                        lb.SelectedItems.Remove(lb.Items[0]);
                    }
                    else
                    {
                        lb.SelectedItems.Add(lb.Items[0]);
                    }
                    
                }
                foreach (var item in lb.Items)
                {

                    if (lb.SelectedItems.Contains(item))
                        //listBoxPrint.Items.Add(item.ToString());
                        listBoxPrint.Text += item.ToString() + "\n";
                }

                //listBoxPrint.Items.Add("");
                if(lb.SelectedItems.Count > 0)
                listBoxPrint.Text += "\n";


            }

            /*listBoxPrint.Items.Clear();

            Console.WriteLine(listBoxAllOrders.SelectedValue);



            for (int i = 0; i < listBoxAllOrders.Items.Count; i++)
            {
               //f(listBoxAllOrders.selecte
            }*/

        }


        // sortowanie do tablic ale chyba nie potrzebne
        private void SortList()
        {

            string resultNumber1 = "";
            string resultName1 = "";
            for (int k = 0; k < numberColumnList.Count; k++)
            {
                resultNumber1 += numberColumnList[k].ToString() + " ; ";
                resultName1 += namesColumnList[k] + " ; ";
            }
            Console.WriteLine("przed sortowaniem");
            Console.WriteLine("lista numer: " + resultNumber1);
            Console.WriteLine("lista name : " + resultName1);

            int tempSortNumber;
            string tempSortName;
            for (int i = 0; i < numberColumnList.Count; i++)
            {
                for (int j = i + 1; j < numberColumnList.Count; j++)
                {

                    if (numberColumnList[i] >= numberColumnList[j])    // Sprawdz czy dany element tablicy nieposortowanej jest mniejszy 
                    {                           // od elementów tablicy posortowanej. Jeśli tak to zamień elementy
                        tempSortNumber = numberColumnList[i];         // miejscami.
                        numberColumnList[i] = numberColumnList[j];
                        numberColumnList[j] = tempSortNumber;

                        tempSortName = namesColumnList[i];         // miejscami.
                        namesColumnList[i] = namesColumnList[j];
                        namesColumnList[j] = tempSortName;
                    }
                }
            }

            string resultNumber = "";
            string resultName = "";
            for (int k = 0; k < numberColumnList.Count; k++)
            {
                resultNumber += numberColumnList[k].ToString() + " ; ";
                resultName += namesColumnList[k] + " ; ";
            }
            Console.WriteLine("po sortowaniu");
            Console.WriteLine("lista numer: " + resultNumber);
            Console.WriteLine("lista name : " + resultName);


        }

        // sprawdzenie tylko pierwszej linii i zwrócenie numerów i nazw kolumn
        private void SaveColumn(string line)
        {

            string word = "";
            int numberCol = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != ';')
                {
                    word += line[i];
                }
                else
                {
                    // potrzebne kolumny
                    // DOKUMENT; ARTYKUŁ; WG_JM2; JM; ILOSC; JM2; ILOSC_JM2; CENA_NETTO; DATA_DST; LOGO; UWAGI_OBD; UWAGI

                    switch (word)
                    {
                        case "DOKUMENT":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("DOKUMENT");
                            break;
                        case "ARTYKUL":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("ARTYKUL");
                            break;
                        case "WG_JM2":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("WG_JM2");
                            break;
                        case "JM":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("JM");
                            break;
                        case "ILOSC":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("ILOSC");
                            break;
                        case "JM2":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("JM2");
                            break;
                        case "ILOSC_JM2":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("ILOSC_JM2");
                            break;
                        case "CENA_NETTO":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("CENA_NETTO");
                            break;
                        case "DATA_DST":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("DATA_DST");
                            break;
                        case "LOGO":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("LOGO");
                            break;
                        case "UWAGI_OBD":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("UWAGI_OBD");
                            break;
                        case "UWAGI":
                            numberColumnList.Add(numberCol);
                            namesColumnList.Add("UWAGI");
                            break;
                        default:

                            break;
                    }
                    word = "";
                    numberCol++;
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("wynik.txt");

            sw.Write(listBoxPrint.Text);
            string folder = Directory.GetCurrentDirectory() + "\\";
            MessageBox.Show("Zapisano w pliku: " + folder + "wynik.txt");
            sw.Close();

        }

        private void btnPdf_Click(object sender, RoutedEventArgs e)
        {
            using (PdfDocument document = new PdfDocument())
            {
                //Add a page to the document
                PdfPage page = document.AddPage();

                //Create PDF graphics for a page
                XGraphics gfx = XGraphics.FromPdfPage(page);

                //Set the standard font
                XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

                XRect rec = new XRect(20, 20, page.Width, page.Height);

                string word = "";

                for (int i = 0; i < listBoxPrint.Text.Length; i++)
                {
                    if (listBoxPrint.Text[i] != '\n')
                    {
                        if (listBoxPrint.Text[i] != '\t')
                        word += listBoxPrint.Text[i];
                    }
                    else
                    {   
                        if(rec.Y > page.Height - 20)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            rec = new XRect(20, 20, page.Width, page.Height);
                        }
                        gfx.DrawString(word, font, XBrushes.Black, rec, XStringFormats.TopLeft);
                        rec.Y += 20;
                        word = "";
                    }


                }


                //Draw the text


                //Save the document
                string folder = Directory.GetCurrentDirectory() + "\\";
                string filename = folder + "DoKrojenia.pdf";
                try
                {

                    //document.Close();
                    MessageBox.Show("Zapisano w pliku: " + filename);
                    document.Save(filename);

                    
                }
                catch (Exception ev)
                {
                    MessageBox.Show(ev.Message);
                }
                
            }
        }
    }
}
