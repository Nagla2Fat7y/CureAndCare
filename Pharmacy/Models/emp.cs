using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

    namespace Pharmacy.Models
    {
        public class emp : IdentityUser
        {
            public  int? Salary { get; set; }
            public string? Address { get; set; }
            public string? Phone { get; set; }
            public string? Shift { get; set; }
            public string? Permission_Level { get; set; }



        public DateTime HireDate { get; set; }

    
            public DateTime LoginTimestamp { get; set; }

            public List<Invoice>? Invoices { get; set; }

        }
    
    }
