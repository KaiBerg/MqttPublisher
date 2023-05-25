using Bogus;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using System.Linq;
using Publisher;
using CsvHelper.Configuration;

var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

// Use TCP connection.
var options = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .WithCredentials("username", "password")
    .Build();

Console.WriteLine("### CONNECTING ###");
try
{
    await mqttClient.ConnectAsync(options, CancellationToken.None);
    Console.WriteLine("### CONNECTED ###");

    var csvFiles = new List<string> { "1.csv", "2.csv", "3.csv", "4.csv", 
"5.csv", "6.csv" };

    while (true)
    {
        foreach (var file in csvFiles)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"File {file} does not exist.");
                continue;
            }

            using (var reader = new StreamReader(file))
            {

                var config = new 
CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    PrepareHeaderForMatch = args => args.Header.ToLower()
                };

                using (var csv = new CsvReader(reader, config))
                {

                    var records = csv.GetRecords<LogData>().ToList();

                    DateTime? lastLogTime = null;
                    foreach (var record in records)
                    {
                        var logTime = DateTime.ParseExact(record.LogTime, 
"dd/MM/yyyy HH.mm.ss", CultureInfo.InvariantCulture);

                        record.LogTime = DateTime.Now.ToString();

                        var message = new MqttApplicationMessageBuilder()
                            .WithTopic("plc_testing")
                            
.WithPayload(JsonConvert.SerializeObject(record))
                            .WithRetainFlag()
                            .Build();

                        
Console.WriteLine(Encoding.UTF8.GetString(message.Payload));

                        await mqttClient.PublishAsync(message, 
CancellationToken.None);

                        lastLogTime = logTime;
                    }
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
