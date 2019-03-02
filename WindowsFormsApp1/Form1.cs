﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Google.Cloud.Vision.V1;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        int num = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            Column1.Width = 350;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\Judy\\costcoapi.json");
        }

        // Parses string[] into an object[4] with the resulting table data.
        public object[] parseDescription(string[] text)
        {
            int index = 1;

            string itemName = "";
            double initPrice = 0;
            double salePrice = 0;
            bool clearance = false;
            bool meat = false;

            foreach (string line in text)
            {
                if (line.Contains("$")) meat = true;

            }

            if (!meat)
            {
                foreach (string line in text)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        // Serial - No current usage
                        var isNumeric = int.TryParse(line, out int n);
                        //if (isNumeric && index == 1) serial = line.Trim();
                        // Item Name 
                        if (index < 5 && IsAllUpper(line) && !isNumeric) itemName += " " + line;

                        else if (line.Contains(".") && !clearance)
                        {
                            var isDouble = double.TryParse(line, out double d);
                            if (isDouble)
                            {
                                double amnt = Convert.ToDouble(line);

                                // Clearance
                                if (line.Last() == '7')
                                {
                                    initPrice = amnt;
                                    clearance = true;
                                }

                                // InitialPrice Discount and SalePrice
                                else if (amnt > initPrice) initPrice = amnt;
                                else if (amnt > (initPrice * 0.5)) salePrice = amnt;
                            }
                        }
                    }
                    index++;
                }
            }
            double discount = (initPrice - salePrice) / initPrice;

            return new object[] { itemName, initPrice, salePrice, discount, clearance, meat };
        }

        // Check if string is allcaps
        public bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                    return false;
            }
            return true;
        }

        // RETRIEVE ON CLICK
        private async void retrieve_Click(object sender, EventArgs e)
        {
            num = 1;

            var data = new MyWebClient().DownloadString(textBox1.Text);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(data);
            var htmlNodeList = doc.DocumentNode.SelectNodes("//dt[@class='gallery-icon landscape']");
            var nodeList = new List<string>();

            foreach (var node in htmlNodeList)
            {
                var src = node.SelectSingleNode(".//img").GetAttributeValue("src", "src not found");
                if (!src.ToString().Contains("Costco"))
                {
                    nodeList.Add(node.SelectSingleNode(".//img").GetAttributeValue("src", "src not found"));
                }
            }
            var link = htmlNodeList[0].GetAttributeValue("src", "not found");
            Console.WriteLine(nodeList[1]);

            
            foreach (string node in nodeList)
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(node, "node" + num.ToString() + ".png");
                    num++;
                }
            }

            for (int i = 1; i <= num; i++)
            {
                // String name of the file.
                string pic = "node" + i.ToString();

                // Load and Save nodes as cropped images (.jpg file format)
                if (File.Exists(pic + ".png"))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(pic + ".png");
                    cropImage(img).Save(pic + ".jpg");
                    string[] text = await RequestGoogleVisionAsync(pic);
                    object[] result = parseDescription(text);

                    // DEBUG WRITELINES
                    Console.WriteLine(text);
                    Console.WriteLine("Results: \n" + result[0]);
                    Console.WriteLine("Price: " + result[1]);
                    Console.WriteLine("Sale Price: " + result[2]);
                    Console.WriteLine("Clearance: " + result[3]);
                    Console.WriteLine("Meat: " + result[4]);

                    // ADD TO ROW
                    dataGridView1.Rows.Add(result[0], result[2], result[1], result[3], "click");

                }
            }

            for (int i = 1; i <= num; i++)
            {
                // String name of the file.
                string pic = "node" + i.ToString();

                // Load and Save nodes as cropped images (.jpg file format)
                if (File.Exists(pic + ".png"))
                {
                    

                }
            }
// authexplicit("favorable - valor - 224609", "c:\\users\\judy\\costcoapi.json");

        }


        // Crop a given image and return it.
        public Bitmap cropImage(System.Drawing.Image img)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(new Rectangle(0, 0, 400, 250), bmpImage.PixelFormat);
            return bmpCrop;
        }

        // HELPER
        private async Task<string[]> RequestGoogleVisionAsync(string filename)
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            Google.Cloud.Vision.V1.Image image = Google.Cloud.Vision.V1.Image.FromFile(filename+".jpg");
            IReadOnlyList<EntityAnnotation> textAnnotations = await client.DetectTextAsync(image);
            string[] text = textAnnotations[0].Description.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return text;
        }

        class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                return request;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine(e.GetType());
        }

        private void button1_click(object sender, EventArgs e)
        {

        }
    }
}
