using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarpatiaHelp
{

    internal class Article
    {
        public string name;// artykuł
        public string wg_jm2; // czy jednostka miary 2
        public string unit1; // jednostka 1 [kg]
        public string unit2; // jednostka 2 [szt]
        public double unitNumber1; // ilosc jednostki miary 1
        public double unitNumber2; // ilosc jednostki miary 2
        public double price; // cena
        public bool isMeatCut;
        public bool isSausageCut;
        public bool isChickenCut;

        public Article()
        {
            // ustawienie początkowych własności
            clear();
        }

        public Article(string name, string wg_jm2, string unit1, string unit2, double unitNumber1, double unitNumber2, double price)
        {
            this.name = name;
            this.wg_jm2 = wg_jm2;
            this.unit1 = unit1;
            this.unit2 = unit2;
            this.unitNumber1 = unitNumber1;
            this.unitNumber2 = unitNumber2;
            this.price = price;

            this.isMeatCut = IsMeatCutting();
            this.isSausageCut = IsSausageCutting();
            this.isChickenCut = IsChickenCutting();
        }

        public void show()
        {
            Console.WriteLine("name:{0} wg_jm2:{1} unit1:{2} unit2:{3} unitNumber1:{4} unitNumber2:{5} price:{6}", name, wg_jm2, unit1, unit2, unitNumber1, unitNumber2, price);
        }

        public void clear()
        {
            this.name = "";
            this.wg_jm2 = "";
            this.unit1 = "";
            this.unit2 = "";
            this.unitNumber1 = 0.0;
            this.unitNumber2 = 0.0;
            this.price = 0.0;
        }

        public void copy(Article ar)
        {
            this.name = ar.name;
            this.wg_jm2 = ar.wg_jm2;
            this.unit1 = ar.unit1;
            this.unit2 = ar.unit2;
            this.unitNumber1 = ar.unitNumber1;
            this.unitNumber2 = ar.unitNumber2;
            this.price = ar.price;
        }

        public bool IsMeatCutting()
        {
            bool answer = false;

            switch (this.name)
            {
                case string n when n.Contains("stekowana"):
                    //Console.WriteLine("zaznaczono stekowana: {0}", this.name);
                    answer = true;
                    break;
                case string n when n.Contains("stek"):
                    //Console.WriteLine("zaznaczono stek: {0}", this.name);
                    answer = true;
                    break;
                case string n when n.Contains("kostka"):
                    if (!n.Contains("smalec"))
                    {
                        //Console.WriteLine("zaznaczono kostka: {0}", this.name);
                        answer = true;
                    }
                    else
                    {
                        //Console.WriteLine("zaznaczono kostkę ale smalec: {0}", this.name);
                        answer = false;
                    }

                    break;
                case string n when n.Contains("ze skrzydełkiem"):
                    //Console.WriteLine("zaznaczono stek: {0}", this.name);
                    answer = true;
                    break;
                case string n when n.Contains("bez lotki"):
                    //Console.WriteLine("zaznaczono stek: {0}", this.name);
                    answer = true;
                    break;
                case string n when n.Contains("ze skórą"):
                    //Console.WriteLine("zaznaczono stek: {0}", this.name);
                    answer = true;
                    break;
            }


            return answer;
        }

        public bool IsSausageCutting()
        {
            bool answer = false;

            switch (this.name)
            {
                case string n when n.Contains("plastry"):
                    if (!n.Contains("gotowany") && !n.Contains("długa") && !n.Contains("goleń"))
                    {
                        //Console.WriteLine("zaznaczono wędline: {0}", this.name);
                        answer = true;
                    }
                    else
                    {
                        //Console.WriteLine("zaznaczono wędline ale boczek: {0}", this.name);
                        answer = false;
                    }

                    break;
                case string n when n.Contains("plast."):
                    Console.WriteLine("zaznaczono wędline: {0}", this.name);
                    answer = true;

                    break;
            }


            return answer;
        }

        public bool IsChickenCutting()
        {
            bool answer = false;

            switch (this.name)
            {
                case string n when n.Contains("b/k/b/s"):
                    answer = true;

                    break;
                case string n when n.Contains("z/k z/s"):
                    
                    answer = true;

                    break;
                case string n when n.Contains("z/s b/k"):

                    answer = true;

                    break;
                case string n when n.Contains("kebab"):

                    answer = true;

                    break;
            }


            return answer;
        }
    }
}
