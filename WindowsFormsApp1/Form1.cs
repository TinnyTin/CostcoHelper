using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using IronOcr;

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

        }



        private void retrieve_Click(object sender, EventArgs e)
        {
            var data = new MyWebClient().DownloadString(textBox1.Text);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(data);
            //Console.WriteLine(data);
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
            //var link = htmlNodeList[0].GetAttributeValue("src", "not found");
            //Console.WriteLine(nodeList[1]);

            //var num = 1; 
            //foreach (string node in nodeList)
            //{
            //    using (WebClient webClient = new WebClient())
            //    {
            //        webClient.DownloadFile(node, "node"+num.ToString()+".png");
            //        num++; 
            //    }
            //}

            //Image img = Image.FromFile("node1.png");
            //Bitmap bmpImage = new Bitmap(img);
            //Bitmap bmpCrop = bmpImage.Clone(new Rectangle(0, 0, 400, 250), bmpImage.PixelFormat);
            //bmpCrop.Save("node1.jpg");


            var Ocr = new AdvancedOcr()
            {
                CleanBackgroundNoise = true,
                EnhanceContrast = true,
                EnhanceResolution = true,
                Language = IronOcr.Languages.English.OcrLanguagePack,
                Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                DetectWhiteTextOnDarkBackgrounds = false,
                InputImageType = AdvancedOcr.InputTypes.Document,
                RotateAndStraighten = true,
                ReadBarCodes = false,
                ColorDepth = 4
            };
            var Area = new Rectangle() { X = 0, Y = 0, Height = 250, Width = 400 };
            var Results = Ocr.Read("test.png");
            Console.WriteLine(Results);
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
