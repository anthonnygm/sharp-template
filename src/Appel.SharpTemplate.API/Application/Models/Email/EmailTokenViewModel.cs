using System;

namespace Appel.SharpTemplate.API.Application.Models
{
    public class EmailTokenViewModel
    {
        public string Email { get; set; }
        public DateTime Validity { get; set; }
    }
}
