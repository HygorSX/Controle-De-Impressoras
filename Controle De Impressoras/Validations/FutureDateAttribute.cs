using System;
using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTimeValue)
        {
            DateTime currentDateTime = DateTime.Now;
            return dateTimeValue <= currentDateTime;
        }
        else if (value is DateTime?)
        {
            DateTime? dateTimeValueNullable = (DateTime?)value;
            if (!dateTimeValueNullable.HasValue)
            {
                // Se o valor é nulo, considerar a validação como válida
                return true;
            }

            DateTime currentDateTime = DateTime.Now;
            return dateTimeValueNullable.Value <= currentDateTime;
        }

        return false;
    }
}
