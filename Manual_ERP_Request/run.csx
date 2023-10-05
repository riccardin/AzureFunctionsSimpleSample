using System;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net;

public static void Run(string input, TraceWriter log)
{
    log.Info($"C# manually triggered function called with input: {input}");

    var url="endpoint_url";
    log.Info(GetEnvironmentVariable("AzureWebJobsStorage"));
    
    string certfile = System.IO.Path.Combine(Environment.ExpandEnvironmentVariable‌​s("%HOME%"), @"site\wwwroot\Manual_ERP_Request\certificateFile.pfx");         
    log.Info(certfile); 
    log.Info(System.IO.File.Exists(certfile).ToString());

     var cert = new X509Certificate2( certfile, "A11111", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
     log.Info(cert.Thumbprint);
   //  var privateKey = cert.PrivateKey;

    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
    httpWebRequest.ClientCertificates.Add(cert);
    httpWebRequest.ContentType = "application/json";
    httpWebRequest.Method = "POST";
    

 ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12;
                                                   
  ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
  using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //381, 461, 684 or 885
               string json="{\"transactionId\":\"885\",\"evCloudUsername\":\"Rick1818_TEST2\",\"advisorMailUsername\":null,\"sourceSystemId\":\"evc\"}";
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

try {
      string result;
            using (var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }

        log.Info($"End function response: {result}");
    }
    catch(Exception ex){

        log.Error($"End function with error: {ex}");
    }


}

public static string GetEnvironmentVariable(string name)
{
    return name + ": " + 
        System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}