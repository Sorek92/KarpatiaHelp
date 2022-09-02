using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarpatiaHelp
{

    internal class Order
    {
        public string customer;// logo
        public string orderNumber;//dokument
        public List<Article> articles;// wszystkie artykuły
        public string remarks; // uwagi
        public string remarksOdb; // uwagi grube
        public string date;

        public Order()
        {
            this.articles = new List<Article>();

            clear();

        }

        public Order(string customer, string orderNumber, List<Article> articles, string remarks, string ramarks_odb, string date)
        {
            this.customer = customer;
            this.orderNumber = orderNumber;
            this.articles = new List<Article>();

            foreach (Article article in articles)
            {
                this.articles.Add(new Article(article.name, article.wg_jm2, article.unit1, article.unit2, article.unitNumber1, article.unitNumber2, article.price));
            }

            this.remarks = remarks;
            this.remarksOdb = ramarks_odb;
            this.date = date;
        }

        public void showOrder()
        {
            Console.WriteLine("customer:{0} orderNumber:{1} remarks:{2} remarks_odb:{3} date:{4}", customer, orderNumber, remarks, remarksOdb, date);

            foreach (var item in articles)
            {
                Console.WriteLine("name:{0} wg_jm2:{1} unit1:{2} unit2:{3} unitNumber1:{4} unitNumber2:{5} price:{6}",
                    item.name, item.wg_jm2, item.unit1, item.unit2, item.unitNumber1, item.unitNumber2, item.price);
            }
        }

        public void clear()
        {
            this.customer = "";
            this.orderNumber = "";
            if (this.articles.Count > 0)
            {
                this.articles.Clear();
            }
            this.remarks = "";
            this.remarksOdb = "";
            this.date = "";
        }

    }
}
