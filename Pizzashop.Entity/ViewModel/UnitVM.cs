using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class UnitVM
{
    public Guid Id { get; set; }

    [StringLength(20)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string Shortname { get; set; } = null!;
}
