using Entities;
using Entities.Journals;

namespace UseCases.Journals;

public class AddJournal : IAddJournal
{
    private readonly IDb _db;
    private readonly ICleanTitle _cleanTitle;
    private readonly ICleanIssn _cleanIssn;

    public AddJournal(IDb db, ICleanTitle cleanTitle, ICleanIssn cleanIssn)
    {
        _db = db;
        _cleanTitle = cleanTitle;
        _cleanIssn = cleanIssn;
    }

    public Journal Respond(IAddJournal.Request request)
    {
        Check.Given(request.Title, () => "عنوان وارد نشده است");
        
        // todo : waiting for db error - title and issn is unique
        // var isDuplicate = _db.Query<Journal>().Any(i => i.Title.Equals(journalTitle));
        // AppValidate.False(isDuplicate, "مجله تکراری است");

        var journalTitle = _cleanTitle.Respond(request.Title);
        var issn = _cleanTitle.Respond(request.Issn);
        var eissn = _cleanTitle.Respond(request.EIssn);
        return _db.Set<Journal>().Add(new Journal
        {
            Title = journalTitle,
            Issn = issn,
            EIssn = eissn,
            WebSite = request.WebSite,
            Publisher = request.Publisher,
            Country = request.Country,
        }).Entity;
    }
}