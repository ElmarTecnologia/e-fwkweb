using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace ELMAR.DevHtmlHelper.Models
{
    public class MailHelper
    {
        private const int Timeout = 180000;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly bool _ssl;
        //private readonly string _displayName;

        public string Sender { get; set; }
        public string DisplayName { get; set; }
        public string ReplyTo { get; set; }
        public string Recipient { get; set; }
        public string RecipientCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentFile { get; set; }
        public string[] mailList { get { return Recipient.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries); } }
        public string[] mailListCC { get { return RecipientCC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries); } }
        public string Result { get; set; }

        public MailHelper()
        {
            //MailServer - Represents the SMTP Server
            _host = ConfigurationManager.AppSettings["MailServer"];
            //Port - Represents the port number
            _port = int.Parse(ConfigurationManager.AppSettings["MailPort"]);
            //MailAuthUser and MailAuthPass - Used for Authentication for sending email
            _user = ConfigurationManager.AppSettings["MailAuthUser"];
            _pass = ConfigurationManager.AppSettings["MailAuthPass"];
            _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["MailEnableSSL"]);
            DisplayName = ConfigurationManager.AppSettings["MailDisplayName"];
            Sender = ConfigurationManager.AppSettings["MailEmailFromAddress"];
            ReplyTo = ConfigurationManager.AppSettings["MailEmailReplyTo"] == null ? string.Empty : ConfigurationManager.AppSettings["MailEmailReplyTo"];
        }

        public MailHelper(string host, string port, string user, string pass, string ssl, string sender, string displayName = "", string replyTo = "")
        {
            //MailServer - Represents the SMTP Server
            _host = host;
            //Port- Represents the port number
            _port = int.Parse(port);
            //MailAuthUser and MailAuthPass - Used for Authentication for sending email
            _user = user;
            _pass = pass;
            _ssl = Convert.ToBoolean(ssl);
            DisplayName = displayName;
            Sender = sender;
            ReplyTo = replyTo;
        }

        public bool Send(bool debug = false)
        {
            bool ok = true;
            try
            {

                // We do not catch the error here... let it pass direct to the caller
                Attachment att = null;
                //var message = new MailMessage(Sender, Recipient, Subject, Body) { IsBodyHtml = true };
                var message = new MailMessage() { From = new MailAddress(Sender, DisplayName), Subject = Subject, Body = Body, IsBodyHtml = true };
                //Adding the ReplyTo Address
                if(!string.IsNullOrEmpty(ReplyTo))
                    message.ReplyToList.Add(new MailAddress(ReplyTo));
                if (Recipient != null)
                {
                    foreach (var item in mailList)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(item) && Util.IsASCII(item))
                                message.To.Add(item.Trim());
                        }
                        catch { }
                    }
                    //message.Bcc.Add(RecipientCC);
                }
                if (RecipientCC != null)
                {
                    foreach (var item in mailListCC)
                    {
                        if (!string.IsNullOrEmpty(item) && !message.To.Contains(new MailAddress(item)) && Util.IsASCII(item))
                            message.Bcc.Add(item.Trim());
                    }
                    //message.Bcc.Add(RecipientCC);
                }
                var smtp = new SmtpClient(_host, _port);

                if (!String.IsNullOrEmpty(AttachmentFile))
                {
                    if (File.Exists(AttachmentFile))
                    {
                        att = new Attachment(AttachmentFile);
                        message.Attachments.Add(att);
                    }
                }

                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.EnableSsl = _ssl;
                }

                smtp.Send(message);
                this.Result = "Mensagem enviada com sucesso";

                if (att != null)
                    att.Dispose();
                message.Dispose();
                smtp.Dispose();
            }

            catch (Exception ex)
            {
                ok = false;
                this.Result = "Falha no envio da mensagem.";
                if (debug) //Detalhes técnicos exibidos apenas em modo debug
                    this.Result += " Detalhes: " + ex.Message;
            }

            return ok;
        }
    }
}