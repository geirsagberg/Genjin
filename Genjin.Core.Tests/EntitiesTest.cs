using FluentAssertions;
using Genjin.Core.Entities;
using Genjin.Example;
using Xunit;

namespace Genjin.Core.Tests;

public class EntitiesTest {
    private class Foo {
    }

    private class Bar {
    }

    [Fact]
    public void GetEntitiesMatchingAll_will_match_entities_that_also_have_other_components() {
        var world = new World(_ => { }, new MessageHub());

        var entity = world.CreateEntity();
        entity.Add(new Foo());
        entity.Add(new Bar());

        world.GetEntitiesMatchingAll(typeof(Foo)).Should().Contain(entity);
    }
}
