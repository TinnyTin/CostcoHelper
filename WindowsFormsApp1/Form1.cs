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
using System.Data;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string credentials = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CostcoHelper\\costcoapi.json";
        string savedata = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CostcoHelper\\savedata.txt";
        string imagedata = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CostcoHelper\\";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Column1.Width = 350;
            dataGridView2.ColumnHeadersVisible = false;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentials);

        }

        // Item Class. Each Item represents row data in the table.
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

        // WebClient Class for HTTP
        class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                return request;
            }
        }

        // Parses string[] into an Item with the resulting table data.
        public Item parseDescription(string[] text, string pic)
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




            return new Item(itemName, initPrice, salePrice, clearance, meat, pic + ".png");
        }

        // RETRIEVE ON CLICK
        private void retrieve_Click(object sender, EventArgs e)
        {
            int num = 1;
            var data = new MyWebClient().DownloadString(textBox1.Text);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(data);
            var htmlNodeList = doc.DocumentNode.SelectNodes("//dt[@class='gallery-icon landscape']");
            var nodeList = new List<string>();
            string folder = parseDate(textBox1.Text);

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


            FileInfo fileInfo = new FileInfo(imagedata + folder);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
                File.Create(savedata).Dispose();
            }

            foreach (string node in nodeList)
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(node, imagedata + folder + "node" + num.ToString() + ".png");
                    num++;
                }
            }

            MessageBox.Show("Download Complete");
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

        // Removes all numerical integers from a string
        public string removeInts(string title)
        {
            title = title.Trim(new Char[] { '"' });
            title = Regex.Replace(title, @"\d{2,}", "");
            Console.WriteLine(title);
            return title;
        }


        // Returns the date represented as a simple string given a http url
        public string parseDate(string url)
        {
            string result = "";
            string[] splitURL = url.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in splitURL)
            {
                var isNumeric = int.TryParse(s, out int i);
                if (isNumeric) result += s;
            }
            return result + "\\";
        }

        // Crop a given image and return it.
        public Bitmap cropImage(System.Drawing.Image img)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(new Rectangle(0, 0, 400, 250), bmpImage.PixelFormat);
            return bmpCrop;
        }

        // Requests Google Vision to process the image
        private async Task<string[]> RequestGoogleVisionAsync(string filepath)
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            Google.Cloud.Vision.V1.Image image = Google.Cloud.Vision.V1.Image.FromFile(filepath + ".jpg");
            IReadOnlyList<EntityAnnotation> textAnnotations = await client.DetectTextAsync(image);
            string[] text = textAnnotations[0].Description.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return text;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine(e.GetType());
        }

        // Parses and organizes the data from Google Vision in to the table categories.
        public async void parsetable_Click(object sender, EventArgs e)
        {

            for (int i = 1; i <= 5; i++)
            {
                // String name of the file.
                string pic = imagedata + "node" + i.ToString();
                // Load and Save nodes as cropped images (.jpg file format)
                if (File.Exists(pic + ".png"))
                {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(pic + ".png");
                    cropImage(image).Save(pic + ".jpg");
                    string[] text = await RequestGoogleVisionAsync(pic);
                    Item result = parseDescription(text, pic);

                    // ADD TO ROW
                    if (!(bool)result.meat && !(result.saleprice == 0 && !result.clearance))
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

    
        // Loads table according to data parsed from savedata.txt
        private void loadsaved_Click(object sender, EventArgs e)
        {
            
            StreamReader reader = File.OpenText(savedata);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] res = line.Split(';');
                dataGridView1.Rows.Add(res[0], res[1], res[2], res[3], res[4], res[5]);

            }
            reader.Close();
        }

        // Save current table data when application closes
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(savedata);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
                File.Create(savedata).Dispose();
            }
            using (TextWriter tw = new StreamWriter(savedata))
            {
                for (int i = 0; i <= dataGridView1.RowCount - 2; i++)
                {
                    for (int j = 0; j <= dataGridView1.ColumnCount - 1; j++)
                    {
                        tw.Write(dataGridView1.Rows[i].Cells[j].Value.ToString() + ';');
                    }
                    tw.WriteLine();
                }
            }
        }

        private void Folder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@imagedata);
        }
    }
}
