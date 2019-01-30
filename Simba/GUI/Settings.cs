using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simba
{
    public partial class SavannahForm
    {
        public float DrawingMovementSpeed { get; set; } = 3.5f;
        public bool ShowGrid { get; set; } = false;

        public int AmountHorizontalFields { get => Savannah.AmountHorizontalFields; }
        public int AmountVerticalFields { get => Savannah.AmountVerticalFields; }

        public const int ImagesWidth = 50;
        public const int ImagesHeight = 50;
        public const int ticksWithRainAllowed = 30;
    }
}