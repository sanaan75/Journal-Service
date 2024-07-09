namespace Entities.Security;

public enum Permission
{
    Users = 10,
    Users_Create = 10_01,
    Users_List = 10_02,
    Users_Details = 10_03,
    Users_Edit = 10_04,
    
    Journals = 20,
    Journals_Create = 20_01,
    Journals_List = 20_02,
    Journals_Details = 20_03,
    Journals_Edit = 20_04,

    Conferences = 30,
    Conferences_Create = 30_01,
    Conferences_List = 30_02,
    Conferences_Details = 30_03,
    Conferences_Edit = 30_04,
    
    Reports = 40,
    Reports_Create = 40_01,
    Reports_List = 40_02,
    Reports_Details = 40_03,

    Permissions = 50,
    Permissions_Create = 50_01,
    Permissions_List = 50_02,
    Permissions_Details = 50_03
}