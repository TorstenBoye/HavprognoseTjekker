using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HavprognoseTjekker
{
    class Program
    {

        //test
        private static Dictionary<int, Color> Colors;

        private const string windScaleColors = "<td id='td1wind' bgcolor='#9696FF' style='border:1px solid black;  height:20 px;'>&nbsp;</td><td bgcolor='#0000FF' style='border:1px solid black;  height:20 px; background:#0000FF !important;'>&nbsp;</td><td bgcolor='#0080FF' style='border:1px solid black;  height:20 px; background:#0080FF !important;'>&nbsp;</td><td bgcolor='#00FFFF' style='border:1px solid black;  height:20 px; background:#00FFFF !important;'>&nbsp;</td> <td bgcolor='#00FF32' style='border:1px solid black;  height:20 px; background:#00FF32 !important;'>&nbsp;</td><td bgcolor='#AAFF55' style='border:1px solid black;  height:20 px; background:#AAFF55 !important;'>&nbsp;</td><td bgcolor='#FFFF00' style='border:1px solid black;  height:20 px; background:#FFFF00 !important;'>&nbsp;</td><td bgcolor='#FFB100' style='border:1px solid black;  height:20 px; background:#FFB100 !important;'>&nbsp;</td><td bgcolor='#FF8000' style='border:1px solid black;  height:20 px; background:#FF8000 !important;'>&nbsp;</td><td bgcolor='#FF0000' style='border:1px solid black;  height:20 px; background:#FF0000 !important;'>&nbsp;</td><td bgcolor='#C80000' style='border:1px solid black;  height:20 px; background:#C80000 !important;'>&nbsp;</td><td bgcolor='#960000' style='border:1px solid black;  height:20 px; background:#960000 !important;'>&nbsp;</td><td bgcolor='#FF00FF' style='border:1px solid black;  height:20 px; background:#FF00FF !important;'>&nbsp;</td><td bgcolor='#A000FF' style='border:1px solid black;  height:20 px; background:#A000FF !important;'>&nbsp;</td><td bgcolor='#6400FF' style='border:1px solid black;  height:20 px; background:#6400FF !important;'>&nbsp;</td><td bgcolor='#4600FF' style='border:1px solid black;  height:20 px; background:#4600FF !important;'>&nbsp;</td>";
        private const string WindScaleNumbers = "<td bgcolor='#FFFFFF'><b>0&nbsp;&nbsp;</b></td><td bgcolor='#FFFFFF'><b>2&nbsp;&nbsp;</b></td><td bgcolor='#FFFFFF'><b>4&nbsp;&nbsp;</b></td><td bgcolor='#FFFFFF'><b>6&nbsp;&nbsp;</b></td><td bgcolor='#FFFFFF'><b>8&nbsp;&nbsp;</b></td><td bgcolor='#FFFFFF'><b>10</b></td><td bgcolor='#FFFFFF'><b>12</b></td><td bgcolor='#FFFFFF'><b>14</b></td><td bgcolor='#FFFFFF'><b>16</b></td><td bgcolor='#FFFFFF'><b>18</b></td><td bgcolor='#FFFFFF'><b>20</b></td><td bgcolor='#FFFFFF'><b>22</b></td><td bgcolor='#FFFFFF'><b>24</b></td><td bgcolor='#FFFFFF'><b>26</b></td><td bgcolor='#FFFFFF'><b>28</b></td><td bgcolor='#FFFFFF'><b>30</b></td>";
        private const string WindScale = "<tr>" + windScaleColors + "<td></td> " + windScaleColors + "</tr><tr>" + WindScaleNumbers + "<td></td> " + WindScaleNumbers + "</tr>";
        static void Main(string[] args)
        {
            //Colors = new Dictionary<int, Color>();
            //Colors.Add(8,Color.FromArgb(, , ));
            Tjek();
            // TestSendMail();
        }

        private static void Log(string text)
        {
            Console.WriteLine(text);
        }



        public static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width;

            int outputImageHeight = firstImage.Height;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(), secondImage.Size),
                    new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        private static void Tjek()
        {
            var prognoseBilleder = new List<PrognoseBillede>();
            DateTime now = DateTime.Now;
            int prognoseTime = GetPrognoseTime();
            DateTime prognoseTidspunkt = new DateTime(now.Year, now.Month, now.Day, prognoseTime, 0, 0);
            DateTime tidspunkt = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            int startTimeFørstedag = tidspunkt.Hour;
            int startPådagen = 9;
            int slutPådagen = 21;

            for (int day = 0; day < 3; day++)
            {

                var dagsPrognoseBilleder = new List<PrognoseBillede>();
                bool found = false;
                for (int hour = startPådagen; hour < slutPådagen; hour++)
                {
                    if (day == 0 && hour < startTimeFørstedag)
                    {
                        continue;
                    }
                    if (day == 2 && hour > prognoseTime + 6)
                    {
                        continue;
                    }

                    tidspunkt = new DateTime(tidspunkt.Year, tidspunkt.Month, tidspunkt.Day, hour, 0, 0);
                    var prognoseBillede = new PrognoseBillede(prognoseTidspunkt, tidspunkt);
                    var image = GetImageFromUrl(prognoseBillede.VindUrl);
                    if (image != null)
                    {
                        Bitmap b = new Bitmap(image);

                        if (!found)
                        {
                            for (int x = 0; x < b.Width; x++)
                            {
                                for (int y = 0; y < b.Height; y++)
                                {
                                    Color c = b.GetPixel(x, y);
                                    if  (CheckColor(c, 8))
                                    {
                                        found = true;
                                        break;
                                    }

                                }
                                if (found)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    dagsPrognoseBilleder.Add(prognoseBillede);

                }
                if (found)
                {
                    prognoseBilleder.AddRange(dagsPrognoseBilleder);
                }
                tidspunkt = tidspunkt.AddDays(1);

            }
            SendMail(prognoseBilleder);
        }

        private static int GetPrognoseTime()
        {
            var currentHour = DateTime.Now.Hour;
            if (currentHour > 5 && currentHour < 11)
            {
                return 0;
            }
            if (currentHour > 11 && currentHour < 17)
            {
                return 6;
            }
            if (currentHour > 17 && currentHour < 23)
            {
                return 12;
            }

            return 18;
        }

        public static Image GetImageFromUrl(string url)
        {
            Image returnImage = null;
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            try
            {

            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    returnImage= Image.FromStream(stream);
                }
            }
            }
            catch (Exception e)
            {
            }

            return returnImage;
        }

        public static bool CheckColor(Color color, int min)
        {

            if (TjekColorDetail(color, 0, 255, 50, min, 8)) return true;  //8 ms
            if (TjekColorDetail(color, 170, 255, 85, min, 10)) return true;  //10 ms
            if (TjekColorDetail(color, 255, 255, 0, min, 12)) return true;  //12 ms
            if (TjekColorDetail(color, 255, 177, 0, min, 14)) return true;  //14 ms
            if (TjekColorDetail(color, 255, 128, 0, min, 16)) return true;  //16 ms
            if (TjekColorDetail(color, 255, 0, 0, min, 18)) return true;  //18 ms
            if (TjekColorDetail(color, 200, 0, 0, min, 20)) return true;  //20 ms
            if (TjekColorDetail(color, 150, 0, 0, min, 22)) return true;  //22 ms
            if (TjekColorDetail(color, 255, 0, 255, min, 24)) return true;  //24 ms
            if (TjekColorDetail(color, 160, 0, 255, min, 26)) return true;  //26 ms
            if (TjekColorDetail(color, 100, 0, 255, min, 28)) return true;  //28 ms
            if (TjekColorDetail(color, 70, 0, 255, min, 30)) return true;  //30 ms

            return false;
        }

        private static bool TjekColorDetail(Color color, int r, int g, int b, int min, int ms)
        {
            return (color.R == r && color.G == g && color.B == b && min <= ms);

        }

        public static void SendMail(List<PrognoseBillede> prognoseBilleder)
        {
            MailMessage mail = new MailMessage();
            if (prognoseBilleder.Any())
            {


                mail.From = new MailAddress("ttb@ditmer.dk", "Torsten Boye");
                foreach (var modtager in GetModtagere())
                {
                    mail.Bcc.Add(modtager);
                }
                mail.Bcc.Add(new MailAddress("ttb@ditmer.dk", "Torsten Boye"));//;torbenhaahr@hotmail.com
                mail.Subject = "Vindmail " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
                mail.IsBodyHtml = true;

                var day = 0;

                for (int i = 0; i < prognoseBilleder.Count() / 2; i++)
                {

                    var index = i * 2;
                    var pBillede1 = prognoseBilleder.ElementAt(index);

                    if (pBillede1.Tidspunkt.Day != day)
                    {
                        if (day != 0)
                        {
                            mail.Body = mail.Body + "<tr><tr><td colspan='33' style='height:30px;'></td></tr>";
                        }

                        mail.Body = mail.Body + string.Format("<tr><td colspan='33'><h1>{0}</h1></td></tr>", pBillede1.DagTekst);
                        day = prognoseBilleder.ElementAt(index).Tidspunkt.Day;
                    }
                    var cell1Html = GetPrognoseHtml(ref mail, prognoseBilleder.ElementAt(index), index);
                    var cell2Html = GetPrognoseHtml(ref mail, prognoseBilleder.ElementAt(index + 1), index + 1);

                    mail.Body = mail.Body + string.Format("<tr><td colspan='16' style='width:640px; height:436px;'>{0}</td><td>&nbsp;</td><td colspan='16' style='width:640px; height:436px;'>{1}</td></tr>{2}", cell1Html, cell2Html, WindScale);

                }


                mail.Body = "<html><body><table cellpadding='0' cellspacing='0'>" + mail.Body + "</table></html></body>";
                string path = @"c:\temp\vind.html";
                File.WriteAllText(path, mail.Body.Replace("cid:",""));
            }
            else
            {
                mail.From = new MailAddress("ttb@ditmer.dk", "Torsten Boye");
                mail.Bcc.Add(new MailAddress("ttb@ditmer.dk", "Torsten Boye"));
                mail.Subject = "NO! Vindmail " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
                mail.IsBodyHtml = true;
            }

            SendMail(mail);


        }

        private static void SendMail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "k2";
            client.Send(mail);
        }

        private static object GetPrognoseHtml(ref MailMessage mail, PrognoseBillede prognoseBillede, int index)
        {
            var imageName = "vind" + index + ".png";
            Attachment image = CreateImageAttachment(prognoseBillede.VindUrl, prognoseBillede.VindRetningsUrl, imageName);
            image.ContentId = imageName;
            image.ContentDisposition.Inline = true;
            image.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            mail.Attachments.Add(image);
            return string.Format("<h3 style='margin-bottom: 0px;'>{0}</h3><img  alt='Klik på hent hele beskeden' src=\"cid:{1}\">", prognoseBillede.TidTekst, imageName);

        }

        private static List<MailAddress> GetModtagere()
        {
            var modtagere = new List<MailAddress>();
            using (StreamReader readFile = new StreamReader(@"Modtagere.csv"))
            {
                string line;
                string[] row;

                while ((line = readFile.ReadLine()) != null)
                {
                    row = line.Split(';');
                    if (row.Count() == 2)
                    {
                        modtagere.Add(new MailAddress(row[0], row[1]));
                    }
                }
            }

            return modtagere;
        }
        public static Attachment CreateImageAttachment(string url1, string url2, string imageName)
        {

            var image1 = GetImageFromUrl(url1);
            var image2 = GetImageFromUrl(url2);
            var image = MergeTwoImages(image1, image2);
            string path = @"c:\temp\" + imageName;
            FileInfo fileinfo = new FileInfo(path);
            if (fileinfo.Exists)
            {
                fileinfo.Delete();
            }
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            MemoryStream memStream = new MemoryStream();
            image.Save(memStream, ImageFormat.Png);
            memStream.Position = 0;
            return new Attachment(memStream, new System.Net.Mime.ContentType("image/png"));

        }

    }
}
