using System;

namespace iSHARE.Identity
{

    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AspNetUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
    }
}
