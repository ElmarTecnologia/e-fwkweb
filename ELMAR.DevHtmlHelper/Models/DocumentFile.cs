namespace ELMAR.DevHtmlHelper.Models
{
    public class DocumentFile
    {
        public int? ID { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public int? ContentLength { get; set; }

        public DocumentFile()
        {
            ID = 0;
            FileName = "New File";
            Data = new byte[] { };
            ContentType = "";
            ContentLength = 0;
        }
    }
}