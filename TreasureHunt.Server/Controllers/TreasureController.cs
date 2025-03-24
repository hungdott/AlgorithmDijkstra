using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TreasureHunt.Entities;

namespace TreasureHunt.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreasureController : ControllerBase
    {
        private readonly TreasureDbContext _context;

        public TreasureController(TreasureDbContext context)
        {
            _context = context;
        }

        [HttpPost("solve")]
        public async Task<IActionResult> SolveTreasurePuzzle([FromBody] TreasurePuzzle puzzleInput)
        {
            int[][] treasureMap = JsonSerializer.Deserialize<int[][]>(puzzleInput.MatrixJson);
            decimal minFuel = Math.Round(CalculateMinimumFuel(treasureMap, puzzleInput.RowCount, puzzleInput.ColumnCount, puzzleInput.MaxChestNumber), 2);

            puzzleInput.MinimumFuelRequired = minFuel;
            _context.TreasurePuzzles.Add(puzzleInput);
            await _context.SaveChangesAsync();
            _context.Dispose();
            return Ok(new { puzzleId = puzzleInput.Id, fuelRequired = minFuel });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _context.TreasurePuzzles
                .OrderByDescending(t => t.CreatedTime)
                .ToListAsync();
            _context.Dispose();
            var result = history
                .Select(t => new
                {
                    t.RowCount,
                    t.ColumnCount,
                    t.MaxChestNumber,
                    Matrix = JsonSerializer.Deserialize<int[][]>(t.MatrixJson),
                    t.MinimumFuelRequired,
                    t.CreatedTime
                });

            return Ok(history);
        }

        private decimal CalculateMinimumFuel(int[][] treasureMap, int rows, int columns, int maxChestNumber)
        {
            var chestPositions = new Dictionary<int, List<(int row, int col)>>();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int chestNumber = treasureMap[row][col];
                    if (!chestPositions.ContainsKey(chestNumber))
                        chestPositions[chestNumber] = new List<(int, int)>();

                    chestPositions[chestNumber].Add((row, col));
                }
            }

            var priorityQueue = new SortedSet<(decimal fuelCost, int row, int col)>();
            priorityQueue.Add((0, 0, 0));

            int currentKey = 0;
            while (currentKey <= maxChestNumber && chestPositions.ContainsKey(currentKey + 1))
            {
                var nextChests = chestPositions[currentKey + 1];
                var newQueue = new SortedSet<(decimal fuelCost, int row, int col)>();

                foreach (var (fuel, currentRow, currentCol) in priorityQueue)
                {
                    foreach (var (nextRow, nextCol) in nextChests)
                    {
                        decimal travelCost = (decimal)Math.Sqrt(Math.Pow(currentRow - nextRow, 2) + Math.Pow(currentCol - nextCol, 2));
                        newQueue.Add((fuel + travelCost, nextRow, nextCol));
                    }
                }

                priorityQueue = newQueue;
                currentKey++;
            }

            return priorityQueue.Min.fuelCost;
        }
    }
}
