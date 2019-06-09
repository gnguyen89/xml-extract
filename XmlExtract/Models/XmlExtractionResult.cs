namespace XmlExtract.Models
{
    public class XmlExtractionResult
    {
        public string Content { get; set; }
        public bool IsValid { get; set; }
        public string Error { get; set; }

        public XmlExtractionResult()
        {
            this.IsValid = true;
        }
    }
}