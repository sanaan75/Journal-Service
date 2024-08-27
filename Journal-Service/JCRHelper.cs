using Journal_Service.Data;
using Journal_Service.Entities;
using OfficeOpenXml;

namespace Journal_Service;

public class JCRHelper
{
    public void ImportData(string filePath, int year)
    {
        List<JCRModel> items = ReadJCRExcelFile(filePath);
        using var db = new AppDbContext();

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Category) || item.IF is null)
                continue;

            var journal = db.Set<Journal>().FirstOrDefault(i =>
                i.NormalizedTitle == item.Title.NormalizeTitle() || i.Issn == item.ISSN.CleanIssn() ||
                i.EIssn == item.EISSN.CleanIssn());

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

                    db.Set<Category>().Add(new Category
                    {
                        Journal = newJournal,
                        Title = item.Category.Trim().ConvertArabicToPersian(),
                        NormalizedTitle = item.Category.NormalizeTitle().ConvertArabicToPersian(),
                        Index = JournalIndex.JCR,
                        QRank = GetQrank(item.Quartile),
                        If = item.IF,
                        Year = year,
                        Customer = "Jiro",
                        Edition = item.Edition.Trim()
                    });
                }
                else
                {
                    journal.Issn = item.ISSN.CleanIssn();
                    journal.EIssn = item.EISSN.CleanIssn();
                    journal.Publisher = item.Publisher.Trim();

                    var record = db.Set<Category>()
                        .Where(i => i.JournalId == journal.Id)
                        .Where(i => i.Year == year)
                        .Where(i => i.Index == JournalIndex.JCR)
                        .FirstOrDefault(i => i.NormalizedTitle == item.Category.NormalizeTitle());

                    if (record != null)
                    {
                        record.If = item.IF;
                        record.QRank = GetQrank(item.Quartile);
                    }
                    else
                    {
                        db.Set<Category>().Add(new Category
                        {
                            JournalId = journal.Id,
                            Title = item.Category.Trim(),
                            NormalizedTitle = item.Category.NormalizeTitle().ConvertArabicToPersian(),
                            Index = JournalIndex.JCR,
                            QRank = GetQrank(item.Quartile),
                            If = item.IF,
                            Year = year,
                            Customer = "Jiro",
                            Edition = item.Edition.Trim(),
                        });
                    }
                }

                db.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"{item.Title}, {item.Category}, {item.IF}, {item.ISSN}");
        }
    }

    public void InsertCategories(string filePath, int year)
    {
        List<MIFModel> categories = ReadMIFExcelFile(filePath);

        using var db = new AppDbContext();

        foreach (var category in categories)
        {
            try
            {
                var items = db.Set<Category>()
                    .Where(i => i.Year == year)
                    .Where(i => i.Index == JournalIndex.JCR)
                    .Where(i => i.NormalizedTitle == category.Title.NormalizeTitle())
                    .ToList();

                foreach (var item in items)
                {
                    item.Mif = category.MIF;
                }

                Console.WriteLine($"{category.Title} - {category.MIF}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        db.Save();
    }

    public void InsertAIF(string filePath, int year)
    {
        List<AIFModel> categories = Read_AIF_ExcelFile(filePath);

        using var db = new AppDbContext();

        foreach (var category in categories)
        {
            try
            {
                var items = db.Set<Category>()
                    .Where(i => i.Year == year)
                    .Where(i => i.Index == JournalIndex.JCR)
                    .Where(i => i.NormalizedTitle == category.Title.NormalizeTitle())
                    .ToList();

                foreach (var item in items)
                {
                    item.Mif = category.MIF;
                    item.Aif = category.AIF;
                }

                Console.WriteLine($"{category.Title} - Mif : {category.MIF} - Aif : " + category.AIF);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
        }

        db.Save();
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
                    Publisher = worksheet.Cells[row, 2].Text,
                    ISSN = worksheet.Cells[row, 3].Text,
                    EISSN = worksheet.Cells[row, 4].Text,
                    Category = worksheet.Cells[row, 5].Text,
                    Edition = worksheet.Cells[row, 6].Text,
                    IF = decimal.TryParse(worksheet.Cells[row, 7].Text, out decimal ifValue) ? ifValue : 0,
                    Quartile = worksheet.Cells[row, 8].Text,
                    JIFRank = worksheet.Cells[row, 9].Text,
                };
                list.Add(journal);
            }
        }

        return list;
    }

    static List<MIFModel> ReadMIFExcelFile(string filePath)
    {
        var list = new List<MIFModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var model = new MIFModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    Group = worksheet.Cells[row, 2].Text,
                    MIF = decimal.TryParse(worksheet.Cells[row, 3].Text, out decimal mifValue) ? mifValue : 0,
                };
                list.Add(model);
            }
        }

        return list;
    }

    static List<MIFModel> ReadVezaratinExcelFile(string filePath)
    {
        var list = new List<MIFModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var model = new MIFModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    Group = worksheet.Cells[row, 2].Text,
                    MIF = decimal.TryParse(worksheet.Cells[row, 3].Text, out decimal mifValue) ? mifValue : 0,
                };
                list.Add(model);
            }
        }

        return list;
    }

    static List<AIFModel> Read_AIF_ExcelFile(string filePath)
    {
        var list = new List<AIFModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var model = new AIFModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    MIF = decimal.TryParse(worksheet.Cells[row, 2].Text, out decimal mifValue) ? mifValue : 0,
                    AIF = decimal.TryParse(worksheet.Cells[row, 3].Text, out decimal aifValue) ? mifValue : 0,
                };
                list.Add(model);
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
        public string Publisher { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string Category { get; set; }
        public string Edition { get; set; }
        public decimal? IF { get; set; }
        public string Quartile { get; set; }
        public string JIFRank { get; set; }
    }

    public class MIFModel
    {
        public string Title { get; set; }
        public string Group { get; set; }
        public decimal MIF { get; set; }
    }

    public class AIFModel
    {
        public string Title { get; set; }
        public decimal MIF { get; set; }
        public decimal AIF { get; set; }
    }
}