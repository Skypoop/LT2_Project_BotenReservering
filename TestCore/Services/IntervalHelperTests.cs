using Moq;
using NUnit.Framework;
using FluentAssertions;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TestCore.Services;

[TestFixture]
public class IntervalHelperTests
{
  [Test]
  public void isIntervalCollisionCountedCorrectly_WhenGivenIntervalListWithCollisions()
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
  public void isIntervalCollisionCountedCorrectly_WhenIntervalListIsEmpty()
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
  public void isIntervalCollisionCountedCorrectly_WhenIntervalIsContainedOrNoCollision()
  {
    // arrange
    float[] a = new float[] { 5.0f, 10.0f };
    float[][] intervalList =
    [
      new float[] { 6.0f, 8.0f }, 
      new float[] { 1.0f, 4.0f }, 
      new float[] { 11.0f, 12.0f }
    ];

    // act
    int count = IntervalHelper.CountIntersectionsWithIntervalList(a, intervalList);

    // acknowledge
    count.Should().Be(1);
  }

}
