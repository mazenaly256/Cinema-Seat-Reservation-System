using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace identity_service;

public class ApplicationUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; }
}
