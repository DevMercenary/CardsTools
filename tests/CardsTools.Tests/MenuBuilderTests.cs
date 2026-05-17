using CardsTools.Menu;
using FluentAssertions;
using Xunit;

namespace CardsTools.Tests;

public sealed class MenuBuilderTests
{
    [Fact]
    public void Root_with_items_collects_them_in_order()
    {
        var menu = MenuBuilder.Root("Top")
            .Item("One", () => { })
            .Item("Two", () => { })
            .Back("Back")
            .Build();

        menu.Header.Should().Be("Top");
        menu.Items.Should().HaveCount(3);
        menu.Items.Select(i => i!.Header).Should().Equal("One", "Two", "Back");
    }

    [Fact]
    public void Submenu_attaches_child_menu_with_items()
    {
        var menu = MenuBuilder.Root("Top")
            .Submenu("Child", c => c
                .Item("A", () => { })
                .Item("B", () => { }))
            .Build();

        menu.Items.Should().HaveCount(1);
        var child = menu[0] as CardsTools.Data.Managers.MenuManager.IMenu;
        child.Should().NotBeNull();
        child!.Items.Should().HaveCount(2);
    }
}
