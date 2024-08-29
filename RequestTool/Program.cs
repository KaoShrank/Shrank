// See https://aka.ms/new-console-template for more information

HttpClient client = new HttpClient();

var response = await client.PostAsync("https://localhost:7293/Test/GetData", new StringContent("1"));


Console.WriteLine(response);
Console.ReadLine();