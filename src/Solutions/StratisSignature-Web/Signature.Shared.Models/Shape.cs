using System;

namespace Signature.Shared.Models
{
    public class Shape
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid SignerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
