using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace D2P_Exam
{
    public class Patient
    {
        [BsonElement("id")]
        public String Id { get; set; }

        [BsonElement("name")]
        public String Name { get; set; }

        [BsonElement("dath of birth")]
        public String DathOfBirth { get; set; }

        [BsonElement("phone")]
        public String Phone { get; set; }

        [BsonElement("email")]
        public String Email { get; set; }

        public List<byte[]> images { get; set; }
        
        public Patient(string id, string name, string dathOfBirth, string phone, string email, List<byte[]> ct)
        {
            Id = id;
            Name = name;
            DathOfBirth = dathOfBirth;
            Phone = phone;
            Email = email;
            images = ct;
        }
    }
}
