//// See https://aka.ms/new-console-template for more information



//using System.Diagnostics;
//using System.Net;
//using k8s;
//using k8s.Models;

//string agicIp = "*.*.*.196";
//string nginxAffinityIp = "*.*.*.208";
//string nginxAntiAffinityIp = "*.*.*.138";

//string affinityHost = "affinity";
//string antiaffinityHost = "antiaffinity";

//int totalRequests = 10;
//int remainingRequests = totalRequests;
//TimeSpan[] latencies = new TimeSpan[totalRequests];



//DeleteAndGo(nginxAffinityIp, affinityHost);

//void DeleteAndGo(string ip, string hostHeader)
//{
//    string appLabel = "";
//    string k8snamespace = "ingress-perf-testing";
//    int errorsThrown = 0;
//    int totalOk = 0;


//    Random rnd = new Random();
    

//    var kubeconfig = KubernetesClientConfiguration.BuildDefaultConfig();
//    var client = new Kubernetes(kubeconfig);

//    if (hostHeader == "affinity")
//    { appLabel = "app=affinity-dummies"; }
//    else { appLabel = "app=anti-affinity-dummies"; }



//    for (int i = 1; i <= totalRequests; i++)
//    {
//        var myPods = client.ListNamespacedPodAsync(k8snamespace, labelSelector: appLabel);
//        int randomPodIndex = rnd.Next(0, myPods.Result.Items.Count);
//        client.DeleteNamespacedPodAsync(myPods.Result.Items[randomPodIndex].Name(), k8snamespace);
//        Console.WriteLine(DateTime.Now.ToString() + ": Deleting App Pod");

//        Console.WriteLine(DateTime.Now.ToString() + ": Requests Remaining: " + remainingRequests);

//            using (HttpClient httpClient = new HttpClient())
//            {                
//                Stopwatch stopwatch = new Stopwatch();
//                httpClient.DefaultRequestHeaders.Add("Host", hostHeader);

//                try
//                {
//                    stopwatch.Start();
//                    Console.WriteLine(DateTime.Now.ToString() + ": Request Kicked off.");
//                    HttpResponseMessage response = httpClient.GetAsync("http://" + ip).Result;
//                    stopwatch.Stop();
//                    Console.WriteLine("Request Latency (ms):" + stopwatch.Elapsed.TotalMilliseconds);
//                    latencies[i - 1] = stopwatch.Elapsed;
//                    if (response.StatusCode == HttpStatusCode.OK) { totalOk++; } else { errorsThrown++; Console.WriteLine("Error Thrown: " + response.StatusCode);  }
//                }
//                catch (Exception e)
//                {
//                    errorsThrown++;
//                    Console.WriteLine(e.Message);
//                }
                
//                Thread.Sleep(500);
//            } 

//        remainingRequests -= 1;
//    }

//    Console.WriteLine("Total Successes: " + totalOk);
//    Console.WriteLine("Total Errors: " + errorsThrown);
//    Console.WriteLine("Latency Average: " + averageLatency(latencies).TotalMilliseconds);
//}

//TimeSpan averageLatency(TimeSpan[] latencies)
//{
//    TimeSpan ts = new TimeSpan();

//    for (int i = 0; i < latencies.Length; i++)
//    {
//        ts += latencies[i];
//    }

//    return ts / latencies.Length;
//}



