using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuditClient
{
    public class Employee
    {
        public Guid Id { get; set; }

        public Guid Name { get; set; }

        public Guid Department { get; set; }

        public Guid Age { get; set; }

        public Employee()
        {
            this.Id = Guid.NewGuid();
            this.Name = Guid.NewGuid();
            this.Department = Guid.NewGuid();
            this.Age = Guid.NewGuid();
        }
    }
}
