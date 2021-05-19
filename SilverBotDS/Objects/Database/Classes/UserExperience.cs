using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Objects.Database.Classes
{
    public class UserExperience
    {
        [Key]
        public ulong Id { get; init; }

        public string XPString
        {
            get => XP.ToString(); set => XP = BigInteger.Parse(value, CultureInfo.InvariantCulture);
        }

        [NotMapped]
        public BigInteger XP { get; set; }

        public void Increase()
        {
            try
            {
                XP++;
            }
            catch (OutOfMemoryException)
            {
                //we ran out of memory
            }
        }

        public void Decrease()
        {
            try
            {
                XP--;
            }
            catch (OutOfMemoryException)
            {
                //we ran out of memory
            }
        }

        public void Increase(ulong count)
        {
            try
            {
                XP += count;
            }
            catch (OutOfMemoryException)
            {
                //we ran out of memory
            }
        }

        public void Decrease(ulong count)
        {
            try
            {
                XP -= count;
            }
            catch (OutOfMemoryException)
            {
                //we ran out of memory
            }
        }
    }
}