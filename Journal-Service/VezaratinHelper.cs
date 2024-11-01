﻿using Journal_Service.Data;
using Journal_Service.Entities;
using OfficeOpenXml;

namespace Journal_Service;

public class VezaratinHelper
{
    public void ImportData(string filePath)
    {
        List<DataModel> items = ReadVezaratinExcelFile(filePath);

        using var db = new AppDbContext();

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                continue;

            List<int> years = new List<int>
            {
                2019,
                2020,
                2021,
                2022
            };

            var normalizeTitle = item.Title.NormalizeTitle();
            var issn = item.ISSN.CleanIssn();
            var eissn = item.EISSN.CleanIssn();

            var journals = db.Set<Journal>().Where(i => i.NormalizedTitle == normalizeTitle || i.Issn == issn);

            if (journals.Any() == false && string.IsNullOrWhiteSpace(eissn) == false)
                journals = journals.Where(i => i.EIssn == eissn);

            var journal = journals.FirstOrDefault();

            try
            {
                if (journal is not null)
                {
                    if (string.IsNullOrWhiteSpace(item.Category))
                        continue;

                    foreach (var year in years)
                    {
                        var categories = db.Query<Category>()
                            .Where(i => i.Journal.NormalizedTitle == normalizeTitle);

                        if (string.IsNullOrWhiteSpace(issn) == false)
                            categories = categories.Where(i => i.Journal.Issn == issn);

                        if (string.IsNullOrWhiteSpace(eissn) == false)
                            categories = categories.Where(i => i.Journal.EIssn == eissn);

                        var dup = categories.Where(i => i.Year == year)
                            .Any(i => i.Index == JournalIndex.ISC);

                        if (dup == true)
                            continue;

                        var category = new Category
                        {
                            Journal = journal,
                            Title = item.Category.Trim().ConvertArabicToPersian(),
                            NormalizedTitle = item.Category.NormalizeTitle().ConvertArabicToPersian(),
                            Index = JournalIndex.ISC,
                            Year = year,
                            Customer = "Jiro"
                        };

                        switch (year)
                        {
                            case 2019:
                            {
                                category.ISCRank = GetValue(item.Year_2019);
                                break;
                            }
                            case 2020:
                            {
                                category.ISCRank = GetValue(item.Year_2020);
                                break;
                            }
                            case 2021:
                            {
                                category.ISCRank = GetValue(item.Year_2021);
                                break;
                            }
                            case 2022:
                            {
                                category.ISCRank = GetValue(item.Year_2022);
                                break;
                            }
                        }

                        db.Set<Category>().Add(category);
                        db.Save();

                        Console.WriteLine(category.Year + "  " + category.Title + " : " + category.ISCRank.ToString());
                    }
                }
                else
                {
                    var newJournal = db.Set<Journal>().Add(new Journal
                    {
                        Title = item.Title.ConvertArabicToPersian(),
                        NormalizedTitle = item.Title.NormalizeTitle().ConvertArabicToPersian(),
                        Issn = item.ISSN.CleanIssn(),
                        EIssn = item.EISSN.CleanIssn()
                    }).Entity;

                    if (string.IsNullOrWhiteSpace(item.Category))
                        continue;

                    foreach (var year in years)
                    {
                        var categories = db.Query<Category>()
                            .Where(i => i.Journal.NormalizedTitle == normalizeTitle);

                        if (string.IsNullOrWhiteSpace(issn) == false)
                            categories = categories.Where(i => i.Journal.Issn == issn);

                        if (string.IsNullOrWhiteSpace(eissn) == false)
                            categories = categories.Where(i => i.Journal.EIssn == eissn);

                        var dup = categories.Where(i => i.Year == year)
                            .Any(i => i.Index == JournalIndex.ISC);

                        if (dup == true)
                            continue;

                        var category = new Category
                        {
                            Journal = newJournal,
                            Title = item.Category.Trim().ConvertArabicToPersian(),
                            NormalizedTitle = item.Category.NormalizeTitle().ConvertArabicToPersian(),
                            Index = JournalIndex.ISC,
                            Year = year,
                            Customer = "Jiro"
                        };

                        switch (year)
                        {
                            case 2019:
                            {
                                category.ISCRank = GetValue(item.Year_2019);
                                break;
                            }
                            case 2020:
                            {
                                category.ISCRank = GetValue(item.Year_2020);
                                break;
                            }
                            case 2021:
                            {
                                category.ISCRank = GetValue(item.Year_2021);
                                break;
                            }
                            case 2022:
                            {
                                category.ISCRank = GetValue(item.Year_2022);
                                break;
                            }
                        }

                        db.Set<Category>().Add(category);
                        db.Save();
                        Console.WriteLine(category.Year + "  " + category.Title + " : " + category.ISCRank.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    static List<DataModel> ReadVezaratinExcelFile(string filePath)
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
                    Category = worksheet.Cells[row, 2].Text,
                    ISSN = worksheet.Cells[row, 7].Text,
                    EISSN = worksheet.Cells[row, 8].Text,
                    Year_2019 = worksheet.Cells[row, 3].Text,
                    Year_2020 = worksheet.Cells[row, 4].Text,
                    Year_2021 = worksheet.Cells[row, 5].Text,
                    Year_2022 = worksheet.Cells[row, 6].Text,
                };
                list.Add(model);
            }
        }

        return list;
    }

    JournalValue? GetValue(string rank)
    {
        switch (rank)
        {
            case "الف":
                return JournalValue.A;
            case "ب":
                return JournalValue.B;
            case "ج":
                return JournalValue.C;
            case "د":
                return JournalValue.D;
            case "بین المللی":
                return JournalValue.International;
            default:
                return null;
        }
    }

    public class DataModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string Category { get; set; }
        public string Year_2019 { get; set; }
        public string Year_2020 { get; set; }
        public string Year_2021 { get; set; }
        public string Year_2022 { get; set; }
    }
}