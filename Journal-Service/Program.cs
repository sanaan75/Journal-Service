using Journal_Service;
using OfficeOpenXml;

Console.WriteLine("start app");
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

string filePath = @"C:\Works\Journals\isc\2020.xlsx";

var jcrHelper = new JCRHelper();
jcrHelper.InsertJournals(filePath, 2023);

// var iscHelper = new ISCHelper();
// iscHelper.InsertIsc(filePath,2022);

Console.ReadKey();

//using var db = new AppDbContext();
//var setting = db.Set<Setting>().Single();

//var journalHelper = new JournalHelper();

//journalHelper.FixData();
//journalHelper.FetchJournal(setting.Offset);

//journalHelper.FetchJCR(2012);

// string username = "your_username";
// string password = "your_password";
// string loginUrl = "https://jcr.clarivate.com/login";
// string dataUrl = "https://jcr.clarivate.com/jcr/browse-journals";
//
// var handler = new HttpClientHandler { UseCookies = true };
// using (var client = new HttpClient(handler))
// {
//     var loginContent = new MultipartFormDataContent
//     {
//         { new StringContent(username), "admin@uok.ac.ir" },
//         { new StringContent(password), "Jiro1542!@" }
//     };
//
//     var loginResponse = await client.PostAsync(loginUrl, loginContent);
//     if (loginResponse.IsSuccessStatusCode)
//     {
//         var dataResponse = await client.GetAsync(dataUrl);
//         var dataPage = await dataResponse.Content.ReadAsStringAsync();
//
//         var htmlDoc = new HtmlDocument();
//         htmlDoc.LoadHtml(dataPage);
//
//         var journals = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'journal-entry')]");
//         foreach (var journal in journals)
//         {
//             var name = journal.SelectSingleNode(".//h2").InnerText.Trim();
//             var issn = journal.SelectSingleNode(".//span[contains(@class, 'issn')]").InnerText.Trim();
//             var impactFactor = journal.SelectSingleNode(".//span[contains(@class, 'impact-factor')]").InnerText.Trim();
//             var mif = journal.SelectSingleNode(".//span[contains(@class, 'mif')]").InnerText.Trim();
//             var qRank = journal.SelectSingleNode(".//span[contains(@class, 'q-rank')]").InnerText.Trim();
//
//             Console.WriteLine($"Journal Name: {name}");
//             Console.WriteLine($"ISSN: {issn}");
//             Console.WriteLine($"Impact Factor: {impactFactor}");
//             Console.WriteLine($"MIF: {mif}");
//             Console.WriteLine($"Q Rank: {qRank}");
//             Console.WriteLine(new string('-', 20));
//         }
//     }
//     else
//     {
//         Console.WriteLine("Login failed");
//     }
// }


// var client = new ClarivateApiClient();
// try
// {
//     string endpoint = "/apis/wos-journals/v1/journals?page=1&limit=20";
//     string result = await client.GetRequestAsync(endpoint);
//     Console.WriteLine(result);
// }
// catch (Exception e)
// {
//     Console.WriteLine($"An error occurred: {e.Message}");
// }

// var client = new ClarivateApiClient();
// try
// {
//     string endpoint = "/apis/wos-journals/v1/journals/PLOS_ONE/reports/year/2023";
//     string result = await client.GetRequestAsync(endpoint);
//     Console.WriteLine(result);
// }
// catch (Exception e)
// {
//     Console.WriteLine($"An error occurred: {e.Message}");
// }