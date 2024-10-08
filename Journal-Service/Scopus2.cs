using Journal_Service.Data;
using Journal_Service.Entities;
using OfficeOpenXml;

namespace Journal_Service;

public class Scopus2Helper
{
    public void ImportData(string filePath)
    {
        List<DataModel> items = ReadRecordsExcelFile(filePath);
        using var db = new AppDbContext();

        int rowNo = 1;
        foreach (var item in items)
        {
            Console.WriteLine("row : " + rowNo);
            rowNo++;
            
            if (string.IsNullOrWhiteSpace(item.Title) && string.IsNullOrWhiteSpace(item.ISSN) &&
                string.IsNullOrWhiteSpace(item.EISSN))
                continue;

            if (item.ActiveState.Trim() == "Inactive" || item.Type.Trim() != "Journal")
                continue;

            var normalizeTitle = item.Title.NormalizeTitle();
            var issn = item.ISSN.CleanIssn();
            var eissn = item.EISSN.CleanIssn();

            var journals = db.Set<Journal>().Where(i => i.NormalizedTitle == normalizeTitle);

            if (string.IsNullOrWhiteSpace(issn) == false)
                journals = journals.Where(i => i.Issn == issn);
                
            if (string.IsNullOrWhiteSpace(eissn) == false)
                journals = journals.Where(i => i.EIssn == eissn);

            var journal = journals.FirstOrDefault();

            var yearsText = item.Coverage.Split(";").FirstOrDefault();

            if (yearsText is null)
                continue;

            try
            {
                var from = Convert.ToInt32(yearsText.Split("-").First());
                var to = yearsText.Split("-").Length > 1 ? Convert.ToInt32(yearsText.Split("-")[1]) : 2024;

                if (to < 2015)
                    continue;

                if (journal is not null)
                {
                    var dup = db.Query<Category>()
                        .Where(i => i.Journal.NormalizedTitle == item.Title.NormalizeTitle() ||
                                    i.Journal.Issn == item.ISSN.CleanIssn() ||
                                    i.Journal.EIssn == item.EISSN.CleanIssn())
                        .Any(i => i.Index == JournalIndex.Scopus);

                    if (dup == true)
                        continue;

                    for (int y = from; y <= to; y++)
                    {
                        var title = "scopus without Q";

                        db.Set<Category>().Add(new Category
                        {
                            Journal = journal,
                            Title = title.ConvertArabicToPersian(),
                            NormalizedTitle = title.NormalizeTitle().ConvertArabicToPersian(),
                            Index = JournalIndex.Scopus,
                            Year = y,
                            Customer = "Jiro"
                        });
                    }

                    db.Save();
                }
                else
                {
                    var newJournal = db.Set<Journal>().Add(new Journal
                    {
                        Title = item.Title.ConvertArabicToPersian(),
                        NormalizedTitle = item.Title.NormalizeTitle().ConvertArabicToPersian(),
                        Issn = item.ISSN.CleanIssn(),
                        EIssn = item.EISSN.CleanIssn(),
                        Publisher = item.Publisher
                    }).Entity;

                    for (int y = from; y <= to; y++)
                    {
                        var dup = db.Query<Category>()
                            .Where(i => i.Journal.NormalizedTitle == item.Title.NormalizeTitle() ||
                                        i.Journal.Issn == item.ISSN.CleanIssn() ||
                                        i.Journal.EIssn == item.EISSN.CleanIssn())
                            .Where(i => i.Year == y)
                            .Any(i => i.Index == JournalIndex.Scopus);

                        if (dup == true)
                            continue;

                        var title = "scopus without Q";

                        db.Set<Category>().Add(new Category
                        {
                            Journal = newJournal,
                            Title = title.ConvertArabicToPersian(),
                            NormalizedTitle = title.NormalizeTitle().ConvertArabicToPersian(),
                            Index = JournalIndex.Scopus,
                            Year = y,
                            Customer = "Jiro"
                        });
                    }

                    db.Save();
                }
            }
            catch (Exception ex)
            {
                //ignored
            }
        }
    }

    static List<DataModel> ReadRecordsExcelFile(string filePath)
    {
        var list = new List<DataModel>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var model = new DataModel
                {
                    Title = worksheet.Cells[row, 1].Text,
                    ISSN = worksheet.Cells[row, 2].Text,
                    EISSN = worksheet.Cells[row, 3].Text,
                    ActiveState = worksheet.Cells[row, 4].Text,
                    Coverage = worksheet.Cells[row, 5].Text,
                    Type = worksheet.Cells[row, 6].Text,
                    Publisher = worksheet.Cells[row, 7].Text,
                };
                list.Add(model);
            }
        }

        return list;
    }

    public class DataModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string ActiveState { get; set; }
        public string Coverage { get; set; }
        public string Type { get; set; }
        public string Publisher { get; set; }
    }
}