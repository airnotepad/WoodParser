using LinqToDB.Mapping;
using System;
using System.Collections.Generic;

namespace WoodParser
{
    [Table(Name = "Deals")]
    public class Deal : IEquatable<Deal>
    {
        private string sellerName;
        private string buyerName;

        [Column(Name = "sellerName", Length = 1000)]
        public string SellerName
        {
            get => sellerName;
            set => sellerName = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "buyerName", Length = 1000)]
        public string BuyerName
        {
            get => buyerName;
            set => buyerName = value?.Trim() ?? string.Empty;
        }

        private string sellerInn;
        private string buyerInn;

        [Column(Name = "sellerInn", Length = 100)]
        public string SellerInn
        {
            get => sellerInn;
            set => sellerInn = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "buyerInn", Length = 100)]
        public string BuyerInn
        {
            get => buyerInn;
            set => buyerInn = value?.Trim() ?? string.Empty;
        }

        private string dealnumber;
        private string dealdate;

        [Column(Name = "dealnumber", Length = 100), PrimaryKey, NotNull]
        public string DealNumber
        {
            get => dealnumber;
            set => dealnumber = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "dealDate")]
        public string DealDate
        {
            get => dealdate;
            set => dealdate = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "woodVolumeSeller"), NotNull]
        public float woodVolumeSeller { get; set; }

        [Column(Name = "woodVolumeBuyer"), NotNull]
        public float woodVolumeBuyer { get; set; }

        public bool Equals(Deal data) => DealNumber.Equals(data.DealNumber, StringComparison.InvariantCultureIgnoreCase);
        public override int GetHashCode() => DealNumber.GetHashCode();
    }


    public class Page
    {
        public Data data { get; set; }
        public class Data
        {
            public SearchReportWoodDeal searchReportWoodDeal { get; set; }
        }
        public class SearchReportWoodDeal
        {
            public List<Deal> content { get; set; }
            public string __typename { get; set; }
        }
    }
}
