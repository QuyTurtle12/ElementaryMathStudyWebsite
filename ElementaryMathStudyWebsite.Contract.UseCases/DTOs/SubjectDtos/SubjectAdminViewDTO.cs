using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos
{
    public class SubjectAdminViewDTO : ISubjectBaseDTO
    {
<<<<<<< HEAD
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
=======
        public required string Id { get; set; }
        public required string SubjectName { get; set; }
        public double? Price { get; set; }
>>>>>>> 979c02fba4e0b01837805fe6c340895673dad8b9
        public bool Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreaterName { get; set; }
        public string? CreaterPhone { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? UpdaterName { get; set; }
        public string? UpdaterPhone { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        double ISubjectBaseDTO.Price { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
