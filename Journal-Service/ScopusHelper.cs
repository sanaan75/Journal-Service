using System.Data;
using ExcelDataReader;
using Journal_Service.Data;
using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journal_Service;

public class ScopusHelper
{
    public void ImportData(string filePath, int year)
    {
        List<string> rows = new List<string>();

        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var dataTable = result.Tables[0];

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string rowString = string.Join("", row.ItemArray);
                        rows.Add(rowString);
                    }
                }
            }

            Parallel.ForEach(rows, new ParallelOptions { MaxDegreeOfParallelism = 8 }, row =>
            {
                string[] columns = new string[] { };
                try
                {
                    if (string.IsNullOrWhiteSpace(row)) return;

                    var sampleText = row.Replace('\"', ' ')
                        .Replace("&amp;", "-")
                        .Replace("&current;", "-");

                    var lastIndex = sampleText.LastIndexOf(')');
                    if (lastIndex != -1)
                        sampleText = sampleText.Substring(0, lastIndex + 1);

                    columns = sampleText.Split(';');

                    if (columns.Length < 22 || columns[2] is null || columns[3] is null) return;

                    if (string.IsNullOrWhiteSpace(columns[3]) == true || string.IsNullOrWhiteSpace(columns[2]))
                        return;

                    if (columns[3].Trim().Equals("journal"))
                    {
                        List<string> catList = new();
                        for (int i = 22; i < columns.Length; i++)
                            catList.Add(columns[i]);

                        using (var db = new AppDbContext())
                        {
                            db.Database.ExecuteSqlRaw("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");

                            foreach (var category in catList)
                            {
                                string issn = null;
                                string eIssn = null;

                                var iisnText = columns[4].Trim().Replace("\"", "").Replace(" ", "");
                                if (iisnText.Length >= 8)
                                {
                                    issn = iisnText.Substring(0, 8);
                                    if (iisnText.Length >= 16)
                                        eIssn = iisnText.Substring(iisnText.Length - 8, 8);
                                }

                                var cat = GetCategory(category);
                                if (cat == null) continue;

                                var model = new DataItem
                                {
                                    Title = columns[2]?.Trim().Replace("\"", "").ConvertArabicToPersian(),
                                    ISSN = issn?.Replace("-", "").CleanIssn(),
                                    EISSN = eIssn?.Replace("-", "").CleanIssn(),
                                    Index = JournalIndex.Scopus,
                                    Category = cat.Title.ConvertArabicToPersian(),
                                    Rank = cat.Rank,
                                    Country = columns[18]?.Trim().Replace("\"", "").ConvertArabicToPersian(),
                                    Publisher = columns[20]?.Trim().Replace("\"", "").ConvertArabicToPersian()
                                };

                                if (model.Title == null) continue;

                                var journals = db.Query<Journal>().Where(j => j.NormalizedTitle == model.Title.NormalizeTitle());

                                if (journals.Any() == false && string.IsNullOrWhiteSpace(model.ISSN) == false)
                                    journals = journals.Where(j => j.Issn == model.ISSN);

                                var journal = journals.SingleOrDefault();

                                if (journal != null)
                                {
                                    if (journal.Country is null && string.IsNullOrWhiteSpace(model.Country) == false)
                                        journal.Country = model.Country;

                                    if (string.IsNullOrWhiteSpace(model.Category)) continue;

                                    var isDuplicate = db.Query<Category>()
                                        .Any(c => c.JournalId == journal.Id &&
                                                  c.Index == JournalIndex.Scopus &&
                                                  c.NormalizedTitle == model.Category.NormalizeTitle() &&
                                                  c.Year == year);

                                    if (isDuplicate) continue;

                                    db.Set<Category>().Add(new Category
                                    {
                                        JournalId = journal.Id,
                                        Year = year,
                                        Title = model.Category,
                                        NormalizedTitle = model.Category.NormalizeTitle(),
                                        Index = JournalIndex.Scopus,
                                        QRank = model.Rank,
                                        If = null,
                                        Mif = null,
                                        Aif = null,
                                        Type = null,
                                        ISCRank = null,
                                        IscClass = null,
                                        Customer = "Jiro"
                                    });

                                    Console.WriteLine($"{model.Category} - {model.Rank}");
                                }
                                else
                                {
                                    var newJournal = db.Set<Journal>().Add(new Journal
                                    {
                                        Title = model.Title,
                                        NormalizedTitle = model.Title.NormalizeTitle(),
                                        Issn = model.ISSN,
                                        EIssn = model.EISSN,
                                        Country = model.Country,
                                        Publisher = model.Publisher
                                    }).Entity;

                                    db.Set<Category>().Add(new Category
                                    {
                                        Journal = newJournal,
                                        Year = year,
                                        Title = model.Category,
                                        NormalizedTitle = model.Category.NormalizeTitle(),
                                        Index = JournalIndex.Scopus,
                                        QRank = model.Rank,
                                        If = null,
                                        Mif = null,
                                        Aif = null,
                                        Type = null,
                                        ISCRank = null,
                                        IscClass = null,
                                        Customer = "Jiro"
                                    });

                                    Console.WriteLine($"{model.Category} - {model.Rank}");
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }


    // public void ImportData(string filePath, int year)
    // {
    //     List<string> rows = new List<string>();
    //     using var db = new AppDbContext();
    //     try
    //     {
    //         using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
    //         {
    //             System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    //
    //             using (var reader = ExcelReaderFactory.CreateReader(stream))
    //             {
    //                 var result = reader.AsDataSet();
    //                 var dataTable = result.Tables[0];
    //
    //                 foreach (DataRow row in dataTable.Rows)
    //                 {
    //                     string rowString = string.Join("", row.ItemArray);
    //                     rows.Add(rowString);
    //                 }
    //             }
    //         }
    //
    //         Parallel.ForEach(rows, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, row =>
    //         {
    //             try
    //             {
    //                 if (string.IsNullOrWhiteSpace(row)) ;
    //
    //                 var sampleText = row.Replace('\"', ' ')
    //                     .Replace("&amp;", "-")
    //                     .Replace("&current;", "-");
    //
    //                 var lastIndex = sampleText.LastIndexOf(')');
    //                 sampleText = sampleText.Substring(0, lastIndex + 1);
    //
    //                 string[] columns = sampleText.Split(';');
    //
    //                 if (columns[2] is null || columns[3] is null)
    //                     return;
    //
    //                 if (columns[3].Trim().Equals("journal"))
    //                 {
    //                     List<string> catList = new();
    //
    //                     for (int i = 22; i <= columns.Length - 1; i++)
    //                         catList.Add(columns[i]);
    //                     using (var db = new AppDbContext())
    //                     {
    //                         db.Database.ExecuteSqlRaw("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
    //
    //                         foreach (var category in catList)
    //                         {
    //                             string issn = null;
    //                             string eIssn = null;
    //
    //                             var iisnText = columns[4].Trim().Replace("\"", "").Replace(" ", "");
    //
    //                             if (iisnText.Length >= 8)
    //                             {
    //                                 issn = iisnText.Substring(0, 8);
    //                                 if (iisnText.Length == 16) eIssn = iisnText.Substring(8, 8);
    //
    //                                 if (iisnText.Length >= 16) eIssn = iisnText.Substring(iisnText.Length - 8, 8);
    //                             }
    //
    //                             if (GetCategory(category) is null)
    //                                 continue;
    //
    //                             var model = new DataItem
    //                             {
    //                                 Title = columns[2]?.Trim().Replace("\"", "").ConvertArabicToPersian(),
    //                                 ISSN = issn?.Replace("-", "").CleanIssn(),
    //                                 EISSN = eIssn?.Replace("-", "").CleanIssn(),
    //                                 Index = JournalIndex.Scopus,
    //                                 Category = GetCategory(category)?.Title.ConvertArabicToPersian(),
    //                                 Rank = GetCategory(category)?.Rank,
    //                                 Country = columns[18]?.Trim().Replace("\"", "").ConvertArabicToPersian(),
    //                                 Publisher = columns[20]?.Trim().Replace("\"", "").ConvertArabicToPersian()
    //                             };
    //
    //                             if (model.Title is null)
    //                                 continue;
    //
    //                             var journals = db.Query<Journal>()
    //                                 .Where(i => i.NormalizedTitle == model.Title.NormalizeTitle());
    //
    //                             if (string.IsNullOrWhiteSpace(model.ISSN) == false)
    //                                 journals = journals.Where(i => i.Issn == model.ISSN);
    //
    //                             var journal = journals.SingleOrDefault();
    //
    //                             if (journal != null)
    //                             {
    //                                 if (string.IsNullOrWhiteSpace(model.Category))
    //                                     continue;
    //
    //                                 var dup = db.Query<Category>()
    //                                     .Where(i => i.JournalId == journal.Id)
    //                                     .Where(i => i.Index == JournalIndex.Scopus)
    //                                     .Where(i => i.NormalizedTitle.Equals(model.Category.NormalizeTitle()))
    //                                     .Any(i => i.Year == year);
    //
    //                                 if (dup == true)
    //                                     continue;
    //
    //                                 db.Set<Category>().Add(new Category
    //                                 {
    //                                     JournalId = journal.Id,
    //                                     Year = year,
    //                                     Title = model.Category,
    //                                     NormalizedTitle = model.Category.NormalizeTitle(),
    //                                     Index = JournalIndex.Scopus,
    //                                     QRank = model.Rank,
    //                                     If = null,
    //                                     Mif = null,
    //                                     Aif = null,
    //                                     Type = null,
    //                                     ISCRank = null,
    //                                     IscClass = null,
    //                                     Customer = "Jiro"
    //                                 });
    //
    //                                 Console.WriteLine(model.Category + " - " + model.Rank);
    //                             }
    //                             else
    //                             {
    //                                 var newJournal = db.Set<Journal>().Add(new Journal
    //                                 {
    //                                     Title = model.Title,
    //                                     NormalizedTitle = model.Title.NormalizeTitle(),
    //                                     Issn = model.ISSN,
    //                                     EIssn = model.EISSN,
    //                                     Country = model.Country,
    //                                     Publisher = model.Publisher
    //                                 }).Entity;
    //
    //                                 db.Set<Category>().Add(new Category
    //                                 {
    //                                     Journal = newJournal,
    //                                     Year = year,
    //                                     Title = model.Category,
    //                                     NormalizedTitle = model.Category.NormalizeTitle(),
    //                                     Index = JournalIndex.Scopus,
    //                                     QRank = model.Rank,
    //                                     If = null,
    //                                     Mif = null,
    //                                     Aif = null,
    //                                     Type = null,
    //                                     ISCRank = null,
    //                                     IscClass = null,
    //                                     Customer = "Jiro"
    //                                 });
    //
    //                                 Console.WriteLine(model.Category + " - " + model.Rank.ToString());
    //                             }
    //                         }
    //
    //                         db.Save();
    //                     }
    //                 }
    //             }
    //             catch (Exception ex)
    //             {
    //                 Console.WriteLine(ex.Message);
    //             }
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //     }
    // }

    public class CategoryModel
    {
        public string Title { get; set; }
        public JournalQRank? Rank { get; set; }
    }

    private CategoryModel? GetCategory(string category)
    {
        if (category.Length > 5)
        {
            var rank = category.Substring(category.Length - 3, 2);
            var QRank = GetQrank(rank);

            var title = rank.StartsWith("Q")
                ? category.Substring(0, category.Length - 4).Trim()
                : category;
            return new CategoryModel
            {
                Title = title,
                Rank = QRank
            };
        }

        return null;
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

    public class DataItem
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public JournalIndex Index { get; set; }
        public string Category { get; set; }
        public string Country { get; set; }
        public string Publisher { get; set; }
        public decimal? IF { get; set; }
        public JournalQRank? Rank { get; set; }
    }
}