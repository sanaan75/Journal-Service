using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Journal_Service.Data;
using Journal_Service.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Journal_Service;

public class JournalHelper
{
    public static int TotalItems = 131000;
    public static int rows = 30;
    HttpClient client = new();

    public JournalHelper()
    {
    }

    public async Task FixData()
    {
        try
        {
            using var db = new AppDbContext();

            var journals = db.Set<Journal>().ToList();

            foreach (var journal in journals)
            {
                journal.NormalizedTitle = NormalizeTitle(journal.Title);
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


    // public async Task FetchJournal(int offset)
    // {
    //     if (TotalItems <= offset)
    //         return;
    //
    //     try
    //     {
    //         Console.Write("start");
    //
    //         string url = $"https://api.crossref.org/journals?rows={rows}&offset={offset}";
    //
    //         var response = await client.GetAsync(url);
    //
    //         Console.Write("\t response");
    //         if (response.IsSuccessStatusCode)
    //         {
    //             Console.WriteLine("\t processing");
    //
    //             string jsonContent = await response.Content.ReadAsStringAsync();
    //
    //             JObject jsonResponse = JObject.Parse(jsonContent);
    //
    //             TotalItems = (int)jsonResponse["message"]["total-results"];
    //
    //             using var db = new AppDbContext();
    //             foreach (JToken item in jsonResponse["message"]["items"])
    //             {
    //                 List<string> subjects = new();
    //                 Dictionary<string, string> issns = new();
    //
    //                 foreach (var subject in item["subjects"])
    //                 {
    //                     subjects.Add(subject["name"].ToString());
    //                 }
    //
    //                 foreach (var issn in item["issn-type"])
    //                 {
    //                     issns.Add(issn["type"].ToString(), issn["value"].ToString());
    //                 }
    //
    //                 var journalTitle = item["title"].ToString().Trim();
    //                 issns.TryGetValue("print", out string Issn);
    //                 issns.TryGetValue("electronic", out string eissn);
    //
    //                 Issn = string.IsNullOrWhiteSpace(Issn) == false ? Issn.Replace("-", String.Empty) : string.Empty;
    //                 eissn = string.IsNullOrWhiteSpace(eissn) == false ? eissn.Replace("-", String.Empty) : string.Empty;
    //
    //
    //                 var dup_journal = db.Query<Journal>()
    //                     .FirstOrDefault(i => i.Title.ToLower().Equals(journalTitle.ToLower()));
    //
    //                 if (dup_journal != null)
    //                     continue;
    //
    //                 // db.Set<Journal>().Add(new Journal
    //                 // {
    //                 //     Title = journalTitle,
    //                 //     Publisher = item["publisher"].ToString(),
    //                 //     Issn = Issn,
    //                 //     EIssn = eissn,
    //                 //     WebSite = string.Empty,
    //                 //     Country = string.Empty
    //                 // });
    //             }
    //
    //             var setting = db.Set<Setting>().Single();
    //             setting.Offset = offset + rows;
    //             //db.Save();
    //
    //             Console.WriteLine("rows " + (offset) + " -> " + (offset + rows) + "  added   (: \n");
    //             await Task.Delay(300);
    //             FetchJournal(offset + rows);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.ForegroundColor = ConsoleColor.Red;
    //         Console.WriteLine(ex.Message);
    //         Console.ForegroundColor = ConsoleColor.White;
    //         FetchJournal(offset);
    //     }
    // }


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