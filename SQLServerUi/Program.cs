using Microsoft.Extensions.Configuration;
using DataAccessLibrary;
using DataAccessLibrary.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        SqlCrud sql = new SqlCrud(GetConnectionString());

        //ReadAllContacts(sql);

        //ReadContact(sql, 10);

        //CreateNewContact(sql);

        //UpdateContact(sql);

        RemovePhoneNumberFromContact(sql, 10, 11);

        Console.WriteLine("Finished!");
    }

    private static void RemovePhoneNumberFromContact(SqlCrud sql, int contactId, int phoneNumberId)
    {
        sql.RemovePhoneNumberFromContact(contactId, phoneNumberId);
    }

    private static void UpdateContact(SqlCrud sql)
    {
        BasicContactModel contact = new BasicContactModel
        {
            Id = 1,
            FirstName = "Tony",
            LastName = "BeardHorn"
        };

        sql.UpdateContactName(contact);
    }

    private static void CreateNewContact(SqlCrud sql)
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

    private static void ReadAllContacts(SqlCrud sql)
    {
        var rows = sql.GetAllContacts();
        foreach (var row in rows)
        {
            Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
        }
    }

    private static void ReadContact(SqlCrud sql, int contactId)
    {
        var contact = sql.GetFullContactById(contactId);
        Console.WriteLine($"{contact.BasicInfo.Id}: {contact.BasicInfo.FirstName} {contact.BasicInfo.LastName}");
        
    }

    private static string GetConnectionString(string connectionStringName = "Default")
    {
        string output = "";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var config = builder.Build();

        output = config.GetConnectionString(connectionStringName);

        return output;
    }
}