using Core.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Reminder : IEntity
    {
        public int Id { get; set; }
        [Required]
        public string SendTo { get; set; }
        [Required]
        public DateTime SendAt { get; set; }
        [Required]
        [MinLength(5)]
        public string Method { get; set; }
        [Required]
        [MinLength(10)]
        public string Content { get; set; }
    }
}
