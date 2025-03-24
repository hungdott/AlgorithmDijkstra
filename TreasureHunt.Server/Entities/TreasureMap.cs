using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Entities
{
    public class TreasurePuzzle
    {
        [Key]
        public int Id { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int MaxChestNumber { get; set; }
        public string MatrixJson { get; set; }
        public decimal MinimumFuelRequired { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
