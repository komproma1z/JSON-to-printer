﻿using System;
using System.Web.Script.Serialization;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;

namespace Printer
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser();

            string sampleName = Environment.GetCommandLineArgs()[0];
            
            new PrintingExample(@"pTalon_big.jpg");
        }

        private static void Parser()
        {

            var dataForPrinting = File.ReadAllText(@"webhook content.json");

            var JSONObj = new JavaScriptSerializer().Deserialize<ContentData>(dataForPrinting);

            var clientName = JSONObj.data.client.name;
            var clientPhone = JSONObj.data.client.phone;
            var staffData = JSONObj.data.staff;
            var date = JSONObj.data.datetime.ToString("yyyy/MM/dd HH:mm");

            Console.WriteLine(staffData);

            Console.WriteLine(clientName, clientPhone);

            Console.WriteLine($"Время занятия: </br> {date}");

            PointF firstLocation = new PointF(220f, 230f);
            PointF secondLocation = new PointF(360f, 550f);
            PointF thirdLocation = new PointF(220f, 680f);
            PointF fourthLocation = new PointF(200f, 1000f);

            string imageFilePath = @"talon_big.jpg";
            Bitmap bitmap = (Bitmap)Image.FromFile(imageFilePath);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font arialFont = new Font("Arial", 36))
                {
                    graphics.DrawString(staffData.ToString(), arialFont, Brushes.Black, firstLocation);
                    graphics.DrawString(clientName.ToString(), arialFont, Brushes.Black, secondLocation);
                    graphics.DrawString(clientPhone.ToString(), arialFont, Brushes.Black, thirdLocation);
                    graphics.DrawString(date, arialFont, Brushes.Black, fourthLocation);
                }
            }

            bitmap.Save("pTalon_big.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }

    class ContentData
    {
        public Data data { get; set; }
    }

    class Staff
    {
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    class Client
    {
        public string name { get; set; }
        public string phone { get; set; }

        public override string ToString()
        {
            return $"{name} </br> {phone}";
        }
    }

    class Data
    {
        public Staff staff { get; set; }
        public Client client { get; set; }
        public DateTime datetime { get; set; }
    }

    public class PrintingExample
    {
        private Font printFont;
        private StreamReader streamToPrint;
        private string filePath;


        public PrintingExample(string path)
        {
            filePath = path;
            Printing();
        }

        // The PrintPage event is raised for each page to be printed.
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {

            // Create image.
            Image newImage = Image.FromFile(@"pTalon_big.jpg");

            // Create point for upper-left corner of image.
            PointF ulCorner = new PointF(-20.0F, 0.0F);

            // Draw image to screen.
            ev.Graphics.DrawImage(newImage, ulCorner);
        }

        // Print the file.
        public void Printing()
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    printFont = new Font("Arial", 10);
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                    // Print the document.
                    pd.Print();
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
