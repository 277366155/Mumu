using EFCache;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace WebApp.Fk.Test.Bill
{
    public class TestDBContext:DbContext
    {

        private static Dictionary<string, TestDBContext> dicCacheDBContext = new Dictionary<string, TestDBContext>();
        public TestDBContext() : base("name=MyDB")
        { 
            
        }
        //public TestDBContext(string  name ="test") 
        //{
        //    if (!dicCacheDBContext.Keys.Any(a => a.Equals(name)))
        //    {
        //        dicCacheDBContext.Add(name, new TestDBContext());
        //    }
        //}

        public TestDBContext GetCacheContext(string name = "test")
        {
            if(dicCacheDBContext.TryGetValue(name, out var result))
            {
                return result;
            }
            return null;
        }
        public DbSet<Person> Persons { get; set; }
    }

    [Table("Person")]
    public class Person
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}