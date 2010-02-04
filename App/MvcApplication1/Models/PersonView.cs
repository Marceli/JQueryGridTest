using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;   

namespace MvcApplication1.Models
{
    public class PersonView
    {
        public virtual int? Id { get; set; }
        [DisplayName("Your first name #")] 
        [Required(ErrorMessage="First Name is required")]
        public virtual string NameFirst { get; set; }
        [StringLength(5, ErrorMessage = "Last name must be less than 5")]
        public virtual string NameLast { get; set; }
    }
}
