using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace WoodParser
{
    public static class API
    {
        #region API Exceptions
        public class BrokenAnswerException : Exception
        {
            public BrokenAnswerException(string message) : base(message) { }
        }
        #endregion

        private const string Url = "https://www.lesegais.ru/open-area/graphql";
        private static object GetCountBody(object filter = null)
        {
            return new
            {
                query = "query SearchReportWoodDealCount($size:Int!,$number:Int!,$filter:Filter,$orders:[Order!]){searchReportWoodDeal(filter:$filter,pageable:{number:$number,size:$size},orders:$orders){total}}",
                variables = new
                {
                    size = 20,
                    number = 0,
                    filter
                },
                operationName = "SearchReportWoodDealCount"
            };
        }
        private static object GetPageBody(int size, int page, object filter = null)
        {
            return new
            {
                query = "query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\n    content {\n      sellerName\n      sellerInn\n      buyerName\n      buyerInn\n      woodVolumeBuyer\n      woodVolumeSeller\n      dealDate\n      dealNumber\n  }\n}\n}\n",
                variables = new
                {
                    size,
                    number = page,
                    filter
                },
                operationName = "SearchReportWoodDeal"
            };
        }

        public static (int total, int pageSize) GetCount(object filter = null)
        {
            var client = new RestClient();
            var request = new RestRequest(Url, Method.Post);

            request.AddParameter("application/json", JsonConvert.SerializeObject(GetCountBody(filter)), ParameterType.RequestBody);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
                throw new BrokenAnswerException($"Запрос количества строк прошел неудачно");

            var data = JsonConvert.DeserializeObject<dynamic>(response.Content);

            var total = Convert.ToInt32(data?.data?.searchReportWoodDeal?.total);
            var size = Convert.ToInt32(data?.data?.searchReportWoodDeal?.size);

            return (total, size);
        }

        public static IEnumerable<Deal> GetPage(int size, int page, object filter = null)
        {
            var client = new RestClient();
            var request = new RestRequest(Url, Method.Post);

            request.AddParameter("application/json", JsonConvert.SerializeObject(GetPageBody(size, page, filter)), ParameterType.RequestBody);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
                throw new BrokenAnswerException($"Запрос количества строк прошел неудачно");

            var data = JsonConvert.DeserializeObject<Page>(response.Content);

            return data.data.searchReportWoodDeal.content;
        }
    }
}
