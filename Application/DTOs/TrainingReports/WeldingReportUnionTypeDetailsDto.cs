using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.TrainingReports
{
    public class WeldingReportUnionTypeDetailsDto
    {
        public int Id { get; set; }

        public int ListNumber { get; set; }

        public string UnionName { get; set; } = string.Empty;
    }
}
