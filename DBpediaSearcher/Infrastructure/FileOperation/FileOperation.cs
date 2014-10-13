using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;


namespace Infrastructure
{
   public class FileOperation
    {
       public Model.RelatedPageLink Parse(string regex, string path)
       {
           Model.RelatedPageLink rpl = new Model.RelatedPageLink();
           rpl.ArticleList = new List<Model.Article>();
           XmlDocument xmlDoc = new XmlDocument();
           // Pattern to check each line
           Regex pattern = new Regex(regex);

           // Read in lines
           string[] lines = File.ReadAllLines(path);


           // Iterate through lines
           foreach (string line in lines)
           {
               // Check if line matches your format here
               var matches = pattern.Matches(line);

               if (matches.Count > 2)
               {
                   Model.Article rplArticle =
                           new Model.Article
                           {
                               Name = matches[0].Groups[0].Value,
                               LinkList = new List<Model.Link>() {
                                   new Model.Link() {
                                      LinkType = matches[1].Groups[0].Value,
                                      LinkName = matches[2].Groups[0].Value
                                   }
                                }
                           };

                   rpl.ArticleList.Add(rplArticle);

                   Console.WriteLine(string.Format("{0}", rpl));
               }
             
           }
         
           XmlSerializer xmlSerializer = new XmlSerializer(typeof(Model.RelatedPageLink));
           using (MemoryStream xmlStream = new MemoryStream())
           {
               xmlSerializer.Serialize(xmlStream, rpl);
               xmlStream.Position = 0;
               xmlDoc.Load(xmlStream);
               //return xmlDoc.InnerXml;
           }
           return rpl;
       }



       public string ParseToXml(string regex, string path)
       {
           Model.RelatedPageLink rpl = new Model.RelatedPageLink();
           rpl.ArticleList = new List<Model.Article>();
           XmlDocument xmlDoc = new XmlDocument();
           // Pattern to check each line
           Regex pattern = new Regex(regex);

           // Read in lines
           string[] lines = File.ReadAllLines(path);


           // Iterate through lines
           foreach (string line in lines)
           {
               // Check if line matches your format here
               var matches = pattern.Matches(line);

               if (matches.Count > 2)
               {
                   Model.Article rplArticle =
                           new Model.Article
                           {
                               Name = matches[0].Groups[0].Value,
                               LinkList = new List<Model.Link>() {
                                   new Model.Link() {
                                      LinkType = matches[1].Groups[0].Value,
                                      LinkName = matches[2].Groups[0].Value
                                   }
                                }
                           };

                   rpl.ArticleList.Add(rplArticle);

                   Console.WriteLine(string.Format("{0}", rpl));
               }

           }

           XmlSerializer xmlSerializer = new XmlSerializer(typeof(Model.RelatedPageLink));
           using (MemoryStream xmlStream = new MemoryStream())
           {
               xmlSerializer.Serialize(xmlStream, rpl);
               xmlStream.Position = 0;
               xmlDoc.Load(xmlStream);
               //return xmlDoc.InnerXml;
           }

           var stringWriter = new StringWriter(new StringBuilder());
           var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
           xmlDoc.Save(xmlTextWriter);
           return stringWriter.ToString();

           //return null;
       }

        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public static Model.RelatedPageLink LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Model.RelatedPageLink));
            return serializer.Deserialize(stringReader) as Model.RelatedPageLink;
        }
           
        }
    }

