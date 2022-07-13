using System.ComponentModel.DataAnnotations;
using static CarSale.Models.Enumerators;

namespace CarSale.Models
{
    public class Car
    {
      public int Id { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }

        public int Year { get; set; }

        public Colour color { get; set; }

        public NewUsed NewOrUsed { get; set; }

        public string? ImageLink { get; set; }

        public Sstates States { get; set; }

        public int Price { get; set; }
        [DataType(DataType.MultilineText)]
        public string? UploadedBy { get; set; }
        
        [DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        [Display(Name ="Posted On")]
        public DateTime? InDate { get; set; }

        //New properties

        /*public byte Audio { get; set; }
        public bool AC { get; set; }
        public bool RemoteKey { get; set; }*/
        


    }
}













