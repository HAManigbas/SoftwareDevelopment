using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace quiz2
{
    public class Diet
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }
        public double Servings { get; set; }
    }
}
