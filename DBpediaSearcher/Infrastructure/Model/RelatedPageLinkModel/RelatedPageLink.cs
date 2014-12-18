using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Model
{
    public class RelatedPageLink
    {
        public List<Link> LinkList { get; set; }

    }

    //public class Category {
    //    public string Name {get; set;}
       
    //}

    //public class Article {
    //    public string Name { get; set; }
    //    public List<Link> LinkList { get; set; }

    //}

    public class Link {
        public string ArticleName { get; set; }
        public string ArticleAbstract { get; set; }
        public string LinkType { get; set; }
        public string LinkName { get; set; }
        public List<PredictionResult> RecomendedDisambiguationLink { get; set; }
        public List<string> Keywords { get; set; }
        public List<DisambiguationLink> DisambiguationLinkList { get; set; }
        public int NumberOfDisambiguationLink { get; set; }
        
    }



    public class DisambiguationLink {
        public string DisambiguationAbstract { get; set; }
        public string Name {get;set;}
        public string Type {get;set;}
        public List<string> Keywords { get; set; }
        public int NumberOfLink { get; set; }
        public decimal ProbabilityRecommendation { get; set; }

    }

    public class PredictionResult
    {
        public string DisambiguationName { get; set; }
        public decimal DisambiguationValue { get; set; }
    }
    //public class KeyWords {
    //    public string Name { get; set; }

    //}

}
