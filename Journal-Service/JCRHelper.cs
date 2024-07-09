using Journal_Service.Data;
using Journal_Service.Entities;
using OfficeOpenXml;

namespace Journal_Service;

public class JCRHelper
{
    public void InsertJournals(string filePath, int year)
    {
        List<JCRModel> journals = ReadJCRExcelFile(filePath);

        using var db = new AppDbContext();

        foreach (var item in journals)
        {
            var categories = item.Categories.Split(",");

            var journal = db.Set<Journal>().FirstOrDefault(i =>
                i.NormalizedTitle == item.Title.NormalizeTitle() || i.Issn == item.ISSN.CleanIssn());

            try
            {
                if (journal is null)
                {
                    var newJournal = db.Set<Journal>().Add(new Journal
                    {
                        Title = item.Title.ConvertArabicToPersian(),
                        NormalizedTitle = item.Title.NormalizeTitle().ConvertArabicToPersian(),
                        Issn = item.ISSN.CleanIssn(),
                        EIssn = item.EISSN.CleanIssn()
                    }).Entity;

                    foreach (var category in categories)
                    {
                        if (string.IsNullOrWhiteSpace(category))
                            continue;

                        db.Set<Category>().Add(new Category
                        {
                            Journal = newJournal,
                            Title = category.Trim(),
                            NormalizedTitle = category.NormalizeTitle(),
                            Index = JournalIndex.JCR,
                            QRank = GetQrank(item.QRank),
                            If = item.IF,
                            Year = year,
                            Customer = "jiro"
                        });
                    }
                }
                else
                {
                    journal.Issn = item.ISSN.CleanIssn();
                    journal.EIssn = item.EISSN.CleanIssn();

                    foreach (var category in categories)
                    {
                        if (string.IsNullOrWhiteSpace(category))
                            continue;

                        if (string.IsNullOrWhiteSpace(category))
                            continue;

                        var record = db.Set<Category>()
                            .Where(i => i.JournalId == journal.Id)
                            .Where(i => i.Year == year)
                            .Where(i => i.Index == JournalIndex.JCR)
                            .FirstOrDefault(i => i.NormalizedTitle == category.NormalizeTitle());

                        if (record != null)
                        {
                            record.If = item.IF;
                            record.QRank = GetQrank(item.QRank);
                        }
                        else
                        {
                            db.Set<Category>().Add(new Category
                            {
                                JournalId = journal.Id,
                                Title = category.Trim(),
                                NormalizedTitle = category.NormalizeTitle(),
                                Index = JournalIndex.JCR,
                                QRank = GetQrank(item.QRank),
                                If = item.IF,
                                Year = year,
                                Customer = "jiro"
                            });
                        }
                    }
                }

                db.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"{item.Title}, {item.Categories}, {item.IF}, {item.ISSN}");
        }
    }


    public void InsertCategories(string path)
    {
    }

    static List<JCRModel> ReadJCRExcelFile(string filePath)
    {
        var list = new List<JCRModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var journal = new JCRModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    ISSN = worksheet.Cells[row, 2].Text,
                    EISSN = worksheet.Cells[row, 3].Text,
                    Categories = worksheet.Cells[row, 4].Text,
                    IF = decimal.TryParse(worksheet.Cells[row, 5].Text, out decimal ifValue) ? ifValue : 0,
                    QRank = worksheet.Cells[row, 6].Text,
                };
                list.Add(journal);
            }
        }

        return list;
    }

    JournalQRank? GetQrank(string rank)
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

    public class JCRModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public decimal IF { get; set; }
        public string QRank { get; set; }
        public string Categories { get; set; }
    }
}