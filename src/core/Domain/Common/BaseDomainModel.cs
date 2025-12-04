

namespace Domain.Common
{
    public abstract class BaseDomainModel
    {
        public int id { get; set; }
        public DateTime createDate { get; set; }
        public string? createBy { get; set; }
        public DateTime lastModifiedDate { get; set; }
        public string? lastModifiedBy { get; set; }
    }
}
