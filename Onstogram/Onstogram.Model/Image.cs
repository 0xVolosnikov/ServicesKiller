using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onstogram.Model
{
    public class Image
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Time { get; set; }
        public byte[] Picture { get; set; }
        public string[] HashTag { get; set; }
    }
}
