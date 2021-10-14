using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Models
{
    public class FilterResultsInputModel : IValidatableObject
    {

        public string PartitionKey { get; set; }

        public string RowKeyDateStart { get; set; }

        public string RowKeyTimeStart { get; set; }


        public string RowKeyDateEnd { get; set; }

        public string RowKeyTimeEnd { get; set; }

        [Range(-100, +200)]
        public double? MinTemperature { get; set; }

        [Range(-100,200)]
        public double? MaxTemperature { get; set; }

        [Range(0, 300)]
        public double? MinPrecipitation { get; set; }

        [Range(0,300)]
        public double? MaxPrecipitation { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (!String.IsNullOrEmpty(RowKeyDateStart) && String.IsNullOrEmpty(RowKeyTimeStart))
                errors.Add(new ValidationResult("A start time must be included if a start date is included"));
            if (String.IsNullOrEmpty(RowKeyDateStart) && !String.IsNullOrEmpty(RowKeyTimeStart))
                errors.Add(new ValidationResult("A start date must be included if a start time is included"));
            if (!String.IsNullOrEmpty(RowKeyDateEnd) && String.IsNullOrEmpty(RowKeyTimeEnd))
                errors.Add(new ValidationResult("A end time must be included if an end date is included"));
            if (String.IsNullOrEmpty(RowKeyDateEnd) && !String.IsNullOrEmpty(RowKeyTimeEnd))
                errors.Add(new ValidationResult("An end date must be included if an end time is included"));

            if (errors.Count > 0)
                return errors;

            return new ValidationResult[] { ValidationResult.Success };
        }
    }
}
