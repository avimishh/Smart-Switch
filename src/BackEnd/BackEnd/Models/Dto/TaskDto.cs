using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BackEnd.Models.Task;

namespace BackEnd.Models.Dto
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public Operations Operation { get; set; }
        public string DeviceMac { get; set; }
        public TaskTypes TaskType { get; set; }
        public int RepeatEvery { get; set; } // minutes
        public DateTime StartDate { get; set; }
    }
}