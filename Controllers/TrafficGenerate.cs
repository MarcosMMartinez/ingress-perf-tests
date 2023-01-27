using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ingress_perf_tests.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TrafficGenerate : ControllerBase
    {
        readonly string agicIp = "*.*.*.196";
        readonly string nginxAffinityIp = "*.*.*.208";
        readonly string nginxAntiAffinityIp = "*.*.*.138";

        readonly string affinityHost = "affinity";
        readonly string antiaffinityHost = "antiaffinity";
        readonly int numberRequests = 100;
        readonly bool isParallel = true;

        [HttpGet]
        public async Task<string> Get()
        {
            string output = "";
            //warmup
            await RunLatencyTests(agicIp, affinityHost);
            //end warmup

            double agicAffinityAverage = await RunLatencyTests(agicIp, affinityHost);
            output += "AGIC, Header:" + affinityHost + ", Average Latency (ms): " + agicAffinityAverage + "\n";

            //warmup
            await RunLatencyTests(nginxAffinityIp, affinityHost);
            //end warmup

            double nginxAffinityAverage = await RunLatencyTests(nginxAffinityIp, affinityHost);
            output += "Nginx, Header:" + affinityHost + ", Average Latency (ms): " + nginxAffinityAverage + "\n";

            //warmup
            await RunLatencyTests(agicIp, antiaffinityHost);
            //end warmup

            double agicAntiAffinityAverage = await RunLatencyTests(agicIp, antiaffinityHost);
            output += "AGIC, Header:" + antiaffinityHost + ", Average Latency (ms): " + agicAntiAffinityAverage + "\n";

            //warmup
            await RunLatencyTests(nginxAntiAffinityIp, antiaffinityHost);
            //end warmup

            double nginxAntiAffinityAverage = await RunLatencyTests(nginxAntiAffinityIp, antiaffinityHost);
            output += "Nginx, Header:" + antiaffinityHost + ", Average Latency (ms): " + nginxAntiAffinityAverage + "\n";

            return output;
        }


        //Returns average latency
        async Task<double> RunLatencyTests(string ip, string hostHeader)
        {
            List<double> elapsedTimes = new();
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("Host", hostHeader);

            ParallelOptions parallelOptions = new();

            if (this.isParallel)
            {
                parallelOptions.MaxDegreeOfParallelism = numberRequests;
            }
            else
            {
                parallelOptions.MaxDegreeOfParallelism = 1;
            }

            await Parallel.ForEachAsync(Enumerable.Range(1, numberRequests), parallelOptions, async (i, token) =>
            {
                try
                {
                    var elapsedTime = Stopwatch.StartNew();
                    var response = await client.GetAsync("http://" + ip);
                    elapsedTimes.Add(elapsedTime.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending HTTP request to ip '{ip}', host '{hostHeader}': {ex.Message}");
                }
            });

            if (elapsedTimes.Count > 0)
            {
                return elapsedTimes.Average();
            }
            else
            {
                return -1;
            }
        }
    }
}
