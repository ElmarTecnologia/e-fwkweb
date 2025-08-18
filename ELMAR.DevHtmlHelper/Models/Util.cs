using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DeviceDetectorNET;
using System.Net;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Util
    {
        public static string RemoveNaoNumericos(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Regex regex = new Regex("[^0-9]");
                return regex.Replace(text, string.Empty);
            }
            return text;
        }

        public static string HtmlDecodeEncode(string valor, bool Decode = true)
        {
            if(Decode)
                return System.Net.WebUtility.HtmlDecode(valor);
            return System.Net.WebUtility.HtmlEncode(valor);
        }

        public static string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static bool ValidaCPF(string cpf)
        {
            cpf = Util.RemoveNaoNumericos(cpf);
            bool result;
            if (cpf.Length > 11)
            {
                result = false;
            }
            else
            {
                while (cpf.Length != 11)
                {
                    cpf = '0' + cpf;
                }
                bool flag = true;
                int i = 1;
                while (i < 11 && flag)
                {
                    if (cpf[i] != cpf[0])
                    {
                        flag = false;
                    }
                    i++;
                }
                if (flag || cpf == "12345678909")
                {
                    result = false;
                }
                else
                {
                    int[] array = new int[11];
                    for (i = 0; i < 11; i++)
                    {
                        array[i] = int.Parse(cpf[i].ToString());
                    }
                    int num = 0;
                    for (i = 0; i < 9; i++)
                    {
                        num += (10 - i) * array[i];
                    }
                    int num2 = num % 11;
                    if (num2 == 1 || num2 == 0)
                    {
                        if (array[9] != 0)
                        {
                            result = false;
                            return result;
                        }
                    }
                    else if (array[9] != 11 - num2)
                    {
                        result = false;
                        return result;
                    }
                    num = 0;
                    for (i = 0; i < 10; i++)
                    {
                        num += (11 - i) * array[i];
                    }
                    num2 = num % 11;
                    if (num2 == 1 || num2 == 0)
                    {
                        if (array[10] != 0)
                        {
                            result = false;
                            return result;
                        }
                    }
                    else if (array[10] != 11 - num2)
                    {
                        result = false;
                        return result;
                    }
                    result = true;
                }
            }
            return result;
        }

        public static bool ValidaCNPJ(string cnpj)
        {
            cnpj = Util.RemoveNaoNumericos(cnpj);
            int[] array = new int[]
			{
				5,
				4,
				3,
				2,
				9,
				8,
				7,
				6,
				5,
				4,
				3,
				2
			};
            int[] array2 = new int[]
			{
				6,
				5,
				4,
				3,
				2,
				9,
				8,
				7,
				6,
				5,
				4,
				3,
				2
			};
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            bool result;
            if (cnpj.Length != 14)
            {
                result = false;
            }
            else
            {
                string text = cnpj.Substring(0, 12);
                int num = 0;
                for (int i = 0; i < 12; i++)
                {
                    num += int.Parse(text[i].ToString()) * array[i];
                }
                int num2 = num % 11;
                if (num2 < 2)
                {
                    num2 = 0;
                }
                else
                {
                    num2 = 11 - num2;
                }
                string text2 = num2.ToString();
                text += text2;
                num = 0;
                for (int i = 0; i < 13; i++)
                {
                    num += int.Parse(text[i].ToString()) * array2[i];
                }
                num2 = num % 11;
                if (num2 < 2)
                {
                    num2 = 0;
                }
                else
                {
                    num2 = 11 - num2;
                }
                text2 += num2.ToString();
                result = cnpj.EndsWith(text2);
            }
            return result;
        }

        public static bool ValidaEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsASCII(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        public static string ToASCII(string value)
        {
            string result = string.Empty;

            byte[] caracteres = new byte[System.Text.Encoding.ASCII.GetByteCount(value)];

            caracteres = System.Text.Encoding.ASCII.GetBytes(value);

            foreach (byte caractere in caracteres)
            {
               result += ((char)caractere).ToString();
            }
            
            return result;
        }

        public static string RemoveEspeciais(string value){
          string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
          string replacement = "";
          Regex rgx = new Regex(pattern);
          return rgx.Replace(value, replacement);
        }

        public static string ConvertToAlphaNumeric(string value)
        {
            return Regex.Replace(value, "[^0-9a-zA-Z]+", "");
        }        

        public static string GetModelStateErrors(ModelStateDictionary ModelState, string separator = ", "){
            return String.Join(separator, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception)).Trim();
        }

        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                return adapter.GetPhysicalAddress().ToString();                                    
            }
            return string.Empty;
        }

        public static string GetIPAddress()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            //Console.WriteLine(hostName);
            // Get the IP  
            return Dns.GetHostEntry(hostName).AddressList[0].ToString();
        } 

        public static List<String> GetMACAddresses()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            List<string> MAC_Addresses = new List<string>();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                MAC_Addresses.Add(adapter.GetPhysicalAddress().ToString());
            }
            return MAC_Addresses;
        }

        public static int GetRandom(int min, int max)
        {
            return new Random().Next(min, max);
        }

        public static bool IsMobileDevice(HttpRequestBase Request)
        {
            try
            {
                DeviceDetectorSettings.RegexesDirectory = ConfigurationManager.AppSettings["pathFisico"] + "\\bin\\";
                var dd = new DeviceDetector(Request.UserAgent);
                dd.Parse();

                var device = dd.GetDeviceName();

                if (device == "tablet") { }
                if (device == "smartphone") { }
                if (device == "desktop") { return false; }

                return true;
            }
            catch { return false; }
        }

        #region StringHelper
        static int ArrayLength = 0;
        public static string InBetween(string StrSource, string StrStart, string StrEnd, ref int Starting)
        {
            int loc1 = StrSource.IndexOf(StrStart, Starting);
            int loc2;
            if (loc1 > -1)
                StrSource = StrSource.Substring(loc1 + StrStart.Length);
            loc2 = StrSource.IndexOf(StrEnd);
            if (loc2 > -1)
                StrSource = StrSource.Remove(loc2, StrSource.Length - loc2);
            if ((loc1 < 0) && (loc2 < 0))
            {
                Starting = -1;
                return "";
            }
            Starting = loc1;
            return StrSource;
        }

        public static string InBetween(string StrSource, string StrStart, string StrEnd, int Starting)
        {
            int loc1 = StrSource.IndexOf(StrStart, Starting);
            int loc2;
            if (loc1 > -1)
                StrSource = StrSource.Substring(loc1 + StrStart.Length);
            loc2 = StrSource.IndexOf(StrEnd);
            if (loc2 > -1)
                StrSource = StrSource.Remove(loc2, StrSource.Length - loc2);
            if ((loc1 < 0) && (loc2 < 0))
            {
                Starting = -1;
                return "";
            }
            return StrSource;
        }

        public static string[] GetAllBetween(string StrSource, string StrStart, string StrEnd)
        {
            int Position = 0;
            int Count = -1;
            while (Position >= 0)
            {
                Position = StrSource.IndexOf(StrStart, Position);
                Position++;
                if (Position == 0)
                    Position = -1;
                Count++;
            }
            string[] result = new string[Count];
            Position = 0;
            for (int i = 0; i < Count; i++)
            {
                /*Position = StrSource.IndexOf(StrStart, Position);
                if (Position>=0)
                    result[i] = InBetween(StrSource, StrStart, StrEnd, Position);
                Position++;*/
                result[i] = InBetween(StrSource, StrStart, StrEnd, ref Position);
                Position++;
                //System.Windows.Forms.MessageBox.Show(Position.ToString());
            }
            ArrayLength = Count;
            return result;
        }        

        public static string GetStringUnicode(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }

        public static string ToHexString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        public static string FromHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return string.Empty;
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }
        #endregion

        #region FileHelper / ZipFile
        public static bool FileWriter(StringBuilder content, string fileName, string filePath = "", bool rewrite = false)
        {
            filePath = string.IsNullOrEmpty(filePath) ? ConfigurationManager.AppSettings["pathFisico"] : filePath;
            if (!string.IsNullOrEmpty(filePath))
            {
                //Adaptação para path por referência
                filePath = filePath.Replace("~", ConfigurationManager.AppSettings["pathFisico"]);
            
                //Cria o subdiretório
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
            }
            try
            {
                string fileFullPath = System.IO.Path.Combine(filePath, fileName);
                //Cria o arquivo caso não exista
                if (!File.Exists(fileFullPath))
                {
                    System.IO.FileStream fs = System.IO.File.Create(fileFullPath);
                    fs.Close();
                }
                using (var writer = new StreamWriter(fileFullPath, !rewrite, Encoding.UTF8))
                {
                    //Grava linha a linha do StringBuilder
                    for (int i = 0; i < content.Length; i++)
                    {
                        // Escreve uma string formatada no arquivo
                        writer.Write(content[i]);
                    }
                }                
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }

        public static bool FileWriter(string content, string fileName, string filePath = "", bool rewrite = false)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(content);
            return FileWriter(strBuilder, fileName, filePath, rewrite);
        }

        public bool FileWriter(string filePath, string texto)
        {
            try
            {
                File.WriteAllText(filePath, texto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static StringBuilder FileReader(string filePath)
        {
            StringBuilder content = new StringBuilder();
            if (File.Exists(filePath))
            {
                string linha;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while ((linha = reader.ReadLine()) != null)
                    {
                        content.Append(linha);
                    }
                }
            }
            return content;
        }

        public static string FileReaderToStr(string filePath)
        {
            //if (System.IO.File.Exists(filePath))
            //{
            //    return System.IO.File.ReadAllText(filePath);
            //}
            //return string.Empty;
            return FileReader(filePath).ToString();
        }

        public static bool RemoveFile(string filePath){
            //Substitui path por referencia pelo físico
            filePath = filePath.Replace("~/", ConfigurationManager.AppSettings["pathFisico"] + "/");

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch { return false; }
        }

        public static DateTime GetFileLastUpdate(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            DateTime dt = fi.LastWriteTime;
            long t = fi.Length;
            return dt;
        }

        public static void CreateZipFile(string zipFile, string pathFile)
        {
            ZipFile.CreateFromDirectory(pathFile, zipFile);
        }

        /// <summary>
        /// Método para salvar imagem codificando valor textual para QRCode (Local: /Images/temp)
        /// </summary>
        /// <param name="valor">Valor textual a ser codificado</param>
        /// <param name="filename">Nome do arquivo de imagem a ser gerado (Formato: bmp)</param>
        /// <param name="createFile">Cria o arquivo fisicamente em um path, caso inativo apenas exibe o resultado direto da memória</param>
        /// <returns>FileContentResult: Pode ser usado para saída visual, implementa 'ActionResult'</returns>
        public static ActionResult CreateQRCodeImage(string valor, string filename = "qrcode.bmp", bool createFile = true)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = qrEncoder.Encode(valor);
            byte[] image;

            string pathFisico = FwkConfig.GetSettingValue("pathFisico");

            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            if (!createFile)
            {
                var bmp = new Bitmap(80, 80);
                MemoryStream mStream = new MemoryStream();
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Bmp, mStream);
                bmp.Save(mStream, ImageFormat.Jpeg);
                return new FileContentResult(mStream.GetBuffer(), "image/Bitmap");
            }
            else
            {
                if(!Directory.Exists(pathFisico + @"\Images\temp\"))
                    Directory.CreateDirectory(pathFisico + @"\Images\temp\");
                using (FileStream stream = new FileStream(pathFisico + @"\Images\temp\" + filename, FileMode.Create))
                {
                    renderer.WriteToStream(qrCode.Matrix, ImageFormat.Bmp, stream);
                }
                image = File.ReadAllBytes(pathFisico + @"\Images\temp\" + filename);            
            }

            return new FileContentResult(image, "image/Bitmap") { FileDownloadName = filename };
        }

        public static Bitmap CreateQRCodeImage(string valor)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = qrEncoder.Encode(valor);
            
            string pathFisico = FwkConfig.GetSettingValue("pathFisico");

            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            //var bmp = new Bitmap(80, 80);
            MemoryStream mStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Jpeg, mStream);
            //bmp.Save(mStream, System.Drawing.Imaging.ImageFormat.Bmp);

            return new Bitmap(mStream);
        }

        public static string ImageToBase64(System.Drawing.Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                try { image.Save(ms, image.RawFormat); }
                catch { image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); }
                //image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                //byte[] imageBytes = ms.ToArray();
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(ms.ToArray());
                return base64String;
            }
        }

        public static string PDFAuthSignHash(string pdfFile, string texto, string textoAutoAuth = "", string qrCode = "", string position = "footer", string sufix = "_signed.pdf", int linha = 0)
        {
            string newFile = pdfFile.ToLower().Replace(".pdf", "") + sufix;

            // cria o PdfReader
            PdfReader reader = new PdfReader(pdfFile);
            iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);

            // cria o PdfWriter
            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                // o conteúdo do PDF
                PdfContentByte cb = writer.DirectContent;

                // as propriedades da fonte
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetColorFill(BaseColor.DARK_GRAY);
                cb.SetFontAndSize(bf, 8);

                cb.BeginText();

                var pos = linha * 10 + 20;

                // Multiple signs auto positions
                if (position.Equals("footer.vertical"))
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, texto, document.Right + pos, document.Bottom + 35, 90); //Vertical para cima
                    //Atualizando a autenticação
                    //if(!string.IsNullOrEmpty(textoAutoAuth))
                    //    cb.ShowTextAligned(Element.ALIGN_LEFT, textoAutoAuth, document.Left - pos, document.Top + 25, -90);
                }
                else if (position.Equals("footer.horizontal")){
                    cb.ShowTextAligned(Element.ALIGN_LEFT, texto, document.Right - 150, document.Bottom - 30, 0); //Vertical para baixo
                }
                else //header
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, texto, document.Left + pos, document.Top + 30, -90); //Vertical para baixo
                }

                //QRCode
                if (!string.IsNullOrEmpty(qrCode) && i == 1) //Apenas adicionar a imagem QRCode na primeira página do PDF
                {
                    FileContentResult res = null;
                    try
                    {
                        res = (FileContentResult)CreateQRCodeImage(qrCode, qrCode + ".bmp", false);
                    }
                    catch { }
                    string pathFisico = FwkConfig.GetSettingValue("pathFisico");
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(res.FileContents);
                    img.ScaleAbsolute(50,50);
                    if (position.Equals("header"))
                    {
                        img.SetAbsolutePosition(document.Left + 20, document.Top - 20);
                    }
                    else 
                    {
                        img.SetAbsolutePosition(document.Right - 15, document.Bottom - 35);
                    }
                    cb.AddImage(img);
                }

                cb.EndText();

                PdfImportedPage page = writer.GetImportedPage(reader, i);                
                cb.AddTemplate(page, 0, 0);
                if (i + 1 <= reader.NumberOfPages)
                {
                    size = reader.GetPageSizeWithRotation(i + 1);
                    document.SetPageSize(size);
                    //writer.SetPageSize(size);
                    document.NewPage();
                }

                //TODO: Adicionar página com o resumo da autenticação e assinatura, informando os hashs gerados
            }

            document.Close();
            fs.Close();
            writer.Close();

            reader.Close();

            return newFile;
        }        

        public static string GetFileExtension(string arquivo){
            return Path.GetExtension(arquivo);
        }

        public static string GetMetaFileInfo(string Source)
        {
            StringBuilder strInfo = new StringBuilder();

            // Create an Image object. 
            System.Drawing.Image image = new Bitmap(Source);

            // Get the PropertyItems property from image.
            PropertyItem[] propItems = image.PropertyItems;

            // Set up the display.
            System.Drawing.Font font = new System.Drawing.Font("Arial", 12);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            int X = 0;
            int Y = 0;

            // For each PropertyItem in the array, display the ID, type, and 
            // length.
            int count = 0;

            foreach (PropertyItem propItem in propItems)
            {
                strInfo.AppendLine("Property Item " + count.ToString());

                strInfo.AppendLine("   id: 0x" + propItem.Id.ToString("x"));

                strInfo.AppendLine("   type: " + propItem.Type.ToString());

                strInfo.AppendLine("   length: " + propItem.Len.ToString() + " bytes");

                count++;
            }
            // Convert the value of the second property to a string, and display 
            // it.
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            string manufacturer = encoding.GetString(propItems[1].Value);

            strInfo.AppendLine("The equipment make is " + manufacturer + ".");

            return strInfo.ToString();
        }
        #endregion

        public static string calculaIdade(DateTime dNascimento, bool full = false)
        {
            int idDias = 0, idMeses = 0, idAnos = 0;
            DateTime dAtual = DateTime.Now;
            DateTime dNascimentoCorrente = new DateTime();
            try
            {
                int ajuste = 0;
                if(dNascimento.Day == 29 && dNascimento.Month == 2 && (dAtual.Year - 1)%4 != 0)
                {
                    ajuste = 1;
                }
                dNascimentoCorrente = DateTime.Parse((dNascimento.Day - ajuste).ToString() + "/" +
                dNascimento.Month.ToString() + "/" + (dAtual.Year - 1).ToString());
            }
            catch
            { 
                return string.Empty;
            }
            string ta = "", tm = "", td = "";
            if (dAtual < dNascimento)
            {
                return "Data de nascimento inválida ";
            }
            idAnos = dAtual.Year - dNascimento.Year;
            if (dAtual.Month < dNascimento.Month || (dAtual.Month ==
            dNascimento.Month && dAtual.Day < dNascimento.Day))
            {
                idAnos--;
            }
                        
            if (idAnos > 1)
                ta = idAnos + " anos ";
            else if (idAnos == 1)
                ta = idAnos + "ano";
            
            return full ? ta + tm + td : ta;
        }

        public static int calculaDias(DateTime dataOriginal)
        {
            int numeroDias = 0;
            DateTime dataAtual = DateTime.Now;
            if ((dataAtual.Month > dataOriginal.Month || dataAtual.Month <
            dataOriginal.Month) && (dataAtual.Day > dataOriginal.Day))
            {
                DateTime dUltima = DateTime.Parse(dataOriginal.Day.ToString() + "/" +
                (dataAtual.Month).ToString() + "/" + (dataAtual.Year).ToString());
                numeroDias = (dataAtual - dUltima).Days;
            }
            else if ((dataAtual.Month > dataOriginal.Month || dataAtual.Month <
            dataOriginal.Month) && (dataAtual.Day < dataOriginal.Day))
            {
                DateTime dUltima = DateTime.Parse(dataOriginal.Day.ToString() + "/" +
                (dataAtual.Month - 1).ToString() + "/" + (dataAtual.Year).ToString());
                numeroDias = (dataAtual - dUltima).Days;
            }
            else if (dataOriginal.Month == dataAtual.Month)
            {
                DateTime dUltima = DateTime.Parse(dataOriginal.Day.ToString() + "/" +
                (dataAtual.Month).ToString() + "/" + (dataAtual.Year).ToString());
                numeroDias = (dataAtual - dUltima).Days;
            }
            return numeroDias;
        }

        public static int calculaMeses(DateTime dataOriginal)
        {
            int numeroMeses = 0;
            DateTime dataAtual = DateTime.Now;
            if ((dataAtual.Month > dataOriginal.Month))
            {
                numeroMeses = dataAtual.Month - dataOriginal.Month;
            }
            else if ((dataAtual.Month < dataOriginal.Month))
            {
                if (dataAtual.Day > dataOriginal.Day)
                {
                    numeroMeses = (12 - dataOriginal.Month) + (dataAtual.Month);
                }
                else if (dataAtual.Day < dataOriginal.Day)
                {
                    numeroMeses = (12 - dataOriginal.Month) + (dataAtual.Month - 1);
                }
            }
            return numeroMeses;
        }

        #region WebServices
        public static Dictionary<string, string> ConsultaCEPCorreios(string cep)
        {
            Dictionary<string, string> result = new Dictionary<string,string>();
            try
            {
                var ws = new WSCorreios.AtendeClienteClient();
                var resposta = ws.consultaCEP(cep);
                result.Add("Endereco", resposta.end);
                result.Add("Numero", resposta.complemento);
                result.Add("Complemento", resposta.complemento);
                result.Add("Complemento2", resposta.complemento2);
                result.Add("Bairro", resposta.bairro);
                result.Add("Cidade", resposta.cidade);
                result.Add("Estado", resposta.uf);
                result.Add("UF", resposta.uf);
                //result.Add("UndPostagem", resposta.unidadesPostagem);
            }
            catch (Exception ex)
            {
                result.Add("Error", string.Format("Erro ao efetuar busca do CEP: {0}", ex.Message));                 
            }
            return result;
        }
        #endregion

        #region Enums
        public static Type GetEnumType(string name)
        {
            return
             (from assembly in AppDomain.CurrentDomain.GetAssemblies()
              let type = assembly.GetType(name)
              where type != null
                 && type.IsEnum
              select type).FirstOrDefault();
        }

        public static List<string> GetListOf(string tipo)
        {
            string text = tipo.ToLower();
            List<string> result = new List<string>();
            //result.Add("-");
            if (text != null)
            {
                if (text == "ufs")
                {
                    result = Enum.GetNames(typeof(Util.UFs)).ToList<string>();
                    return result;
                }
            }
            result = null;
            return result;
        }

        public static List<FwkDominioItem> ConvertEnumToFwkDominioItem(string dominio, string ctx, object enumItem, Dictionary<string, string> infoParser = null)
        {
            string text = dominio.ToLower();
            List<string> result = new List<string>();
            List<FwkDominioItem> lstDominioItem = new List<FwkDominioItem>();
            //result.Add("-");
            if (text != null)
            {
                result = Enum.GetNames(enumItem.GetType()).ToList<string>();
                int codigo = 0;
                foreach (string item in result)
                {                    
                    FwkDominioItem fwkDomItem = new FwkDominioItem();
                    fwkDomItem.valor = codigo.ToString();
                    fwkDomItem.descricao = item;
                    //Verifica se existe nome qualificado para a descrição do item
                    if (infoParser != null)
                    {
                        if (infoParser.ContainsKey(item))
                        {
                            fwkDomItem.descricao = infoParser[item];
                        }
                    }
                    fwkDomItem.ctx = ctx;
                    fwkDomItem.dominio = dominio;
                    //Add Item
                    lstDominioItem.Add(fwkDomItem);
                    codigo++;
                }
                return lstDominioItem;                
            }
            return null;
        }

        public enum UFs
        {
            AC,
            AL,
            AP,
            AM,
            BA,
            CE,
            DF,
            ES,
            GO,
            MA,
            MT,
            MS,
            MG,
            PR,
            PB,
            PA,
            PE,
            PI,
            RJ,
            RN,
            RS,
            RO,
            RR,
            SC,
            SE,
            SP,
            TO
        }

        public enum NivelEscolaridade
        {            
        }
        #endregion
    }
}