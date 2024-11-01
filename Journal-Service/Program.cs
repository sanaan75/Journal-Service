﻿using Journal_Service;
using Journal_Service.Data;
using Journal_Service.Entities;
using OfficeOpenXml;

Console.WriteLine("start app");
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

using var db = new AppDbContext();
// var setting = db.Set<Setting>().Single();
//
// var journalHelper = new JournalHelper();
// journalHelper.FetchJournal(setting.Offset);
// journalHelper.FetchJournalCountry();

// for (int year = 2012; year <= 2022; year++)
// {
//     var iscHelper = new IscHelper();
//     string iscPath = @"C:\Works\Jiro-Journals\isc\" + year + ".xlsx";
//     iscHelper.ImportData(iscPath, year);
//     Console.WriteLine("finish ISC " + year);
// }

// for (int year = 2014; year <= 2023; year++)
// {
//     var jcrHelper = new JCRHelper();
//     string jcr_Path = @"C:\Works\Jiro-Journals\jcr\" + year + ".xlsx";
//     jcrHelper.ImportData(jcr_Path, year);
//
//     var aif_Path = @"C:\Works\Jiro-Journals\jcr\" + year + "\\" + year + "-aif.xlsx";
//     jcrHelper.InsertAif(aif_Path, year);
// }

int year = 2015;

var scopusHelper = new ScopusHelper();
string scopus_path = @"C:\Works\Jiro-Journals\scopus\" + year + "\\" + year + "-a.xlsx";
scopusHelper.ImportData(scopus_path, year);

string scopus_path2 = @"C:\Works\Jiro-Journals\scopus\" + year + "\\" + year + "-b.xlsx";
scopusHelper.ImportData(scopus_path2, year);

// string scopus_path3 = @"C:\Works\Jiro-Journals\scopus\" + year + "\\" + year + "-c.xlsx";
// scopusHelper.ImportData(scopus_path3, year);

// var vezaratinHelper = new VezaratinHelper();
// vezaratinHelper.ImportData(@"C:\Works\Journals\vezaratin.xlsx");

// var journalHelper = new JournalHelper();
// string path = @"C:\Works\Journals\journal-countries.xlsx";
// journalHelper.ImportCountries(path);

Console.ReadKey();


// jcr_Path = @"D:\Jiro\Journals\jcr\" + year + ".xlsx";
// jcrHelper.ImportData(jcr_Path, year);

// aif_Path = @"D:\Jiro\Journals\jcr\" + year + "\\" + year + "-aif.xlsx";
// jcrHelper.InsertAif(aif_Path, year);
// Console.WriteLine("Finish AIF " + year);

// var vezaratinHelper = new VezaratinHelper();
// vezaratinHelper.ImportData(@"D:\Jiro\Journals\vezaratin.xlsx");


// string aif_Path = @"D:\Jiro\Journals\jcr\" + year + "\\" + year + "-aif.xlsx";
// jcrHelper.InsertAif(aif_Path, year);
// Console.WriteLine("Finish AIF " + year);

// var scopusHelper = new ScopusHelper();
// string scopus_path = @"D:\Jiro\Journals\scopus\" + year + "\\" + year + "-a.xlsx";
// scopusHelper.ImportData(scopus_path, year);
//
// string scopus_path2 = @"D:\Jiro\Journals\scopus\" + year + "\\" + year + "-b.xlsx";
// scopusHelper.ImportData(scopus_path2, year);
//
// string scopus_path3 = @"D:\Jiro\Journals\scopus\" + year + "\\" + year + "-c.xlsx";
// scopusHelper.ImportData(scopus_path3, year);

// using var db = new AppDbContext();
// var setting = db.Set<Setting>().Single();
//
// var journalHelper = new JournalHelper();
// journalHelper.FetchJournalCountry();


// var journalHelper = new JournalHelper();
// string path = @"D:\Jiro\Journals\journal-countries.xlsx";
// journalHelper.ImportCountries(path);


//journalHelper.FetchJournal(setting.Offset);

// var scopusHelper = new ScopusHelper();
// string scopus_path1 = @"C:\Works\Journals\scopus\" + year + "\\" + year + "-a.xlsx";
// scopusHelper.InsertScopusFromSJR(scopus_path1, year);
// Console.WriteLine("Finish Scopus " + year + " A");
//
// string scopus_path2 = @"C:\Works\Journals\scopus\" + year + "\\" + year + "-b.xlsx";
// scopusHelper.InsertScopusFromSJR(scopus_path2, year);
// Console.WriteLine("Finish Scopus " + year + " B");
//
// string scopus_path3 = @"C:\Works\Journals\scopus\" + year + "\\" + year + "-c.xlsx";
// scopusHelper.InsertScopusFromSJR(scopus_path3, year);
// Console.WriteLine("Finish Scopus " + year + " C");


// var iscHelper = new ISCHelper();
// string isc_Path = @"C:\Works\Journals\isc\" + year + ".xlsx";
// iscHelper.InsertIsc(isc_Path, year);
// Console.WriteLine("finish ISC " + year);

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