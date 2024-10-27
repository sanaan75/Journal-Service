using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Journal_Service.Data;
using Journal_Service.Entities;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace Journal_Service;

public class JournalHelper
{
    public static int TotalItems = 131000;
    public static int rows = 30;
    HttpClient client = new();

    public JournalHelper()
    {
    }

    public void ImportCountries(string path)
    {
        List<CountryModel> countries = ReadExcelFile(path);
        using var db = new AppDbContext();

        foreach (var item in countries)
        {
            if (string.IsNullOrWhiteSpace(item.Country) == true)
                continue;

            var journal = db.Set<Journal>().FirstOrDefault(i => item.Title.NormalizeTitle() == i.NormalizedTitle || i.Issn == item.Issn.CleanIssn());

            if (journal is null)
                continue;

            if (journal.Country != null)
            {
                journal.Country = item.Country.ToLower().Trim();
                Console.WriteLine(journal.Title + " -> " + journal.Country);
                db.Save();
            }
        }
    }

    static List<CountryModel> ReadExcelFile(string filePath)
    {
        var list = new List<CountryModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var item = new JournalHelper.CountryModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    Issn = worksheet.Cells[row, 2].Text,
                    Country = worksheet.Cells[row, 3].Text,
                };
                list.Add(item);
            }
        }

        return list;
    }

    public class CountryModel
    {
        public string Title { get; set; }
        public string Issn { get; set; }
        public string Country { get; set; }
    }

    public async Task<string> GetJournalCountryAsync(string issn)
    {
        string apiUrl = $"https://portal.issn.org/resource/ISSN/{issn}";
        string country = null;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

                    if (jsonDocument.RootElement.TryGetProperty("country", out JsonElement countryElement))
                        country = countryElement.GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        return country;
    }

    static async Task<List<string>> GetJournalList(string url)
    {
        List<string> journals = new List<string>();

        try
        {
            using HttpClient client = new HttpClient();
            string html = await client.GetStringAsync(url);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var journalNodes = document.DocumentNode.SelectNodes("//div[@class='journal-title']/a");

            if (journalNodes != null)
            {
                foreach (var node in journalNodes)
                {
                    string journalTitle = node.InnerText.Trim();
                    journals.Add(journalTitle);
                }
            }
            else
            {
                Console.WriteLine("No journals found on the page.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return journals;
    }


    public async Task FetchFromClarivate()
    {
        string username = "nikravesh100@gmail.com";
        string password = "meshK!1350:-B";
        string apiEndpoint = "https://api.clarivate.com/jcr/journals";

        string basicAuthValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}"));

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthValue);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var journalData = JsonSerializer.Deserialize<JournalData[]>(jsonResponse);

                    foreach (var journal in journalData)
                    {
                        Console.Write($"Journal Name: {journal.Name} \t");
                        Console.Write($"ISSN: {journal.ISSN} \t");
                        Console.Write($"eISSN: {journal.eISSN} \t");
                        Console.Write($"Category: {journal.Category} \t");
                        Console.Write($"Edition: {journal.Edition} \t");
                        Console.Write($"Total Citations: {journal.TotalCitations} \t");
                        Console.Write($"2023 JIF: {journal.JIF} \t");
                        Console.Write($"JIF Quartile: {journal.JIFQuartile} \t");
                        Console.Write($"JCI Rank: {journal.JCIRank} \t");
                        Console.Write($"JCI Quartile: {journal.JCIQuartile} \t");
                        Console.Write($"JIF Rank: {journal.JIFRank} \t");
                        Console.WriteLine("---------------------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public class JournalData
    {
        public string Name { get; set; }
        public string ISSN { get; set; }
        public string eISSN { get; set; }
        public string Category { get; set; }
        public string Edition { get; set; }
        public int TotalCitations { get; set; }
        public double JIF { get; set; }
        public string JIFQuartile { get; set; }
        public string JCIRank { get; set; }
        public string JCIQuartile { get; set; }
        public string JIFRank { get; set; }
    }


    public async Task FixData()
    {
        try
        {
            using var db = new AppDbContext();

            var journals = db.Set<Journal>().ToList();

            foreach (var journal in journals)
            {
                journal.NormalizedTitle = journal.Title.NormalizeTitle();
                journal.Issn = journal.Issn != null ? CleanIssn(journal.Issn) : null;
                journal.EIssn = journal.EIssn != null ? CleanIssn(journal.EIssn) : null;
                Console.WriteLine(journal.Id + "  done.");
            }

            var categories = db.Set<Category>().ToList();

            foreach (var category in categories)
            {
                category.NormalizedTitle = NormalizeTitle(category.Title);
                Console.WriteLine(category.Id + "  done.");
            }

            db.Save();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    // public async Task FetchJCRFromSamp(int year)
    // {
    //     HtmlWeb web = new HtmlWeb();
    //     string url = $"https://samp.znu.ac.ir/web/jcr/view?id={year}";
    //
    //     var doc = web.Load(url);
    //
    //     var tags = doc.DocumentNode.SelectNodes(".//tr");
    //     using var db = new AppDbContext();
    //
    //     foreach (HtmlNode tr in tags)
    //     {
    //         try
    //         {
    //             var cells = tr.SelectNodes(".//td");
    //             var title = cells[0].InnerText.Trim().ToLower();
    //             var issn = cells[2].InnerText.Trim().Replace("-", "");
    //
    //             if (string.IsNullOrWhiteSpace(title) == true)
    //                 continue;
    //
    //             var journal = db.Query<Journal>()
    //                 .FirstOrDefault(i => i.Title.ToLower().Trim().Equals(title) ||
    //                                      i.Issn.Replace("-", "").Trim().Equals(issn));
    //
    //             if (journal is null)
    //             {
    //                 journal = db.Set<Journal>().Add(new Journal
    //                 {
    //                     Title = title,
    //                     Issn = issn,
    //                 }).Entity;
    //             }
    //
    //             var categories = cells[4].InnerText.Trim().Split(",");
    //
    //             foreach (var category in categories)
    //             {
    //                 if (category.Trim().Equals("etc"))
    //                     continue;
    //
    //                 var cleanCategory = category.ToLower().Trim().Replace("&amp;", "&");
    //
    //                 var historyAlreadyExists = db.Query<JournalRecord>()
    //                     .Where(i => i.JournalId == journal.Id)
    //                     .Where(i => i.Category.ToLower().Trim().Equals(cleanCategory))
    //                     .Any(i => i.Year == year);
    //
    //                 if (historyAlreadyExists)
    //                     continue;
    //
    //                 db.Set<JournalRecord>().Add(new JournalRecord
    //                 {
    //                     Journal = journal,
    //                     Year = year,
    //                     Category = cleanCategory,
    //                     QRank = GetQrank(cells[7].InnerText.Trim()),
    //                     If = Convert.ToDecimal(cells[3].InnerText),
    //                     Mif = Convert.ToDecimal(cells[5].InnerText),
    //                     Aif = Convert.ToDecimal(cells[6].InnerText),
    //                     Index = JournalIndex.JCR,
    //                     CreatorId = 1,
    //                     CreateDate = DateTime.Now,
    //                 });
    //
    //                 Console.WriteLine($"add : {journal.Id} -->  cat : {cleanCategory} --> 2022 ");
    //                 db.Save();
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine(ex);
    //         }
    //     }
    //
    //     //db.Save();
    // }


    public async Task FetchJournal(int offset)
    {
        if (100000 < offset)
            return;

        try
        {
            Console.Write("start");

            string url = $"https://api.crossref.org/journals?rows={rows}&offset={offset}";

            var response = await client.GetAsync(url);

            Console.Write("\t response");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\t processing");

                string jsonContent = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(jsonContent);

                TotalItems = (int)jsonResponse["message"]["total-results"];

                using var db = new AppDbContext();
                foreach (JToken item in jsonResponse["message"]["items"])
                {
                    //List<string> subjects = new();
                    Dictionary<string, string> issns = new();

                    // foreach (var subject in item["subjects"])
                    //     subjects.Add(subject["name"].ToString());

                    foreach (var issn in item["issn-type"])
                        issns.Add(issn["type"].ToString(), issn["value"].ToString());

                    var journalTitle = item["title"].ToString().Trim();
                    var normalizedTitle = journalTitle.NormalizeTitle();

                    issns.TryGetValue("print", out string Issn);
                    issns.TryGetValue("electronic", out string eissn);

                    Issn = Issn.CleanIssn() ?? "-";
                    eissn = eissn.CleanIssn() ?? "-";

                    if (string.IsNullOrWhiteSpace(Issn) == true)
                        Issn = "-";

                    if (string.IsNullOrWhiteSpace(eissn) == true)
                        eissn = "-";

                    var dupJournal = db.Query<Journal>()
                        .Any(i => i.NormalizedTitle == normalizedTitle || i.Issn == Issn);

                    if (dupJournal == true)
                        continue;

                    db.Set<Journal>().Add(new Journal
                    {
                        Title = journalTitle,
                        NormalizedTitle = journalTitle.NormalizeTitle(),
                        Publisher = item["publisher"].ToString(),
                        Issn = Issn,
                        EIssn = eissn,
                        Country = string.Empty
                    });
                }

                var setting = db.Set<Setting>().Single();
                setting.Offset = offset + rows;
                db.Save();

                Console.WriteLine("rows " + (offset) + " -> " + (offset + rows) + "  added   (: \n");
                await Task.Delay(250);
                FetchJournal(offset + rows);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
            FetchJournal(offset);
        }
    }

    public async Task FetchJournalCountry()
    {
        string url = "https://portal.issn.org/resource/ISSN/";

        using var db = new AppDbContext();
        var journals = db.Set<Journal>().ToList();

        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(2)
        };
        var tasks = new ConcurrentBag<Task>();

        foreach (var journal in journals)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    string pageContent = await httpClient.GetStringAsync(url + journal.Issn);
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(pageContent);

                    var countryNode = document.DocumentNode.SelectSingleNode("//p[span/text()='Country: ']");
                    string country = string.Empty;

                    if (countryNode != null)
                    {
                        var countryText = countryNode.InnerText;
                        country = countryText.Replace("Country: ", "").Trim();
                    }

                    if (string.IsNullOrEmpty(journal.Country) && !string.IsNullOrEmpty(country))
                    {
                        journal.Country = country.ToLower();
                        Console.WriteLine($"{journal.Title} : {country}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"error for ISSN {journal.Issn} : {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }));
        }

        await Task.WhenAll(tasks);

        db.Save();
    }

    private JournalQRank? GetQrank(string rank)
    {
        switch (rank)
        {
            case "Q1":
                return JournalQRank.Q1;
            case "Q2":
                return JournalQRank.Q2;
            case "Q3":
                return JournalQRank.Q3;
            case "Q4":
                return JournalQRank.Q4;
            default:
                return null;
        }
    }

    // public void FixJournalTitle()
    // {
    //     using var db = new AppDbContext();
    //     var journals = db.Set<Journal>().ToList();
    //
    //     foreach (var item in journals)
    //     {
    //         item.Issn = CleanIssn(item.Issn);
    //         item.EIssn = CleanIssn(item.EIssn);
    //     }
    //
    //     db.Save();
    // }
    //
    // static string VacuumString(string input)
    // {
    //     string noWhitespace = Regex.Replace(input, @"\s+", "");
    //     return noWhitespace.Trim().ToLower();
    // }

    public static string NormalizeTitle(string input)
    {
        //string noWhitespace = Regex.Replace(input, "[^a-zA-Z]", "");
        string noWhitespace = Regex.Replace(input, "[^a-zA-Z0-9\u0600-\u06FF\u0750-\u077F]", "");

        return noWhitespace.Trim().ToLower();
    }

    public static string CleanIssn(string issn)
    {
        return issn.Trim().Replace("-", String.Empty).ToUpper();
    }
}