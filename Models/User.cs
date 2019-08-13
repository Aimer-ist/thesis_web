using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EyeWeb_Again.Models {
    public class User {
        public int Id { get; set; }
        public int order { get; set; }
        public int left { get; set; }
        public int right { get; set; }
    }
}