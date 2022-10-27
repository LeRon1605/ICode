using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Data
{
    public static class TestcaseData
    {
        public static List<TestcaseDTO> GetAll()
        {
            return new List<TestcaseDTO>()
            {
                new TestcaseDTO
                {
                    ID = "1",
                    Input = "1",
                    Output = "1",
                    MemoryLimit = 5000,
                    TimeLimit = 0.001f
                }
            };
        }
    }
}
