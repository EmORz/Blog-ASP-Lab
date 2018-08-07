using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogV3.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [ForeignKey(name:"Author")]
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }
    }
}
