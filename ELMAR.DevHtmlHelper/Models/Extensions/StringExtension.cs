using System.Text;

namespace ELMAR.DevHtmlHelper.Models.Extensions
{
    public static class StringExtension
    {
        public static string ConvertToUTF8(this string texto, string currentEncoding)
        {
            byte[] iso88591data = Encoding.GetEncoding(currentEncoding).GetBytes(texto);
            return Encoding.UTF8.GetString(iso88591data);
        }
    }
}