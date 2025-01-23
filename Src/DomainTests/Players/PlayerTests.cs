﻿using Domain.Cards;
using Domain.Players;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTests.Players;

[TestClass]
public class PlayerTests
{
    Player sut;

    [TestInitialize]
    public void Setup()
    {
        sut = new GreedyPlayer();
    }

    [TestMethod]
    public void EmptyCards_IsZeroPoints()
    {
        var result = sut.CardPoints();

        result.Should().Be(0);
    }

    [TestMethod]
    public void OneCard_IsPoint_OfCardTest()
    {
        var value = 5;
        var card = new Card(value);
        sut.AcceptCard(card);

        var result = sut.CardPoints();

        result.Should().Be(value);
    }

    [TestMethod]
    public void TwoCards_WithGap_IsSumTest()
    {
        var value1 = 5;
        var value2 = value1 + 2;
        var card1 = new Card(value1);
        var card2 = new Card(value2);
        sut.AcceptCard(card1);
        sut.AcceptCard(card2);

        var result = sut.CardPoints();

        result.Should().Be(value1 + value2);
    }


    [TestMethod]
    public void LowestCard_ForStreetTest()
    {
        var value = 5;
        var card1 = new Card(value);
        var card2 = new Card(value + 1);
        sut.AcceptCard(card1);
        sut.AcceptCard(card2);

        var result = sut.CardPoints();

        result.Should().Be(value);
    }


    [TestMethod]
    public void LowestCard_ForThreeStreetTest()
    {
        var value = 5;
        var card1OfStreet1 = new Card(value);
        var card2OfStreet1 = new Card(value + 1);

        sut.AcceptCard(card1OfStreet1);
        sut.AcceptCard(card2OfStreet1);

        var card1OfStreet2 = new Card(value + 5);
        var card2OfStreet2 = new Card(value + 6);
        sut.AcceptCard(card1OfStreet2); 
        sut.AcceptCard(card2OfStreet2);

        var result = sut.CardPoints();

        result.Should().Be(card1OfStreet1.Value + card1OfStreet2.Value);
    }
}
