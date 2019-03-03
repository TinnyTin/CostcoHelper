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

        public Form1()
        {
            InitializeComponent();
        }

        int num = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            Column1.Width = 350;
            dataGridView2.ColumnHeadersVisible = false;
            
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


                foreach (string line in text)
                {
                    if (!string.IsNullOrEmpty(line) && !meat)
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
            

            double discount = Math.Round(((initPrice - salePrice) / initPrice)*100,2);
            if (!Double.IsNaN(discount))
            {
                Math.Round(discount, 2);
                if (discount == 100)
                {
                    discount = 0;
                }
            }

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

        }

        public async void loadtable_Click(object sender, EventArgs e)
        {

            List<string> meats = new List<string>();
            //for (int i = 1; i <= 15; i++)
            {
                // String name of the file.
                string pic = "node22";
                //string pic = "node" + i.ToString();

                // Load and Save nodes as cropped images (.jpg file format)
                if (File.Exists(pic + ".png"))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(pic + ".png");
                    cropImage(img).Save(pic + ".jpg");
                    string[] text = await RequestGoogleVisionAsync(pic);
                    object[] result = parseDescription(text);

                    // DEBUG WRITELINES
                    foreach (string s in text)
                    {
                        Console.WriteLine(s);
                    }

                    Console.WriteLine("Results: \n" + result[0]);
                    //Console.WriteLine("Price: " + result[1]);
                    //Console.WriteLine("Sale Price: " + result[2]);
                    //Console.WriteLine("Discount: " + result[3]);
                    //Console.WriteLine("Clearance: " + result[4]);
                    //Console.WriteLine("Meat: " + result[5]);
                    
                    // ADD TO ROW
                    if (!(bool)result[5])
                    {
                        dataGridView1.Rows.Add(removeInts(result[0].ToString()), result[2], result[1], result[3], result[4].ToString(), "click");
                    }
                    else
                    {
                        DataGridViewColumn col = new DataGridViewColumn();
                        DataGridViewCell cell = new DataGridViewTextBoxCell();
                        col.CellTemplate = cell;
                        cell.Value = result[0].ToString();
                        dataGridView2.Columns.Add(col);
                        meats.Add(result[0].ToString());
                        //Console.WriteLine("Judy is beautiful:" + result[0]);

                    }


                }
            }
            dataGridView2.Rows.Add();
            int i = 0;
            foreach (string meat in meats)
            {
                dataGridView2[i, 0].Value = meat;
                i++;
            }
            

        }
    

    }
}
