using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using IronOcr;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using System.Drawing;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            Column1.Width = 350;
            for (int i = 1; i < 2; i++)
            {
                string pic = "node" + i.ToString();
                System.Drawing.Image img = System.Drawing.Image.FromFile(pic + ".png");
                Bitmap bmpImage = new Bitmap(img);
                Bitmap bmpCrop = bmpImage.Clone(new Rectangle(0, 0, 400, 250), bmpImage.PixelFormat);
                bmpCrop.Save(pic + ".jpg");
                Google.Cloud.Vision.V1.Image image = Google.Cloud.Vision.V1.Image.FromFile(pic + ".jpg");
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\Judy\\costcoapi.json");
                ImageAnnotatorClient client = ImageAnnotatorClient.Create();
                IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);
                string[] text = textAnnotations[0].Description.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(textAnnotations[0].Description);


                object[] result = parseDescription(text);
                Console.WriteLine("Results: \n" + result[0]);
                Console.WriteLine("Price: " + result[1]);
                Console.WriteLine("Sale Price: " + result[2]);
                Console.WriteLine("Clearance: " + result[3]);
                Console.WriteLine("Meat: " + result[4]);

                dataGridView1.Rows.Add(result[0], result[2], result[1], result[3], "click");

            }



        }

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
            return new object[] { itemName, initPrice, salePrice, (initPrice - salePrice) / initPrice, clearance, meat };
        }

        public bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                    return false;
            }
            return true;
        }

        private void retrieve_Click(object sender, EventArgs e)
        {
            var data = new MyWebClient().DownloadString(textBox1.Text);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(data);
            //Console.WriteLine(data);
            var htmlNodeList = doc.DocumentNode.SelectNodes("//dt[@class='gallery-icon landscape']");
            var nodeList = new List<string>();

            //foreach (var node in htmlNodeList)
            //{
            //    var src = node.SelectSingleNode(".//img").GetAttributeValue("src", "src not found");
            //    if (!src.ToString().Contains("Costco"))
            //    {
            //        nodeList.Add(node.SelectSingleNode(".//img").GetAttributeValue("src", "src not found"));
            //    }
            //}
            //var link = htmlNodeList[0].GetAttributeValue("src", "not found");
            //Console.WriteLine(nodeList[1]);

            //var num = 1;
            //foreach (string node in nodeList)
            //{
            //    using (WebClient webClient = new WebClient())
            //    {
            //        webClient.DownloadFile(node, "node" + num.ToString() + ".png");
            //        num++;
            //    }
            //}



            //   var ocr = new advancedocr()
            //    {
            //       cleanbackgroundnoise = true,
            //       enhancecontrast = true,
            //       enhanceresolution = true,
            //       language = ironocr.languages.english.ocrlanguagepack,
            //       strategy = ironocr.advancedocr.ocrstrategy.advanced,
            //       colorspace = advancedocr.ocrcolorspace.color,
            //       detectwhitetextondarkbackgrounds = false,
            //       inputimagetype = advancedocr.inputtypes.document,
            //       rotateandstraighten = true,
            //       readbarcodes = false,
            //       colordepth = 4
            //   };
            //  var area = new rectangle() { x = 0, y = 0, height = 250, width = 400 };
            //   var results = ocr.read("test.png");
            //   console.writeline(results);

            // authexplicit("favorable - valor - 224609", "c:\\users\\judy\\costcoapi.json");

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


    }
}
