﻿using System;
using System.Collections.Generic;

namespace tvz2api_cqrs.Models.DTO
{
  public class ExamDTO
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public int TimeNeeded { get; set; }
  }

  public class UnfinishedExamDTO
  {
    public int Id { get; set; }
    public int? CourseId { get; set; }
    public int? CreatedById { get; set; }
  }
}
