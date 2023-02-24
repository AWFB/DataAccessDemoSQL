﻿using DataAccessLibrary.Models;

namespace DataAccessLibrary;

public class SqlCrud
{
    private readonly string _connectionString;
    private SqlDataAccess db = new SqlDataAccess();

    public SqlCrud(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<BasicContactModel> GetAllContacts()
    {
        string sql = "select Id, FirstName, LastName from dbo.Contacts";

        // new { } - No parameters, passing in an empty object
        return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
    }

    public FullContactModel GetFullContactById(int id)
    {
        string sql = "select Id, FirstName, LastName from dbo.Contacts where Id = @Id";
        FullContactModel output = new FullContactModel();

        output.BasicInfo = db.LoadData<BasicContactModel, dynamic>(sql, new { Id = id }, _connectionString).FirstOrDefault();
        if (output.BasicInfo == null)
        {
            //Do something to tell user that no record was found
            throw new Exception("User not found");
        }

        sql = @"select e.* from dbo.EmailAddresses e
                inner join dbo.ContactEmail ce on ce.EmailAddressId = e.Id
                where ce.ContactId = 1";

        output.EmailAddresses = db.LoadData<EmailAddressModel, dynamic>(sql, new { Id = id }, _connectionString);

        sql = @"select p.* from dbo.PhoneNumbers p
               inner join dbo.ContactPhoneNumbers cp on cp.PhoneNumberId = p.Id
               where cp.ContactId = @Id";

        output.PhoneNumbers = db.LoadData<PhoneNumberModel, dynamic>(sql, new { Id = id }, _connectionString);

        return output;
    }

    public void CreateContact(FullContactModel contact)
    {
        string sql = "insert into dbo.Contacts (FirstName, LastName) values (@FirstName, @LastName);";

        db.SaveData(sql,
                    new { contact.BasicInfo.FirstName, contact.BasicInfo.LastName },
                    _connectionString);

        sql = "select Id from dbo.Contacts where FirstName = @FirstName and @LastName = @LastName;";

        int contactId = db.LoadData<IdLookupModel, dynamic>(
            sql,
            new { FirstName = contact.BasicInfo.FirstName, LastName = contact.BasicInfo.LastName },
            _connectionString).First().Id;


        foreach (var phoneNumber in contact.PhoneNumbers)
        {
            if (phoneNumber.Id == 0) // If phone number exists
            {
                sql = "insert into dbo.PhoneNumbers (PhoneNumber) values (@PhoneNumber);";
                db.SaveData(sql, new { phoneNumber.PhoneNumber }, _connectionString);

                sql = "select Id from dbo.phoneNumbers where PhoneNumber = @PhoneNumber;";
                phoneNumber.Id = db.LoadData<IdLookupModel, dynamic>(
                    sql,
                    new { phoneNumber.PhoneNumber },
                    _connectionString).First().Id;
            }

            sql = "insert into dbo.ContactPhoneNumbers (ContactId, PhoneNumberId) values (@ContactId, @PhoneNumberId);";
            db.SaveData(sql, new { ContactId = contactId, PhoneNumberId = phoneNumber.Id }, _connectionString);
        }

        foreach (var email in contact.EmailAddresses)
        {
            if (email.Id == 0)
            {
                sql = "insert into dbo.EmailAddresses (EmailAddress) values (@EmailAddress);";
                db.SaveData(sql, new { email.EmailAddress }, _connectionString);

                sql = "select Id from dbo.EmailAddresses where EmailAddress = @EmailAddress;";
                email.Id = db.LoadData<IdLookupModel, dynamic>(
                    sql,
                    new { email.EmailAddress },
                    _connectionString).First().Id;
            }

            sql = "insert into dbo.ContactEmail (ContactId, EmailAddressId) values (@ContactId, @EmailAddressId);";
            db.SaveData(sql, new { ContactId = contactId, EmailAddressId = email.Id }, _connectionString);
        }
    }

    public void UpdateContactName(BasicContactModel contact)
    {
        string sql = "update dbo.Contacts set FirstName = @Firstname, LastName = @LastName where Id = @Id;";
        db.SaveData(sql, contact, _connectionString);
    }

    public void RemovePhoneNumberFromContact(int contactId, int phoneNumberId)
    {
        string sql = "select Id, ContactId, PhoneNumberId from dbo.ContactPhoneNumbers where PhoneNumberId = @PhoneNumberId;";
        var links = db.LoadData<ContactPhoneNumberModel, dynamic>(sql, new { PhoneNumberId = phoneNumberId }, _connectionString);

        sql = "delete from dbo.ContactPhoneNumbers where PhoneNumberId = @PhoneNumberId and ContactId = @ContactId;";
        db.SaveData(sql, new { phoneNumberId = phoneNumberId, ContactId = contactId }, _connectionString);

        if (links.Count == 1) //more than 1 record
        {
            sql = "delete from dbo.phoneNumbers where Id = @PhoneNumberId;";
            db.SaveData(sql, new { phoneNumberId = phoneNumberId }, _connectionString);
        }
    }

}