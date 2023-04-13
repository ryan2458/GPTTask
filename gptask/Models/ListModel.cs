using SQLite;

namespace gptask.Models
{
    public class ListModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
    }
}
