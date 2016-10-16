using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onstogram.Model
{
    public class Comment
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
    }
}
