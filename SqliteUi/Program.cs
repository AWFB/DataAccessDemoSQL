

using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;

SqliteCrud sql = new SqliteCrud(GetConnectionString());

//ReadAllContacts(sql);

//ReadContact(sql, 2);

//CreateNewContact(sql);

UpdateContact(sql);
ReadAllContacts(sql);

//RemovePhoneNumberFromContact(sql, 10, 11);

Console.WriteLine("Finished (Sqlite)");


static void RemovePhoneNumberFromContact(SqliteCrud sql, int contactId, int phoneNumberId)
{
    sql.RemovePhoneNumberFromContact(contactId, phoneNumberId);
}

static void UpdateContact(SqliteCrud sql)
{
    BasicContactModel contact = new BasicContactModel
    {
        Id = 1,
        FirstName = "Tony",
        LastName = "BeardHorn"
    };

    sql.UpdateContactName(contact);
}

static void CreateNewContact(SqliteCrud sql)
{
    FullContactModel user = new FullContactModel
    {
        BasicInfo = new BasicContactModel
        {
            FirstName = "Becca",
            LastName = "Marshall"
        }
    };

    user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "Bec@Marshall.com" });
    user.EmailAddresses.Add(new EmailAddressModel { Id = 2, EmailAddress = "AWFB@gmail.com" });

    user.PhoneNumbers.Add(new PhoneNumberModel { Id = 1, PhoneNumber = "0151 252 5486" });
    user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "0151 252 6666" });

    sql.CreateContact(user);
}

static void ReadAllContacts(SqliteCrud sql)
{
    var rows = sql.GetAllContacts();
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
    }
}

static void ReadContact(SqliteCrud sql, int contactId)
{
    var contact = sql.GetFullContactById(contactId);
    Console.WriteLine($"{contact.BasicInfo.Id}: {contact.BasicInfo.FirstName} {contact.BasicInfo.LastName}");

}

static string GetConnectionString(string connectionStringName = "Default")
{
    string output = "";

    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    var config = builder.Build();

    output = config.GetConnectionString(connectionStringName);

    return output;
}
