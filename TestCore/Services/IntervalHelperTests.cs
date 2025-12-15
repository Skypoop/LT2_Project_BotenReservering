using FluentAssertions;
using ProjectBotenReservering.Core.Helpers;

namespace TestCore.Services;

[TestFixture]
public class IntervalHelperTests
{
  [Test]
  public void IsIntervalCollisionCountedCorrectly_WhenGivenIntervalListWithCollisions()
  {
    // arrange
    float[] a = new float[] { 1.0f, 4.0f };
    float[][] intervalList =
    [
      new float[] { 1.0f, 4.0f },
      new float[] { 3.0f, 5.0f },
      new float[] { 7.0f, 8.0f },
      new float[] { 0.0f, 2.0f }
    ];
    // act
    int count = IntervalHelper.CountIntersectionsWithIntervalList(a,intervalList);
    // acknowledge
    count.Should().Be(3);
  }
  [Test]
  public void IsIntervalCollisionCountedCorrectly_WhenIntervalListIsEmpty()
  {
    // arrange
    float[] a = new float[] { 1.0f, 4.0f };
    float[][] intervalList = [];

    // act
    int count = IntervalHelper.CountIntersectionsWithIntervalList(a, intervalList);

    // acknowledge
    count.Should().Be(0);
  }
  
  [Test]
  public void IsIntervalCollisionCountedCorrectly_WhenIntervalIsContainedOrNoCollision()
  {
    // arrange
    float[] a = new float[] { 5.0f, 6.0f };
    float[][] intervalList =
    [
      new float[] { 7.0f, 8.0f }, 
      new float[] { 1.0f, 4.0f }, 
      new float[] { 11.0f, 12.0f }
    ];

    // act
    int count = IntervalHelper.CountIntersectionsWithIntervalList(a, intervalList);

    // acknowledge
    count.Should().Be(0);
  }

}
