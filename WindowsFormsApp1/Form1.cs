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
        static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CostcoHelper";
        string credentials = directory + "\\costcoapi.json";
        string folder = "";

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
            if (textBox1.Text.Contains("cocowest.ca"))
            {
                int num = 1;
                var data = new MyWebClient().DownloadString(textBox1.Text);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(data);
                var htmlNodeList = doc.DocumentNode.SelectNodes("//dt[@class='gallery-icon landscape']");
                var nodeList = new List<string>();
                folder = parseDate(textBox1.Text);
                DateLabel.Text = "Date: " + folder;

                foreach (var node in htmlNodeList)
                {
                    var src = node.SelectSingleNode(".//img").GetAttributeValue("src", "src not found");
                    if (!src.ToString().Contains("Costco"))
                    {
                        nodeList.Add(node.SelectSingleNode(".//img").GetAttributeValue("src", "src not found"));
                    }
                }
                var link = htmlNodeList[0].GetAttributeValue("src", "not found");

                // Check if directory exists
                FileInfo fileInfo = new FileInfo(directory + "\\" + folder + "\\node1.png");
                if (!fileInfo.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                    File.Create(directory + folder).Dispose();
                    foreach (string node in nodeList)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(node, directory + "\\" + folder + "\\node" + num.ToString() + ".png");
                            num++;
                        }
                    }
                    MessageBox.Show("Download Complete");

                }
                else MessageBox.Show("You already have those images!");
            }
            else
            {
                MessageBox.Show("Please enter a cocowest.ca URL");
            }
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
            string[] hyphenChunks = splitURL.Last().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in hyphenChunks)
            {
                var isNumeric = int.TryParse(s, out int i);
                if (isNumeric || isMonth(s)) result += s+"-";
            }
            return result.Substring(0, result.Length - 1); ;
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



        // Parses and organizes the data from Google Vision in to the table categories.
        public async void parsetable_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
            string path = "";
            using (var fldrDlg = new FolderBrowserDialog())
            {
                fldrDlg.SelectedPath = directory;
                if (fldrDlg.ShowDialog() == DialogResult.OK)
                {

                    if (isMonth(fldrDlg.SelectedPath))
                    {
                        path = fldrDlg.SelectedPath;
                        folder = fldrDlg.SelectedPath.ToString().Split(new[] {"\\"} ,StringSplitOptions.RemoveEmptyEntries).Last();
                        DateLabel.Text = "Date: " + folder;
                    }
                        
                    else
                    {
                        MessageBox.Show("Please select a Date folder!");
                        return;
                    }
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                // String name of the file.
                string pic = path + "\\node" + i.ToString();
                // Load and Save nodes as cropped images (.jpg file format)
                if (File.Exists(pic + ".png"))
                {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(pic + ".png");
                    cropImage(image).Save(pic + ".jpg");
                    string[] text = await RequestGoogleVisionAsync(pic);
                    Item result = parseDescription(text, pic);

                    // ADD TO ROW
                    if (!(bool)result.meat && !(result.saleprice == 0 && !result.clearance) && result.discount > 0)
                    {
                        dataGridView1.Rows.Add(removeInts(result.name), result.initprice, result.saleprice, result.discount, result.clearance, result.img);
                    }
                    else
                    {
                        dataGridView2.Rows.Add(result.name, result.img);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a Date folder!");
                    return;
                }
            }
            saveTableData();
            numLabel.Text = "Number of Sale Items: " + numItems().ToString();
        }



        // Loads table according to data parsed from savedata.txt
        private void loadsaved_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
            string path = "";
            using (var fldrDlg = new FolderBrowserDialog())
            {
                fldrDlg.SelectedPath = directory + "\\" + folder;
                if (fldrDlg.ShowDialog() == DialogResult.OK)
                {
                    if (isMonth(fldrDlg.SelectedPath))
                    {
                        path = fldrDlg.SelectedPath;
                        folder = fldrDlg.SelectedPath.ToString().Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                        DateLabel.Text = "Date: " + folder;
                    }
                    else
                    {
                        MessageBox.Show("Table is already loaded.");
                        return;
                    }
                }

                string savedata = directory + "\\" + folder + "\\savedata.txt";
                FileInfo fileInfo = new FileInfo(savedata);

                if (fileInfo.Exists)
                {
                    StreamReader reader = File.OpenText(savedata);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == "@meat") break;
                        string[] colData = line.Split(';');
                        dataGridView1.Rows.Add(colData[0], colData[1], colData[2], colData[3], colData[4], colData[5]);
                        
                    }
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] colData = line.Split(';');
                        dataGridView2.Rows.Add(colData[0], colData[1]);

                    }
                        reader.Close();
                }
                else
                {
                    MessageBox.Show("Save file does not exist for this folder.");
                    return;
                }
              }
            numLabel.Text = "Number of Sale Items: " + numItems().ToString();
        }

        //returns the # of rows in both tables
        private int numItems()
        {
            return dataGridView1.RowCount + dataGridView2.RowCount - 2;
        }

        // Save current table data
        private void saveTableData()
        {
            string savedata = directory + "\\" + folder + "\\savedata.txt";
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
                tw.WriteLine("@meat");
                for (int i = 0; i <= dataGridView2.RowCount - 2; i++)
                {
                    for (int j = 0; j <= dataGridView2.ColumnCount - 1; j++)
                    {
                        tw.Write(dataGridView2.Rows[i].Cells[j].Value.ToString() + ';');
                    }
                    tw.WriteLine();
                }
            }
        }

        private bool isMonth(string s)
        {
            return (s == "jan" || s == "feb" || s == "mar" || s == "apr" || s == "may" || s == "jun"
                || s == "jul" || s == "aug" || s == "sep" || s == "oct" || s == "nov" || s == "dec");
        }

        private void Folder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(directory);
        }
    }
}
