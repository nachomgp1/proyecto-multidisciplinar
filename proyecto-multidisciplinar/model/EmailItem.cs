using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAIL
{
    public class EmailItem
    {
        public string MessageId { get; set; }
        public string From { get; set; }
        public string Hour { get; set; }
        public string Subject { get; set; }
        public bool IsRead { get; set; }
    }
}
