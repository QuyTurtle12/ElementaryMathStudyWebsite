﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionCreateDto
    {
        public string QuizId { get; set; } = string.Empty;
        public string QuestionContext { get; set; } = string.Empty;

    }
}