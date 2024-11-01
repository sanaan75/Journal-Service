﻿using Journal_Service.Data;
using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Journal_Service;

public class JCRHelper
{
    public void ImportData(string filePath, int year)
    {
        List<JCRModel> items = ReadJCRExcelFile(filePath);

        Parallel.ForEach(items, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, item =>
        {
            using (var db = new AppDbContext())
            {
                db.Database.ExecuteSqlRaw("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");

                if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Category) ||
                    item.IF is null)
                    return;
                try
                {
                    var normalizeTitle = item.Title.NormalizeTitle();
                    var categoryTitle = item.Category.Trim().ConvertArabicToPersian();
                    var categoryNormalizedTitle = item.Category.NormalizeTitle().ConvertArabicToPersian();
                    var issn = item.ISSN.CleanIssn();
                    var eissn = item.EISSN.CleanIssn();

                    var journals = db.Set<Journal>().Where(i => i.NormalizedTitle == normalizeTitle || i.Issn == issn);

                    if (journals.Any() == false && string.IsNullOrWhiteSpace(eissn) == false)
                        journals = journals.Where(i => i.EIssn == eissn);

                    var journal = journals.FirstOrDefault();


                    decimal? rank = null;
                    if (string.IsNullOrWhiteSpace(item.JIFRank) == false)
                    {
                        var rankStr = item.JIFRank.Split("/");
                        rank = (Convert.ToDecimal(rankStr[0]) / Convert.ToDecimal(rankStr[1])) * 100;
                    }

                    var qRank = GetQrank(item.Quartile, rank);
                    var editions = item.Edition.Split(",");

                    foreach (var edition in editions)
                    {
                        if (journal is null)
                        {
                            var newJournal = db.Set<Journal>().Add(new Journal
                            {
                                Title = item.Title.ConvertArabicToPersian(),
                                NormalizedTitle = normalizeTitle.ConvertArabicToPersian(),
                                Issn = issn == String.Empty ? "-" : issn,
                                EIssn = eissn == String.Empty ? "-" : eissn
                            }).Entity;

                            db.Set<Category>().Add(new Category
                            {
                                Journal = newJournal,
                                Title = categoryTitle,
                                NormalizedTitle = categoryNormalizedTitle,
                                Index = JournalIndex.JCR,
                                QRank = qRank,
                                If = item.IF,
                                Year = year,
                                Customer = "Jiro",
                                Edition = edition.Trim()
                            });
                        }
                        else
                        {
                            if (journal.Publisher is null)
                                journal.Publisher = item.Publisher.Trim();

                            var record = db.Set<Category>()
                                .Where(i => i.JournalId == journal.Id)
                                .Where(i => i.Year == year)
                                .Where(i => i.Index == JournalIndex.JCR)
                                .Where(i => i.Edition == edition.Trim())
                                .FirstOrDefault(i => i.NormalizedTitle == categoryNormalizedTitle);

                            if (record is null)
                            {
                                db.Set<Category>().Add(new Category
                                {
                                    JournalId = journal.Id,
                                    Title = categoryTitle,
                                    NormalizedTitle = categoryNormalizedTitle,
                                    Index = JournalIndex.JCR,
                                    QRank = qRank,
                                    If = item.IF,
                                    Year = year,
                                    Customer = "Jiro",
                                    Edition = edition.Trim(),
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

                Console.WriteLine(item.Category);
            }
        });
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

    public void InsertAif(string filePath, int year)
    {
        List<AIFModel> categories = Read_AIF_ExcelFile(filePath);

        using var db = new AppDbContext();

        foreach (var category in categories)
        {
            try
            {
                var editions = category.Edition.Split(",");

                foreach (var edition in editions)
                {
                    if (string.IsNullOrWhiteSpace(edition) == true)
                        continue;

                    var items = db.Set<Category>()
                        .Where(i => i.Year == year)
                        .Where(i => i.Index == JournalIndex.JCR)
                        .Where(i => i.Edition == edition.Trim())
                        .Where(i => i.NormalizedTitle == category.Title.NormalizeTitle())
                        .ToList();

                    foreach (var item in items)
                    {
                        item.Mif = category.MIF;
                        item.Aif = category.AIF;
                    }

                    Console.WriteLine($"{category.Title} - Mif : {category.MIF} - Aif : " + category.AIF);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            db.Save();
        }
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
                    Quartile = worksheet.Cells[row, 7].Text,
                    IF = decimal.TryParse(worksheet.Cells[row, 8].Text, out decimal ifValue) ? ifValue : 0,
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
                    AIF = decimal.TryParse(worksheet.Cells[row, 3].Text, out decimal aifValue) ? aifValue : 0,
                    Edition = worksheet.Cells[row, 4].Text,
                };
                list.Add(model);
            }
        }

        return list;
    }

    JournalQRank? GetQrank(string quartile, decimal? rank)
    {
        if (rank is not null)
        {
            if (rank < 2)
                return JournalQRank.OnePercent;
            if (rank < 6)
                return JournalQRank.FivePercent;
            if (rank < 11)
                return JournalQRank.TenPercent;
        }

        switch (quartile)
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
        public string Edition { get; set; }
    }
}