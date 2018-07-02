using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Drawing;
using System.Drawing.Text;

namespace birthProjectMaster
{
    class Program
    {
        const string subscriptionKey = "6bc6787d18c64c13ad2b166901515976";
        const string uriBase =
               "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/generateThumbnail";

        const string filePath = "C:\\Users\\pbarbolla\\Desktop";
        const string projectPath = "C:\\Users\\pbarbolla\\source\\repos\\birthProject\\OpenSans";
        const string backImg1Path = "\\cumpleaños-1persona-01.png";
        const string backImg2Path = "\\2cumples-separados-02.jpg";
        const string backImg3Path = "\\3cumples-separados-02-03.jpg";

        const string backValePath = "\\bon-masajes-cumpleaños-sin-datos.jpg";

        const string imgFelicitacion1 = "\\Aniversari-1.jpg";
        const string imgFelicitacion2 = "\\Aniversari-2.jpg";
        const string imgFelicitacion3 = "\\Aniversari-3.jpg";
        const string imgFelicitacion4 = "\\Aniversari-4.jpg";

        static void Main(string[] args)
        {
            bool error = false;
            List<Person> list = new List<Person>();
            List<string> felicitacionesList = new List<string>();

            felicitacionesList.Add(imgFelicitacion1);
            felicitacionesList.Add(imgFelicitacion2);
            felicitacionesList.Add(imgFelicitacion3);
            felicitacionesList.Add(imgFelicitacion4);

            bool photosFlag, emailFlag, appFlag = false;

            if (args == null)
            {
                Console.WriteLine();
                Console.WriteLine("args are null");
                Console.WriteLine();
            }
            else
            {
                
                //factory.CreatePhotoValidator().IsValid();

                for (int i = 0; i < args.Length; i++)
                {

                    if (args.Contains("-f"))
                    {
                        photosFlag = true;
                    }
                    else
                    {
                        if (args.Contains("-e"))
                        {
                            emailFlag = true;
                        }
                        else if (args.Contains("-a"))
                        {
                            appFlag = true;
                        }
                    }
                }

                for(int i = 0; i < args.Length; i++)
                {
                    Console.Write(args[i]);
                    Console.Write("|");
                }

                MyValidatorFactory factory = new MyValidatorFactory();

                //Execució del metode de validacio d'arguments d'entrada
                //validateArguments();

            }

        }

        public static Image putValeData(string name)
        {
            Image valeBackground = Image.FromFile(filePath + backValePath);

            Graphics gName = Graphics.FromImage(valeBackground);
            var heigthImg = valeBackground.Height / 2;
            var widthImg = valeBackground.Width / 2;

            using (Font arialFont = new Font("Arial", 50, FontStyle.Bold))
            {
                gName.DrawString(name, arialFont, Brushes.DarkOrange, widthImg - 205, heigthImg + 30);
            }

            var birthDate = DateTime.Now;

            var expirationDate = birthDate.AddYears(1).ToString("dd/MM/yyyy");

            string strBirth = expirationDate.Split(' ')[0];

            using (Font arialFont = new Font("Arial", 30, FontStyle.Bold))
            {
                gName.TranslateTransform(widthImg + 395, heigthImg + 67);
                gName.RotateTransform(-90);
                gName.DrawString(strBirth, arialFont, Brushes.Gray, 0, 0);
            }

            return valeBackground;
        }

        public static async Task<Image> ResizeImgAsync3(Person p1, Person p2, Person p3)
        {
            Image background = Image.FromFile(filePath + backImg3Path);

            Image resizedImg1 = await MakeThumbNailRequest(p1.getImgPath(), 440, 440);
            Image resizedImg2 = await MakeThumbNailRequest(p2.getImgPath(), 440, 440);
            Image resizedImg3 = await MakeThumbNailRequest(p3.getImgPath(), 440, 440);


            Bitmap img1 = rotateImg(resizedImg1, -5.3);
            Bitmap img2 = rotateImg(resizedImg2, 9);
            Bitmap img3 = rotateImg(resizedImg3, 7.5);

            //posem resizedImage dins de BackImg
            Graphics g = Graphics.FromImage(background);

            var widthImg = background.Width / 4;
            g.DrawImage(img1, widthImg - 235, 45);
            g.DrawImage(img2, widthImg + 305, 95);
            g.DrawImage(img3, widthImg - 200, 472);

            return background;
        }

        public static async Task<Image> ResizeImgAsync1(Person p1)
        {
            Image background = Image.FromFile(filePath + backImg1Path);

            Image resizedImg = await MakeThumbNailRequest(p1.getImgPath(), 645, 645);

            Bitmap img1 = rotateImg(resizedImg, -5.3);

            //posem resizedImage dins de BackImg
            Graphics g = Graphics.FromImage(background);

            var widthImg = background.Width / 4;
            g.DrawImage(img1, widthImg - 49, 65);

            return background;
        }

        public static async Task<Image> ResizeImgAsync2(Person p1, Person p2)
        {
            Image background = Image.FromFile(filePath + backImg2Path);

            Image resizedImg1 = await MakeThumbNailRequest(p1.getImgPath(), 490, 490);
            Image resizedImg2 = await MakeThumbNailRequest(p2.getImgPath(), 490, 490);


            Bitmap img1 = rotateImg(resizedImg1, -5.3);
            Bitmap img2 = rotateImg(resizedImg2, 8);

            //posem resizedImage dins de BackImg
            Graphics g = Graphics.FromImage(background);

            var widthImg = background.Width / 4;
            g.DrawImage(img1, widthImg - 193, 115);
            g.DrawImage(img2, widthImg + 265, 190);

            return background;
        }

        static async Task<Image> MakeThumbNailRequest(string imageFilePath, int width, int height)
        {
            byte[] thumbnailImageData;
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters.
                string requestParameters = "width=" + width + "&height=" + height + "&smartCropping=true";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Request body.
                // Posts a locally stored JPEG image.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Make the REST API call.
                    response = await client.PostAsync(uri, content);
                }

                if (response.IsSuccessStatusCode)
                {

                    // Get the image data.
                    thumbnailImageData =
                        await response.Content.ReadAsByteArrayAsync();

                    // Save the thumbnail to the same folder as the original image,
                    // using the original name with the suffix "_thumb".
                    // Note: This will overwrite an existing file of the same name.

                    Image resizedImg = (Bitmap)((new ImageConverter()).ConvertFrom(thumbnailImageData));
                    return resizedImg;
                }
                else
                {
                    // Display the JSON error data.
                    string errorString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\nResponse:\n{0}\n");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                return null;
            }


        }

        public static Bitmap rotateImg(Image img, double angle)
        {
            float height = img.Height;
            float width = img.Width;
            int hypotenuse = System.Convert.ToInt32(System.Math.Floor(Math.Sqrt(height * height + width * width)));
            Bitmap rotatedImage = new Bitmap(hypotenuse, hypotenuse);
            using (Graphics graph = Graphics.FromImage(rotatedImage))
            {
                graph.TranslateTransform((float)rotatedImage.Width / 2, (float)rotatedImage.Height / 2); //set the rotation point as the center into the matrix
                graph.RotateTransform((float)angle); //rotate
                graph.TranslateTransform(-(float)rotatedImage.Width / 2, -(float)rotatedImage.Height / 2); //restore rotation point into the matrix
                graph.DrawImage(img, (hypotenuse - width) / 2, (hypotenuse - height) / 2, width, height);
            }

            return rotatedImage;
        }

        public static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        public static void sendEmail(string mail, Image img)
        {
            var stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;

            String userName = "pbarbolla@pasiona.com";
            String password = "BarboSound1994";
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(mail));
            msg.From = new MailAddress(userName);
            msg.Subject = "Feliz Cumpleaños!!! (Disfruta del masaje gentileza de pasiona)";
            msg.Body = "Testing email using Office 365 account.";
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.office365.com";
            client.Credentials = new System.Net.NetworkCredential(userName, password);
            client.Port = 587;
            client.EnableSsl = true;

            var imageResource = new LinkedResource(stream, "image/png");
            var alternateView = AlternateView.CreateAlternateViewFromString(msg.Body, msg.BodyEncoding, MediaTypeNames.Text.Html);

            alternateView.LinkedResources.Add(imageResource);
            msg.AlternateViews.Add(alternateView);

            client.Send(msg);
        }

        public static Image randomImage(List<string> list, string name)
        {
            Random rand = new Random();

            var index = rand.Next(1, 4);

            Image background = Image.FromFile(filePath + list[index]);

            Graphics gName = Graphics.FromImage(background);

            var heigthImg = background.Height / 2;
            var widthImg = background.Width / 2;

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(projectPath + "\\OpenSans-ExtraBold.ttf");

            var onlyName = name.Split(' ')[0];

            switch (list[index])
            {
                case "\\Aniversari-1.jpg":
                    using (Font arialFont = new Font(pfc.Families[0], 18, FontStyle.Bold))
                    {
                        gName.TranslateTransform(widthImg + 115, heigthImg - 116);

                        gName.DrawString(onlyName + "!", arialFont, Brushes.White, 0, 0);
                    }
                    break;
                case "\\Aniversari-2.jpg":
                    using (Font arialFont = new Font(pfc.Families[0], 18, FontStyle.Bold))
                    {
                        gName.TranslateTransform(widthImg - 140, heigthImg - 172);

                        gName.DrawString(onlyName + "!", arialFont, Brushes.White, 0, 0);
                    }
                    break;
                case "\\Aniversari-3.jpg":
                    using (Font arialFont = new Font(pfc.Families[0], 18, FontStyle.Bold))
                    {
                        gName.TranslateTransform(widthImg + 175, heigthImg + 52);

                        gName.DrawString(onlyName + "!", arialFont, Brushes.White, 0, 0);
                    }
                    break;
                case "\\Aniversari-4.jpg":
                    using (Font arialFont = new Font(pfc.Families[0], 18, FontStyle.Bold))
                    {
                        gName.TranslateTransform(widthImg + 175, heigthImg + 33);

                        gName.DrawString(onlyName + "!", arialFont, Brushes.White, 0, 0);
                    }
                    break;
            }

            return background;


        }


        public bool ValidateArguments()
        {
            if (validatePhoto() && photosFlag)
            {
                //aplicar metode de crear la foto

                //throw arguments Photo incorrecte
            }

            if (validateEmail() && photosEmail)
            {
                //aplicar metode de crear la Email

                //throw arguments Email incorrecte
            }

            if (validateApp() && photosApp)
            {
                //aplicar metode de crear la App

                //throw arguments App incorrecte
            }
        }

    }

    class MyValidatorFactory
    {
        IDictionary<Type, MyValidatorBase> validators = new Dictionary<Type, MyValidatorBase>();

        public MyValidatorBase CreatePhotoValidator()
        {
            if (validators.Keys.Contains(typeof(MyValidatorPhoto)))
            {
                return validators[typeof(MyValidatorPhoto)];
            }

            var validator = new MyValidatorPhoto();
            validators.Add(validator.GetType(), validator);
            return validator;
        }


    }

    class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    abstract class MyValidatorBase
    {
        public abstract ValidationResult IsValid(string[] arguments);
        //{
        //    return new ValidationResult { IsValid = false, ErrorMessage = "Expected file path is invalid" };
        //}
    }

    class MyValidatorPhoto : MyValidatorBase
    {
        public override ValidationResult IsValid(string[] arguments)
        {
            //Logica de la validacio      
            if(arguments)

            return new ValidationResult { IsValid = false, ErrorMessage = "Expected arguments for generating a new photo are invalid" };
            //return base.IsValid(arguments);
        }
    }
    
    class MyValidatorEmail : MyValidatorBase
    {
        public override ValidationResult IsValid(string[] arguments)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = "Expected arguments for sending a new email are invalid" };


            //return base.IsValid(arguments);
        }
    }

    class MyValidatorApp : MyValidatorBase
    {
        public override ValidationResult IsValid(string[] arguments)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = "Expected arguments to publish a new post at APPasiona are invalid" };


            //return base.IsValid(arguments);
        }
    }


}
