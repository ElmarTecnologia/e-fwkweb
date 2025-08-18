using System;
using System.Configuration;

namespace ELMAR.DevHtmlHelper.Models
{
    public class License
    {
        private static License LicenseInstance;

        public bool Licensed = false;
        public DateTime Data = DateTime.Now;
        public string AppName = string.Empty;
        public string AppSerial = string.Empty;
        public string Info = "Licença inválida ou corrompida.";

        public License(){
            this.AppName = ConfigurationManager.AppSettings["appName"];
            this.AppSerial = ConfigurationManager.AppSettings["License"];
            this.Load();
        }

        //Construtor parametrizado
        public License(string appName, string appSerial)
        {
            this.AppName = appName;
            this.AppSerial = appSerial;
            this.Load();
        }

        public static License getLicense
        {
            get
            {
                //LicenseInstance = new License();
                if (LicenseInstance == null)
                {
                    LicenseInstance = new License();
                }
                return LicenseInstance;
            }
        }

        public License Load()
        {
            try
            {
                string strLicense = Core.Decrypt(this.AppSerial);
                string[] strLicenseAttr = strLicense.Split('|');
                Licensed = true;
                if (!strLicenseAttr[1].Equals(string.Empty))
                {
                    Data = DateTime.Parse(strLicenseAttr[1]);
                    Licensed = Data > DateTime.Now;
                    if (!Licensed)
                        Info = Environment.NewLine + "Licença expirada em '" + Data.ToShortDateString()+"'.";
                }
                //Licensed = Licensed && strLicenseAttr[0].Equals(Util.GetMACAddress());
                Licensed = Licensed && Util.GetMACAddresses().Contains(strLicenseAttr[0]);
                if (!Licensed)
                    Info = Environment.NewLine + "Este computador não foi registrado. "+ Util.GetMACAddress(); 
                Licensed = Licensed && strLicenseAttr[2].Equals(this.AppName);
                if (!Licensed)
                    Info = Environment.NewLine + "Esta aplicação não foi registrada. (" + this.AppName + " | " + Util.GetMACAddress() + ")";
                if (Licensed && (strLicenseAttr[1].Equals(string.Empty) || Data.Equals(DateTime.Parse("01/01/2200"))))
                {
                    Info = "Aplicação licenciada com licença permanente.";
                }
                else if(Licensed && !Data.Equals(DateTime.Parse("01/01/2200"))){
                    Info = "Aplicação com licença temporária. Expira em "+Data.ToShortDateString()+ " ("+(Data - DateTime.Now).Days +"dia(s) restante(s))";
                }                
            }
            catch { Licensed = false; /*Erro desconhecido*/ }
            return this;
        }        
    }
}