using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.Models.Stratedgeme.Strategy
{
    public class CrossConfigModel
    {
        public string Cross { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DefaultTicketSize { get; set; }
    }
}
