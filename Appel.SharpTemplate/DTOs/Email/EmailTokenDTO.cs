using System;

namespace Appel.SharpTemplate.DTOs.Email
{
    public class EmailTokenDTO
    {
        public DateTime Validity { get; set; }
        public string Email { get; set; }
    }
}
