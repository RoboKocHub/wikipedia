using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Model
{
    public class RelatedPageLink
    {
        public List<Article> ArticleList { get; set; }

    }

    //public class Category {
    //    public string Name {get; set;}
       
    //}

    public class Article {
        public string Name { get; set; }
        public List<Link> LinkList { get; set; }
    }

    public class Link {
        public string LinkType { get; set; }
        public string LinkName { get; set; }
        public List<DisambiguationLink> DisambiguationLinkList { get; set; }
    }



    public class DisambiguationLink {
        public string Name {get;set;}
        public string Type {get;set;}

    }

}
