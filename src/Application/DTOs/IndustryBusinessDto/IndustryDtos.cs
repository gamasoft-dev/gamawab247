using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateIndustryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class IndustryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class UpdateIndustryDto : CreateIndustryDto
    {
    }
}
