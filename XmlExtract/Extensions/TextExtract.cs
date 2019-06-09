using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using XmlExtract.Models;

namespace XmlExtract.Extentions
{
  public class TextExtract 
  {
    private string text;

    private string[] WRAPPER_TAGS = new String[] {"<expense_data>", "</expense_data>"};
    private string[] EXPENSE_TAGS = new string[] {"<expense>", "</expense>"};
    private string[] TOTAL_TAGS = new string[] {"<total>", "</total>"};
    private string[] GST_EXCLUSIVE_TAGS = new string[] {"<total_exclusive>", "</total_exclusive>"};
    private string[] COST_CENTRE_TAGS = new string[] {"<cost_centre>", "</cost_centre>"};
    private string COST_CENTRE_DEFAULT = "UNKNOWN";

    private double GST_RATE = 0.15;

    public TextExtract(string textToRead)
    {
      text = textToRead;
    }

    private bool ValidateTotal()
    {
      if (text.IndexOf(TOTAL_TAGS[0]) == -1 || text.IndexOf(TOTAL_TAGS[1]) == -1)
      {
        return false;
      }
      return true;
    }

    private string CheckCostCentre(string textToCheck)
    {
      if (textToCheck.IndexOf(COST_CENTRE_TAGS[0]) == -1 && textToCheck.IndexOf(COST_CENTRE_TAGS[1]) == -1)
      {
        string insertion = COST_CENTRE_TAGS[0] + COST_CENTRE_DEFAULT + COST_CENTRE_TAGS[1];
        textToCheck = textToCheck.Insert(textToCheck.IndexOf(EXPENSE_TAGS[0]) + EXPENSE_TAGS[0].Length, insertion);
      }
      return textToCheck;
    }

    private string CalculateGstExclusive(string textToModify)
    {
      string total = textToModify.Substring(textToModify.IndexOf(TOTAL_TAGS[0]) + TOTAL_TAGS[0].Length, textToModify.IndexOf(TOTAL_TAGS[1]) - textToModify.IndexOf(TOTAL_TAGS[0]) - TOTAL_TAGS[0].Length);
      double gstExclusive = Double.Parse(total) * (1 - GST_RATE);

      string insertion = GST_EXCLUSIVE_TAGS[0] + Math.Round(gstExclusive, 2).ToString() + GST_EXCLUSIVE_TAGS[1];
      textToModify = textToModify.Insert(textToModify.IndexOf(TOTAL_TAGS[1]) + TOTAL_TAGS[1].Length, insertion);

      return textToModify;
    }

    private string AppendWrapper(string textToAppend)
    {
      textToAppend = textToAppend.Insert(0, WRAPPER_TAGS[0]);
      textToAppend = textToAppend.Insert(textToAppend.Length, WRAPPER_TAGS[1]);
      return textToAppend;
    }

    public XmlExtractionResult ExtractXmlFromText()
    {
      XmlExtractionResult extracted = new XmlExtractionResult();
      bool isTotalValid = ValidateTotal();

      if (isTotalValid == false)
      {
        extracted.IsValid = false;
        extracted.Error = "Cannot process data. <total> is missing.";
        return extracted;
      }
      
      string curText = text;
      string output = "";

      string startTag = "";
      string endTag = "";

      bool isExtracting = true;

      while (isExtracting) {
        int begin = curText.IndexOf("<");
        int end = curText.IndexOf(">");

        // set xml tags to start extraction
        if (begin != -1 && end != -1)
        {
          startTag = curText.Substring(begin, end - begin + 1);
          endTag = startTag.Replace("<", "</");
        }

        // extract xml blob with closing tag
        if (curText.IndexOf(endTag) != -1) {
          output =  output + curText.Substring(curText.IndexOf(startTag), curText.IndexOf(endTag) - curText.IndexOf(startTag) + endTag.Length);
          curText = curText.Substring(curText.IndexOf(endTag) + endTag.Length);
          startTag = "";
          endTag = "";
        }
        // extract self-closing xml tag
        else if (startTag.IndexOf("/") != -1)
        {
          output =  output + curText.Substring(curText.IndexOf(startTag), startTag.Length + 1);
          curText = curText.Substring(curText.IndexOf(startTag) + startTag.Length);
          startTag = "";
          endTag = "";
        }
        // Check invalid xml and move forward
        else
        {
          // Check if email <email@domain.com>
          if (startTag.IndexOf("@") == -1) 
          {
            extracted.IsValid = false;
            extracted.Error = "Xml element needs a closing tag: " + startTag;
            return extracted;
          }
          curText = curText.Substring(curText.IndexOf(">") + 1);
        }

        // post transform and ends extraction
        if (begin == -1 && end == -1) {
          output = CalculateGstExclusive(output);
          output = CheckCostCentre(output);
          output = AppendWrapper(output);
          output = Regex.Replace(output, @"\t|\n|\r", "");

          isExtracting = false;
          extracted.Content = output;
        }
      }

      return extracted;
    }
  }
}
