using Infrastructure.Model;
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
       public List<string> DisambiguationLinks { get; set; }
       public string DisambiguationFilePath = @"C:\D\Skola\STUBA\2roc\VI\Project\DBpediaSearcher\Files\disambiguations_en.ttl";
       //public string DisambiguationFilePath = @"C:\D\Skola\STUBA\2roc\VI\Project\DBpediaSearcher\Files\disambiguations_en_uris_eu.ttl";
       public string ShortFilePath = @"C:\D\Skola\STUBA\2roc\VI\Project\DBpediaSearcher\Files\short_abstracts_en.ttl";
       public List<string> ShortAbstracts { get; set; }
       public int SuccessRecommendedDisLink {get;set;}
       public int NotSuccessRecommendedDisLink { get; set; }
       public int ArticleCounter { get; set; }
       public int NotFindDisambiguationLink { get; set; }

       public FileOperation() {
           var lines = File.ReadLines(DisambiguationFilePath).ToList();
           DisambiguationLinks = lines;
           ShortAbstracts = File.ReadAllLines(ShortFilePath).ToList();
       }

       //public Model.RelatedPageLink Parse(string regex, string path, bool onlyDisambiguationLink)
       //{
       //    Model.RelatedPageLink rpl = new Model.RelatedPageLink();
       //    rpl.ArticleList = new List<Model.Article>();
       //    XmlDocument xmlDoc = new XmlDocument();
       //    // Pattern to check each line
       //    Regex pattern = new Regex(regex);

       //    // Read in lines
       //    string[] lines = File.ReadAllLines(path);


       //    // Iterate through lines
       //    foreach (string line in lines)
       //    {
       //        // Check if line matches your format here
       //        var matches = pattern.Matches(line);

       //        if (!onlyDisambiguationLink) return null;

       //        if (matches.Count > 2 && matches[2].Groups[0].Value.Contains("_(disambiguation)"))
       //        {
       //            Model.Article rplArticle =
       //                    new Model.Article
       //                    {
       //                        Name = matches[0].Groups[0].Value,
       //                        LinkList = new List<Model.Link>() {
       //                            new Model.Link() {
       //                               LinkType = matches[1].Groups[0].Value,
       //                               LinkName = matches[2].Groups[0].Value
       //                            }
       //                         }
       //                    };

       //            rpl.ArticleList.Add(rplArticle);

       //            Console.WriteLine(string.Format("{0}", rpl));
       //        }
             
       //    }
         
       //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Model.RelatedPageLink));
       //    using (MemoryStream xmlStream = new MemoryStream())
       //    {
       //        xmlSerializer.Serialize(xmlStream, rpl);
       //        xmlStream.Position = 0;
       //        xmlDoc.Load(xmlStream);
       //        //return xmlDoc.InnerXml;
       //    }
       //    return rpl;
       //}

       /// <summary>
        /// parsovanie liniek v clanku
       /// </summary>
       /// <param name="regex"></param>
       /// <param name="path"></param>
       /// <param name="onlyDisambiguationLink"></param>
       /// <returns></returns>
       public string ParseToXml(string regex, string path, bool onlyDisambiguationLink)
       {
           Model.RelatedPageLink rpl = new Model.RelatedPageLink();
           rpl.LinkList = new List<Model.Link>();
           XmlDocument xmlDoc = new XmlDocument();
           // Pattern to check each line
           Regex pattern = new Regex(regex);

           System.IO.StreamReader file = new System.IO.StreamReader(path);
           string result = "";
           // Iterate through lines
           string line;
           while ((line = file.ReadLine()) != null)
           {
               // Check if line matches your format here
               var matches = pattern.Matches(line);

               if (!onlyDisambiguationLink) return null;

               if (matches.Count > 2 && matches[2].Groups[0].Value.Contains("_(disambiguation)"))
               {
                   Console.WriteLine("--------------------------------------------------------------");
                   var articleKeywords = GetShortAbstract(matches[0].Groups[0].Value).ToList();
                   var link = new Model.Link()
                                   {
                                       ArticleName = matches[0].Groups[0].Value,
                                       //ArticleAbstract = GetShortAbstract(matches[0].Groups[0].Value),
                                       LinkType = matches[1].Groups[0].Value,
                                       LinkName = matches[2].Groups[0].Value,
                                       Keywords = articleKeywords, //ParseKeyWords(matches[2].Groups[0].Value),
                                       DisambiguationLinkList = GetLinksOfDisambiguetionPage(matches[2].Groups[0].Value, regex, articleKeywords),
                                      
                                   };
                   rpl.LinkList.Add(link);
                   ArticleCounter += 1;
                   
                   link.RecomendedDisambiguationLink = GetPrediction(link.DisambiguationLinkList);
                   if (link.RecomendedDisambiguationLink.Count >= 1 && link.RecomendedDisambiguationLink.First().DisambiguationValue != 0)
                   {
                       SuccessRecommendedDisLink += 1;
                       Console.WriteLine(string.Format("<ENDLINK number:{0}  link name: {1}, recomended link: {2} ENDLINK>", ArticleCounter, link.LinkName, link.RecomendedDisambiguationLink.First().DisambiguationValue));
                   }
                   else
                   {
                       NotSuccessRecommendedDisLink += 1;
                       Console.WriteLine(string.Format("<ENDLINK number:{0}  link name: {1}, recomended link: {2} ENDLINK>", ArticleCounter, link.LinkName, 0));
                   }
                   
                  
                   if (ArticleCounter > 50) {
                       file.Close();
                       break;

                   }
               }

           }
           //var tt = rpl.LinkList.Where(w => w.RecomendedDisambiguationLink.First().DisambiguationValue != 0);
           //if (tt.Count() > 1)
           //{
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
          // }
           //return null;

         
           //return null;
       }



       
       
       /// <summary>
       /// parsovanie disambiguacnych liniek
       /// </summary>
       /// <param name="linkname"></param>
       /// <param name="regex"></param>
       /// <param name="articleKeywords"></param>
       /// <returns></returns>
        public List<Infrastructure.Model.DisambiguationLink> GetLinksOfDisambiguetionPage(string linkname, string regex, List<string> articleKeywords){
            Console.WriteLine(string.Format("START GetLinksOfDisambiguetionPage for linkname: {0}", linkname));
            
            List<Model.DisambiguationLink> dsgList = new List<Model.DisambiguationLink>();
            XmlDocument xmlDoc = new XmlDocument();
            // Pattern to check each line
            Regex pattern = new Regex(regex);

            System.IO.StreamReader file = new System.IO.StreamReader(DisambiguationFilePath);
            string result = "";
            // Iterate through lines
            IEnumerable<string> lines = DisambiguationLinks.FindAll(a => a.Contains(linkname));
            foreach (string line in lines)
            {
                // Check if line matches your format here
                var matches = pattern.Matches(line);
                if (matches.Count > 2 && matches[0].Groups[0].Value.Contains(linkname))
                {
                    var dsg = new Model.DisambiguationLink();
                    dsg.Type = matches[0].Groups[0].Value;
                    dsg.Name = matches[2].Groups[0].Value;
                    dsg.Keywords = GetShortAbstract(matches[2].Groups[0].Value).ToList(); //ParseKeyWords(matches[2].Groups[0].Value);
                    dsg.ProbabilityRecommendation = GetprobabilityOfOcurrenceWords(articleKeywords, dsg.Keywords);
                    //dsg.DisambiguationAbstract = GetShortAbstract(matches[2].Groups[0].Value);
                    dsgList.Add(dsg);
                    // }
                }
            }
            Console.WriteLine(string.Format("END GetLinksOfDisambiguetionPage CountDISLinks: {0}", dsgList.Count()));
            return dsgList;
            
        }



       /// <summary>
       /// parsovat skrateny abstrakt
       /// </summary>
       /// <param name="linkname"></param>
       /// <returns></returns>
        public IEnumerable<string> GetShortAbstract(string linkname) {
            Console.WriteLine(string.Format("START GetShortAbstract for linkname: {0}", linkname));
              // Pattern to check each line
            Regex patternToFindID = new Regex(@"(?<http>\w+):\/\/(?<dbpedia>[\w@][\w.:@]+)\/?[\w\.?()_=%&=\-@/$,]*");
            //Regex patternToFindTextInQuotation = new Regex(@"([^"]*)")
             
            string result = "";
            // Iterate through lines
          
            //const Int32 BufferSize = 1024;
            //using (var fileStream = File.OpenRead(ShortFilePath))
            //using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            //{
            //    String line;
                //while ((line = streamReader.ReadLine()) != null)

            string line = ShortAbstracts.Find(a => a.Contains(linkname));

            //foreach(var line in ShortAbstracts )
            //    {
            //        // Process line
            if (!string.IsNullOrEmpty(line))
            {
                var matches = patternToFindID.Matches(line);
                if (matches.Count == 2 && matches[0].Groups[0].Value.Contains(linkname))
                {
                    Regex patternToFindAbstract = new Regex(@"!(?<http>\w+):\/\/(?<dbpedia>[\w@][\w.:@]+)\/?[\w\.?()_=%&=\-@/$,]*");
                    string[] stringSeparators = new string[] { "rdf-schema#comment>" };
                    result = line.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)[1];
                    //break;
                }
                // }
                //}
                //Console.WriteLine(string.Format("keywords: {0}", result.Length));
                var source = result.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var matchQuery = from word in source
                                 where word.Length >= 4 && word != "\"" && !word.Contains("lly") && !word.Contains("ed") && word != "Such"
                                 select word;
                Console.WriteLine(string.Format("END GetShortAbstract CountAstracts: {0}", matchQuery.Count()));
                return matchQuery;
            }
            else {
                Console.WriteLine(string.Format("END GetShortAbstract: NoLineFound!!!"));
                return new List<string>();
            }
        }


        #region Prediction

        public List<PredictionResult> GetPrediction(List<DisambiguationLink> dl)
        {
            var result = new List<PredictionResult>();
            if (dl.Count() >= 1)
            {
                decimal resultvalue = dl.Max(m => m.ProbabilityRecommendation);
                var resultDLObject = dl.Find(f => f.ProbabilityRecommendation == resultvalue);

                result.Add(new PredictionResult { DisambiguationValue = resultDLObject.ProbabilityRecommendation, DisambiguationName = resultDLObject.Name });
            }
            else {
                Console.Write("NOT FOUND DISAMBIGUATION LINK");
                NotFindDisambiguationLink += 1;
            }
            return result;
        }

        #endregion

      


        #region Additional methods

        public List<string> ParseKeyWords(string name) {

           var arrayName = name.Split('/');
            var resultList = new List<string>();
           var result = arrayName[arrayName.Length - 1];

            if(result.Contains("_")) {
                if (result.Contains("disambiguation"))
                    resultList.Add(result.Split('_')[0]);
                else
                    resultList = result.Split('_').ToList();
            }
            else { resultList.Add(result.Split('_')[0]); } 
        
           return resultList;
               
        }

       public decimal GetprobabilityOfOcurrenceWords(List<string> ArticleKeywords,List<string> DisambiguationKeywords){
           int counter = 0;
           foreach(var ak in ArticleKeywords){
               foreach (var dk in DisambiguationKeywords) {
                   if (dk.Contains(ak)) {
                       counter++;
                   }
               }
           }

           return counter;
               //(counter != 0 ) ? counter/DisambiguationKeywords.Count() : 0;
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
           

        #endregion

    }
    }

