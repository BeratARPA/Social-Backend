using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Dtos
{
    public class ConfirmPhoneRequestDto
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
