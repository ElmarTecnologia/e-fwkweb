using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Configuration;
using System.Net;
using System.Security;

namespace ELMAR.DevHtmlHelper.Models
{
    public class MailHelper
    {
        //private const int Timeout = 180000;
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

                /*Attachment att = null;
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
                */

                /*var smtp = new SmtpClient(_host, _port);

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
                */

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(DisplayName, Sender));
                message.To.Add(new MailboxAddress(null, Recipient));
                if (RecipientCC != null)
                    message.Cc.Add(new MailboxAddress(RecipientCC, RecipientCC));
                message.Subject = Subject;
                message.Body = new TextPart("html")
                {
                    Text = Body
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ConnectAsync(_host, _port, _ssl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);

                    var securePassword = new SecureString();

                    foreach (char c in _pass)
                        securePassword.AppendChar(c);

                    securePassword.MakeReadOnly();

                    var creds = new NetworkCredential(_user, securePassword);

                    client.AuthenticateAsync(creds);

                    client.Send(message);

                    client.DisconnectAsync(true);
                }

                //smtp.Send(message);
                this.Result = "Mensagem enviada com sucesso";

                //if (att != null)
                //    att.Dispose();

                message.Dispose();

                //smtp.Dispose();
            }

            catch (Exception ex)
            {
                ok = false;
                this.Result = "Falha no envio do email.";
                if (debug) //Detalhes técnicos exibidos apenas em modo debug
                    this.Result += " Detalhes: " + ex.Message;
            }

            return ok;
        }

        public bool Send(out string result, bool debug = false)
        {
            bool ok = true;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(DisplayName, Sender));
                message.To.Add(new MailboxAddress(null, Recipient));
                if(RecipientCC != null)
                    message.Cc.Add(new MailboxAddress(RecipientCC, RecipientCC));
                message.Subject = Subject;
                message.Body = new TextPart("html")
                {
                    Text = Body
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(_host, _port, _ssl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);

                    var securePassword = new SecureString();

                    foreach (char c in _pass)
                        securePassword.AppendChar(c);

                    securePassword.MakeReadOnly();

                    var creds = new NetworkCredential(_user, securePassword);

                    client.Authenticate(creds);
                    
                    client.Send(message);

                    client.Disconnect(true);
                }

                this.Result = "Mensagem enviada com sucesso";

                message.Dispose();
            }

            catch (Exception ex)
            {
                ok = false;
                this.Result = "Falha no envio do email.";
                if (debug) //Detalhes técnicos exibidos apenas em modo debug
                    this.Result += " Detalhes: " + ex.Message;
            }

            result = this.Result;

            return ok;
        }
    }
}