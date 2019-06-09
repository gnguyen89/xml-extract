using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using XmlExtract.Extentions;

namespace XmlExtract.Tests
{
    [TestClass]
    public class XmlExtractTests
    {
        [TestMethod]
        public void Should_Validate_Total_Correctly()
        {
            string txt = "<total>5</total>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();

            Assert.IsTrue(extracted.IsValid, "Should validate total correctly.");
        }

        [TestMethod]
        public void Should_Return_InValid_If_Missing_Tag()
        {
            string txt = "<total>5</total><test>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();

            Assert.IsFalse(extracted.IsValid, "Should validate correctly when a tag is missing");
        }

        [TestMethod]
        public void Should_Return_InValid_If_Missing_Total()
        {
            string txt = "<mytag>5</mytag>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();

            Assert.IsFalse(extracted.IsValid, "Should validate correctly when total tag is missing.");
        }

        [TestMethod]
        public void Should_Insert_CostCentre_If_None()
        {
            string ccTxt = "<cost_centre>UNKNOWN</cost_centre>";
            string txt = "<expense>5</expense><total>5</total>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();
            bool result = extracted.Content.IndexOf(ccTxt) != -1;

            Assert.IsTrue(result, "Should insert cost centre if none");
        }

        [TestMethod]
        public void Should_Calculate_GST_Exclusive_Total()
        {
            string txt = "<expense>5</expense><total>10</total>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();
            bool gstExclusiveTag = extracted.Content.IndexOf("total_exclusive") != -1;
            bool correctValue = extracted.Content.IndexOf("8.5") != -1;

            Assert.IsTrue(gstExclusiveTag && correctValue, "Should correctly calculate gst exclusive total");
        }

        [TestMethod]
        public void Should_Append_Wrapper_Tag()
        {
            string txt = "<expense>5</expense><total>10</total>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();
            bool hasWrapper = extracted.Content.IndexOf("expense_data") != -1;

            Assert.IsTrue(hasWrapper, "Should correctly add wrapper xml tag to data");
        }

        [TestMethod]
        public void Should_Extract_Successfully()
        {
            string txt = "<expense>5</expense><total>5</total><self_closing /><text>Foo bar</text>";
            TextExtract txtExt = new TextExtract(txt);
            var extracted = txtExt.ExtractXmlFromText();

            Assert.IsTrue(extracted.Content.Length > 0, "Should extract successfully");
        }
    }
}
