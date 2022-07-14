using LinqToDB.Data;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace WoodParser
{
    public class Database : DataConnection
    {
        private const string DatabaseConnection = "Server=localhost\\SQLEXPRESS;Database=parsingdb;User Id=sa;Password=123;TrustServerCertificate=True;";

        public ITable<Deal> Deals => this.GetTable<Deal>();
        public ITable<Deal> TempDeals { get; }

        public Database() : base(ProviderName.SqlServer, DatabaseConnection)
        {
            this.CreateTable<Deal>(tableName: "Deals", tableOptions: TableOptions.CreateIfNotExists);

            TempDeals = this.CreateTempTable<Deal>(tableName: "Temp_Deals",
                tableOptions: TableOptions.CreateIfNotExists);
        }

        public void AddNewDeals(IEnumerable<Deal> deals)
        {
            //Консалидация хаоса в таблице ЕГАИС
            var dublicates = deals.GroupBy(x => x.DealNumber).SelectMany(g => g.Skip(1)).Distinct().ToList();
            var list = deals.Distinct();
            foreach (var data in dublicates)
            {
                var deal = list.FirstOrDefault(x => x.Equals(data));

                if (string.IsNullOrEmpty(deal.DealDate))
                    deal.DealDate = data.DealDate;

                if (string.IsNullOrEmpty(deal.SellerName))
                    deal.SellerName = data.SellerName;
                if (string.IsNullOrEmpty(deal.BuyerName))
                    deal.BuyerName = data.BuyerName;

                if (string.IsNullOrEmpty(deal.SellerInn))
                    deal.SellerInn = data.SellerInn;
                if (string.IsNullOrEmpty(deal.BuyerInn))
                    deal.BuyerInn = data.BuyerInn;

                if (deal.woodVolumeSeller == 0)
                    deal.woodVolumeSeller = data.woodVolumeSeller;
                if (deal.woodVolumeBuyer == 0)
                    deal.woodVolumeBuyer = data.woodVolumeBuyer;
            }

            TempDeals.BulkCopy(list);

            Deals.Merge()
                .Using(TempDeals)
                .OnTargetKey()
                .UpdateWhenMatched()
                .InsertWhenNotMatched()
                .Merge();

            TempDeals.Delete();
        }
    }
}
