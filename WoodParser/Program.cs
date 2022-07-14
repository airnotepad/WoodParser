using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WoodParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var database = new Database();

            var count = API.GetCount();

            Console.WriteLine($"Всего записей {count.total}");

            var defaultPageSize = count.total / 5;
            var totalDeals = count.total;
            var currentPage = 0;

            while (totalDeals > 0)
            {
                var pageSize = totalDeals > defaultPageSize ? defaultPageSize : totalDeals;
                totalDeals -= pageSize;
                currentPage++;

                Console.WriteLine($"В работе страница #{currentPage}, {pageSize} элементов");

                var data = API.GetPage(pageSize, currentPage);

                database.AddNewDeals(data);

                Task.Delay(2000).Wait();
            }

            watch.Stop();

            Console.WriteLine($"База синхронизирована за {watch.Elapsed.TotalSeconds} с.");
            Console.WriteLine("Далее будет периодическая проверка с интервалом в 10 минут");

            while (true)
            {
                Task.Delay(TimeSpan.FromMinutes(10)).Wait();

                var filter = GenerateNewFilter(DateTime.Now.AddDays(-1));

                var dayCount = API.GetCount(filter);

                if (dayCount.total > 0)
                {
                    var data = API.GetPage(
                        size: dayCount.total,
                        page: 0,
                        filter: filter);

                    database.AddNewDeals(data);
                }

                Console.WriteLine($"Проверка на актуальность : {DateTime.Now.ToLongTimeString()}, сделок со вчера: {dayCount.total}");
            }
        }

        static object GenerateNewFilter(DateTime date)
        {
            return new
            {
                items = new[]
                {
                    new Dictionary<string, object>
                    {
                        ["property"] = "dealDate",
                        ["operator"] = "GTE",
                        ["value"] = date.Date

                    }
                }
            };
        }
    }
}