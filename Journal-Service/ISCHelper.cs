using Journal_Service.Data;
using Journal_Service.Entities;
using OfficeOpenXml;

namespace Journal_Service;

public class IscHelper
{
    public void ImportData(string filePath, int year)
    {
        List<IscModel> items = ReadISCExcelFile(filePath);
        using var db = new AppDbContext();

        foreach (var item in items)
        {
            var categories = item.Categories.Split(",");

            var normalizeTitle = item.Title.NormalizeTitle();
            var issn = item.ISSN.CleanIssn();
            var eissn = item.EISSN.CleanIssn();

            if (issn.Length > 8 || issn.Length < 8)
                issn = string.Empty;
            
            if (eissn.Length > 8 || eissn.Length < 8)
                eissn = string.Empty;
            
            var journals = db.Query<Journal>().Where(i => i.NormalizedTitle == normalizeTitle);

            if (journals.Any() == false && string.IsNullOrWhiteSpace(issn) == false)
                journals = journals.Where(i => i.Issn == issn);

            if (journals.Any() == false && string.IsNullOrWhiteSpace(eissn) == false)
                journals = journals.Where(i => i.EIssn == eissn);

            var journal = journals.SingleOrDefault();

            try
            {
                if (journal is null)
                {
                    var newJournal = db.Set<Journal>().Add(new Journal
                    {
                        Title = item.Title.ConvertArabicToPersian(),
                        NormalizedTitle = normalizeTitle.ConvertArabicToPersian(),
                        Issn = issn,
                        EIssn = eissn
                    }).Entity;

                    foreach (var cat in categories)
                    {
                        var startIndex = cat.IndexOf('(');
                        var qRank = string.Empty;
                        var category = string.Empty;

                        if (startIndex > 0)
                        {
                            qRank = cat.Substring(startIndex).Trim();
                            qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                            category = cat.Substring(0, startIndex).Trim().ConvertArabicToPersian();
                        }
                        else
                        {
                            category = cat.Trim().ConvertArabicToPersian();
                        }

                        if (string.IsNullOrWhiteSpace(category))
                            continue;

                        db.Set<Category>().Add(new Category
                        {
                            Journal = newJournal,
                            Title = category.Trim(),
                            NormalizedTitle = category.NormalizeTitle(),
                            Index = JournalIndex.ISC,
                            QRank = GetQrank(qRank),
                            If = item.IF,
                            Year = year,
                            Customer = "Jiro"
                        });
                    }
                }
                else
                {
                    foreach (var cat in categories)
                    {
                        var startIndex = cat.IndexOf('(');
                        var qRank = string.Empty;
                        var category = string.Empty;

                        if (startIndex > 0)
                        {
                            qRank = cat.Substring(startIndex).Trim();
                            qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                            category = cat.Substring(0, startIndex).Trim().ConvertArabicToPersian();
                        }
                        else
                        {
                            category = cat.Trim().ConvertArabicToPersian();
                        }

                        if (string.IsNullOrWhiteSpace(category))
                            continue;

                        var record = db.Set<Category>()
                            .Where(i => i.JournalId == journal.Id)
                            .Where(i => i.Year == year)
                            .Where(i => i.Index == JournalIndex.ISC)
                            .FirstOrDefault(i => i.NormalizedTitle == category.NormalizeTitle());

                        if (record != null)
                        {
                            record.If = item.IF;
                            record.QRank = GetQrank(qRank);
                        }
                        else
                        {
                            db.Set<Category>().Add(new Category
                            {
                                JournalId = journal.Id,
                                Title = category.Trim(),
                                NormalizedTitle = category.NormalizeTitle(),
                                Index = JournalIndex.ISC,
                                QRank = GetQrank(qRank),
                                If = item.IF,
                                Year = year,
                                Customer = "Jiro"
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

    static List<IscModel> ReadISCExcelFile(string filePath)
    {
        var list = new List<IscModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var journal = new IscModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    ISSN = worksheet.Cells[row, 2].Text,
                    EISSN = worksheet.Cells[row, 3].Text,
                    IF = decimal.TryParse(worksheet.Cells[row, 4].Text, out decimal ifValue) ? ifValue : 0,
                    Year = int.TryParse(worksheet.Cells[row, 5].Text, out int yearValue) ? yearValue : 0,
                    Categories = worksheet.Cells[row, 6].Text,
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

    public class IscModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public decimal IF { get; set; }
        public int Year { get; set; }
        public string Categories { get; set; }
    }
}