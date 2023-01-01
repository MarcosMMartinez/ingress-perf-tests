using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

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

    readonly TimeSpan[] latencies = new TimeSpan[100];


    [HttpGet]
        public string Get()
        {
            string output = "";
            //warmup
            RunLatencyTests(agicIp, affinityHost);
            //end warmup

            RunLatencyTests(agicIp, affinityHost);
            TimeSpan agicAffinityAverage = averageLatency(latencies);
            output += "AGIC, Header:" + affinityHost + ", Average Latency (ms): " + agicAffinityAverage.TotalMilliseconds + "\n";             
            
            //warmup
            RunLatencyTests(nginxAffinityIp, affinityHost);
            //end warmup

            RunLatencyTests(nginxAffinityIp, affinityHost);
            TimeSpan nginxAffinityAverage = averageLatency(latencies);
            output += "Nginx, Header:" + affinityHost + ", Average Latency (ms): " + nginxAffinityAverage.TotalMilliseconds + "\n";

            //warmup
            RunLatencyTests(agicIp, antiaffinityHost);
            //end warmup

            RunLatencyTests(agicIp, antiaffinityHost);
            TimeSpan agicAntiAffinityAverage = averageLatency(latencies);
            output += "AGIC, Header:" + antiaffinityHost + ", Average Latency (ms): " + agicAntiAffinityAverage.TotalMilliseconds + "\n";

            //warmup
            RunLatencyTests(nginxAntiAffinityIp, antiaffinityHost);
            //end warmup

            RunLatencyTests(nginxAntiAffinityIp, antiaffinityHost);
            TimeSpan nginxAntiAffinityAverage = averageLatency(latencies);
            output += "Nginx, Header:" + antiaffinityHost + ", Average Latency (ms): " + nginxAntiAffinityAverage.TotalMilliseconds + "\n";
            
            return output;
        }

        void RunLatencyTests(string ip, string hostHeader)
        {
            for (int i = 0; i < latencies.Length; i++)
            {
                latencies[i] = RunLatencyTestResult(ip, hostHeader);
            }
        }

        TimeSpan averageLatency(TimeSpan[] latencies)
        {
            TimeSpan ts = new TimeSpan();
            for (int i = 0; i < latencies.Length; i++)
            {
                ts += latencies[i];
            }

            return ts / latencies.Length;
        }

        TimeSpan RunLatencyTestResult(string ip, string hostHeader)
        {
            using (HttpClient client = new HttpClient())
            {
                Stopwatch stopwatch = new Stopwatch();
                client.DefaultRequestHeaders.Add("Host", hostHeader);
                stopwatch.Start();
                HttpResponseMessage response = client.GetAsync("http://" + ip).Result;
                stopwatch.Stop();
                //Console.WriteLine(ip + ", Header:" + hostHeader + ", Latency: " + stopwatch.Elapsed);
                return stopwatch.Elapsed;
            }
        }
    }
}
