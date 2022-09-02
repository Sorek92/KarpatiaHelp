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
            writeFile = new File("posortowane.txt");
            ordersList = new List<Order>();
            articlesList = new List<Article>();
            numberColumnList = new List<int>();
            namesColumnList = new List<string>();

            // 
            this.textBoxForLoadFile.Text = readFile.FileName;
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
            textBoxForLoadFile.Text = readFile.LoadFile();
        }

        private void BtnLoadFile_Click(object sender, RoutedEventArgs ev)
        {
            // zmiana pliku do wczytania
            readFile.ChangePath(textBoxForLoadFile.Text);

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

            for (int i = 0; i < ordersList.Count; i++)
            {
                ListBox l = new ListBox();
                l.SelectionMode = SelectionMode.Multiple;
                l.Width = 200;
                l.Items.Add(ordersList[i].customer + " " + ordersList[i].orderNumber);
                l.Margin = new Thickness(0, 20, 0, 20);

                foreach (var article in ordersList[i].articles)
                {
                    if (article.wg_jm2 == "TAK" || article.wg_jm2 == "tak")
                    {

                        l.Items.Add(article.name + "  " + article.unitNumber2 + " " + article.unit2);

                    }
                    else
                    {
                        l.Items.Add(article.name + "  " + article.unitNumber1 + " " + article.unit1);
                    }
                }

                panelOrder.Children.Add(l);

            }

            /*for (int i = 0; i < ordersList.Count; i++)
            {
                ListBox l = new ListBox();
                l.SelectionMode = SelectionMode.Multiple;
                l.Width = 200;
                l.Items.Add(ordersList[i].customer + " " + ordersList[i].orderNumber);
                l.Margin = new Thickness(0,20,0,20);
                foreach(var article in ordersList[i].articles)
                {
                    l.Items.Add(article.name + " " + article.price);
                }

                panelOrder.Children.Add(l);

            }*/




            /*            listBoxAllOrders.Items.Clear();

                        List<int> selectList = new List<int>();

                        foreach (var order in ordersList)
                        {

                            int itemListNumber = listBoxAllOrders.Items.Add(order.customer.ToUpper() + "\t" + order.orderNumber);


                            // zapis nazwy zamówienia
                            selectList.Add(itemListNumber);

                            foreach (var article in order.articles)
                            {

                                if (article.wg_jm2 == "TAK" || article.wg_jm2 == "tak")
                                {

                                    var ii = listBoxAllOrders.Items.Add(article.name + "  " + article.unitNumber2 + " " + article.unit2);

                                    if ((bool)checkBoxMeat.IsChecked && article.isMeatCut)
                                    {
                                        Console.WriteLine("zaznaczono mięso {0} {1}", listBoxAllOrders.Items.Count - 1, ii);

                                        selectList.Add(ii);

                                    }

                                    if ((bool)checkBoxSausage.IsChecked && article.isSausageCut)
                                    {
                                        Console.WriteLine("zaznaczono wędline {0} {1}", listBoxAllOrders.Items.Count - 1, ii);

                                        selectList.Add(ii);
                                    }
                                }
                                else
                                {

                                    var ii = listBoxAllOrders.Items.Add(article.name + "  " + article.unitNumber1 + " " + article.unit1);
                                    if ((bool)checkBoxMeat.IsChecked && article.isMeatCut)
                                    {
                                        Console.WriteLine("zaznaczono {0}", listBoxAllOrders.Items.Count - 1);
                                        selectList.Add(ii);
                                    }

                                    if ((bool)checkBoxSausage.IsChecked && article.isSausageCut)
                                    {
                                        Console.WriteLine("zaznaczono wędline {0} {1}", listBoxAllOrders.Items.Count - 1, ii);

                                        selectList.Add(ii);
                                    }
                                }

                            }

                            Console.Write("selected list before: ");
                            for (int j=0; j< selectList.Count; j++)
                            {
                                Console.Write(selectList[j] + " ; " );
                            }
                            Console.WriteLine();
                            selectList.Sort();

                            Console.Write("selected list po: ");
                            for (int j = 0; j < selectList.Count; j++)
                            {
                                Console.Write(listBoxAllOrders.Items[j] + " ; ");
                            }

                            Console.WriteLine();
                            if (selectList.Count > 1)
                            {
                                for(int i = 0; i < selectList.Count; i++)
                                {
                                    listBoxAllOrders.SelectedItems.Add(listBoxAllOrders.Items[selectList[i]]);
                                    Console.WriteLine(selectList[i]);
                                }
                                selectList.Clear();
                            }
                            else
                            {
                                selectList.Clear();
                            }




                        }

                        foreach (var o in listBoxAllOrders.Items)
                        {
                            Console.WriteLine();
                        }*/


        }


        private void BtnMoveItemsToPrint_Click(object sender, RoutedEventArgs e)
        {

            listBoxPrint.Text = "";
            foreach (ListBox lb in panelOrder.Children)
            {
                if (lb.SelectedItems.Count > 0)
                {
                    lb.SelectedItems.Add(lb.Items[0]);
                }
                foreach (var item in lb.Items)
                {

                    if (lb.SelectedItems.Contains(item))
                        //listBoxPrint.Items.Add(item.ToString());
                        listBoxPrint.Text += item.ToString() + "\n";
                }

                //listBoxPrint.Items.Add("");
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
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

                XRect rec = new XRect(0, 0, page.Width, page.Height);

                string word = "";

                for (int i = 0; i < listBoxPrint.Text.Length; i++)
                {
                    if (listBoxPrint.Text[i] != '\n')
                    {
                        word += listBoxPrint.Text[i];
                    }
                    else
                    {
                        gfx.DrawString(word, font, XBrushes.Black, rec, XStringFormats.TopLeft);
                        rec.Y += 25;
                        word = "";
                    }


                }


                //Draw the text


                //Save the document
                string filename = "HelloWorld.pdf";
                document.Save(filename);
            }
        }
    }
}
