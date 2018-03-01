using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
    public class Related
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        [NotMapped]
        public string ContentTitle { get; set; }
        public long RelevanceId { get; set; }
        public int TitleRelevance { get; set; }
        public int CollRelevance { get; set; }
        public int OrderRelevance { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
