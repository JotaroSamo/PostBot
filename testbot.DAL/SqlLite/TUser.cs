using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testbots.DAL.SqlLite
{
    public class TUser
    {

        public TUser() { } // Параметрless конструктор

        public TUser(long userMovieId) // Измените имя параметра здесь
        {
            UserMovieId = userMovieId;
        }

        [Key]
        public int Id { get; set; }
        public long UserMovieId { get; set; }

    }
}
