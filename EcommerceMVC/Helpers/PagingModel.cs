﻿namespace EcommerceMVC.Helpers
{
    public class PagingModel
    {
        public int totalProduct { get; set; }
        public int currentPage { get; set; }
        public int countPage { get; set; }
        public Func<int?, string> generateUrl { get; set; }
    }
}
