using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Google.Cloud.Vision.V1;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public class Item
        {
            public string name = "";
            public double initprice = 0;
            public double saleprice = 0;
            public double discount = 0;
            public bool clearance = false;
            public bool meat = false;
            public string img = "";

            public Item(string name, double initprice, double saleprice, bool clearance, bool meat, string image)
            {
                this.name = name;
                this.initprice = initprice;
                this.saleprice = saleprice;
                discount = calcDiscount(initprice, saleprice);
                this.clearance = clearance;
                this.meat = meat;
                img = image;
            }
           
            public double calcDiscount(double init, double sale)
            {
                double discount = Math.Round(((init - sale) / init) * 100, 2);
                if (!Double.IsNaN(discount))
                {
                    Math.Round(discount, 2);
                    if (discount == 100)
                    {
                        discount = 0;
                    }
                }
                return discount;
            }

       }

        public Form1()
        {
            InitializeComponent();
        }

        int num = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            Column1.Width = 350;
            dataGridView2.ColumnHeadersVisible = false;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "G:\\Users\\Martin\\Downloads\\costcoapi.json");

        }

        // Parses string[] into an Item with the resulting table data.
        public Item parseDescription(string[] text)
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


                foreach (string line in text)
                {
                    // Meat case
                    if (meat) itemName += " " + line;

                    // All other cases
                    else if (!string.IsNullOrEmpty(line) && !meat)
                    {
                        // Serial - No current usage
                        var isNumeric = double.TryParse(line, out double n);
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




            return new Item(itemName, initPrice, salePrice, clearance,meat,"");
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
        private void retrieve_Click(object sender, EventArgs e)
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

            
            

            //for (int i = 1; i <= 2; i++)
            //{
            //    // String name of the file.
            //    string pic = "node" + i.ToString();

            //    // Load and Save nodes as cropped images (.jpg file format)
            //    if (File.Exists(pic + ".png"))
            //    {
                    

            //    }
            //}
// authexplicit("favorable - valor - 224609", "c:\\users\\judy\\costcoapi.json");

        }

        public string removeInts(string title)
        {
            title = title.Trim(new Char[] { '"' });
            title = Regex.Replace(title, @"\d{2,}", "");
            Console.WriteLine(title);
            return title;
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

        private void delete_click(object sender, EventArgs e)
        {
            for (int i = 1; i < 200; i++)
                {
                    if (File.Exists("node" + i.ToString() + ".png"))
                    {
                        if (File.Exists("node" + i.ToString() + ".jpg"))
                        {
                            File.Delete("node" + i.ToString() + ".png");
                            File.Delete("node" + i.ToString() + ".jpg");
                        }
                        else
                        {
                            File.Delete("node" + i.ToString() + ".png");
                        }
                    }
                }
            
        }

        public async void loadtable_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 5; i++)
            {
                // String name of the file.
                //string pic = "node22";
                string pic = "node" + i.ToString();

                // Load and Save nodes as cropped images (.jpg file format)
                if (File.Exists(pic + ".png"))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(pic + ".png");
                    cropImage(img).Save(pic + ".jpg");
                    string[] text = await RequestGoogleVisionAsync(pic);
                    Item result = parseDescription(text);
                    result.img = "G:\\Users\\Judy\\source\\repos\\WindowsFormsApp1\\WindowsFormsApp1\\bin\\Debug\\" + pic + ".jpg";
                    // DEBUG WRITELINES
                    foreach (string s in text)
                    {
                        Console.WriteLine(s);
                    }

                    //Console.WriteLine("Results: \n" + result.name);
                    //Console.WriteLine("Price: " + result.initprice);
                    //Console.WriteLine("Sale Price: " + result.saleprice);
                    //Console.WriteLine("Discount: " + result.discount);
                    //Console.WriteLine("Clearance: " + result.clearance);
                    //Console.WriteLine("Meat: " + result.meat);
                    
                    // ADD TO ROW
                    if (!(bool)result.meat)
                    {
                        dataGridView1.Rows.Add(removeInts(result.name), result.initprice, result.saleprice, result.discount, result.clearance, result.img);
                    }
                    else
                    {
                        dataGridView2.Rows.Add(result.name, result.img);
                    }


                }
            }
            

        }
    

    }
}
